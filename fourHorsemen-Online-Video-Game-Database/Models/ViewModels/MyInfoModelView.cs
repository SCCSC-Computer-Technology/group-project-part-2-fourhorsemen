namespace fourHorsemen_Online_Video_Game_Database.ViewModels
{
    public class MyInfoViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime? JoinDate { get; set; }

        public int FavoritesCount { get; set; }
        public int OwnedCount { get; set; }
        public int WishlistCount { get; set; }
        public int DefeatedCount { get; set; }


    }
}