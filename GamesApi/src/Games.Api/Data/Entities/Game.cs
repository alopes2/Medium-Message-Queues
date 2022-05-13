namespace Games.Api.Data.Entities;

public class Game
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Ranking { get; set; } = 0m;
}
