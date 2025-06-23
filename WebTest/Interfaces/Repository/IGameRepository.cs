using WebTest.Models;

namespace WebTest.Interfaces.Repository;

public interface IGameRepository
{
    Task<List<Game>> GetGamesByIdAsync(string userId);
    Task<Game?> FindGameForSearch(Game g);
    Task<Game> FindGameForIdPlayers(string x);
    Task<Game> FindGameForIdPlayers(string a, string y);
    Task<Game?> FindCreateGameForSearch(string a);
    Task<Game> FindGameForIdPlayersElse(string a, string y);
    Task<bool> AddGame(Game game);
    Task<bool> DeleteGame(Game game);
    Task<bool> UpdateGame(Game game);
    
    
}