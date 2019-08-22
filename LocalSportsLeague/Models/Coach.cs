using System;
using System.Collections.Generic;

namespace LocalSportsLeague.Models
{
    public partial class Coach
    {
        public Coach()
        {
            Team = new HashSet<Team>();
        }

        public int Coachid { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }

        public virtual ICollection<Team> Team { get; set; }
    }
}
