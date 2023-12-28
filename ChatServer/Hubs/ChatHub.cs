using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ChatServer.Hubs;

    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, byte[]> PublicKeys = new ConcurrentDictionary<string, byte[]>();
        private static ConcurrentDictionary<string, string> KeyExchangeStates = new ConcurrentDictionary<string, string>();

        public async Task SendPublicKey(string user, byte[] publicKey)
        {
            PublicKeys[user] = publicKey;

            // Check if the user has initiated a key exchange
            if (KeyExchangeStates.TryGetValue(user, out var targetUser))
            {
                // Notify the target user that a public key is ready for exchange
                await Clients.Client(targetUser).SendAsync("InitiateKeyExchange", user, publicKey);

                // Clear the key exchange state
                KeyExchangeStates.TryRemove(user, out _);
                KeyExchangeStates.TryRemove(targetUser, out _);
            }
        }

        public async Task InitiateKeyExchange(string user, string targetUser)
        {
            // Set the key exchange state for both users
            KeyExchangeStates[user] = targetUser;
            KeyExchangeStates[targetUser] = user;

            // Notify the target user to expect a public key
            await Clients.Client(targetUser).SendAsync("ExpectPublicKey", user);
        }

        public async Task CompleteKeyExchange(string user)
        {
            // Notify the user that the key exchange is complete
            await Clients.Client(user).SendAsync("KeyExchangeComplete");
        }

        public async Task SendMessage(string user, string message)
        {
            // Broadcast the message to all clients
            // In a real-world scenario, you might send this to specific clients only
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
}
