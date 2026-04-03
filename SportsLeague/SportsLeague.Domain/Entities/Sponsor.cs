using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Entities
{
    public class Sponsor : AuditBase

    {
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string Phone { get; set; }
        public string WebsiteUrl { get; set; }
        public SponsorCategory Category { get; set; }
    


        public ICollection<TournamentSponsor> TournamentSponsors { get; set; } = new List<TournamentSponsor>();

    }
}
