using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Riddlersoft.Core;
using Riddlersoft.Core.Input;

using Riddlersoft.Core.Screen;

namespace WinForms
{

    public class ListBox: Componate
    {
        /// <summary>
        /// a list box item
        /// </summary>
        public struct ListBoxItem
        {
            public struct ContentItem
            {
                public string Text;
                public int Width;

                public ContentItem(string text, int width)
                {
                    Text = text;
                    Width = width;
                }
            }

            public List<ContentItem> Contents;

            public ListBoxItem(string item1, SpriteFont font)
            {
                Contents = new List<ContentItem>();
                Contents.Add(new ContentItem(item1, (int)font.MeasureString(item1).X + 2));
            }

            public ListBoxItem(List<string> items, SpriteFont font)
            {
                Contents = new List<ContentItem>();
                for (int i =0; i < items.Count; i++)
                Contents.Add(new ContentItem(items[i], (int)font.MeasureString(items[i]).X + 2));
            }
        }

        private int _spacing = 40;

        private float _scrollAmount = 0;
        
        
        private List<ListBoxItem> _items = new List<ListBoxItem>();

        /// <summary>
        /// a readonly list of items
        /// </summary>
        public List<ListBoxItem> Items {  get { return _items; } }

        public List<string> HilightedItem = new List<string>();

        public string CurrentItemName { get
            {
                if (_items.Count > 0 && _selectedItem >= 0 && _selectedItem < _items.Count)
                    return _items[_selectedItem].Contents[0].Text;

                    return null; //retun nothing
            } }
        public delegate void _onItemClick(string id, int selection);
        public _onItemClick OnItemClick;

        /// <summary>
        /// wether the listbox should automaticly resize when items are added or removed
        /// </summary>
        public bool AutoResize = false;

        /// <summary>
        /// where or not this list box will collapse or not
        /// </summary>
        public bool Collapsible = false;

        /// <summary>
        /// used when using a collapsible listbox
        /// when true it means the listbox is open
        /// false mean closed
        /// </summary>
        private bool _isOpen = false;

        public bool ListOpen { get { return _isOpen; } }

        public void HideListBoxOptions()
        {
            _isOpen = false;
        }

        public int NumberOfItems { get { return _items.Count; } }

        public delegate void _onItemSelected(ListBoxItem item, int selection);

        /// <summary>
        /// this function fires when an item is selected
        /// </summary>
        public _onItemSelected OnItemSelected;

        private int _collapsibleHeight;
        public ListBox(Rectangle area, Componate parentcomponate, List<string> items, bool collapsible) : base(area, parentcomponate)
        {
            List<ListBoxItem> newitems = new List<ListBoxItem>();

            Collapsible = collapsible;
            _collapsibleHeight = area.Height;
            
            for (int i=0; i < items.Count; i++)
                newitems.Add(new ListBoxItem(items[i], Font));

            _items.AddRange(newitems);
        }

        public ListBox(Rectangle area, Componate parentcomponate, List<ListBoxItem> items) : base(area, parentcomponate)
        {
            if (items == null)
                return;
            _items.AddRange(items);
        }
        private short _hilightedItem = -1;
        private int _selectedItem = -1;
        /// <summary>
        /// gives you the selected item out of the list box
        /// </summary>
        public int SelectedItem { get { return _selectedItem; } }

       
        /// <summary>
        /// the last amount of items
        /// </summary>
        private int _lastcount = 0;
        private bool _itemClicked = false;
        private bool _isScrolable = false;

        private void OpenList()
        {
            if (Items.Count == 0)
                return;

            _isOpen = true;
            _areaOriginal.Height = NumberOfItems * _spacing;
        }

        private void CloseList(Point p)
        {
            _isOpen = false;

            _areaOriginal.Height = _collapsibleHeight;
        }

        public override bool OnLeftMouseClick(Point p)
        {
            if (Collapsible)
                if (_isOpen)
                {
                    GetClickedItem(p);
                    CloseList(p);
                }
                else
                    OpenList();
                        
            return base.OnLeftMouseClick(p);
        }

        private void UpdateCollapsible(float dt)
        {
            //     if (!_isOpen) //if list box is not open
            //       _originalArea = _areaCurrent; //get the initial size
            // else
            //            {
            //              _areaCurrent.Width = _originalArea.Width; //reset the widht
            //            _areaCurrent.Height = _originalArea.Height; //reset the height
            //      }

            if (_isOpen)
                UpdateAllItems();
        }

