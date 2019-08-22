using System;
using System.Collections.Generic;

namespace LocalSportsLeague.Models
{
    public partial class Team
    {
        public Team()
        {
            GameAteam = new HashSet<Game>();
            GameHteam = new HashSet<Game>();
            GameWinnerNavigation = new HashSet<Game>();
            SeasonTeam = new HashSet<SeasonTeam>();
        }

        public int Teamid { get; set; }
        public int? Coachid { get; set; }
        public int? Sportid { get; set; }
        public string Name { get; set; }

        public virtual Coach Coach { get; set; }
        public virtual Sport Sport { get; set; }
        public virtual ICollection<Game> GameAteam { get; set; }
        public virtual ICollection<Game> GameHteam { get; set; }
        public virtual ICollection<Game> GameWinnerNavigation { get; set; }
        public virtual ICollection<SeasonTeam> SeasonTeam { get; set; }
    }
}
