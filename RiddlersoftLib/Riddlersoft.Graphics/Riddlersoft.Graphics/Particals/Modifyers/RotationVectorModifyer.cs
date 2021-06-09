using System;
using Microsoft.Xna.Framework;
using Riddlersoft.Core.Xml;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public class RotationVectorModifyer : CustomModifyer
    {
        public float Rotation;
        public int t;
        public bool b;
        public string s;

        public RotationVectorModifyer()
        {
            XmlTag = "RotationVector";
            Rotation = 0;
        }


        public override void SetUpEditorProperties()
        {
            EditorProperties.Clear();
            EditorProperties.Add(new EditorData("Rotation", EditorInputType.Textbox, EditorInputDataType.Float)
            {
                Set = (object input) =>
                {
                    Rotation = (float)Convert.ToDouble((string)input);
                },

                Read = () =>
                {
                    return Rotation;
                }

            });

            EditorProperties.Add(new EditorData("Color", EditorInputType.Textbox, EditorInputDataType.Int)
            {
                Set = (object input) =>
                {
                    t = ConvertToint(input);
                },

                Read = () =>
                {
                    return t;
                }

            });

            EditorProperties.Add(new EditorData("name", EditorInputType.Textbox, EditorInputDataType.String)
            {
                Set = (object input) =>
                {
                   s = ConvertToString(input);
                },

                Read = () =>
                {
                    return s;
                }

            });
        }

        protected override CustomModifyer _create()
        {
            //set up editor stuff
          
            
            
            return new RotationVectorModifyer();
        }


        public override void ReadFromFile(CustomXmlReader reader)
        {
            Rotation = reader.ReadAttributeFloat("Rot");
        }

        public override void WriteToFile(CustomXmlWriter writer)
        {
            writer.WriteAttributeString("Type", XmlTag);
            writer.WriteAttributeFloat("Rot", Rotation);
        }



        protected override void _prosses(Partical input, float dt)
        {
            input.Rotation = (float)Math.Atan2(input.Velocity.Y, input.Velocity.X) + MathHelper.PiOver2 + Rotation;
        }
    }
}