        private int _lastItemSelected = -1;
        private void GetClickedItem(Point p)
        {
            p.Y += Convert.ToInt32(_scrollAmount);
            for (int i = 0; i < _items.Count; i++)
                if (new Rectangle(_areaCurrent.X, _areaCurrent.Y + i * _spacing, _areaCurrent.Width, _spacing).Contains(p))
                {
                    _hilightedItem = (short)i;

                    if (MouseTouch.LButtonClick)
                    {
                        _selectedItem = (short)i;
                        MouseTouch.Reset();
                        if (OnItemSelected != null) //if a function is attached
                            OnItemSelected(_items[_selectedItem], i); //run the function
                        
                        if (OnItemClick != null)
                            OnItemClick(_items[_selectedItem].Contents[0].Text, i);

                        _lastItemSelected = i;
                        return;
                    }
                    for (int c = 0; c < _items[_hilightedItem].Contents.Count; c++)
                        HilightedItem.Add(_items[_hilightedItem].Contents[c].Text);
                    break;
                }
            _isScrolable = false;
        }

        private void UpdateAllItems()
        {
            if (_isScrolable)
            {
                if (KeyboardAPI.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                    _scrollAmount += _spacing;

                if (KeyboardAPI.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                    _scrollAmount -= _spacing;

                _scrollAmount -= MouseTouch.ScrollAmount * 40;
                if (_scrollAmount < 0)
                    _scrollAmount = 0;

                if (_scrollAmount + _areaCurrent.Height > Items.Count * _spacing)
                    _scrollAmount = Items.Count * _spacing - _areaCurrent.Height;
            }

            _hilightedItem = -1;
            HilightedItem.Clear();

            int starti = (int)(_scrollAmount / 40);
            Point p = new Point((int)MouseTouch.Position.X, (int)MouseTouch.Position.Y);

            if (!Collapsible)
                if (_areaCurrent.Contains(p))
                {
                    GetClickedItem(p);
                }

            if (_selectedItem != -1 && _hilightedItem == -1) //if there is an item select and currently no item hilighted
            {
                if (Items.Count == 0)
                    return;
                for (int c = 0; c < _items[_selectedItem].Contents.Count; c++) //retun the selected item
                    HilightedItem.Add(_items[_selectedItem].Contents[c].Text);
            }
        }

        protected override void Update(float dt)
        {
            if (AutoResize && !Collapsible) //if we have a none collapsible list it need to be resized
            {
                if (_lastcount != _items.Count)
                    CalculateSize();

            }

            

            if (Collapsible)
            {
                UpdateCollapsible(dt);
                return;
            }
            UpdateAllItems();
           
        }

        
        /// <summary>
        /// set the item we want to be seelcted
        /// </summary>
        /// <param name="selected">the selected it</param>
        public void SetSelectedItem(int selected)
        {
            MathHelper.Clamp(selected, 0, _items.Count - 1); //make sure the new selected item is within the list
            _selectedItem = selected;
            _lastItemSelected = selected;
        }

        private void CalculateSize()
        {
            _areaOriginal.Height = _spacing * _items.Count;
            
        }

        public void ClearList()
        {
            _items.Clear();
            _selectedItem = -1;
            _lastItemSelected = -1;
            _hilightedItem = -1;
            _isScrolable = false;
            _scrollAmount = 0;
            
        }

        public void AddListItem(string itemname)
        {
            _items.Add(new ListBoxItem(itemname, Font));
            _isScrolable = false;
            _scrollAmount = 0;
        }

        public void RemoveListItem(int id)
        {
            if (id > -1 && id < Items.Count)
                Items.RemoveAt(id);

            if (Items.Count > 0)
                _selectedItem = 0;
            else
                _selectedItem = -1;
        }

        public void RemoveCurrentSelectedListItem()
        {
            Items.RemoveAt(SelectedItem);
            if (Items.Count > 0)
                _selectedItem = 0;
            else
                _selectedItem = -1;
        }

        public void AddListItem(ListBoxItem itemname)
        {
            _items.Add(itemname);
            _isScrolable = false;
            _scrollAmount = 0;
        }

        private Viewport _oldView;

        private void RenderCollapsible(ref SpriteBatch sb)
        {
            if (Items.Count == 0)
                return;
            int tmpx;
            if (!_isOpen)
            {
                
                tmpx = Area.X;
               // sb.Draw(bg, new Rectangle(Area.X, Area.Y - (int)_scrollAmount, Area.Width, _spacing), Controler.SecondryBG);
                if (SelectedItem == -1)
                {
                    if (Items.Count == 0)
                        sb.DrawString(Font, "No Items Avalible", new Vector2(tmpx + 4, Area.Y + 4 - (int)_scrollAmount), Controler.PrimaryText,
                            0f, Vector2.Zero, MasterScale, SpriteEffects.None, 0f);
                    else
                        sb.DrawString(Font, "Please Select An Item", new Vector2(tmpx + 4, Area.Y + 4 - (int)_scrollAmount), Controler.PrimaryText,
                            0f, Vector2.Zero, MasterScale, SpriteEffects.None, 0f);
                }
                else
                    

                  for (int c = 0; c < _items[SelectedItem].Contents.Count && c < 1; c++)
                  {
                      sb.DrawString(Font, _items[SelectedItem].Contents[c].Text, new Vector2(tmpx + 4, Area.Y + 4 - (int)_scrollAmount), Controler.SecondryText,
                          0f, Vector2.Zero, MasterScale, SpriteEffects.None, 0f);
                      tmpx += _items[SelectedItem].Contents[c].Width;
                  }

                return;
            }

            RenderAllItems(ref sb);
            return;
            tmpx = _areaOriginal.X + 5;
            int starti = (int)(_scrollAmount / 40f);
            if (starti < 0)
            {
                return;
            }

            if (_selectedItem == -1)
                _selectedItem = 0;

            if (_items.Count == 0)
                return;

            for (int c = 0; c < _items[_selectedItem].Contents.Count && c < 1; c++)
                sb.DrawString(Font, _items[SelectedItem].Contents[c].Text, new Vector2(tmpx, _areaOriginal.Y + 4), Controler.PrimaryText,
                            0f, Vector2.Zero, MasterScale, SpriteEffects.None, 0f);

            if (_isOpen) //if we want to select an item from the list
            {
                for (int i = starti; i < _items.Count; i++)
                {
                    tmpx = _areaOriginal.X;
                    for (int c = 0; c < _items[i].Contents.Count && c < 1; c++)
                    {
                        sb.Draw(bg, new Rectangle(_areaOriginal.X, Convert.ToInt32(_areaOriginal.Y + (i + 1) * (float)_spacing * MasterScale) - (int)_scrollAmount, _areaOriginal.Width, Convert.ToInt32((float)_spacing * MasterScale)),
                            new Color(Controler.SecondryBG.R, Controler.SecondryBG.G, Controler.SecondryBG.B));
                        sb.DrawString(Font, _items[i].Contents[c].Text, new Vector2(tmpx, _areaOriginal.Y + 4 + (i + 1) * _spacing * MasterScale - (int)_scrollAmount), Controler.PrimaryText,
                            0f, Vector2.Zero, MasterScale, SpriteEffects.None, 0f);
                        tmpx += _items[i].Contents[c].Width;
                    }
                }
            }

        }



        private void RenderAllItems(ref SpriteBatch sb)
        {
         
            float viewHeight = Items.Count * _spacing;
            int drawY = Area.Y;
            int drawX = Area.X;
            bool viewportSet = false;
            if (viewHeight > Area.Height) //if it is to big
            {
             /*   drawY = 0;
                drawX = 0;
                _oldView = Device.Viewport;
                viewportSet = true;
                sb.End();
        
                Device.Viewport = new Viewport(ScreenMath.Resize(Area)); //set new viewport
                sb.Begin(SpriteSortMode.Deferred, null,null,null,null,null, Controler.matrix);
                */
                _isScrolable = true;
            }
            else
                _scrollAmount = 0;

            int tmpx = drawX;
            int starti = (int)(_scrollAmount / 40f);
            if (starti < 0 && !_isScrolable)
            {
                return;
            }
            int hightestY = 0; //used for the scroll bar
            for (int i = starti; i < _items.Count; i++)
            {
                tmpx = drawX;
                if (drawY + i * _spacing - (int)_scrollAmount - Area.Height + 80 > Area.Height && _isScrolable)
                    continue;
                if (_hilightedItem == i)
                {
                    sb.Draw(bg, new Rectangle(drawX, drawY + i * _spacing - (int)_scrollAmount, Area.Width, _spacing), Controler.SecondryBG * .5f);
                    for (int c = 0; c < _items[i].Contents.Count && c < 1; c++)
                    {
                        sb.DrawString(Font, _items[i].Contents[c].Text, new Vector2(tmpx + 4, drawY - (int)_scrollAmount + 4 + i * _spacing), Controler.PrimaryText,
                            0f, Vector2.Zero, MasterScale, SpriteEffects.None, 0f);
                        tmpx += _items[i].Contents[c].Width;
                    }
                }
                else
                if (_selectedItem == i)
                {
                    sb.Draw(bg, new Rectangle(drawX, drawY + i * _spacing - (int)_scrollAmount, Area.Width, _spacing), Controler.SecondryBG);
                    for (int c = 0; c < _items[i].Contents.Count && c < 1; c++)
                    {
                        sb.DrawString(Font, _items[i].Contents[c].Text, new Vector2(tmpx + 4, drawY + 4 + i * _spacing - (int)_scrollAmount), Controler.PrimaryText,
                            0f, Vector2.Zero, MasterScale, SpriteEffects.None, 0f);
                        tmpx += _items[i].Contents[c].Width;
                    }
                }
                else
                {
                    if (i % 2 == 0)
                        sb.Draw(bg, new Rectangle(drawX, drawY + i * _spacing - (int)_scrollAmount, Area.Width, _spacing), Color.White * .9f);
                    else
                        sb.Draw(bg, new Rectangle(drawX, drawY + i * _spacing - (int)_scrollAmount, Area.Width, _spacing), Color.Wheat * .8f);

                    for (int c = 0; c < _items[i].Contents.Count && c < 1; c++)
                    {
                        sb.DrawString(Font, _items[i].Contents[c].Text, new Vector2(tmpx + 4, drawY + 4 + i * _spacing - (int)_scrollAmount), Color.Black,
                            0f, Vector2.Zero, MasterScale, SpriteEffects.None, 0f);
                        tmpx += _items[i].Contents[c].Width;
                    }
                }
                hightestY = drawY - (int)_scrollAmount + 4 + i * _spacing;
            }

            if (_isScrolable)
            {
                sb.Draw(bg, new Rectangle(Area.Width - 16 + _parent.Area.X, _parent.Area.Y + 40, 16, Area.Height), new Color(BgColourSecondry, 1f));
                sb.Draw(bg, new Rectangle(Area.Width - 14 + _parent.Area.X, _parent.Area.Y + 42, 12, Area.Height - 4), new Color(BgColourPrimary, 1f));

                float minper = _areaCurrent.Height / (float)(Items.Count * _spacing);
                float per = (_scrollAmount + _areaCurrent.Height) / ((float)(Items.Count * _spacing));
                per -= minper;
                per *= (1f + minper / 1.1f);
                sb.Draw(bg, new Rectangle(Area.Width - 12 + _parent.Area.X, _parent.Area.Y + Convert.ToInt32(Area.Height * per) + 44, 8, 8), new Color(BgColourSecondry, 1f));
            }

            if (viewportSet)
            {

                sb.End();
                Device.Viewport = _oldView;
                sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Controler.matrix);
            };
        }

        protected override void Render(ref SpriteBatch sb)
        {
            if (Area.Height == 0)
                return;

            _isScrolable = false;
            
            Point listboxOffset = new Point(Area.X, Area.Y);
            sb.Draw(bg, Area, TextColourPrimary);
            sb.Draw(bg, new Rectangle(Area.X, Area.Y, Area.Width, 2), BgColourSecondry);
            sb.Draw(bg, new Rectangle(Area.X, Area.Y, 2, Area.Height), BgColourSecondry);
            sb.Draw(bg, new Rectangle(Area.X, Area.Y + Area.Height - 2, Area.Width, 2), BgColourSecondry);
            sb.Draw(bg, new Rectangle(Area.X + Area.Width - 2, Area.Y, 2, Area.Height), BgColourSecondry);

            if (Collapsible)
                RenderCollapsible(ref sb);
            else
                RenderAllItems(ref sb);
        }
    }
}
