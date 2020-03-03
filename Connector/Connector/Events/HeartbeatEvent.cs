using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Events
{
    /// <summary>
    /// The occurrence of the HeartbeatEvent indicates a finished tick of the universe. You should see this event exactly 20 times the second in average.
    /// </summary>
    /// <remarks>
    /// Events in general have a secured order of occurrence. However, if your user code is too slow to parse those events or to gather them fast enough those events may have hyper updated data.
    /// 
    /// An example of hyper updated data: It may happen that more than one tick is waiting if your code backloggs because it can't parse those events fast enough. If this happens, then events may
    /// reference players, etc. which may already have more recent data as when you really were in that tick you currently think you have.
    /// </remarks>
    public class HeartbeatEvent : FlattiverseEvent
    {
        internal readonly static HeartbeatEvent Event = new HeartbeatEvent();

        internal HeartbeatEvent()
        {
        }

        /// <summary>
        /// This event as string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return "[HEARTBEAT]";
        }
        public override FlattiverseEventKind Kind => FlattiverseEventKind.Heartbeat;
    }
}
