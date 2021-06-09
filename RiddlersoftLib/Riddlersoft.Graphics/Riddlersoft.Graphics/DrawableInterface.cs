using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics
{
    public interface DrawableInterface
    {
        void Update(float dt);
        void Draw(SpriteBatch sb, Vector2 position);
    }
}
