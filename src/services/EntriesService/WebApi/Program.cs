// unsetz
using AppCore.CreateEntry;
using AppCore.GetEntries;

using Mediator.DependencyInjection;
using Mediator.Dispatcher;

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMediator(typeof(Program).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/entries", async (
    IDispatcher dispatcher,
    CancellationToken cancellationToken) =>
{
    var query = new GetEntriesQuery();
    return await dispatcher.HandleAsync(query, cancellationToken);
});

app.MapGet("/entries/{id}", async ([FromRoute] int id,
    IDispatcher dispatcher,
    CancellationToken cancellationToken) =>
{
    var query = new GetEntriesQuery() { Id = id };
    return await dispatcher.HandleAsync(query, cancellationToken);
});

app.MapPost("/entries", async ([FromBody] CreateEntryCommand command,
    IDispatcher dispatcher,
    CancellationToken cancellationToken) =>
{
    return await dispatcher.HandleAsync(command, cancellationToken);
});

app.Run();