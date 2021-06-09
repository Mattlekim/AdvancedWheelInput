using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Riddlersoft.Core;

namespace WinForms
{
    /// <summary>
    /// A list menu. appears at the specifyed location
    /// </summary>
    public class FloatingListMenu : Componate
    {
        public struct Item
        {
            public int Id;
            public string Text;
            public Texture2D Img;
            public Rectangle ImgArea;
            public bool HaveImage;
        }

        public delegate void _onItemClick(string id);
        public _onItemClick OnItemClick;

        /// <summary>
        /// the item that was selected from the list
        /// </summary>
        public object SelectedItem { get; protected set; }

        private List<Item> _items = new List<Item>();
        public List<Item> Items {  get { return _items; } }

        private int _maxItemHeight = 0;

        public FloatingListMenu(int height , Componate parentcomponate) : 
            base(new Rectangle(0,0, 100, height), parentcomponate)
        {
            _visible = false;
            Active = false;
        }

        private int _maxItemWidth;
        private float _itemScale = .6f;

       

        public void AddItem(string text, int id, Texture2D img = null, Rectangle imgArea = new Rectangle())
        {
            Item i = new Item()
            {
                Text = text,
                Id = id,
                Img = img,
                ImgArea = imgArea,
                HaveImage = img != null,
            };
            _areaOriginal.Width = 0;
            if (i.HaveImage)
            {
                if (i.ImgArea.Width > _areaOriginal.Width - 5)
                    _areaOriginal.Width = i.ImgArea.Width + 5;

                if (i.ImgArea.Height > _maxItemHeight - 5)
                    _maxItemHeight = i.ImgArea.Height + 5;
            }
            else
            {
                if (Font.MeasureString(i.Text).X > _areaOriginal.Width - 5)
                    _areaOriginal.Width = (int)Font.MeasureString(i.Text).X + 5;

                if (Font.MeasureString(i.Text).Y > _maxItemHeight - 5)
                    _maxItemHeight = (int)Font.MeasureString(i.Text).Y + 5;
            }
            _items.Add(i);
            _maxItemWidth = _areaOriginal.Width;
        }

        /// <summary>
        /// keeps track of the top most item of the list
        /// </summary>
        private int _itemIndex = 0;
        public override bool OnLeftMouseClick(Point p)
        {
            int offset = 0;

            if (new Rectangle(_areaCurrent.X, _areaCurrent.Y, _areaCurrent.Width, 20).Contains(p)) //if up arrow
            {
                if (_itemIndex > 0)
                    _itemIndex--;
                return base.OnLeftMouseClick(p);
            }

            if (new Rectangle(_areaCurrent.X, _areaCurrent.Y + _areaCurrent.Height - 20, _areaCurrent.Width, 20).Contains(p)) //if down arrow
            {
                if (_itemIndex < Items.Count)
                    _itemIndex++;
                return base.OnLeftMouseClick(p);
            }

            Item item;
            for (int i = _itemIndex; i < Items.Count; i++)
            {
                item = Items[i]; //grab the item
                if (new Rectangle(_areaCurrent.X, _areaCurrent.Y + offset, _areaCurrent.Width, (int)(item.ImgArea.Height * _itemScale)).Contains(p)) //if item clicked
                {
                    if (OnItemClick != null)
                        OnItemClick(item.Id.ToString());
                    CloseMenu(item); //close the menu
                    return base.OnLeftMouseClick(p);
                }
                offset += Convert.ToInt32(_maxItemHeight * _itemScale);
            }

            return base.OnLeftMouseClick(p);
        }

        /// <summary>
        /// closes the menu while setting the selected item
        /// </summary>
        /// <param name="item">the item to make selcted</param>
        protected void CloseMenu(Item item)
        {
            SelectedItem = item; //set the item
            DeActivate(); //close this
        }

        protected override void Render(ref SpriteBatch sb)
        {
            int menuoffset = 0;
            Item item;
            for (int i =_itemIndex; i < Items.Count; i++) //loop through every item
            {
                item = Items[i]; //grab the item
                if (item.HaveImage)
                {
                    if (menuoffset >= 0 && menuoffset + _maxItemHeight * _itemScale <= _areaCurrent.Height) //make sure we want to draw it       
                    {
                        sb.Draw(item.Img, new
                        Rectangle(_areaCurrent.X, _areaCurrent.Y + menuoffset, item.ImgArea.Width, item.ImgArea.Height), item.ImgArea, Color.White);
                        sb.DrawString(Font, item.Text, new Vector2(_areaCurrent.X + _maxItemWidth, _areaCurrent.Y + menuoffset + 15), Color.White,
                                0f, Vector2.Zero, MasterScale, SpriteEffects.None, 0f);
                    }
                    menuoffset += Convert.ToInt32(_maxItemHeight * _itemScale);
                }
            }

            sb.Draw(_arrow, new Rectangle(_areaCurrent.X + 60, _areaCurrent.Y, _areaCurrent.Width - 120, 20), Color.White);
            sb.Draw(_arrow, new Rectangle(_areaCurrent.X + 60, _areaCurrent.Y + _areaCurrent.Height - 20, _areaCurrent.Width - 120, 20), null, Color.White,
                 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
        }

        public void ClearItems()
        {
            _items.Clear();
        }

        public void Show(Point pos)
        {
            base.Activate();

            SelectedItem = null; //set to null so that we will know if it has been set or not

            _itemIndex = 0;
            _areaOriginal.X = pos.X;
            _areaOriginal.Y = pos.Y;

            int textWidth = 0;
            int tmp = 0;
            foreach (Item i in Items)
            {
                tmp = (int)Font.MeasureString(i.Text).X;
                if (tmp > textWidth)
                    textWidth = tmp;
            }
            //calculate height
            _areaOriginal.Height = _maxItemHeight * Items.Count; //calculate the height of the bar
            _areaOriginal.Width = _maxItemWidth + textWidth;
            float mostBottom = _areaOriginal.Height +_areaOriginal.Y + _parent.Area.Y; //the bottom most part to render
            if (mostBottom > ScreenOldMethord.ActualResolution.Height) //if its to big;
            {
                _areaOriginal.Height = ScreenOldMethord.ActualResolution.Height - _areaOriginal.Y - _parent.Area.Y - 10; //calculate the maximum height
                _verticalScrole = true;
            }
            else
                _verticalScrole = false;
            
        }


        public void Show(Vector2 pos)
        {
            Show(pos.ToPoint());
        }

        public void Show(Point pos, List<Item> items)
        {
            _items.Clear();
            _items.AddRange(items);
            Show(pos);
        }

        public void Show(Vector2 pos, List<Item> items)
        {
            Show(pos.ToPoint(), items);
        }
    }
}
