using Microsoft.AspNetCore.Identity;
using System;

namespace fourHorsemen_Online_Video_Game_Database.Models
{
    public class SiteUser : IdentityUser
    {
        public DateTime JoinDate { get; set; }

        // add anything else you want on the profile later (e.g., Bio, AvatarURL, etc)
    }
}