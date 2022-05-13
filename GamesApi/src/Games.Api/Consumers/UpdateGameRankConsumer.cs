using BusContracts.Messages;
using Games.Api.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Games.Api.Consumers;

public class UpdateGameRankConsumer : IConsumer<UpdateRank>
{
    private readonly GamesDbContext _dbContext;

    public UpdateGameRankConsumer(GamesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<UpdateRank> context)
    {
        Console.WriteLine($"Received game ranked event for game {context.Message.GameId}");

        var gameRanked = context.Message;
        var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.Id == gameRanked.GameId);

        if (game is null)
        {
            Console.WriteLine($"No game found");
            return;
        }

        game.Ranking = gameRanked.UpdatedRank;

        await _dbContext.SaveChangesAsync();

        Console.WriteLine($"Game {game.Name} has been ranked {game.Ranking}");
    }
}