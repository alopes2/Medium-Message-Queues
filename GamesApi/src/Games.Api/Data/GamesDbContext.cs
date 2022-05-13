using Games.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Games.Api.Data;

public class GamesDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }

    public GamesDbContext(DbContextOptions<GamesDbContext> options)
        : base(options)
    {
    }
}
