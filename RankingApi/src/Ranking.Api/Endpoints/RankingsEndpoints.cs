using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ranking.Api.Data;
using Ranking.Api.Data.Entities;
using Ranking.Api.Resources;
using BusContracts.Messages;

namespace Rankings.Api.Endpoints;

public static class RankingsEndpoints
{
    public static WebApplication MapRankingsEndpoints(this WebApplication app)
    {
        app.MapGet("/rankings", async(RankingDbContext dbContext) =>
        {
            var rankResources = await dbContext.Ranks
                .Select(rank => new RankResource(
                    rank.Id.ToString(),
                    rank.GameId.ToString(),
                    rank.Value
                ))
                .ToListAsync();

            return Results.Ok(rankResources);
        });

        // GET /Rankings/{id}
        app.MapGet("/Rankings/{id}", async(
            RankingDbContext dbContext,
            string id) =>
        {
            var rank = await dbContext.Ranks.FirstOrDefaultAsync(g => g.Id.ToString() == id);

            if (rank is null)
            {
                return Results.NotFound();
            }

            var rankResource = new RankResource(
                Id: rank.Id.ToString(),
                GameId: rank.GameId.ToString(),
                Rank: rank.Value
            );

            return Results.Ok(rankResource);
        });

        // POST /Rankings
        app.MapPost("/rankings", async(
            RankingDbContext dbContext,
            ISendEndpointProvider sendEndpointProvider,
            [FromBody] SaveRankResource saveRankResource) =>
        {
            var newRank = new Rank
            {
                GameId = Convert.ToInt32(saveRankResource.GameId),
                Value = saveRankResource.Rank
            };

            await dbContext.Ranks.AddAsync(newRank);
            await dbContext.SaveChangesAsync();

            decimal ranksSum = await dbContext.Ranks.Where(r => r.GameId == newRank.GameId).SumAsync(r => r.Value);
            decimal ranksCount = await dbContext.Ranks.CountAsync(r => r.GameId == newRank.GameId);
            var updatedRank = ranksSum / ranksCount;

            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:games-ranking-queue"));
            await endpoint.Send<UpdateRank>(new 
            {
                GameId = newRank.GameId,
                UpdatedRank = updatedRank
            });

            return Results.Ok(newRank);
        });

        // PUT /Rankings/{id}
        app.MapPut("rankings/{id}", async(
            RankingDbContext dbContext, [FromBody] SaveRankResource saveRankResource, [FromRoute] int id) =>
        {
            var rank = await dbContext.Ranks
                .FirstOrDefaultAsync(g => g.Id == id && g.GameId.ToString() == saveRankResource.GameId);

            if (rank is null)
            {
                return Results.NotFound();
            }

            rank.Value = saveRankResource.Rank;

            await dbContext.SaveChangesAsync();

            var rankResource = new RankResource(
                Id: rank.Id.ToString(),
                GameId: rank.GameId.ToString(),
                Rank: rank.Value
            );

            return Results.Ok(rankResource);
        });

        return app;
    }
}