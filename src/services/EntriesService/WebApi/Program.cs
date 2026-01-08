// unsetz
using EntriesService.Api;
using EntriesService.AppCore.Configuration;

using Mediator.DependencyInjection;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = SerilogConfiguration.ConfigureSerilog();

builder.Services.AddOpenApi();
builder.Services.AddVersionedEndpoints(typeof(Program).Assembly);
builder.Services.AddMediator(typeof(Program).Assembly);
builder.Services.AddPipelineBehaviours();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapVersionedEndpoints();

app.Run();