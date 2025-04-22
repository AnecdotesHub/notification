using Ardalis.Result;
using Duende.IdentityModel.Client;
using Jevstafjev.Anecdotes.Notification.Application.Messaging.DeliveryMessages.ViewModels;
using Jevstafjev.Anecdotes.Notification.Application.Services;
using MediatR;
using System.Text.Json;

namespace Jevstafjev.Anecdotes.Notification.Application.Messaging.DeliveryMessages.Queries;

public record SendAnecdoteToAllSubscribersRequest(AnecdoteViewModel Model) : IRequest<Result>;

public class SendAnecdoteToAllSubscribersHandler(IHttpClientFactory httpClientFactory, IEmailService emailService, IConfiguration configuration)
    : IRequestHandler<SendAnecdoteToAllSubscribersRequest, Result>
{
    public async Task<Result> Handle(SendAnecdoteToAllSubscribersRequest request, CancellationToken cancellationToken)
    {
        var authClient = httpClientFactory.CreateClient();
        var discoveryDocument = await authClient.GetDiscoveryDocumentAsync(configuration["AuthServer:Url"]);
        var tokenResponse = await authClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = discoveryDocument.TokenEndpoint,
            ClientId = "notification-service-client",
            ClientSecret = "secret",
            Scope = "SubscriberApi"
        });

        var subscriberClient = httpClientFactory.CreateClient();
        subscriberClient.SetBearerToken(tokenResponse.AccessToken!);

        var response = await subscriberClient.GetAsync($"{configuration["ServiceUrls:SubscriberApi"]}/api/subscribers/get-all");
        if (!response.IsSuccessStatusCode)
        {
            return Result.Error();
        }

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Result<List<SubscriberViewModel>>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        foreach (var subscriber in result!.Value)
        {
            await emailService.SendEmailAsync(subscriber.Email, $"New anecdote: {request.Model.Title}", request.Model.Content, request.Model.Content);
        }

        return Result.Success();
    }
}
