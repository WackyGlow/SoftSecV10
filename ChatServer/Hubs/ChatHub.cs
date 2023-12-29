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
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }


        public User GetUserByName(string userName)
        {
            var _repo = LoginRepo.Instance;
           
            var userDetails = _repo.GetUsers().FirstOrDefault(user => user.Name != userName);

            return userDetails; 
        }

    }
}