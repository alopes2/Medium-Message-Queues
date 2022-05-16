using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ranking.Api.Consumers;
using Ranking.Api.Data;
using Rankings.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<RankingDbContext>(config => config.UseInMemoryDatabase("Ranking"));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GameDeletedConsumer>();
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, rabbit) =>
    {
        rabbit.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        rabbit.ReceiveEndpoint("game-deletion-queue", e =>
        {
            e.ConfigureConsumer<GameDeletedConsumer>(context);
        });
    });

    // config.UsingAmazonSqs((context, aws) =>
    // {
    //     aws.UseMessageRetry(x => x.Interval(2, 1000));
    
    //     aws.Host("eu-central-1", h =>
    //     {
    //         h.AccessKey("ACCESS_KEY");
    //         h.SecretKey("SECRET_KEY");
    //     });

    //     aws.ReceiveEndpoint("games-ranking-queue", e =>
    //     {
    //         e.ConfigureConsumer<DeleteGameConsumer>(context);
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

app.MapRankingsEndpoints();

app.Run();