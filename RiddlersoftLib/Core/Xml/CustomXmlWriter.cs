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
    public class CustomXmlWriter : IDisposable
    {
        private XmlWriter Writer;
        private CustomXmlWriter(Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = true,
            };
            Writer = XmlWriter.Create(stream, settings);
        }

        public static CustomXmlWriter Create(Stream stream)
        {
#if WINDOWS
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
#endif
            return new Xml.CustomXmlWriter(stream);
        }

        public void WriteStartDocument()
        {
            Writer.WriteStartDocument();
        }

        public void WriteEndDocument()
        {
            Writer.WriteEndDocument();
        }

        public void WriteStartElement(string name)
        {
            Writer.WriteStartElement(name);
        }

        public void WriteEndElement()
        {
            Writer.WriteEndElement();
        }

        public void WriteElementString(string name, string value)
        {
            Writer.WriteElementString(name, value);
        }

        public void WriteAttributeString(string name, string value)
        {
            Writer.WriteAttributeString(name, value);
        }

        public void WriteNumber<T>(string name, T value)
        {
            Writer.WriteAttributeString(name, value.ToString());
        }

        public void WriteAttributeInt(string name, int value)
        {
            Writer.WriteAttributeString(name, value.ToString());
        }

        public void WriteAttributeUInt(string name, uint value)
        {
            Writer.WriteAttributeString(name, value.ToString());
        }

        public void WriteAttributeLong(string name, long value)
        {
            Writer.WriteAttributeString(name, value.ToString());
        }

        public void WriteAttributeULong(string name, ulong value)
        {
            Writer.WriteAttributeString(name, value.ToString());
        }

        public void WriteAttributeByte(string name, byte value)
        {
            Writer.WriteAttributeString(name, value.ToString());
        }

        public void WriteAttributeFloat(string name, float value)
        {
            Writer.WriteAttributeString(name, value.ToString());
        }

        public void WriteAttributeDouble(string name, double value)
        {
            Writer.WriteAttributeString(name, value.ToString());
        }

        public void WriteAttributeBool(string name, bool value)
        {
            WriteAttributeString(name, value.ToString());
        }

        public void WriteAttributeVector2(string name, Vector2 value)
        {
            WriteAttributeString(name, value.ToString());
        }

        public void WriteAttributePoint(string name, Point value)
        {
            WriteAttributeString(name, value.ToString());
        }

        public void WriteAttributeRectangle(string name, Rectangle value)
        {
            WriteAttributeString(name, value.ToString());
        }

        public void WriteAttributeEnum<T>(string name, T value)
        {
            WriteAttributeString(name, value.ToString());
        }

        public void WriteAttributeColor(string name, Color color)
        {
            WriteAttributeString(name, color.ToString());
        }
        public void WriteAttributeArray<T>(string name, T[] value)
        {
            string str = string.Empty;
            if (value != null)
            {
                foreach (object o in value)
                    str = $"{str}{o.ToString()},";
            }
            WriteAttributeString(name, str);
        }

        public void WriteAttributeList<T>(string name, List<T> value)
        {
            string str = string.Empty;
            if (value != null)
            {
                foreach (object o in value)
                    str = $"{str}{o.ToString()},";
            }
            WriteAttributeString(name, str);
        }

        public void Close()
        {
            Writer.Close();
        }

        public void Dispose()
        {
            Writer.Close();
        }
    }
}
