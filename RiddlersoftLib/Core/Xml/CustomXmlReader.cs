using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.IO;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Core.Xml
{
    public class CustomXmlReader : IDisposable
    {
        private XmlReader Reader;
        private CustomXmlReader(Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = true,
            };
            Reader = XmlReader.Create(stream);
            
        }

        public bool IsEmptyElement { get { return Reader.IsEmptyElement; } }

        public string Name { get { return Reader.Name; } }
        
        public string Value { get { return Reader.Value; } }

        public bool Read()
        {
            return Reader.Read();
        }

        public bool IsStartElement()
        {
            return Reader.IsStartElement();
        }

        public string GetAttribute(string name)
        {
            return Reader.GetAttribute(name);
        }

        public static CustomXmlReader Create(Stream stream)
        {
#if WINDOWS
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
#endif
            return new CustomXmlReader(stream);
        }

        public string ReadAttributeString(string name)
        {
            return Reader.GetAttribute(name);
        }

        public int ReadAttributeInt(string name)
        {
            return RsSerilization.DecodeInt(Reader.GetAttribute(name));
        }

        public uint ReadAttributeUInt(string name)
        {
            return RsSerilization.DecodeUInt(Reader.GetAttribute(name));
        }

        public long ReadAttributeLong(string name)
        {
            return RsSerilization.DecodeLong(Reader.GetAttribute(name));
        }

        public ulong ReadAttributeULong(string name)
        {
            return RsSerilization.DecodeULong(Reader.GetAttribute(name));
        }

        public byte ReadAttributeByte(string name)
        {
            return RsSerilization.DecodeByte(Reader.GetAttribute(name));
        }

        public float ReadAttributeFloat(string name)
        {
            return RsSerilization.DecodeFloat(Reader.GetAttribute(name)); ;
        }

        public double ReadAttributeDouble(string name)
        {
            return RsSerilization.DecodeDouble(Reader.GetAttribute(name)); 
        }

        public bool ReadAttributeBool(string name)
        {
            return RsSerilization.DecodeBool(Reader.GetAttribute(name));
        }

        public Vector2 ReadAttributeVector2(string name)
        {
            return RsSerilization.DecodeVector2(Reader.GetAttribute(name));
            
        }

        public Point ReadAttributePoint(string name)
        {
            return RsSerilization.DecodePoint(Reader.GetAttribute(name));

        }

        public Rectangle ReadAttributeRectangle(string name)
        {
            return RsSerilization.DecodeRectangle(Reader.GetAttribute(name));
        }

        public T ReadAttributeEnum<T>(string name)
        {
            string s = Reader.GetAttribute(name);
            if (s == null)
                return (T)Enum.Parse(typeof(T), Enum.GetNames(typeof(T))[0], true);
            return (T)Enum.Parse(typeof(T), s, true);
        }

        public Color ReadAttributeColor(string name)
        {
            string tmp = Reader.GetAttribute(name);
            int index = 0, nextIndex = 0;
            Color col = new Color();
            index = tmp.IndexOf("R:") + 2;
            nextIndex = tmp.IndexOf(" ", index);
            col.R = Convert.ToByte(tmp.Substring(index, nextIndex - index));

            index = tmp.IndexOf("G:") + 2;
            nextIndex = tmp.IndexOf(" ", index);
            col.G = Convert.ToByte(tmp.Substring(index, nextIndex - index));

            index = tmp.IndexOf("B:") + 2;
            nextIndex = tmp.IndexOf(" ", index);
            col.B = Convert.ToByte(tmp.Substring(index, nextIndex - index));

            index = tmp.IndexOf("A:") + 2;
            nextIndex = tmp.IndexOf("}", index);
            col.A = Convert.ToByte(tmp.Substring(index, nextIndex - index));
            return col;
        }

        public T[] ReadAttributeArrayOfEnum<T>(string name)
        {
            return RsSerilization.DecodeArrayEnum<T>(Reader.GetAttribute(name));
        }

        public float[] ReadAttributeArrayOfFloat(string name)
        {
            return RsSerilization.DecodeArrayFloat(Reader.GetAttribute(name));
        }

        public bool[] ReadAttributeArrayOfBool(string name)
        {
            return RsSerilization.DecodeArrayBool(Reader.GetAttribute(name));
        }

        public int[] ReadAttributeArrayOfInt(string name)
        {
            return RsSerilization.DecodeArrayInt(Reader.GetAttribute(name));
        }

        public List<float> ReadAttributeListOfFloat(string name)
        {
            return RsSerilization.DecodeListFloat(Reader.GetAttribute(name));
        }

        public List<bool> ReadAttributeListOfBool(string name)
        {
            return RsSerilization.DecodeListBool(Reader.GetAttribute(name));
        }

        public List<int> ReadAttributeListOfInt(string name)
        {
            return RsSerilization.DecodeListInt(Reader.GetAttribute(name));
        }

        public List<Vector2> ReadAttributeListOfVector2(string name)
        {
            return RsSerilization.DecodeListVector2(Reader.GetAttribute(name));
        }

        public List<Point> ReadAttributeListOfPoint(string name)
        {
            return RsSerilization.DecodeListPoint(Reader.GetAttribute(name));
        }

        public Rectangle[] ReadAttributeArrayOfRectangle(string name)
        {
            return RsSerilization.DecodeArrayRectangle(Reader.GetAttribute(name));
        }

        public List<Rectangle> ReadAttributeListOfRectangle(string name)
        {
            return RsSerilization.DecodeListRectangle(Reader.GetAttribute(name));
        }

        public Vector2[] ReadAttributeArrayOfVector2(string name)
        {
            return RsSerilization.DecodeArrayVector2(Reader.GetAttribute(name));
        }

        public List<byte> ReadAttributeListOfByte(string name)
        {
            return RsSerilization.DecodeListByte(Reader.GetAttribute(name));
        }

        public byte[] ReadAttributeArrayOfByte(string name)
        {
            return RsSerilization.DecodeArrayByte(Reader.GetAttribute(name));
        }

        public void Close()
        {
            Reader.Close();
        }

        public void Dispose()
        {
            Reader.Close();
        }
    }
}
