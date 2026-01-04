// unsetz
using EntriesService.Api;

using Mediator.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddMediator(typeof(Program).Assembly);
builder.Services.AddVersionedEndpoints(typeof(Program).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapVersionedEndpoints();

app.Run();