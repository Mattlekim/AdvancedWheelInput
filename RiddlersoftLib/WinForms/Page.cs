using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace WinForms
{
    /// <summary>
    /// a page for a book
    /// </summary>
    public class Page : Form
    {
        public string Title { get; set; } = "page";

        public Page(Componate parentcomponate, string title) : base(new Rectangle(0,4,100,96), parentcomponate)
        {
            Title = title;
            Movable = false;
            HaveTitleBar = false;
            HaveCloseButton = false;
            Units = MesurementUnit.Percentage;
           // _internalRender = false;
        }

      
    }
}
