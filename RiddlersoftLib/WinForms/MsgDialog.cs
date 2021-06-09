using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Riddlersoft.Core.Input;

namespace WinForms
{
    /// <summary>
    /// Shows a new message box
    /// </summary>
    public class MsgDialog: Form
    {
        WinLable wlText;

        List<Button> bntFunctions = new List<Button>();

        private float _timeOut = 0;
        private bool _useTimeOut = false;

        private Action<object, Point> _responce;

        public MsgDialog(Componate parentcomponate) :
            base(new Rectangle(305, 305, 300, 300), parentcomponate)
        {
            Units = MesurementUnit.Px;
            Title = "";
            Active = false;
            _visible = false;
            Docable = false;
            Movable = false;
            HaveCloseButton = false;
        }

        /// <summary>
        /// shows a new mesage box
        /// </summary>
        /// <param name="title">The Title</param>
        /// <param name="description">the description</param>
        /// <param name="buttons">list of buttons</param>
        /// <param name="ResponeHandle">return function</param>
        public void ShowDialog(string title, string description,
            List<string> buttons, Action<object, Point> ResponeHandle)
        {
            Componates.Clear();//remove any old compaontas
            bntFunctions.Clear();
            Title = title; //set the title
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
                });

                if (ResponeHandle != null) //if we have a retun fucntion
                    bntFunctions[i].OnLeftClick = ResponeHandle;
            }

            Componates.Add(wlText); //add the label
            for (int i = 0; i < bntFunctions.Count; i++)
                Componates.Add(bntFunctions[i]); //add the buttons
            _useTimeOut = false;
            _timeOut = 0;
            _responce = ResponeHandle;
            this.Activate(true);
        }

        public void ShowDialog(string title, string description,
            List<string> buttons, Action<object, Point> ResponeHandle, float timeOut)
        {
            ShowDialog(title, description, buttons, ResponeHandle);
            _timeOut = timeOut;
            _useTimeOut = true;
        }

        /// <summary>
        /// shows a standard error msg with no responce handling and one ok button
        /// </summary>
        /// <param name="title">The Title</param>
        /// <param name="description">the description</param>
        public void ShowErrorDialog(string title, string description)
        {
            
            Componates.Clear(); //remove old componates
            bntFunctions.Clear();
            Title = title; //set the title
            wlText = new WinLable(new Rectangle(50, 50, 0, 0), this) //add the descrition to the lable
            {
                Text = description,
            };

            float buttonSpacing = 150 - 45; //calculate spacing between buttons

            bntFunctions.Add(new Button(new Rectangle((int)(50 + buttonSpacing), 200, 90, 50), this) //add all buttons
            {
                Text = "Ok",
                CloseParent = true,
            });

            
            Componates.Add(wlText); //add the label
            for (int i = 0; i < bntFunctions.Count; i++)
                Componates.Add(bntFunctions[i]); //add the buttons

            this.Activate(true);
        }

        protected virtual void Update(float dt)
        {
            if (_useTimeOut)
                _timeOut -= dt;

            if (KeyboardAPI.IsKeyPressed(Keys.Escape) || _useTimeOut && _timeOut <= 0)
            {
                DeActivate();
                if (_responce != null)
                    _responce(null, Point.Zero);
            }
        }


    }
}
