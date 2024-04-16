namespace PrimeiraApi.Configuration
{
    public static class CorsConfig
    {
        public static WebApplicationBuilder AddCorsConfig(this WebApplicationBuilder builder)
        {
            //Mais segurança ma aplicação para casos de subir em produção
            //Politica recomendada pela microsoft
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Development", builder =>
                builder
                        .WithOrigins()
                        .AllowAnyMethod()
                            .AllowAnyHeader());

                options.AddPolicy("Production", builder =>
                builder
                        .WithOrigins("http://localhost:9000")
                        .WithMethods("POST")
                            .AllowAnyHeader());
            });
            return builder;
        }
    }

}


