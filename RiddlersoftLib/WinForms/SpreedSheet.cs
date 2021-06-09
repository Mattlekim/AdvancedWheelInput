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
    public class SpreedSheet : Componate
    {
        public struct Row
        {
            public List<String> Columns;

            public Row(List<string> columns)
            {
                Columns = columns;
            }
        }

        private List<int> _rowWidths = new List<int>();

        public Row Titles { get; protected set; }
        public List<Row> Data = new List<Row>();
        private int _spacing = 40;

        public Row GetSelection()
        {
            return Data[_selectedOption];
        }

        public void DeleteSelected()
        {
            if (_selectedOption < 0 || _selectedOption >= Data.Count)
                return; //make sure we can delete the record

            Data.RemoveAt(_selectedOption);
            _selectedOption = -1; //set selection to 0
        }

        public SpreedSheet(Rectangle area, Componate parentcomponate, List<string> titles) : base(area, parentcomponate)
        {
            Titles = new Row(titles);
            for (int i =0; i < titles.Count; i++)
                _rowWidths.Add(Convert.ToInt32(Font.MeasureString(titles[i]).X + 20));
        }

        public SpreedSheet(Rectangle area, Componate parentcomponate, List<string> titles, List<int> titleWidths) : base(area, parentcomponate)
        {
            Titles = new Row(titles);
            _rowWidths = titleWidths;
        }

        private int _highlightedOption;
        private int _selectedOption;
        /// <summary>
        /// returns the current selected option
        /// </summary>
        public int CurrentSelection {  get { return _selectedOption; } }
        protected override void Update(float dt)
        {
            Point p = MouseTouch.Position.ToPoint();
            if (_areaCurrent.Contains(p)) //if mouse or touch point within area
            {
                _highlightedOption = (p.Y - _areaCurrent.Y) / _spacing - 1;
                
                if (MouseTouch.LButtonClick)
                {
                    _selectedOption = _highlightedOption;
                }
            }
        }

        protected override void Render(ref SpriteBatch sb)
        {
           
            sb.Draw(bg, _areaCurrent, Controler.TextBoxColourBG);
            sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, _areaCurrent.Width, 2), Controler.SecondryBG);
            sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, 2, _areaCurrent.Height), Controler.SecondryBG);
            sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y + _areaCurrent.Height - 2, _areaCurrent.Width, 2), Controler.SecondryBG);
            sb.Draw(bg, new Rectangle(_areaCurrent.X + _areaCurrent.Width - 2, _areaCurrent.Y, 2, _areaCurrent.Height), Controler.SecondryBG);

            int spreadsheetOffset = 0;
            sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, _areaCurrent.Width, _spacing), BgColourPrimary);
            for (int i =0; i < Titles.Columns.Count; i++)
            {
                sb.DrawString(Font, Titles.Columns[i], new Vector2(_areaCurrent.X + spreadsheetOffset + 5, _areaCurrent.Y), TextColourPrimary);
                spreadsheetOffset += _rowWidths[i];
            }

            spreadsheetOffset = 40;
            int xoffset;
            for (int rowCounter = 0; rowCounter < Data.Count; rowCounter++)
            {
                xoffset = 0;
                for (int i = 0; i < Titles.Columns.Count; i++)
                {
                    if (rowCounter == _highlightedOption)
                        sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y + spreadsheetOffset, _areaCurrent.Width, _spacing), BgColourSecondry * .5f);
                    else
                    if (rowCounter == _selectedOption)
                        sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y + spreadsheetOffset, _areaCurrent.Width, _spacing), BgColourPrimary * .8f);
                    else
                        if (rowCounter % 2 == 0)
                            sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y + spreadsheetOffset, _areaCurrent.Width, _spacing), Color.White * .05f);

                    sb.DrawString(Font, Data[rowCounter].Columns[i], new Vector2(_areaCurrent.X + 5 + xoffset, _areaCurrent.Y + spreadsheetOffset), TextColourPrimary);
                    xoffset += _rowWidths[i];
                }
                spreadsheetOffset += 40;

            }
            spreadsheetOffset = 0;

            sb.Draw(bg, new Rectangle(_areaCurrent.X + spreadsheetOffset, _areaCurrent.Y, 2, _areaCurrent.Height), BgColourSecondry);
            for (int i = 0; i < _rowWidths.Count; i++)
            {
                spreadsheetOffset += _rowWidths[i];
                sb.Draw(bg, new Rectangle(_areaCurrent.X + spreadsheetOffset, _areaCurrent.Y, 2, _areaCurrent.Height), BgColourSecondry);
            }
          
        }
    }
}
