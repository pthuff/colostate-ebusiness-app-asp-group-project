using System;
using System.Collections.Generic;

namespace LocalSportsLeague.Models
{
    public partial class Sport
    {
        public Sport()
        {
            SeasonSport = new HashSet<SeasonSport>();
            Team = new HashSet<Team>();
        }

        public int Sportid { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SeasonSport> SeasonSport { get; set; }
        public virtual ICollection<Team> Team { get; set; }
    }
}
