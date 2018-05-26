using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Starfleet.Ops.Utility;

namespace Starfleet.Ops.Domain.GameState
{
    public class GameStateFactory
    {
        public GameState CreateGameState(GameStateSpecification spec)
        {
            var gs = new GameState();
            foreach (var ship in spec.Ships)
                gs.Pawns.Add(Pawn.FromSpecification(ship));

            return gs;
        }


    }

    public class GameState
    {
        public List<Pawn> Pawns { get; set; } = new List<Pawn>();
    }


    public class Pawn
    {


        public static Pawn FromSpecification(ShipSpecification spec)
        {
            return new Pawn
            {
                Id = Guid.NewGuid(),
                SpecificationCode = spec.Code,
                ComponentsRemaining = spec.Components.ToDictionary(x => x.Key, x => x.Value)
            };
        }

        public Guid Id { get; set; }

        public string SpecificationCode { get; set; }

        public Dictionary<string, int> ComponentsRemaining { get; set; } = new Dictionary<string, int>();
    }
}
