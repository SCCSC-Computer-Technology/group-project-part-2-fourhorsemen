﻿//test note
namespace fourHorsemen_Online_Video_Game_Database.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public string System { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int NumberOfPlayers { get; set; }
        public long Sales { get; set; }

        //public string Description { get; set; }
        //public string Platform { get; set; }
        //public string CoverImageUrl { get; set; }
    }

}
