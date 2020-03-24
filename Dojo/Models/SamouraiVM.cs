using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dojo.Models
{
    public class SamouraiVM
    {
        public Samourai Samourai { get; set; }
        public List<Arme> Armes { get; set; }
        public int? SelectedArme { get; set; }
        public List<ArtMartial> ArtMartiaux { get; set; }
        public List<int> SelectedArtMartiaux { get; set; }
    }
}