using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace Riddlersoft.Graphics
{
 
    public class DrawLayer
    {
        public bool Enabled;
        public Color Colour;
        public Rectangle Area;
    }   
    /// <summary>
    /// like a texture2d but can have multiplue textures
    /// </summary>
    public class Texture2DMulti : DrawableInterface
    {
        /// <summary>
        /// for simplicity we will just do width wise
        /// </summary>
        Texture2D _spiretSheet = null;

        List<DrawLayer> _layers = new List<DrawLayer>();

        public DrawLayer GetLayer(int layer)
        {
            return _layers[layer];
        }

        private List<int> _drawLayers = new List<int>() { 0 };

        private GraphicsDevice _device;

        public Texture2DMulti(GraphicsDevice device)
        {
            _device = device;
        }

        public void SetTexture(Texture2D texture)
        {
            if (_spiretSheet != null)
                throw new Exception("Texture can only be set once");
            _spiretSheet = texture;
        }
        private int textureSpacing = 1;

        private void AddLayer(Rectangle layer)
        {
            _layers.Add(new DrawLayer() { Enabled = true, Area = layer, Colour = Color.White, });
        }

        public void SetLayerVisiblity(int layer, bool visible)
        {
            _layers[layer].Enabled = visible;
        }

        public void SetLayerColour(int layer, Color colour)
        {
            _layers[layer].Colour = colour;
        }

        public void Add(Texture2D tex)
        {
            Color[] cols = new Color[tex.Width * tex.Height]; //get current texture data
            tex.GetData<Color>(cols);
            if (_spiretSheet == null)
            {
                _spiretSheet = new Texture2D(_device, tex.Width, tex.Height);
                _spiretSheet.SetData<Color>(cols); //set the data 
                AddLayer(new Rectangle(0, 0, tex.Width, tex.Height));
            }
            else //if we already have data
            {
                //get original sprite sheet data
                Color[] spriteSheet = new Color[_spiretSheet.Width * _spiretSheet.Height];
                Point spriteSheetSize = new Point(_spiretSheet.Width, _spiretSheet.Height);
                _spiretSheet.GetData<Color>(spriteSheet); //get texture

                Point newSize = new Point(_spiretSheet.Width, _spiretSheet.Height);
                if (_spiretSheet.Height < tex.Height) //if the height is to low
                    newSize.Y = tex.Height;
                newSize.X += tex.Width + textureSpacing;

                Rectangle? tmp = new Rectangle(0, 0, spriteSheetSize.X, spriteSheetSize.Y);
                //create new spritesheet texure
                _spiretSheet = new Texture2D(_device, newSize.X, newSize.Y);
                //put the original texutre back in
                _spiretSheet.SetData<Color>(0, 0, tmp, spriteSheet, 0, spriteSheetSize.X * spriteSheetSize.Y);

                tmp = new Rectangle(spriteSheetSize.X + textureSpacing, 0, tex.Width, tex.Height);
                //put the new texture in
                _spiretSheet.SetData<Color>(0, 0, tmp, cols, 0, tex.Width * tex.Height);
                AddLayer((Rectangle)tmp);
            }

        }

        public void SelectLayersToDraw(int layer)
        {
            _drawLayers.Clear();
            _drawLayers.Add(layer);
        }

        public void SelectLayersToDraw(List<int> layers)
        {
            _drawLayers.Clear();
            _drawLayers.AddRange(layers);
        }

        public void AddFrameInformaion()
        {

        }

        public void Draw(SpriteBatch sb, Vector2 position)
        {
            for (int i =0; i < _layers.Count; i++)
                if (_layers[i].Enabled)
                    sb.Draw(_spiretSheet, position, _layers[i].Area, _layers[i].Colour);
        }

        public void Draw(SpriteBatch sb, Vector2 position, int layer)
        {
            sb.Draw(_spiretSheet, position, _layers[layer].Area, _layers[layer].Colour);
        }

        public void Draw(SpriteBatch sb, Vector2 position, int layer, Vector2 origin, float rotation, float scale = 1)
        {
            sb.Draw(_spiretSheet, position, _layers[layer].Area, _layers[layer].Colour, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        public void Draw(SpriteBatch sb, Vector2 position, int layer, Vector2 origin)
        {
            sb.Draw(_spiretSheet, position, _layers[layer].Area, _layers[layer].Colour, 0f, origin, 1f, SpriteEffects.None, 0f);
        }

        public void Update(float dt)
        {
        
        }

        public static implicit operator Texture2DMulti(Texture2D input)
        {
            Texture2DMulti tm = new Graphics.Texture2DMulti(input.GraphicsDevice);
            tm.SetTexture(input);
            return tm;
        }

       
    }
}
