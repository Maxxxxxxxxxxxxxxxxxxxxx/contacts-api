using System.Text;
using ContactsAPI.Config;
using ContactsAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .Build();

// JWT-based authentication setup
builder.Services.AddAuthentication(x =>
{
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = config["JWTSettings:Issuer"],
        ValidAudience = config["JWTSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWTSettings:Secret"]!)),
        ValidateIssuer = config.GetValue<bool>("JWTSettings:ValidateIssuer"),
        ValidateAudience = config.GetValue<bool>("JWTSettings:ValidateAudience"),
        ValidateLifetime = config.GetValue<bool>("JWTSettings:ValidateLifetime"),
        ValidateIssuerSigningKey = config.GetValue<bool>("JWTSettings:ValidateIssuerSigningKey"),
    };
});

var CorsOrigins = "_origins";

// mounting remaining services to the app
builder.Services.Configure<MongoSettings>(config.GetSection("Mongo"));
builder.Services.AddSingleton<CollectionsService>();
builder.Services.AddSingleton<LoginService>();
builder.Services.AddSingleton(config);
builder.Services.AddControllersWithViews();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsOrigins,
        policy  =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors(CorsOrigins);

app.Run();

