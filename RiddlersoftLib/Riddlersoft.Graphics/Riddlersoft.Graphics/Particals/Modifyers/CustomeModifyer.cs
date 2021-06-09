using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riddlersoft.Core.Xml;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public class CustomModifyer : Modifyer
    {
        public enum EditorInputType { Textbox, TickBox }
        public enum EditorInputDataType { Bool, Int, Float, String, Colour, Vector2 }

        public class EditorData
        { 

            public EditorInputType InputType;
            public EditorInputDataType DataType;

            public string Name;

            public Action<object> Set;
            public Func<object> Read;

            public EditorData(string name, EditorInputType inputType, EditorInputDataType dataType)
            {
                Name = name;
                InputType = inputType;
                DataType = dataType;
                Set = null;
                Read = null;
            }
        }

        public List<EditorData> EditorProperties = new List<EditorData>();

        public virtual void SetUpEditorProperties() { }

        public override void ReadFromFile(CustomXmlReader reader)
        {
          
        }

    

        public override void WriteToFile(CustomXmlWriter writer)
        {
           
        }

        protected override CustomModifyer _create()
        {
            return null;
        }

        protected override void _prosses(Partical input, float dt)
        {
            
        }

        protected string ConvertToString(object o)
        {
            return (string)o;
        }

        protected Color ConvertToColor(object o)
        {
            return (Color)o;
        }

        protected int ConvertToint(object o)
        {
            return Convert.ToInt32(o);
        }

        protected float ConvertToByte(object o)
        {
            return (float)Convert.ToDouble(o);
        }

        protected bool ConvertToBool(object o)
        {
            return Convert.ToBoolean(o);
        }

        private Vector2 ConvertToVector2(object o)
        {
            string input = (string)o;
            if (input.Contains(","))
            {
                input = input.Replace(" ", "");
                float x = 0, y = 0;

                int index = input.IndexOf(",");
                try
                {
                    x = (float)Convert.ToDecimal(input.Substring(0, index));
                    y = (float)Convert.ToDecimal(input.Substring(index + 1));
                }
                catch
                {
                    x = 0;
                    y = 0;
                }

                return new Vector2(x, y);
            }

            return Vector2.Zero;
        }

      
    }
}
