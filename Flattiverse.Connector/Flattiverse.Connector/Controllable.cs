using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector
{
    // TOG: Schöne, ausführliche KOmmentare schrieben und auch erwähnen welche Exceptions passieren können. Zudem noch remarks verwenden und erklären,w as Leute bei den entsprechenden Kommandos zu erwarten haben.
    //      Diese Zeile stehen lassen, bis Sonntag Abend.
    public class Controllable
    {
        public readonly string Name;

        public readonly int ID;

        internal Controllable(string name, int id)
        {
            Name = name;
            ID = id;
        }
    }
}
