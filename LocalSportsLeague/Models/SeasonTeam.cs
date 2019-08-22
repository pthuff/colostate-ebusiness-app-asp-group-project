using System;
using System.Collections.Generic;

namespace LocalSportsLeague.Models
{
    public partial class SeasonTeam
    {

        public int SeasonTeamId { get; set; }
        public int Seasonid { get; set; }
        public int Teamid { get; set; }

        public virtual Season Season { get; set; }
        public virtual Team Team { get; set; }
    }
}
