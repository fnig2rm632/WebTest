using WebTest.Interfaces.Manager;

namespace WebTest.Middleware;

public class WebSocketMiddleware(
    RequestDelegate next, 
    IWedSocketManager webSocketManager,
    ILogger<WebSocketMiddleware> logger)
{

    public async Task Invoke(HttpContext context)
    {
        try
        {
            logger.LogInformation("WebSocketMiddleware called");
            if (context.WebSockets.IsWebSocketRequest && context.Request.Path == "/game-ws")
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var playerId = context.Request.Query["playerId"];

                logger.LogInformation($"WebSocket connected with id {playerId}");
            
                if (string.IsNullOrEmpty(playerId))
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                await webSocketManager.AddPlayer(Guid.Parse(playerId), webSocket);
            }
            else
            {
                await next(context);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "WebSocketMiddleware exception");
        }
        
    }
}