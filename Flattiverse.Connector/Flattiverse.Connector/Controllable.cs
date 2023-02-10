using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Flattiverse.Connector
{
    // TOG: Schöne, ausführliche Kommentare schrieben und auch erwähnen welche Exceptions passieren können. Zudem noch remarks verwenden und erklären,w as Leute bei den entsprechenden Kommandos zu erwarten haben.
    //      Diese Zeile stehen lassen, bis Sonntag Abend.
    public class Controllable
    {
        public readonly UniverseGroup Group;
        public readonly string Name;
        public readonly int ID;

        public readonly double Radius;
        public readonly Vector Position;
        public readonly Vector Movement;
        public readonly double Direction;
        public readonly Team? Team;
        public readonly double Gravity;
        public readonly Mobility Mobility;
        public readonly double EnergyOutput;
        public readonly bool IsAlive;
        public readonly double TurnRate;

        public readonly PlayerUnitSystem Hull;
        public readonly PlayerUnitSystem CellsEnergy;
        public readonly PlayerUnitSystem BatteryEnergy;
        public readonly PlayerUnitSystem Thruster;
        public readonly PlayerUnitSystem Nozzle;
        public readonly PlayerUnitSystem Scanner;
        public readonly PlayerUnitSystem? Armor;
        public readonly PlayerUnitSystem? Shield;
        public readonly PlayerUnitSystem? Analyzer;
        public readonly PlayerUnitSystem? CellsParticles;
        public readonly PlayerUnitSystem? BatteryParticles;
        public readonly PlayerUnitSystem? WeaponLauncher;
        public readonly PlayerUnitSystem? WeaponPayloadDamage;
        public readonly PlayerUnitSystem? WeaponPayloadRadius;
        public readonly PlayerUnitSystem? WeaponFactory;
        public readonly PlayerUnitSystem? WeaponStorage;
        public readonly PlayerUnitSystem? CargoIron;
        public readonly PlayerUnitSystem? CargoCarbon;
        public readonly PlayerUnitSystem? CargoSilicon;
        public readonly PlayerUnitSystem? CargoPlatinum;
        public readonly PlayerUnitSystem? CargoGold;
        public readonly PlayerUnitSystem? CargoSpecial;
        public readonly PlayerUnitSystem? ExtractorIron;
        public readonly PlayerUnitSystem? ExtractorCarbon;
        public readonly PlayerUnitSystem? ExtractorSilicon;
        public readonly PlayerUnitSystem? ExtractorPlatinum;
        public readonly PlayerUnitSystem? ExtractorGold;

        internal Controllable(UniverseGroup group, string name, int id)
        {
            Group = group;
            Name = name;
            ID = id;
        }

        public async Task Continue()
        {
            if (Hull.Value > 0.0)
                throw new GameException(0x20);

            using (Query query = Group.connection.Query("controllableContinue"))
            {
                query.Write("controllable", ID);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }

        public async Task Kill()
        {
            if (Hull.Value <= 0.0)
                throw new GameException(0x22);

            using (Query query = Group.connection.Query("controllableKill"))
            {
                query.Write("controllable", ID);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }
    }
}
