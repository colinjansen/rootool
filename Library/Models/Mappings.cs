using System.Collections.Generic;

namespace Replication.RooTool.Library.Models
{
    public class Mapping
    {
        public int Offset { get; set; }
    }
    public class TokenMapping : Mapping
    {
        public List<char> Seperators { get; set; }
    }
    public class PhotoMapping : TokenMapping
    {
        public bool Extension { get; set; }
    }
    public class NoteMapping : Mapping
    { 
        public string Prefix { get; set; }
        public string Suffix { get; set; }
    }
    public class Mappings
    {
        public Mapping Operator { get; set; }
        public Mapping Disposition { get; set; }
        public Mapping Criteria { get; set; }
        public PhotoMapping ASEAerialPhotos { get; set; }
        public Mapping Latitude { get; set; }
        public Mapping Longitude { get; set; }
        public Mapping LandscapeComments { get; set; }
        public List<NoteMapping> VegetationComments { get; set; }
        public Mapping LevelOfDistruption { get; set; }
        public Mapping SoilZone { get; set; }
        public Mapping SubRegion { get; set; }
        public Mapping EcoSite { get; set; }
        public Mapping Regeneration { get; set; }
        public TokenMapping LegalLocation { get; set; }
        public List<NoteMapping> Notes { get; set; }
    }
}
