using Games.Api.Consumers;
using Games.Api.Data;
using Games.Api.Data.Entities;
using Games.Api.Endpoints;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.RabbitMqTransport.Topology;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<GamesDbContext>(config => config.UseInMemoryDatabase("Games"));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UpdateGameRankConsumer>();

    // Set endpoint name construction with kebab case
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, rabbit) =>
    {
        // Set message retry if the message processing fails
        rabbit.UseMessageRetry(x => x.Interval(2, 1000));

        rabbit.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Register a receive endpoint
        rabbit.ReceiveEndpoint("games-ranking-queue", endpoint =>
        {
            // Sets the consumer to be used for the endpoint
            endpoint.Consumer<UpdateGameRankConsumer>(context);
        });
    });

    // x.UsingAmazonSqs((context, aws) =>
    // {
    //     aws.UseMessageRetry(x => x.Interval(2, 1000));

    //     aws.Host("eu-central-1", h =>
    //     {
    //         h.AccessKey("ACCESS_KEY");
    //         h.SecretKey("SECRET_KEY");
    //     });

    //     aws.ReceiveEndpoint("games-ranking-queue", endpoint =>
    //     {
    //         endpoint.Consumer<RankingGamesConsumer>(context);
    //     });
    // });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGamesEndpoints();

app.Run();