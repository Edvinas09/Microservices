using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Models;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SQL Server Database");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else
{
    Console.WriteLine("--> Using InMemory Database");
    builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddGrpc();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5175, o => o.Protocols = HttpProtocols.Http2);
});

Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.proto", async context =>
{
    await context.Response.WriteAsync(System.IO.File.ReadAllText("Protos/platforms.proto"));
});
PrepDb.PrepPopulation(app, builder.Environment.IsProduction()); // Seed the database with initial data
app.Run();
