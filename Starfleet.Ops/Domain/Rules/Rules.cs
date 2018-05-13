using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Starfleet.Ops.Utility;

namespace Starfleet.Ops.Domain.Rules
{
    public static class GameRules
    {
        private static IEnumerable<ShipSpecification> _ships;
        private static IReadOnlyCollection<ComponentSpecification> _components;
        private static DamageAllocationChart _damageAllocationChart;

        public static DamageAllocationTrack GetDamageAllocationTrack(int trackNumber)
        {
            var allComponents = GetComponents().ToList();
            
            if (_damageAllocationChart == null)
            {
                var file =
                    new JsonDataReader().ReadSingleSpecification<DamageAllocationChartFile>(
                        AppEnvironment.Current.AppData + "\\Rules\\DamageAllocationChart.json");

                _damageAllocationChart = new DamageAllocationChart();
                foreach (var kvp in file)
                {
                    var track = new DamageAllocationTrack();
                    _damageAllocationChart.Tracks.Add(kvp.Key, track);
                    foreach (var trackUnitCode in kvp.Value)
                    {
                        var code = trackUnitCode;
                        var oncePerVolley = trackUnitCode.StartsWith("_");
                        if (oncePerVolley)
                            code = trackUnitCode.Substring(1);

                        var isAny = trackUnitCode.StartsWith("*");
                        if (isAny)
                            code = trackUnitCode.Substring(1);

                        if (isAny)
                        {
                            foreach (var cmp in _components)
                            {
                                if (cmp.ComponentType == trackUnitCode)
                                {
                                    var unit = new DamageAllocationTrackUnit
                                    {
                                        Id = $"{kvp.Key}-{code}",
                                        Code = code,
                                        OncePerVolley = oncePerVolley,
                                    };
                                    track.Units.Add(unit);
                                }
                            }
                        }
                        else
                        {
                            var unit = new DamageAllocationTrackUnit
                            {
                                Id = $"{kvp.Key}-{code}",
                                Code = code,
                                OncePerVolley = oncePerVolley,
                            };
                            track.Units.Add(unit);
                        }

                    }
                }
            }

            return _damageAllocationChart.Tracks[trackNumber];
        }
    

        public static IEnumerable<ComponentSpecification> GetComponents()
        {
            if (_components == null)
            {
                var file = new JsonDataReader().ReadSingleSpecification<ComponentSpecificationFile>(
                    AppEnvironment.Current.AppData + "\\Rules\\Components.json");

                var localComponents = new List<ComponentSpecification>();
                foreach (var code in file.Weapons)
                    localComponents.Add(new ComponentSpecification {Code=code,ComponentType = KnownComponentTypes.Weapon});

                foreach (var code in file.Hulls)
                    localComponents.Add(new ComponentSpecification { Code = code, ComponentType = KnownComponentTypes.Hull });

                foreach (var code in file.Specials)
                    localComponents.Add(new ComponentSpecification { Code = code, ComponentType = KnownComponentTypes.Special });

                foreach (var code in file.Engines)
                    localComponents.Add(new ComponentSpecification { Code = code, ComponentType = KnownComponentTypes.Engines });


                _components = localComponents;
            }

            return _components;
        }

        public static IEnumerable<ShipSpecification> GetShips()
        {
            if (_ships == null)
            {
                _ships = new JsonDataReader().ReadAllSpecifications<ShipSpecification>(
                    AppEnvironment.Current.AppData + "\\Rules\\Ships");
            }

            return _ships;
        }

        public static ShipSpecification GetShipByCode(string code)
        {
            var matchingShip = GetShips().SingleOrDefault(x => x.Code == code);
            if (matchingShip == null)
                throw new Exception("Could not find a ship with specification code: "+code);

            return matchingShip;
        }



        public static ComponentSpecification GetComponentByCode(string code)
        {
            var matchingShip = GetComponents().SingleOrDefault(x => x.Code == code);
            if (matchingShip == null)
                throw new Exception("Could not find a component with specification code: " + code);

            return matchingShip;
        }
    }
}

