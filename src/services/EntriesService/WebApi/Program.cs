// unsetz

using AppCore;
using AppCore.Configuration;

using EntriesService.Api;

using Infra;

using Mediator.DependencyInjection;

using Microsoft.AspNetCore.Diagnostics;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = SerilogConfiguration.ConfigureSerilog();

builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddVersionedEndpoints(typeof(Program).Assembly);
builder.Services.AddValidators();
builder.Services.AddMediator(typeof(Program).Assembly);
builder.Services.AddPipelineBehaviours();
builder.Services.AddRespositories();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler(HandleEndpointBindingException());
app.MapVersionedEndpoints();

app.Run();

static Action<IApplicationBuilder> HandleEndpointBindingException()
{
    return appError =>
        appError.Run(
            async context =>
            {
                context.Response.ContentType = "application/json";
                var contextFeature = context.Features
                    .Get<IExceptionHandlerFeature>();

                if (contextFeature is not null)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsJsonAsync(Result.Failure(
                        System.Net.HttpStatusCode.BadRequest,
                        ["Message body failed conversion to expected model"]));
                }
            });
}