using ContactsAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var loggerFactory = new LoggerFactory();

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var startup = new Startup(config);

startup.ConfigureWebServices(builder.Services);
startup.ConfigureMongo(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Logger.LogInformation($"MongoDB connection at mongodb://{config["MONGO_HOST"]}:{config.GetValue<int>("MONGO_PORT")}");
app.Run();