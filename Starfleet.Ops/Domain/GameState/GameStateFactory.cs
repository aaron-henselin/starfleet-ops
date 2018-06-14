using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Starfleet.Ops.Infrastructure;
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
    
    [Table("Game")]
    public class GameState : CosmosEntity
    {
        [JsonIgnore]
        public List<Pawn> Pawns { get; set; } = new List<Pawn>();

        [JsonIgnore]
        public List<Fleet> Fleets { get; set; } = new List<Fleet>();


        public string Name { get; set; }
    }

    [Table("Fleet")]
    public class Fleet : CosmosEntity
    {
        public string Name { get; set; }

        public Guid GameStateId { get; set; }

    }


    [Table("Pawn")]
    public class Pawn : CosmosEntity
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


        public Guid GameStateId { get; set; }
        public Guid FleetId { get; set; }

        public string SpecificationCode { get; set; }

        [ComplexData]
        public Dictionary<string, int> ComponentsRemaining { get; set; } = new Dictionary<string, int>();

        [ComplexData]
        public List<string> BattleLog { get; set; } = new List<string>();
        public string Name { get; set; }
    }
}
