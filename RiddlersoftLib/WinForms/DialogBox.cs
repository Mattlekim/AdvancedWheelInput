using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


using Riddlersoft.Core.Input;

namespace WinForms
{
    public class DialogBox : Form
    {
        WinLable wlText;

        List<Button> bntFunctions = new List<Button>();

        private Action<object> _sender;
        private float _timeOut;
        private bool _useTimeOut = false;
        /// <summary>
        /// the text of the last button pressed
        /// </summary>
        private string _optionSelected = null;
        public string Description { get { return wlText.Text; } set { wlText.Text = value; } }

        public DialogBox(Action<object> sender, string title, string description, List<string> buttons, float timeout) : base(new Rectangle(300, 300, 400, 400), null)
        {
            Units = MesurementUnit.Px;
            Movable = false;
            _sender = sender;
            Componates.Clear();//remove any old compaontas
            bntFunctions.Clear();
            Title = title; //set the title
            _optionSelected = null; 
            wlText = new WinLable(new Rectangle(50, 50, 0, 0), this) //add the descrition to the lable
            {
                Text = description,
            };

            float buttonSpacing = 300 / ((float)buttons.Count + 1); //calculate spacing between buttons
            for (int i = 0; i < buttons.Count; i++)
            {
                bntFunctions.Add(new Button(new Rectangle((int)(50 + buttonSpacing * i), 200, 90, 50), this) //add all buttons
                {
                    Text = buttons[i],
                    id = buttons[i],
                    CloseParent = true,
                    OnLeftClick = (object parent, Point p) =>
                    {
                        _optionSelected = (parent as Button).Text; //set the text for the button presed
                    }
                });
            }


            Componates.Add(wlText); //add the label
            for (int i = 0; i < bntFunctions.Count; i++)
                Componates.Add(bntFunctions[i]); //add the buttons
            if (_timeOut > 0)
                _useTimeOut = true;
            else
                _useTimeOut = false;
            _timeOut = timeout;

        }

        public override void DeActivate()
        {
            if (_sender != null)
                _sender(_optionSelected);
            base.DeActivate();
        }

        protected virtual void Update(float dt)
        {
            if (_useTimeOut)
                _timeOut -= dt;

            if (KeyboardAPI.IsKeyPressed(Keys.Escape) || _useTimeOut && _timeOut <= 0)
            {
                DeActivate();
                
            }
        }
        
    }
}
