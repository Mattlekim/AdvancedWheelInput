using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Graphics.Particals.Emitters
{
    public interface EmitterInterface
    {
        /// <summary>
        /// the amount of particals to trigger
        /// </summary>
        /// <param name="amount"></param>
        void Trigger(Point position, int amount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="amount"></param>
        void Trigger(Vector2 position, int amount);
        /// <summary>
        /// updates the emitter
        /// </summary>
        /// <param name="dt"></param>
        void Update(float dt);

        /// <summary>
        /// renders all particals to screen
        /// </summary>
        /// <param name="sb">the spritebatch to render with</param>
        void Render(SpriteBatch sb);

        /// <summary>
        /// renders all particals to screen
        /// </summary>
        /// <param name="sb">the spritebatch to render with</param>
        void Render(SpriteBatch sb, Vector2 camera);


    }
}
