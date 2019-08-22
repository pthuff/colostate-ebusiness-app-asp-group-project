using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace LocalSportsLeague.Models
{
    public partial class Season
    {
        public Season()
        {
            Game = new HashSet<Game>();
            SeasonSport = new HashSet<SeasonSport>();
            SeasonTeam = new HashSet<SeasonTeam>();
        }

        public int Seasonid { get; set; }
        public string Name { get; set; }
        public DateTime Sdate { get; set; }
        public DateTime? Edate { get; set; }

        public virtual ICollection<Game> Game { get; set; }
        public virtual ICollection<SeasonSport> SeasonSport { get; set; }
        public virtual ICollection<SeasonTeam> SeasonTeam { get; set; }
    }
}
