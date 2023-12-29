using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Security.Cryptography;
using ChatClient;

Console.WriteLine("Enter your name:");
var userName = Console.ReadLine();
while (string.IsNullOrEmpty(userName))
{
    Console.WriteLine("Name cannot be empty. Please enter your name:");
    userName = Console.ReadLine();
}

var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7246/chathub")
    .Build();

var dh = new DiffieHellman();
byte[] sharedKey = null;
CryptoHelper cryptoHelper = null;

connection.On<string, byte[]>("ExpectPublicKey", async (user, publicKey) =>
{
    Console.WriteLine($"ExpectPublicKey event received from {user}");
    sharedKey = dh.GenerateSharedKey(publicKey);
    cryptoHelper = new CryptoHelper(sharedKey, new byte[16]); // Use an IV as needed
    await connection.SendAsync("SendPublicKey", userName, dh.PublicKey);
    Console.WriteLine($"Public key sent to {user}");
});

connection.On<string>("KeyExchangeComplete", user =>
{
    Console.WriteLine($"Key exchange with {user} complete.");
});

connection.On<string, string>("ReceiveMessage", (user, message) =>
{
    var decryptedMessage = DecryptMessage(message);
    Console.WriteLine($"{user}: {decryptedMessage}");
});

async Task SendMessageEncrypted(string user, string message)
{
    var encryptedMessage = EncryptMessage(message);
    await connection.SendAsync("SendMessage", userName, encryptedMessage);
    Console.WriteLine($"Encrypted message sent to {user}");
}

string EncryptMessage(string message)
{
    if (cryptoHelper == null)
    {
        Console.WriteLine("Shared key not established. Cannot encrypt message.");
        return message;
    }
    return cryptoHelper.EncryptMessage(message);
}

string DecryptMessage(string encryptedMessage)
{
    if (cryptoHelper == null)
    {
        Console.WriteLine("Shared key not established. Cannot decrypt message.");
        return encryptedMessage;
    }
    return cryptoHelper.DecryptMessage(encryptedMessage);
}

connection.Closed += async (error) =>
{
    Console.WriteLine($"Connection closed: {error?.Message}");
    await Task.Delay(new Random().Next(0, 5) * 1000);
    await connection.StartAsync();
};

await connection.StartAsync();
Console.WriteLine("Connection started.");

while (!connection.State.Equals(HubConnectionState.Connected))
{
    await Task.Delay(100); // Allow the connection to complete
}

while (true)
{
    var message = Console.ReadLine();

    if (string.IsNullOrEmpty(message))
    {
        break;
    }

    Console.WriteLine("Enter the name of the user to initiate key exchange:");
    var targetUser = Console.ReadLine();

    await connection.SendAsync("InitiateKeyExchange", userName, targetUser);

    // Wait fo
