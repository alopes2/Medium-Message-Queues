using BusContracts.Events;
using Games.Api.Data;
using Games.Api.Data.Entities;
using Games.Api.Resources;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Games.Api.Endpoints;

public static class GamesEndpoints
{
    public static WebApplication MapGamesEndpoints(this WebApplication app)
    {
        app.MapGet("/games", async(GamesDbContext dbContext) =>
        {
            var gameResources = await dbContext.Games
                .Select(game => new GameResource(game.Id.ToString(), game.Name, game.Description, game.Ranking))
                .ToListAsync();

            return Results.Ok(gameResources);
        });

        // GET /games/{id}
        app.MapGet("/games/{id}", async(
            GamesDbContext dbContext,
            string id) =>
        {
            var game = await dbContext.Games.FirstOrDefaultAsync(g => g.Id.ToString() == id);

            if (game is null)
            {
                return Results.NotFound();
            }

            var gameResource = new GameResource(
                Id: game.Id.ToString(),
                Name: game.Name,
                Description: game.Description,
                Ranking: game.Ranking
            );

            return Results.Ok(gameResource);
        });

        // POST /games
        app.MapPost("/games", async(
            GamesDbContext dbContext,
            [FromBody] SaveGameResource saveGameResource) =>
        {
            var game = new Game
            {
                Name = saveGameResource.Name,
                Description = saveGameResource.Description
            };

            await dbContext.Games.AddAsync(game);
            await dbContext.SaveChangesAsync();

            return Results.Ok(game);
        });

        // PUT /games/{id}
        app.MapPut("games/{id}", async(
            GamesDbContext dbContext, [FromBody] SaveGameResource saveGameResource, [FromRoute] string id) =>
        {
            var game = await dbContext.Games.FirstOrDefaultAsync(g => g.Id.ToString() == id);

            if (game is null)
            {
                return Results.NotFound();
            }

            game.Name = saveGameResource.Name;
            game.Description = saveGameResource.Description;

            await dbContext.SaveChangesAsync();

            var gameResource = new GameResource(
                Id: game.Id.ToString(),
                Name: game.Name,
                Description: game.Description,
                Ranking: game.Ranking
            );

            return Results.Ok(gameResource);
        });

        // DELETE /games/{id}
        app.MapDelete("games/{id}", async(
            GamesDbContext dbContext,
            IPublishEndpoint publishEndpoint,
            [FromRoute] string id) =>
        {
            var game = await dbContext.Games.FirstOrDefaultAsync(g => g.Id.ToString() == id);

            if (game is null)
            {
                return Results.NotFound();
            }

            dbContext.Games.Remove(game);
            await dbContext.SaveChangesAsync();

            // Publishes GameDeleted event
            await publishEndpoint.Publish<GameDeleted>(new { GameId = game.Id });

            return Results.NoContent();
        });

        return app;
    }
}