using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
namespace Riddlersoft.Core
{
    public static class MathFunctions
    {

        public static Rectangle Vector2ToRectangle(Vector2 pos, int size)
        {
            return new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), size, size);
        }

        /// <summary>
        /// floors the current input
        /// </summary>
        /// <param name="input">input vector2</param>
        /// <returns></returns>
        public static Vector2 Floor(Vector2 input)
        {
            return new Vector2((float)Math.Floor(input.X), (float)Math.Floor(input.Y));
        }

        /// <summary>
        /// snapes the given cordents to a given grid size
        /// </summary>
        /// <param name="cords">the input cordents</param>
        /// <param name="gridSize">the current grid size</param>
        /// <returns></returns>
        public static Vector2 SnapToGrid(Vector2 cords, int gridSize)
        {
            cords.X = cords.X / (float)gridSize;
            cords.Y = cords.Y / (float)gridSize;

            cords = Floor(cords);

            cords.X = cords.X * gridSize;
            cords.Y = cords.Y * gridSize;
            return cords;
        }

        /// <summary>
        /// returns the inverted direction
        /// </summary>
        /// <param name="source">the direction to invert</param>
        /// <returns></returns>
        public static CardinalDirection GetCardinalInverse(CardinalDirection source)
        {
            switch (source)
            {
                case CardinalDirection.Up:
                    return CardinalDirection.Down;
                case CardinalDirection.Down:
                    return CardinalDirection.Up;
                case CardinalDirection.Left:
                    return CardinalDirection.Right;
            }
            return CardinalDirection.Left;
        }

        /// <summary>
        /// returns true if the cardianal direction is vertical
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsVerticalDirection(CardinalDirection source)
        {
            if (source == CardinalDirection.Up || source == CardinalDirection.Down)
                return true;
            return false;
        }

        public static float CardinalDirectionToRadin(CardinalDirection source)
        {
            return (float)((int)source) * MathHelper.PiOver2;
        }

        public static Rectangle RotateRectangle(Rectangle source, float angle, Vector2 origin)
        {
            Vector2[] points = new Vector2[4]; //we need all points for the rectangle
            //p[0] starts top left and points go clockwise
            points[0] = new Vector2(source.X, source.Y);
            points[1] = new Vector2(source.X + source.Width, source.Y);
            points[2] = new Vector2(source.X + source.Width, source.Y + source.Height);
            points[3] = new Vector2(source.X, source.Y + source.Height);

            //now calculate the points based on the origin
            for (int i = 0; i < 4; i++)
                points[i] = points[i] - origin;

            //now rotate each point
            for (int i = 0; i < 4; i++)
                points[i] = Vector2.Transform(points[i], Matrix.CreateRotationZ(angle));

            //find the min and max points
            float maxx = float.NegativeInfinity, maxy = float.NegativeInfinity;
            float minx = float.PositiveInfinity, miny = float.PositiveInfinity;

            for (int i = 0; i < 4; i++)
            {
                if (points[i].X > maxx)
                    maxx = points[i].X;

                if (points[i].Y > maxy)
                    maxy = points[i].Y;

                if (points[i].X < minx)
                    minx = points[i].X;

                if (points[i].Y < miny)
                    miny = points[i].Y;
            }

            //create the rectangle
            Rectangle tmp = new Rectangle(Convert.ToInt32(minx), Convert.ToInt32(miny), Convert.ToInt32(maxx - minx), Convert.ToInt32(maxy - miny));
            return tmp;
        }

        public static Vector2? FindIntersection(Line l1, Line l2)
        {
            return FindIntersection(l1.Start, l1.End, l2.Start, l2.End);
        }

        public static Vector2? FindIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
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

        public static Rectangle Shrink(Rectangle source, int amountInPx)
        {
            return new Rectangle(source.X + amountInPx, source.Y + amountInPx, source.Width - amountInPx * 2, source.Height - amountInPx * 2);
        }
    }
}



