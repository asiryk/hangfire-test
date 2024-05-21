using Microsoft.AspNetCore.SignalR;

namespace HangfireDemo.SignalR
{
    public class JobHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            Console.WriteLine("client invocation with arguments {0}, {1}", user, message);
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
