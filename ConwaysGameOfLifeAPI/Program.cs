using ConwaysGameOfLifeAPI.Models;
using ConwaysGameOfLifeAPI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Production Ready
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // HTTP port
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseHttps(); // Enable HTTPS
    });
});

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog for logging

builder.Services.AddControllers();
builder.Services.AddSingleton<IGameOfLifeService, GameOfLifeService>();
builder.Services.Configure<GameOfLifeSettings>(
    builder.Configuration.GetSection("GameOfLifeSettings"));
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache(); // Add memory cache service

var app = builder.Build();
app.UseSerilogRequestLogging(); // Log requests automatically
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Conway's Game of Life API V1");
    });
}

app.UseHttpsRedirection();   // for production

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health");
});

app.Run();
