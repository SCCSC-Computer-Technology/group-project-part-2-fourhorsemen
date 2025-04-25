namespace fourHorsemen_Online_Video_Game_Database.Controllers
{
    public class FunFactController
    {
        public string Category { get; set; }
        public string Game { get; set; }
        public string Fact { get; set; }

        public FunFactController(string category, string game, string fact)
        {
            Category = category;
            Game = game;
            Fact = fact;
        }
    }
}
