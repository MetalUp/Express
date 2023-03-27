using System.Globalization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

static IEnumerable<string> ValidAudiences(ConfigurationManager configuration) {
    var audiences = Environment.GetEnvironmentVariable("ValidAudiences") ?? configuration["ValidAudiences"] ?? "";
    return audiences.Split(',');
}

builder.Services.AddLocalization();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
    options.TokenValidationParameters = new TokenValidationParameters {
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
        ValidateIssuer = true,
        ValidAudiences = ValidAudiences(builder.Configuration)
    };
});

builder.Services.AddControllers();

var app = builder.Build();

const string enGb = "en-GB";
app.UseRequestLocalization(new RequestLocalizationOptions {
    DefaultRequestCulture = new RequestCulture(enGb),
    SupportedCultures = new[] { new CultureInfo(enGb) },
    FallBackToParentCultures = false
});
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture(enGb);

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();