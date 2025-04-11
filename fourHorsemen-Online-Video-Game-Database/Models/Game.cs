

namespace fourHorsemen_Online_Video_Game_Database.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public string System { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int NumberOfPlayers { get; set; }
        public long Sales { get; set; }
    }

}
