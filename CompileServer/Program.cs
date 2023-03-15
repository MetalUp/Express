using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
    options.TokenValidationParameters = new TokenValidationParameters {
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
        ValidateIssuer = true,
        ValidAudiences = new List<string> {
            "https://metalupadminserver.azurewebsites.net",
            "https://metalupadminserverdevelopment.azurewebsites.net",
            "https://metalupadminservertest.azurewebsites.net"
        }
    };
});

builder.Services.AddControllers();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();