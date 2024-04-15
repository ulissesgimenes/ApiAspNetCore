using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PrimeiraApi.Data;
using PrimeiraApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Não recomendado apenas para testes
///normal deixar apenas builder.Services.AddControllers();

//builder.Services.AddControllers();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddEndpointsApiExplorer();

//Caso não vá usar auth na Api use apenas o comando abaixo
//builder.Services.AddSwaggerGen();

//Para liberar token para a API
builder.Services.AddSwaggerGen( c => {
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o Token JWT desta maneira. Example: Bearer {token}",
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
}

);

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Para Identity na API com perfis de usuário
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApiDbContext>();

//Para autenticação tokken
//para pegar do appsettings.json
//Configurando o JwtSettings
var JwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
//Populando a minha classe de token com os dados do appsettings.json
builder.Services.Configure<JwtSettings>(JwtSettingsSection);
//pegando os dados populados
var JwtSettings = JwtSettingsSection.Get<JwtSettings>();
//Criando a chave de segurança
var key = Encoding.ASCII.GetBytes(JwtSettings.keySecurity);
//Adicionando autenticação
builder.Services.AddAuthentication(options =>
{
    //Esquema de autenticação para usar o JasonWebToken
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //verifica se realmente esse token é valido
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        //Valida a chave de segurança
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //geração da chave de segurança
            //ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            
            //valida quem é o emissor
            ValidateIssuer = true,
            
            //ValidIssuer = JwtSettings.Sender,
            
            //Valida se a minha audiencia e compativel com a do token
            ValidateAudience = true,
            
            //Valida se é o mesmo dentro do meu appsethings.json
            ValidAudience = JwtSettings.Audience,
            
            //ValidateLifetime = true
            
            //Valida se é o mesmo dentro do meu appsethings.json
            ValidIssuer = JwtSettings.Sender,
        };
    });

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//ordem autenticar depois autorizar
app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();
