using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjetoIBGE.Configuration;
using ProjetoIBGE.Data;
using ProjetoIBGE.Services;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

ConfigureMvc(builder);
ConfigureServices(builder);
ConfigureAuthentication(builder);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c =>
{
   // g.SwaggerDoc("v1", new OpenApiInfo { Title = "Projeto IBGE", Version = "v1" });
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IBGE API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
       
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
                         new string[] {}
                    }
                });


});


var app = builder.Build();
Configuracao.Key = app.Configuration.GetValue<string>("JwtKey");


    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Projeto IBGE");
        c.RoutePrefix = string.Empty;
    
    });

if (app.Environment.IsProduction())
{
    Console.WriteLine(" ambiente de produção");
}

app.MapControllers();
app.Run();


void ConfigureMvc(WebApplicationBuilder builder)
{
    builder.Services.AddMemoryCache();


    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>  
        {
            options.SuppressModelStateInvalidFilter = true;

        });
}
void ConfigureServices(WebApplicationBuilder builder)
{

    builder.Services.AddDbContext<IBGEDataContext>(options => {
        
        //options.UseSqlServer(Configuracao.Decrypt(builder.Configuration.GetConnectionString("DefaultConnection"),builder.Configuration.GetValue<string>("_"))); //só funciona em modo debug
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    builder.Services.AddTransient<TokenService>();

}
void ConfigureAuthentication(WebApplicationBuilder builder)
{
   
    var key = Encoding.ASCII.GetBytes(Configuracao.Key);

    builder.Services.AddAuthentication(x =>
    {

        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    }).AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });
}