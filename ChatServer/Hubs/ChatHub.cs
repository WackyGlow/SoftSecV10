using System.Threading.Tasks;
using ChatServer.Repository;
using Microsoft.AspNetCore.SignalR;
using Shared;

namespace ChatServer.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            Console.WriteLine(message);
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}