using Microsoft.EntityFrameworkCore;
using WebTest.Data;
using WebTest.Dtos.Game;
using WebTest.Interfaces;
using WebTest.Interfaces.Repository;
using WebTest.Models;

namespace WebTest.Repository;

public class GameRepository(ApplicationDbContext context) : IGameRepository
{
    public async Task<List<Game>> GetGamesByIdAsync(string userId)
    {
        return await context.Game
            .Where(g => g.PlayerBlackId == userId || g.PlayerWhiteId == userId) 
            .OrderByDescending(g => g.StartTime)
            .ToListAsync();
    }
    

    public async Task<bool> AddGame(Game game)
    {

        game.Id = context.Game.Count() + 1;
        
        var i = await context.Game.AddAsync(game);
        
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<Game> FindGameForIdPlayers(string a, string y)
    {
        var game = context.Game.FirstOrDefault(x => x.PlayerWhiteId == a && x.PlayerBlackId == y);

        if (game == null)
        {
            return null;   
        }
        
        return game;
    }
    
    public async Task<Game> FindGameForIdPlayersElse(string a, string y)
    {
        var game = context.Game.FirstOrDefault(x => x.PlayerWhiteId == a && x.PlayerBlackId == y && x.WinnerId == null);

        if (game == null)
        {
            return null;   
        }
        
        return game;
    }
    
    public async Task<Game> FindGameForIdPlayers(string a)
    {
        var game = context.Game.FirstOrDefault(x =>
            x.PlayerWhiteId == a && 
            x.PlayerBlackId != "e1856c00-222b-4857-b1ce-c303945b9967" &&
            x.WinnerId == null);

        if (game == null)
        {
            return null!;   
        }
        
        return game;
    }
    
    public async Task<Game?> FindGameForSearch(Game g)
    {
        var game = context.Game.FirstOrDefault(x => 
            x.BoardSize == g.BoardSize && 
            x.Type == g.Type && 
            x.PlayerBlackId == "e1856c00-222b-4857-b1ce-c303945b9967");
        
        return game;
    }

    public async Task<Game?> FindCreateGameForSearch(string a)
    {
        var game = context.Game.FirstOrDefault(x => (x.PlayerBlackId == a || x.PlayerWhiteId == a) && x.WinnerId == null);
        
        return game;
    }
    
    public async Task<bool> UpdateGame(Game game)
    {
        if (game == null)
        {
            return false;
        }
        context.Game.Update(game);
        
        return await context.SaveChangesAsync() > 0;
    }
    
    public async Task<bool> DeleteGame(Game game)
    {
        if (game == null)
        {
            return false;
        }
        context.Game.Remove(game);
        
        return await context.SaveChangesAsync() > 0;
    }
    
    
    
}