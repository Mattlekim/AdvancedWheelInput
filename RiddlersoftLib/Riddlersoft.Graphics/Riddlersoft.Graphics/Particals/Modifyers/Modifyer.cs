using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Riddlersoft.Core.Xml;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public abstract class Modifyer
    {
        public static Dictionary<string, CustomModifyer> CustomModfyers { get; internal set; } = new Dictionary<string, CustomModifyer>();

        public static void AddCustomModifyer(CustomModifyer m)
        {
            if (m.XmlTag == string.Empty)
                return;

            CustomModfyers.Add(m.XmlTag, m);
        }

        public static Action OnSetUp;

        internal static void Intialize()
        {
            if (OnSetUp != null)
            {
                OnSetUp();
            }
        }

   
        

        public string XmlTag = "";

        public float StartTime = 0;
        public float EndTime = 100;
        public bool UsePercentageFromTime = false;


        public Modifyer()
        {

        }

        public CustomModifyer Create(string tag)
        {
            if (tag == string.Empty)
                return new CustomModifyer();

            if (!CustomModfyers.ContainsKey(tag))
                throw new Exception("Modifyer dos not exist");
        
            return CustomModfyers[tag]._create();
        }

        protected abstract CustomModifyer _create();
        
        protected abstract void _prosses(Partical input, float dt);

        public abstract void WriteToFile(CustomXmlWriter writer);

        public abstract void ReadFromFile(CustomXmlReader reader);

        public void Prosses(Partical input, float dt)
        {
            if (!UsePercentageFromTime)
            {
                if (input.Age < StartTime)
                    return;

                if (input.Age > EndTime)
                    return;
            }
            else
            {
                float maxTime = input.LifeTime + input.Age;
               
                float d = input.Age / maxTime;
                if (d < StartTime)
                    return;

                if (d > EndTime)
                    return;
            }

            _prosses(input, dt);
        }
    }
}
