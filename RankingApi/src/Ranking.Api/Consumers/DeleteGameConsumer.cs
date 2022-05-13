using MassTransit;
using Ranking.Api.Data;
using BusContracts.Events;

namespace Ranking.Api.Consumers;

public class GameDeletedConsumer : IConsumer<GameDeleted>
{
    public readonly RankingDbContext _dbContext;

    public GameDeletedConsumer(RankingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<GameDeleted> context)
    {
        Console.WriteLine("GameDeleted: " + context.Message.GameId);

        var gameId = context.Message.GameId;

        var gameRanks = _dbContext.Ranks.Where(r => r.GameId == gameId);
        _dbContext.Ranks.RemoveRange(gameRanks);

        Console.WriteLine("Ranks for GameId {0} deleted", gameId);

        await _dbContext.SaveChangesAsync();
    }
}