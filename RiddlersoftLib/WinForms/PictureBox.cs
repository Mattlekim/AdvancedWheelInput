using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;
using Riddlersoft.Core.Input;

namespace WinForms
{
    public class PictureBox : Componate
    {
        /// <summary>
        /// the img that we want to draw
        /// </summary>
        public Texture2D Img;
        public float? ImgSacle;
        /// <summary>
        /// the source rectange to draw with
        /// </summary>
        public Rectangle? Source = null;

        /// <summary>
        /// any data that we want to store in this pictureBox
        /// </summary>
        public int Data;

        /// <summary>
        /// another data set that the user can use
        /// </summary>
        public int Data2;

        public bool ReturnColourUnderMouse = true;
        public Action<Color> OnColourSelected;

        public Action<object> OnSelected;
        private bool _isSelected = false;
        public bool CanBeSelected = false;

        public Color ImgColour = Color.White;

        public bool HaveFrame = true;
        /// <summary>
        /// creates a new picture box
        /// </summary>
        /// <param name="area">the area to draw the picture</param>
        /// <param name="parentcomponate">the parent</param>
        /// <param name="img">the image to draw</param>
        /// <param name="source">the part of the image to draw</param>
        public PictureBox(Rectangle area, Componate parentcomponate, Texture2D img, Rectangle? source) : base(area, parentcomponate)
        {
            Img = img;
            Source = source;
            MouseTouch.OnLeftMouseDown += OnMouseDownEvent;
            MouseTouch.OnLeftMouseRelease += OnMouseReleaseEvent;
        }

        /// <summary>
        /// creates a new picture box
        /// </summary>
        /// <param name="area">the area to draw the picture</param>
        /// <param name="parentcomponate">the parent</param>
        /// <param name="img">the image to draw</param>
        /// <param name="source">the part of the image to draw</param>
        public PictureBox(Vector2 pos, Componate parentcomponate, Texture2D img, Rectangle? source) : 
            base(new Rectangle((int)pos.X, (int)pos.Y, img.Width, img.Height), parentcomponate)
        {
            Img = img;
            Source = source;
            MouseTouch.OnLeftMouseDown += OnMouseDownEvent;
            MouseTouch.OnLeftMouseRelease += OnMouseReleaseEvent;
        }

        private Color[,] colourArray;
        protected override void Update(float dt)
        {
            if (ReturnColourUnderMouse && _adustValue)
            {
                //first get mouse locatoin
                Point loc = MouseTouch.Position.ToPoint();

                if (colourArray == null) //if we have not inisalized the color array
                { //create it
                    Color[] tmpcolorarray = new Color[Img.Width * Img.Height];
                    Img.GetData(tmpcolorarray);

                    colourArray = new Color[Img.Width, Img.Height];
                    int count = 0;
                    for (int y = 0; y < Img.Height; y++)
                        for (int x = 0; x < Img.Width; x++)
                        {
                            colourArray[x, y] = tmpcolorarray[count];
                            count++;
                        }
                }

                //now find out the location in th image
                loc.X -= Area.X;
                loc.Y -= Area.Y;
                loc.X = MathHelper.Clamp(loc.X, 0, Img.Width - 1);
                loc.Y = MathHelper.Clamp(loc.Y, 0, Img.Height - 1);
                //now get the color
                if (OnColourSelected != null)
                    OnColourSelected(colourArray[loc.X, loc.Y]);

                /*                for (int y = 0; y < 338; y++)
                                    for (int x = 0; x < 338; x++)
                                        if (colourArray[x, y] == new Color(texturecolor.R, texturecolor.G, texturecolor.B))
                                            colourlocation = new Vector2(x, y);
                                            */
            }

            base.Update(dt);
        }

        

        protected override void Render(ref SpriteBatch sb)
        {
            _internalRender = HaveFrame;

            if (_isSelected)
                sb.Draw(bg, new Rectangle(Area.X + 2, Area.Y + 2, Area.Width - 4, Area.Height - 4), BgColourSecondry);

            if (Img != null)
            {

                if (ImgSacle == null)
                    sb.Draw(Img, _areaCurrent, Source, ImgColour, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                else
                    sb.Draw(Img,
                        new Vector2(_areaCurrent.X + _areaCurrent.Width / 2f, _areaCurrent.Y + _areaCurrent.Height / 2f),
                        Source, ImgColour, 0f, 
                        new Vector2(Source.Value.Width / 2f, Source.Value.Height / 2f), (float)ImgSacle, SpriteEffects.None, 0f);

                
            }

        }

        private bool _adustValue = false;
        private bool _initalPress = true;

        public void OnMouseDownEvent(Vector2 location)
        {
            if (!_initalPress)
                return;

            _initalPress = false;
            if (Area.Contains(location.ToPoint()))
            {
                _adustValue = true;
            }
        }

        public override bool OnLeftMouseClick(Point p)
        {
            if (CanBeSelected)
            {
                if (OnSelected != null)
                    OnSelected(this);
                _isSelected = true;
            }
            return base.OnLeftMouseClick(p);
        }

        public void ClearSelected()
        {
            _isSelected = false;
        }

        public void OnMouseReleaseEvent(Vector2 location)
        {
            _adustValue = false;
            _initalPress = true;
        }
    }
}
