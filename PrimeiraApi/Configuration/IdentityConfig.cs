using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PrimeiraApi.Data;
using PrimeiraApi.Models;
using System.Text;

namespace PrimeiraApi.Configuration
{
    public static class IdentityConfig
    {
        public static WebApplicationBuilder AddIdentityConfig(this WebApplicationBuilder builder)
        {
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
            return builder;
        }
    }  

}


