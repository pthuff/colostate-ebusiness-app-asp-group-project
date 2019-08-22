using System;
using System.Collections.Generic;

namespace LocalSportsLeague.Models
{
    public partial class SeasonSport
    {
        public int SeasonSportId { get; set; }
        public int Seasonid { get; set; }
        public int Sportid { get; set; }

        public virtual Season Season { get; set; }
        public virtual Sport Sport { get; set; }
    }
}
