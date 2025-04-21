namespace fourHorsemen_Online_Video_Game_Database.Models
{
    public class FunFacts
    {
        public string Category { get; set; }
        public string Game { get; set; }
        public string Fact { get; set; }

        public FunFacts(string category, string game, string fact)
        {
            Category = category;
            Game = game;
            Fact = fact;
        }
    }
}
