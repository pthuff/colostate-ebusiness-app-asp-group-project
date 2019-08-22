using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

namespace LocalSportsLeague.Models
{
    public partial class Official
    {
        public Official()
        {
            Game = new HashSet<Game>();
        }

        public int Officialid { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }

        [Required(ErrorMessage = "Please enter an email address")]
        [RegularExpression(@"^[A-Za-z0-9]+\@[A-Za-z].[A-Za-z]$", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [RegularExpression(@"^[A-Za-z0-9\!\#]+$", ErrorMessage = "Letters, numbers, ! or # only please")]
        [UIHint("password")]
        public string Password { get; set; }

        public virtual ICollection<Game> Game { get; set; }
    }
}
