using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WinForms
{
    public class Book : Componate
    {
        public List<Page> Pages { get; protected set; } = new List<Page>();

        public int CurrentPage { get; protected set; } = 0;

        public Action<Page> OnPageChange;

        private int _lastBntPos = 0;

        public Book(Rectangle area, Componate parentcomponate) : base(area, parentcomponate)
        {
            this._internalRender = false;
           // BgColourPrimary = Color.Red;
            AddPage("Page 1");
        }

        /// <summary>
        /// adds a new page to the book
        /// </summary>
        /// <param name="name">the name of the page to add</param>
        public void AddPage(string name)
        {
            //add the page
            Pages.Add(new Page(this, name)
            {
               
            });
            AddPageButton(name);
        }

        private void AddPageButton(string name)
        {
            int bntwidth = Convert.ToInt32(Font.MeasureString(name).X) + 6;
            Componates.Add(new Button(new Rectangle(_lastBntPos, 0, bntwidth, 24), this)
            {
                Units = MesurementUnit.Px,
                Text = name,
                CustomData = Pages.Count - 1,
                OnLeftClick = (object sender, Point p) =>
                {
                    Button bnt = sender as Button;
                    SelectPage(Convert.ToInt32(bnt.CustomData));
                },
            });
            _lastBntPos += bntwidth;
        }
        /// <summary>
        /// adds a new page to the book
        /// </summary>
        /// <param name="page">the page we want to add</param>
        public void AddPage(Page page)
        {
            Pages.Add(page);
            AddPageButton(page.Title);
        }

        /// <summary>
        /// repalce the given page with a new one
        /// </summary>
        /// <param name="pagenumber">the page to replace</param>
        /// <param name="page">the new page</param>
        public void ReplacePage(int pagenumber, Page page)
        {
            //make sure page is in range
            if (pagenumber >= 0 && pagenumber < Pages.Count)
            {
                Pages[pagenumber] = page;
            }

            Button bnt = Componates[pagenumber] as Button;
            bnt.Text = page.Title;
        }

        /// <summary>
        /// set the current page
        /// </summary>
        /// <param name="page"></param>
        public void SelectPage(int page)
        {
            if (Componates.Count > 0)
            {
                Button bnt = Componates[CurrentPage] as Button;
                bnt.ClearState();
            }
            //make sure page is in range
            if (page >= 0 && page < Pages.Count)
            {
                CurrentPage = page;
                if (OnPageChange != null)
                    OnPageChange(Pages[CurrentPage]);
            }
        }

        public override bool ComponatsCheckLeftClick(Point p)
        {

            if (!Pages[CurrentPage].ComponatsCheckLeftClick(p)) //check page compoantas for a click
            {
                if (Pages[CurrentPage].Area.Contains(p)) //if page is clicked
                    return true;
            }
            else
                return true;

            return base.ComponatsCheckLeftClick(p);
        }



        public override Componate FindComponateById(string name)
        {
            Componate result = base.FindComponateById(name);

            if (result == null)
            {
                foreach (Page p in Pages)
                {
                    result = p.FindComponateById(name);
                    if (result != null)
                        return result;
                }
            }
            return result;
        }
        protected override void Update(float dt)
        {
            //will not work due to order
            Pages[CurrentPage].InternalUpdate(dt);
            if (Componates.Count > 0)
            {
                Button bnt = Componates[CurrentPage] as Button;
                bnt.ForceSelected();
            }


        }

        protected override void Render(ref SpriteBatch sb)
        {
           
            Pages[CurrentPage].InternalRender(ref sb);
        
        }
    }
}
