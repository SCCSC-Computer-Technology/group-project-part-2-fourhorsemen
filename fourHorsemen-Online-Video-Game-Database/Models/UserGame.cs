using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace fourHorsemen_Online_Video_Game_Database.Models
{
    public class UserGame
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public SiteUser User { get; set; }

        public string GameTitle { get; set; }
        public Game Game { get; set; }
        public int GameId { get; set; }

        public GameCategory Category { get; set; }
    }

    public enum GameCategory
    {
        Favorite,
        Owned,
        Wishlist,
        Defeated
    }
}
