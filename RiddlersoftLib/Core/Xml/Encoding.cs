using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Riddlersoft.Core.Xml
{
    public static class RsSerilization
    {
        //#if !SWITCH
//        private static System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-gb");
//#else
        private static System.Globalization.CultureInfo ci = null;
//#endif

        public static int DecodeInt(string value)
        {
            return Convert.ToInt32(value, ci);
        }

        public static uint DecodeUInt(string value)
        {
            return Convert.ToUInt32(value, ci);
        }

        public static long DecodeLong(string value)
        {
            return Convert.ToInt64(value, ci);
        }

        public static ulong DecodeULong(string value)
        {
            return Convert.ToUInt64(value, ci);
        }

        public static byte DecodeByte(string value)
        {
            return Convert.ToByte(value, ci);
        }

        public static float DecodeFloat(string value)
        {
            if (value == null || value.Contains("E"))
            {
                return 0;
            }
            return (float)Convert.ToDouble(value);
        }

        public static double DecodeDouble(string value)
        {
            return (double)Convert.ToDouble(value, ci);
        }

        public static T DecodeEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static Vector2 DecodeVector2(string value)
        {
            if (value == null)
                return Vector2.Zero; //return empty
            float x, y;
            int index = value.IndexOf(" Y");
            string tmp = value.Substring(0, index);
            x = (float)Convert.ToDouble(tmp.Substring(tmp.IndexOf(":") + 1), ci);
            tmp = value.Substring(index + 3);
            y = (float)Convert.ToDouble(tmp.Substring(0, tmp.Length - 1), ci);
            return new Vector2(x, y);
        }

        public static Point DecodePoint(string value)
        {
            if (value == null)
                return new Point(0); //return empty
            int x, y;
            int index = value.IndexOf(" Y");
            string tmp = value.Substring(0, index);
            x = Convert.ToInt32(tmp.Substring(tmp.IndexOf(":") + 1), ci);
            tmp = value.Substring(index + 3);
            y = Convert.ToInt32(tmp.Substring(0, tmp.Length - 1), ci);
            return new Point(x, y);
        }

        public static Vector3 DecodeVector3(string value)
        {
            throw new NotImplementedException();
        }

        public static bool DecodeBool(string value)
        {
            if (value == "True")
                return true;
            return false;
        }

        public static Rectangle DecodeRectangle(string value)
        {
            if (value == null)
                return new Rectangle(0, 0, 0, 0);

            Rectangle val = new Rectangle();
            //get x
            int index = value.IndexOf("X:");
            index += 2;
            int end = value.IndexOf(" ", index);
            val.X = Convert.ToInt32(value.Substring(index, end - index), ci);

            //get y
            index = value.IndexOf("Y:");
            index += 2;
            end = value.IndexOf(" ", index);
            val.Y = Convert.ToInt32(value.Substring(index, end - index), ci);

            //get width
            index = value.IndexOf("Width:");
            index += 6;
            end = value.IndexOf(" ", index);
            val.Width = Convert.ToInt32(value.Substring(index, end - index), ci);

            //get height
            index = value.IndexOf("Height:");
            index += 7;
            end = value.IndexOf("}", index);
            val.Height = Convert.ToInt32(value.Substring(index, end - index), ci);
            return val;
        }

        public static string EncodeArray<T>(T[] array)
        {
            if (array == null)
                return string.Empty;
            string str = string.Empty;
            foreach (object o in array)
                str = $"{str}{o.ToString()},";
            return str;
        }

        public static string EncodeList<T>(List<T> array)
        {
            return EncodeArray<T>(array.ToArray());
        }


        public static List<float> DecodeListFloat(string value)
        {
            int index = 0;
            int nextIndex = 0;
            List<float> tmp = new List<float>();
         
            while (nextIndex != -1)
            {
                nextIndex = value.IndexOf(",", index);
                if (nextIndex != -1) //if element found
                {
                    tmp.Add((float)Convert.ToDouble(value.Substring(index, nextIndex - index), ci));
                    index = nextIndex + 1;
                    if (index >= value.Length)
                        break;
                }
            }
            return tmp;
        }

        

        public static T[] DecodeArrayEnum<T>(string value)
        {
            int index = 0;
            int nextIndex = 0;
            List<T> tmp = new List<T>();

            while (nextIndex != -1)
            {
                nextIndex = value.IndexOf(",", index);
                if (nextIndex != -1) //if element found
                {
                    tmp.Add(DecodeEnum<T>(value.Substring(index, nextIndex - index)));
                    index = nextIndex + 1;
                    if (index >= value.Length)
                        break;
                }
            }
            return tmp.ToArray();
        }

        public static float[] DecodeArrayFloat(string value)
        {
            return DecodeListFloat(value).ToArray();
        }

        public static List<int> DecodeListInt(string value)
        {
            if (value == null)
                return new List<int>();
            int index = 0;
            int nextIndex = 0;
            List<int> tmp = new List<int>();
            while (nextIndex != -1)
            {
                nextIndex = value.IndexOf(",", index);
                if (nextIndex != -1) //if element found
                {
                    tmp.Add(Convert.ToInt32(value.Substring(index, nextIndex - index), ci));
                    index = nextIndex + 1;
                    if (index >= value.Length)
                        break;
                }
            }
            return tmp;
        }

        public static int[] DecodeArrayInt(string value)
        {
            return DecodeListInt(value).ToArray();
        }

        public static List<byte> DecodeListByte(string value)
        {
            if (value == null)
                return new List<byte>();

            int index = 0;
            int nextIndex = 0;
            List<byte> tmp = new List<byte>();
            while (nextIndex != -1)
            {
                nextIndex = value.IndexOf(",", index);
                if (nextIndex != -1) //if element found
                {
                    tmp.Add(Convert.ToByte(value.Substring(index, nextIndex - index), ci));
                    index = nextIndex + 1;
                    if (index >= value.Length)
                        break;
                }
            }
            return tmp;
        }

        public static byte[] DecodeArrayByte(string value)
        {
            if (value == null)
                return new byte[0];
            return DecodeListByte(value).ToArray();
        }

        public static List<bool> DecodeListBool(string value)
        {
            int index = 0;
            int nextIndex = 0;
            List<bool> tmp = new List<bool>();
            while (nextIndex != -1)
            {
                nextIndex = value.IndexOf(",", index);
                if (nextIndex != -1) //if element found
                {
                    tmp.Add(Convert.ToBoolean(value.Substring(index, nextIndex - index), ci));
                    index = nextIndex + 1;
                    if (index >= value.Length)
                        break;
                }
            }
            return tmp;
        }

        public static bool[] DecodeArrayBool(string value)
        {
            return DecodeListBool(value).ToArray();
        }

        public static List<Vector2> DecodeListVector2(string value)
        {
            if (value == null)
                return new List<Vector2>();
            int index = 0;
            int nextIndex = 0;
            List<Vector2> tmp = new List<Vector2>();
            while (nextIndex != -1)
            {
                nextIndex = value.IndexOf(",", index);
                if (nextIndex != -1) //if element found
                {
                    tmp.Add(DecodeVector2(value.Substring(index, nextIndex - index)));
                    index = nextIndex + 1;
                    if (index >= value.Length)
                        break;
                }
            }
            return tmp;
        }

        public static List<Point> DecodeListPoint(string value)
        {
            if (value == null)
                return new List<Point>();
            int index = 0;
            int nextIndex = 0;
            List<Point> tmp = new List<Point>();
            while (nextIndex != -1)
            {
                nextIndex = value.IndexOf(",", index);
                if (nextIndex != -1) //if element found
                {
                    tmp.Add(DecodePoint(value.Substring(index, nextIndex - index)));
                    index = nextIndex + 1;
                    if (index >= value.Length)
                        break;
                }
            }
            return tmp;
        }

        public static List<Rectangle> DecodeListRectangle(string value)
        {
            int index = 0;
            int nextIndex = 0;
            List<Rectangle> tmp = new List<Rectangle>();
            while (nextIndex != -1)
            {
                nextIndex = value.IndexOf(",", index);
                if (nextIndex != -1) //if element found
                {
                    tmp.Add(RsSerilization.DecodeRectangle(value.Substring(index, nextIndex - index)));
                    index = nextIndex + 1;
                    if (index >= value.Length)
                        break;
                }
            }
            return tmp;
        }

        public static Rectangle[] DecodeArrayRectangle(string value)
        {
            return DecodeListRectangle(value).ToArray();
        }

        public static Vector2[] DecodeArrayVector2(string value)
        {
            return DecodeListVector2(value).ToArray();
        }



    }
}
