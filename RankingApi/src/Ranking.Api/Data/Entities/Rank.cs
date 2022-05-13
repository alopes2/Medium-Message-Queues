using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ranking.Api.Data.Entities;

public class Rank
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public int Value { get; set; }
}
