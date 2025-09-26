using Mc2.CrudTest.Application.Common.Behaviors;
using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Application.Customers.Commands.CreateCustomer;
using Mc2.CrudTest.Infrastructure.Persistence;
using Mc2.CrudTest.Infrastructure.Repositories;
using Mc2.CrudTest.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateCustomerCommand).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(CreateCustomerCommandValidator).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddSingleton<IPhoneNumberValidator, PhoneNumberValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error is ValidationException validationException)
        {
            var errors = validationException.Errors
                .Select(e => new { e.PropertyName, e.ErrorMessage });

            await context.Response.WriteAsJsonAsync(new { Errors = errors });
        }
        else
        {
            await context.Response.WriteAsJsonAsync(new { Error = "An unexpected error occurred." });
        }
    });
});


app.MapPost("/api/customers", async (IMediator mediator, [FromBody] CreateCustomerCommand command) =>
{
    try
    {
        var customer = await mediator.Send(command);
        return Results.Created($"/api/customers/{customer.Id}", customer);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 400);
    }
})
.WithName("CreateCustomer")
.WithOpenApi();

app.Run();

