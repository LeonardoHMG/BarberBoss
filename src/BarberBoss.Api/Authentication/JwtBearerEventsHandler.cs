using BarberBoss.Communication.Responses;
using BarberBoss.Exception;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Json;

namespace BarberBoss.Api.Authentication;

public static class JwtBearerEventsHandler
{
    public static JwtBearerEvents Create()
    {
        return new JwtBearerEvents
        {
            OnChallenge = OnChallenge,
            OnForbidden = OnForbidden
        };
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static async Task OnChallenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var errorResponse = new ResponseErrorJson(ResourceErrorMessages.UNAUTHORIZED);

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, _jsonOptions));
    }

    private static async Task OnForbidden(ForbiddenContext context)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";

        var errorResponse = new ResponseErrorJson(ResourceErrorMessages.FORBIDDEN);

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, _jsonOptions));
    }
}