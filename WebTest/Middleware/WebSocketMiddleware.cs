using WebTest.Interfaces.Manager;

namespace WebTest.Middleware;

public class WebSocketMiddleware(RequestDelegate next, IWedSocketManager webSocketManager)
{

    public async Task Invoke(HttpContext context)
    {
        Console.WriteLine("Начало");
        if (context.WebSockets.IsWebSocketRequest && context.Request.Path == "/game-ws")
        {
            Console.WriteLine("Прошел");
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var playerId = context.Request.Query["playerId"];

            Console.WriteLine(playerId);
            
            if (string.IsNullOrEmpty(playerId))
            {
                context.Response.StatusCode = 400;
                return;
            }

            await webSocketManager.AddPlayer(Guid.Parse(playerId), webSocket);
        }
        else
        {
            Console.WriteLine("Ошибка" + context.WebSockets.IsWebSocketRequest);
            await next(context);
        }
    }
}