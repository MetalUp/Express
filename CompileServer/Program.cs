using Microsoft.AspNetCore.Authentication.JwtBearer;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
    //options.Audience = builder.Configuration["Auth0:Audience"];
    //options.TokenValidationParameters.NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";


    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
        ValidateIssuer = true,
        ValidAudiences = new List<string> 
        {
            "https://metalupadminserver.azurewebsites.net",
            "https://metalupadminserverdevelopment.azurewebsites.net",
            "https://metalupadminservertest.azurewebsites.net"
        }
    };
});

builder.Services.AddControllers();

//builder.Services.AddCors(options => {
//    options.AddPolicy("_myAllowSpecificOrigins", builder => {
//        builder
//            .WithOrigins(
//                "http://localhost:5001",
//                "http://localhost:49998",
//                "https://express.metalup.org")
//            .AllowAnyHeader()
//            .WithExposedHeaders("Warning", "ETag", "Set-Cookie")
//            .AllowAnyMethod()
//            .AllowCredentials();
//    });
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) { }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("_myAllowSpecificOrigins");

app.Run();