// unsetz

using AppCore.Configuration;

using EntriesService.Api;

using Infra;

using Mediator.DependencyInjection;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = SerilogConfiguration.ConfigureSerilog();

builder.Services.AddOpenApi();
builder.Services.AddVersionedEndpoints(typeof(Program).Assembly);
builder.Services.AddValidators();
builder.Services.AddMediator(typeof(Program).Assembly);
builder.Services.AddPipelineBehaviours();
builder.Services.AddRespositories();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapVersionedEndpoints();

app.Run();