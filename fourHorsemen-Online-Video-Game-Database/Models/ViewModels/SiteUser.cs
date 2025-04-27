using Microsoft.AspNetCore.Identity;
using System;

namespace fourHorsemen_Online_Video_Game_Database.Models
{
    public class SiteUser : IdentityUser
    {
        public DateTime JoinDate { get; set; }

        public string AvatarUrl { get; set; }

        
    }
}