using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Particals.Emitters
{
    public class RectangleEmitter : Emitter
    {
        public Rectangle Area { get; private set; }

        public void SetArea(Rectangle area)
        {
            Area = area;
            Name = "Rectangle";
        }

        public RectangleEmitter(Rectangle area): base()
        {
            SetArea(area);
        }

        protected override void TriggerParticals(Vector2 pos, int amount)
        {
            for (int i=0; i < amount; i++)
            {
                Particals.Add(GenerateParticalProperties(
                    new Vector2(_random.Next(Area.X, Area.X + Area.Width) + pos.X,
                                _random.Next(Area.Y, Area.Y + Area.Height) + pos.Y)
                                ));
            }
        }

       
    }
}
