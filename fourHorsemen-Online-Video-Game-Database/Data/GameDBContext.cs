using fourHorsemen_Online_Video_Game_Database.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;



namespace fourHorsemen_Online_Video_Game_Database.Data
{
    public class GameDBContext : DbContext
    {
        public GameDBContext(DbContextOptions<GameDBContext> options) 
            : base(options) 
        { 
        
        }

        public DbSet<Game> Games { get; set; }
    }
}
