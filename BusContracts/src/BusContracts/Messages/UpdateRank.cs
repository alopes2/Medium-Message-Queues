namespace BusContracts.Messages;

public interface UpdateRank
{
    int GameId { get; set; }

    decimal UpdatedRank { get; set; }    
}
