using System.Text;
using ContactsAPI.Config;
using ContactsAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ContactsAPI;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        config = configuration;
    }

    public IConfiguration config { get; }
    
    public void ConfigureAuth(IServiceCollection services)
    {
        services.AddAuthentication(x =>
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
    }

    public void ConfigureMongo(IServiceCollection services)
    {
        services.Configure<MongoSettings>(config.GetSection("Mongo"));
    }

    public void ConfigureWebServices(IServiceCollection services)
    {
        // services.AddRazorPages();
        
        var CorsOrigins = "_origins";

        // mounting remaining services to the app
        services.AddSingleton<CollectionsService>();
        services.AddSingleton<LoginService>();
        services.AddSingleton(config);
        services.AddControllersWithViews();
        services.AddCors(options =>
        {
            options.AddPolicy(name: CorsOrigins,
                policy  =>
                {
                    policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors("_origins");

        // app.UseRouting();
        
        // app.UseEndpoints(endpoints =>
        // {
        //     // Map AuthController to api/auth
        //     endpoints.MapControllerRoute(
        //         name: "auth",
        //         pattern: "api/auth/",
        //         defaults: new { controller = "Auth" }
        //     );
        //
        //     // Map ContactController to api/contact
        //     endpoints.MapControllerRoute(
        //         name: "contact",
        //         pattern: "api/contact/",
        //         defaults: new { controller = "Contact" }
        //     );
        // });
    }
}