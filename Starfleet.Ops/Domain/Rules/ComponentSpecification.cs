using System.Collections.Generic;

namespace Starfleet.Ops.Utility
{
    public class ComponentSpecificationFile
    {
        public List<string> Weapons { get; set; }
        public List<string> Hulls { get; set; }
        public List<string> Engines { get; set; }
        public List<string> Specials { get; set; }
        //"Weapons": ["Phaser","Drone","Torp"],
        //"Hull": [ "A Hull", "F Hull" ],
        //"Engines": [ "Left W En", "Right W En","Impulse" ],
        //"Specials": ["Lab","APR","Shuttle","Cargo","Emer Bridge","Aux Control","Scanner","Trans"]
    }

    public class DamageAllocationChartFile : Dictionary<int, List<string>>
    {
        
    }

    public class DamageAllocationChart
    {
        public Dictionary<int, DamageAllocationTrack> Tracks { get; set; } = new Dictionary<int, DamageAllocationTrack>();
    }

    public class DamageAllocationTrack
    {
        public List<DamageAllocationTrackUnit> Units { get; set; } = new List<DamageAllocationTrackUnit>();
    }

    public class DamageAllocationTrackUnit
    {
        public string Code { get; set; }
        public bool OncePerVolley { get; set; }
        public string Id { get; set; }
    }

    public class ComponentSpecification
    {
        public string Code { get; set; }

        public string ComponentType { get; set; }
    }

    public struct KnownComponentTypes
    {
        public const string Weapon = "Weapon";
        public const string Hull = "Hull";
        public const string Special = "Special";
        public const string Engines = "Engine";
        public const string ExcessDamage = "ExcessDamage";
    }
}