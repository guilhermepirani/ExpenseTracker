// unsetz
using EntriesService.Api;
using EntriesService.Api.Behaviours;

using Mediator.DependencyInjection;

using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

var logOutput = "[{Timestamp:HH:mm:ss} {Level:u3}] [{TraceId}] {Message:lj}{NewLine}{Exception}";
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        restrictedToMinimumLevel: LogEventLevel.Debug,
        outputTemplate: logOutput)
    .WriteTo.File("logs/apiLogFile.txt", outputTemplate: logOutput)
    .CreateLogger();

builder.Services.AddOpenApi();
builder.Services.AddMediator(typeof(Program).Assembly);
builder.Services.AddBehaviour(typeof(LoggingBeheviour<,>));
builder.Services.AddBehaviour(typeof(ValidationBeheviour<,>));
builder.Services.AddVersionedEndpoints(typeof(Program).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapVersionedEndpoints();

app.Run();