using PrimeiraApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApiConfig()
    .AddCorsConfig()
    .AddSwaggerConfig()
    .AddDbContextConfig()
    .AddIdentityConfig();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("Development");
}
else
{
    //Politica recomendada pela microsoft
    app.UseCors("Production");
}

app.UseHttpsRedirection();

//ordem autenticar depois autorizar
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
