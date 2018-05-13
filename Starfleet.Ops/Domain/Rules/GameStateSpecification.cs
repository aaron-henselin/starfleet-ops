using System.Collections.Generic;

namespace Starfleet.Ops.Utility
{
    public class GameStateSpecification
    {
        public List<ShipSpecification> Ships { get; set; } = new List<ShipSpecification>();
    }
}