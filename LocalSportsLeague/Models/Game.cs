using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LocalSportsLeague.Models
{
    public partial class Game
    {
        public int Gameid { get; set; }

        [Required (ErrorMessage = "Please select a home team")]
        public int? Hteamid { get; set; }

        [Required(ErrorMessage = "Please select an away team")]
        public int? Ateamid { get; set; }

        public int? Sportid { get; set; }

        public int? Seasonid { get; set; }

        public int? Officialid { get; set; }

        public int? Winner { get; set; }

        [Required(ErrorMessage = "Please enter a home score")]
        //[MaxLength(3)]
        //[Range(0,999,ErrorMessage = "Please select a number between 0 and 999")]
        public int Hscore { get; set; }

        [Required(ErrorMessage = "Please enter an away score")]
        //[MaxLength(3)]
        //[Range(0, 999, ErrorMessage = "Please select a number between 0 and 999")]
        public int Ascore { get; set; }

        public bool Ot { get; set; }

        [Required(ErrorMessage = "Please select a date and time")]
        public DateTime Datetime { get; set; }

        public virtual Team Ateam { get; set; }
        public virtual Team Hteam { get; set; }
        public virtual Official Official { get; set; }
        public virtual Season Season { get; set; }
        public virtual Team WinnerNavigation { get; set; }
        public virtual Sport SportNavigation { get; set; }
    }
}
