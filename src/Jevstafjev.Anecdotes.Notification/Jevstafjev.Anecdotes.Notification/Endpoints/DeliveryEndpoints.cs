using Ardalis.Result;
using Duende.IdentityModel.Client;
using Jevstafjev.Anecdotes.Notification.Application.Messaging.DeliveryMessages;
using Jevstafjev.Anecdotes.Notification.Application.Messaging.DeliveryMessages.Queries;
using Jevstafjev.Anecdotes.Notification.Application.Messaging.DeliveryMessages.ViewModels;
using Jevstafjev.Anecdotes.Notification.Application.Services;
using Jevstafjev.Anecdotes.Notification.Definitions.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace Jevstafjev.Anecdotes.Notification.Endpoints;

public class DeliveryEndpoints : AppDefinition
{
    public override void ConfigureApplication(WebApplication app)
    {
        app.MapDeliveryEndpoints();
    }
}

internal static class DeliveryEndpointsExtensions
{
    public static void MapDeliveryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/delivery/").WithTags("Delivery");

        group.MapPost("send-to-all", async ([FromServices] IMediator mediator, AnecdoteViewModel model, HttpContext context) =>
            await mediator.Send(new SendAnecdoteToAllSubscribersRequest(model), context.RequestAborted))
            .RequireAuthorization(AppData.DefaultPolicyName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();
    }
}

