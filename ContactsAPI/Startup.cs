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

    public void ConfigureMongo(IServiceCollection services)
    {
        var envPort = config.GetValue<int>("MONGO_PORT");
        var envHost = config["MONGO_HOST"];
        
        // Console.WriteLine($"mongodb://{envHost}:{envPort}");

        if (envPort != 0 && envHost != null)
        {
            MongoSettings settings = new MongoSettings($"mongodb://{envHost}:{envPort}");
            
            services.AddSingleton(settings);
            services.AddOptions<MongoSettings>().Configure(options =>
            {
                options.DatabaseName = settings.DatabaseName;
                options.AccountCollection = settings.AccountCollection;
                options.ContactCollection = settings.ContactCollection;
                options.CredentialsCollection = settings.CredentialsCollection;
                options.ConnectionString = settings.ConnectionString;
            });
        }
        else
        {
            services.Configure<MongoSettings>(config.GetSection("Mongo"));
        }
    }

    public void ConfigureWebServices(IServiceCollection services)
    {
        // services.AddRazorPages();
        
        var CorsOrigins = "_origins";

        services.AddSwaggerGen();

        // mounting remaining services to the app
        services.AddSingleton<CollectionsService>();
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
    }
}