using Duende.IdentityModel;
using Jevstafjev.Anecdotes.Notification.Definitions.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace Jevstafjev.Anecdotes.Notification.Definitions.Authorization;

public class AuthorizationDefinition : AppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        var url = builder.Configuration.GetSection("AuthServer").GetValue<string>("Url");

        builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, "Bearer", options =>
                {
                    options.SaveToken = true;
                    options.Authority = url;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.Name,
                        ValidateAudience = true,
                        ValidAudience = "Notification"
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            if (string.IsNullOrEmpty(context.Error))
                            {
                                context.Error = "invalid_token";
                            }

                            if (string.IsNullOrEmpty(context.ErrorDescription))
                            {
                                context.ErrorDescription = "This request requires a valid JWT access token to be provided";
                            }

                            if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                var authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
                                context.Response.Headers.Append("x-token-expired", authenticationException?.Expires.ToString("o"));
                                context.ErrorDescription = $"The token expired on {authenticationException?.Expires:o}";
                            }

                            return context.Response.WriteAsync(JsonSerializer.Serialize(new
                            {
                                error = context.Error,
                                error_description = context.ErrorDescription
                            }));
                        }
                    };
                });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(AppData.DefaultPolicyName, x =>
            {
                x.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                x.RequireAssertion(context =>
                {
                    var clientId = context.User.FindFirst("client_id")?.Value;
                    if (clientId == "anecdote-service-client")
                        return true;

                    return context.User.IsInRole("Administrator");
                });
            });
        });
    }

    public override void ConfigureApplication(WebApplication app)
    {
        app.UseCors(AppData.PolicyCorsName);
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
