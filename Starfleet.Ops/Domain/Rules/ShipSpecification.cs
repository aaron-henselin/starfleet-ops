using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Starfleet.Ops.Utility
{
    public class ShipSpecification
    {
        public string Code { get; set; }
        public string Faction { get; set; }
        public string Name { get; set; }
        public Dictionary<string,int> Components { get; set; } = new Dictionary<string, int>();
    }
}
