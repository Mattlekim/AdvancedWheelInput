using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Riddlersoft.Core.Input
{
    public struct DragEvent
    {
        public bool Active { get; private set; }
        public Vector2 Delta { get; private set; }
        public Vector2 StartPos { get; private set; }

        public DragEvent(Vector2 delta, Vector2 startpos, bool active)
        {
            Active = active;
            Delta = delta;
            StartPos = startpos;
        }

        public static DragEvent Empty = new DragEvent(Vector2.Zero, Vector2.Zero, false);
    }
}
