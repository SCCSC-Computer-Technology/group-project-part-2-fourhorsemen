using fourHorsemen_Online_Video_Game_Database.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;




namespace fourHorsemen_Online_Video_Game_Database.Data
{
    public class GameDBContext : IdentityDbContext<SiteUser>
    {
        public GameDBContext(DbContextOptions<GameDBContext> options) 
            : base(options) 
        { 
        
        }

        public DbSet<Game> Games { get; set; }

        public DbSet<UserGame> UserGames { get; set; }
    }
}
