using ContactsAPI;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .Build();

var startup = new Startup(config);

startup.ConfigureWebServices(builder.Services);
// startup.ConfigureAuth(builder.Services);
startup.ConfigureMongo(builder.Services);

var app = builder.Build();


startup.Configure(app, app.Environment);

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();