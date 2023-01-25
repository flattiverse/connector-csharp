using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Events
{
    public class TickCompleteEvent : FlattiverseEvent
    {
        public readonly int Tick;

        internal TickCompleteEvent(int tick)
        {
            Tick = tick;
        }   
    }
}
