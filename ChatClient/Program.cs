using System.Threading.Tasks;
using ChatClient;
using Microsoft.AspNetCore.SignalR.Client;
using Shared;

var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7160/chathub")
    .Build();



connection.On<string, string>("ReceiveMessage", (user, message) =>
{
    Console.WriteLine($"{user}: {message}");
});

await connection.StartAsync();

Console.WriteLine("Connected. Enter your name:");
var userName = Console.ReadLine();
Console.WriteLine("Write your Secret Key (Write a number)");
var secretKey = new PrivateKey
{
    privatekey = Convert.ToInt32(Console.ReadLine())
};




await connection.SendAsync("PGRequest");

connection.On<PublicSharedKey>("RecievePG", x => new PublicSharedKey
{
    g = x.g,
    p = x.p,
});

    

//Menu Loop
while (true)
{
    var message = Console.ReadLine();

    if (string.IsNullOrEmpty(message))
    {
        break;
    }

    await connection.SendAsync("SendMessage", userName, message);
}

await connection.DisposeAsync();