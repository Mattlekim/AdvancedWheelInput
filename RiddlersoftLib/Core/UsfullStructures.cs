using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Riddlersoft.Core
{
    //now in xna 3d is represented by x horizonatal y height and z depth therefore I have created a new point sytem

    /// <summary>
    /// this is very simular to an xna Point but has z instead of y
    /// </summary>
    public struct Point2D
    {
        public int X, Z;

        /// <summary>
        /// Creates a new Point2D Class
        /// </summary>
        /// <param name="x">the intital x value</param>
        /// <param name="z">the intital y value</param>
        public Point2D(int x, int z)
        {
            X = x;
            Z = z;
        }

        /// <summary>
        /// a Point2D with x and y set to 0
        /// </summary>
        public static Point2D Zero = new Point2D();


        public static Point2D Error = new Point2D(int.MinValue, int.MinValue);

        public static Point2D P16 = new Point2D(16, 16);
        public static int DistanceSquared(Point2D p1, Point2D p2)
        {
            return (p2.X - p1.X) * (p2.X - p1.X) + (p2.Z - p1.Z) * (p2.Z - p1.Z);
        }
    }

    /// <summary>
    /// Simular to a Point3D but x,y and z are int and not floats
    /// </summary>
    public struct Point3D
    {
        public int X, Y, Z;

        //overload the equals operator
        public static bool operator ==(Point3D p1, Point3D p2)
        {
            if (p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z)
                return true;
            return false;
        }

        //overload the not equeal operator
        public static bool operator !=(Point3D p1, Point3D p2)
        {
            if (p1.X != p2.X || p1.Y != p2.Y || p1.Z != p2.Z)
                return true;
            return false;
        }
        /// <summary>
        /// creates a new Point 3d instance
        /// </summary>
        /// <param name="x">initial x value</param>
        /// <param name="y">initial y value</param>
        /// <param name="z">initial z value</param>
        public Point3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// retuns a vector3 of the equivlent
        /// </summary>
        /// <returns></returns>
        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }



        /// <summary>
        /// convets a vector3 to a point3D
        /// </summary>
        /// <param name="input">the input vector3</param>
        /// <returns></returns>
        public static Point3D FromVector3(Vector3 input)
        {
            return new Point3D(Convert.ToInt32(input.X), Convert.ToInt32(input.Y), Convert.ToInt32(input.Z));
        }
        /// <summary>
        /// retruns a Point3d with x,y and z set to 0
        /// </summary>
        public static Point3D Zero = new Point3D();

        public static Point3D Error = new Point3D(int.MaxValue, int.MinValue, int.MinValue);

        public static Point3D operator +(Point3D p1, Point3D p2)
        {
            return new Point3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }
    }

    public struct VectorXZ
    {
        public float X, Z;

        public VectorXZ(float x, float z)
        {
            X = x;
            Z = z;
        }

        public static VectorXZ Zero = new VectorXZ();

    }

    /// <summary>
    /// contiains a min and max
    /// </summary>
    public struct Variation
    {
        /// <summary>
        /// the min and max variables
        /// </summary>
        public int Min, Max;

        public int Attempts {  get { return Min; } }
        public int Proberbility {  get { return Max; } }

        /// <summary>
        /// creates a new variation
        /// </summary>
        /// <param name="min">the min value</param>
        /// <param name="max">the max value</param>
        public Variation(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }

    public struct Point4
    {
        public int X, Y, Z, W;

        public Point4(int value)
        {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }

        public Point4(int x, int y, int z, int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }

    /// <summary>
    /// contains rotation data for 3 axis
    /// </summary>
    public struct Rotation3
    {
        /// <summary>
        /// a rotation variable
        /// </summary>
        public float Yaw, Pitch, Roll;

        /// <summary>
        /// intializes all rotaions at the given value
        /// </summary>
        /// <param name="value">the value to set all rotations to</param>
        public Rotation3(float value)
        {
            Yaw = value;
            Pitch = value;
            Roll = value;
        }

        /// <summary>
        /// intitalizes all rotations at the given valuse
        /// </summary>
        /// <param name="yaw">yaw rotation</param>
        /// <param name="pitch">pitch rotation</param>
        /// <param name="roll">roll rotation</param>
        public Rotation3(float yaw, float pitch, float roll)
        {
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
        }

        public static Rotation3 Lerp(Rotation3 v1, Rotation3 v2, float amount)
        {
            if (amount < 0)
                return v1;
            if (amount > 1)
                return v2;
            return new Rotation3(v1.Yaw + (v2.Yaw - v1.Yaw) * amount,
                v1.Pitch + (v2.Pitch - v1.Pitch) * amount,
                v1.Roll + (v2.Roll - v1.Roll) * amount);
        }

        public static Rotation3 Zero = new Rotation3(0);
        public static Rotation3 Back = new Rotation3(MathHelper.Pi, 0, 0);

        public static Rotation3 Bottom = new Rotation3(0, MathHelper.PiOver2, 0);
        public static Rotation3 Top = new Rotation3(0, -MathHelper.PiOver2, 0);
        public static Rotation3 Right = new Rotation3(MathHelper.PiOver2, 0, 0);
        public static Rotation3 Left = new Rotation3(-MathHelper.PiOver2, 0, 0);
    }

}
