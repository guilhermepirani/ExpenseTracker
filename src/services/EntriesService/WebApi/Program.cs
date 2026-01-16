using Api.Configuration;
using AppCore;
using AppCore.Configuration;
using Infra.Configuration;
using Mediator.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = SerilogConfiguration.ConfigureSerilog();

builder.Services.AddOpenApi();
builder.Services.AddValidation();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.RequireHttpsMetadata = false;
        opts.Audience = builder.Configuration["Authentication:Audience"];
        opts.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Authentication:ValidIssuer"]
        };
    });

builder.Services.AddVersionedEndpoints(typeof(Program).Assembly);
builder.Services.AddValidators();
builder.Services.AddMediator(typeof(Program).Assembly);
builder.Services.AddPipelineBehaviours();
builder.Services.AddRepositories();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler(HandleEndpointBindingException());
app.MapVersionedEndpoints();

app.UseAuthentication();
app.UseAuthorization();

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