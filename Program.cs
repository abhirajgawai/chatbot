using Chatbot.Core.Service;
using Chatbot.Infrastructure.Middleware;
using FluentValidation;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//configure appsettings.json and environment variables
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

//configure serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console() // Logs to the console
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) // File logging with daily rolling
    .CreateLogger();

//add serilog to the application
builder.Host.UseSerilog();

// add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddCors();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; }); // Path to the Angular app
builder.Services.AddMediatR(cgd => cgd.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<UserSettingService>();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); // Enhanced error page for development
}

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<BrowserSettingMiddleware>();
app.UseStaticFiles();
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()); // Enable CORS
app.UseHttpsRedirection();
app.UseSerilogRequestLogging(); // Log HTTP requests
app.UseRouting();
app.MapControllers();

//todo: add authentication and authorization
//app.UseAuthorization();
//app.UseAuthentication();

//todo: configureRoutes()

// Serve static files and configure SPA
if (!builder.Environment.IsEnvironment("Local"))
{
    app.UseSpaStaticFiles(); // Use pre-built Angular files in production
}

app.UseSpa(sap =>
{
    sap.Options.SourcePath = "...ClientApp"; // Angular development path
    if (builder.Environment.IsEnvironment("Local"))
    {
        sap.UseProxyToSpaDevelopmentServer(builder.Configuration.GetValue<string>("Local:BaseUrl") ??
            throw new Exception("Local base URL is null"));
    }
});

// run the application
try
{
    Log.Information("Starting Chatbot");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Chatbot failed to start correctly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}


//app.MapControllers();

//app.Run();
