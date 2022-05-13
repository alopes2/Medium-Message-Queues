using Microsoft.EntityFrameworkCore;
using Ranking.Api.Data.Entities;

namespace Ranking.Api.Data;

public class RankingDbContext : DbContext
{
    public DbSet<Rank> Ranks { get; set; }

    public RankingDbContext(DbContextOptions<RankingDbContext> options)
        : base(options)
    {
    }
}
