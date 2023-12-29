using System.Security.Cryptography;
using ChatServer.Hubs;
using ChatServer.Repository;
using Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



var _bob = new User
{
    Id = 1,
    Name = "Bob",
    Password = "1234",
    PublicKey = 0
};

var _alice = new User
{
    Id = 1,
    Name = "Alice",
    Password = "1234",
    PublicKey = 0
};

var _repo = LoginRepo.Instance;
_repo.AddOrUpdateUser(_bob);
_repo.AddOrUpdateUser(_alice);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder => builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
