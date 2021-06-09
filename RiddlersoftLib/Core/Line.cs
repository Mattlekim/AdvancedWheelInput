using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Riddlersoft.Core
{
    public struct Line
    {
        public Vector2 Start, End;
        public Line(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }

        public Vector2? Intersects(Rectangle rectangle)
        {
            Vector2 s, e;
            s = new Vector2(rectangle.X, rectangle.Y);
            e = new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
            Vector2? r = Intersects(s, new Vector2(e.X, s.Y), Start, End);
            if (r== null)
                r = Intersects(s, new Vector2(s.X, e.Y), Start, End);
            if (r == null)
                r = Intersects(new Vector2(e.X, s.Y), new Vector2(e.X, e.Y), Start, End);
            if (r == null)
                r = Intersects(new Vector2(s.X, s.Y), new Vector2(e.X, e.Y), Start, End);
            return r;
        }

        public Vector2? Intersects(Line l1)
        {
            return Intersects(l1.Start, l1.End, Start, End);
        }

        public static Vector2? Intersects(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            // Get the segments' parameters.
            float dx12 = p2.X - p1.X;
            float dy12 = p2.Y - p1.Y;
            float dx34 = p4.X - p3.X;
            float dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 =
                ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                    / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                return null;
            }
            float t2 =
                ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
                    / -denominator;

            // The segments intersect if t1 and t2 are between 0 and 1.
            if ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1))
                return new Vector2(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            return null;
        }
    }
}
