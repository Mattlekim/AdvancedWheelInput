using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Riddlersoft.Core;
using Riddlersoft.Core.Input;
using Riddlersoft.Core.Screen;

namespace WinForms
{
    /// <summary>
    /// the main windows form controler
    /// </summary>
    public class Controler
    {
        /// <summary>
        /// list of all activated componates
        /// </summary>
        public static List<Componate> Componates = new List<Componate>();

        public static Componate ComponateInFocase = null;

        public static bool Activate = true;

        public static Color PrimaryBG = new Color(30, 61, 154) * .8f;
        public static Color SecondryBG = new Color(0, 148, 255) * .8f;

        public static Color PrimaryText = Color.White;
        public static Color SecondryText = new Color(0, 122, 204);

        public static Color TextBoxColourBG = new Color(51, 51, 55) * .8f;

        public static float DefaultFade = .8f;

        public static float MinScale = .1f, MaxScale = 1.1f;
        public static float ScaleSpeed = 1f;

        public static Texture2D XIcon;

        public static int BoarderSize = 2;

        public static Texture2D BackGround;

        public static MsgDialog MessageBox = new MsgDialog(null);

        private static bool _hasLeftClicked = false;

        public static ContentManager Content;

        private static List<Layout> _layouts = new List<Layout>();
        private static bool _haveSetUpLayouts = false;

        private List<string> _activeComponateIds = new List<string>();
        private List<Componate> _activeCompoants = new List<Componate>();

        public static Matrix matrix = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        public static void AddLayout(Layout layout)
        {
            _layouts.Add(layout);
            if (_haveSetUpLayouts)
                layout.Intitalize();
        }

        private static void IntitalizeLayouts()
        {
            foreach (Layout layout in _layouts)
                layout.Intitalize();
        }
        /// <summary>
        /// this bool retuns true if a UI componate has been clicked on with this update scyle
        /// </summary>
        public static bool HasLeftClicked { get { return _hasLeftClicked; } }


        public static void LoadContent(ContentManager content)
        {
            XIcon = null;
            BackGround = new Texture2D(_device, 1, 1);
            BackGround.SetData<Color>(new Color[] { Color.White });

            Componate.Initalize(BackGround, XIcon, _device, content);

            Content = content;
            LoadMsgBox();
        }

        public static void LoadContent(ContentManager content, string xicon, string bg)
        {
            XIcon = content.Load<Texture2D>(xicon);
            BackGround = content.Load<Texture2D>(bg);

            Componate.Initalize(BackGround, XIcon, _device, content);

            Content = content;
            LoadMsgBox();
        }

        private static void LoadMsgBox()
        {

            Componates.Add(MessageBox);
        }

        private static void OnResolutionChange(ScreenFormatter screen)
        {

        }

        private static bool _isIntizalized = false;
        private static GraphicsDevice _device;
        public static void Intizalize(GraphicsDevice device)
        {
            if (!ScreenManiger.IsInitalized)
                throw new Exception("Need screen to be intalized");

            if (_isIntizalized)
                throw new Exception("Cannot be intizlized more than once");
            //hook events
            MouseTouch.OnLeftMouseClick += Controler.OnLeftClick;
            MouseTouch.OnLeftMouseDown += OnLeftButtonDown;
            MouseTouch.OnRightMouseClick += Controler.OnRightClick;

            _isIntizalized = true;
            ScreenManiger.OnResolutionChange += OnResolutionChange;

            _device = device;
        }

        /// <summary>
        /// this is fired when a componate is click
        /// if componate type is gamewind this is skiped
        /// </summary>
        public static void TriggerComponateClicked(Componate c)
        {
            if (!(c is GameScreen))
                _hasLeftClicked = true;
        }

        public static bool Update(float dt)
        {
            _hasLeftClicked = false;

            if (!Activate)
                return false;

            if (!_haveSetUpLayouts)
            {
                _haveSetUpLayouts = true;
                IntitalizeLayouts();
            }
            if (ComponateInFocase != null)
                ComponateInFocase.InternalUpdate(dt);

            for (int i = 0; i < Componates.Count; i++) //update all componates
                Componates[i].InternalUpdate(dt);
            return true;
        }

        public static void OnLeftButtonDown(Vector2 pos)
        {
            Point p = new Point((int)pos.X, (int)pos.Y);

            if (ComponateInFocase != null)
            {
                if (!ComponateInFocase.ComponatsCheckLeftMouseDown(p))
                {
                    if (ComponateInFocase.Area.Contains(p))
                    {
                        ComponateInFocase.OnLeftMouseButtonDown(p);
                        return;
                    }
                }
                else
                    return;
            }

            for (int count = Componates.Count - 1; count >= 0; count--) //check in reverse order so as to always check top most forms and componates frits
            {
                if (Componates[count].id == "gs_EditorScreen")
                {

                }
                if (Componates[count].Active && !Componates[count].IsFocuse)
                    if (!Componates[count].ComponatsCheckLeftMouseDown(p))
                    {
                        if (ComponateInFocase != null)
                            if (!Componates[count].AllowToRunWhenNotInFocuse || Componates[count].IsFocuse)
                                continue;

                        if (Componates[count].Area.Contains(p))
                        {
                            Componates[count].OnLeftMouseButtonDown(p);
                            break;
                        }
                    }
                    else
                        break;


            }

        }

        public static void OnLeftClick(Vector2 pos, Ray r)
        {
            Point p = new Point((int)pos.X, (int)pos.Y);

            foreach (Componate c in Componates)
            {
                c.ClickReset();
            }
                    

            if (ComponateInFocase != null)
            {
                if (!ComponateInFocase.ComponatsCheckLeftClick(p))
                {
                    if (ComponateInFocase.Area.Contains(p))
                    {
                        ComponateInFocase.OnLeftMouseClick(p);
                        return;
                    }
                }
                else
                    return;
            }

            for (int count = Componates.Count - 1; count >= 0; count--) //check in reverse order so as to always check top most forms and componates frits
            {

                if (Componates[count].Active && !Componates[count].IsFocuse)
                    if (!Componates[count].ComponatsCheckLeftClick(p))
                    {
                        if (ComponateInFocase != null)
                            if (!Componates[count].AllowToRunWhenNotInFocuse)
                                continue;

                        if (Componates[count].Area.Contains(p))
                        {
                            Componates[count].OnLeftMouseClick(p);
                            break;
                        }
                    }
                    else
                        break;

            }
        }

        public static void OnRightClick(Vector2 pos, Ray r)
        {
            Point p = new Point((int)pos.X, (int)pos.Y);

            if (ComponateInFocase != null)
            {
                if (!ComponateInFocase.ComponatsCheckRightClick(p))
                {
                    if (ComponateInFocase.Area.Contains(p))
                    {
                        if (ComponateInFocase.OnRightClick != null)
                            ComponateInFocase.OnRightClick(ComponateInFocase, p);
                        return;
                    }
                }
                else
                    return;
            }


            for (int count = Componates.Count - 1; count >= 0; count--) //check in reverse order so as to always check top most forms and componates frits
            {
                if (Componates[count].Active && !Componates[count].IsFocuse)
                    if (!Componates[count].ComponatsCheckRightClick(p))
                    {
                        if (ComponateInFocase != null)
                            if (!Componates[count].AllowToRunWhenNotInFocuse || Componates[count].IsFocuse)
                                continue;

                        if (Componates[count].Area.Contains(p))
                        {
                            Componates[count].OnRightMouseClick(p);
                            break;
                        }
                    }
                    else
                        break;
            }

        }

        public static void Render(ref SpriteBatch sb)
        {
            if (ComponateInFocase != null)
            {
                for (int i = 0; i < Componates.Count; i++)
                    Componates[i].InternalRender(ref sb, true);

                ComponateInFocase.InternalRender(ref sb);
            }
            else
                for (int i = 0; i < Componates.Count; i++)
                    Componates[i].InternalRender(ref sb);

        }

        public static void RenderPrimitivs(GraphicsDevice device, BasicCamera camera)
        {
            for (int i = 0; i < Componates.Count; i++)
                Componates[i].InternalRenderPrimitives(device, camera);
        }

        public static List<Componate> OldFocuse = new List<Componate>();

        /// <summary>
        /// trys to show the componate with the given id
        /// </summary>
        /// <param name="id">the id of the componate to show</param>
        /// <param name="ob">the data you want to pass in</param>
        public static bool ShowComponate(string id, object ob = null, bool isfocuse = false)
        {
            for (int i = 0; i < Componates.Count; i++)
            {
                if (Componates[i].id == id)
                {
                    Componates[i].Activate(isfocuse, ob);
                    return true;
                }
                else
                    if (Componates[i].ShowComponate(id, ob, isfocuse))
                    return true;
            }

#if DEBUG
            throw new Exception($"Componate {id} Not Found");
#endif
            return false;
        }

        /// <summary>
        /// trys to hide the componate with the given id
        /// </summary>
        /// <param name="id">the id of the componate to hide</param>
        public static bool HideComponate(string id)
        {
            for (int i = 0; i < Componates.Count; i++)
            {
                if (Componates[i].id == id)
                {
                    Componate cm = Componates[i];
                    cm.DeActivate();
                    return true;
                }
                else
                    if (Componates[i].HideComponate(id))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// finds a compoante using the given id
        /// </summary>
        /// <param name="name">the id of the compoant to show</param>
        /// <returns></returns>
        public static Componate FindComponateById(string name)
        {
            for (int i = 0; i < Componates.Count; i++)
            {
                if (Componates[i].id == name)
                    return Componates[i];
                else
                {
                    Componate returnValue = Componates[i].FindComponateById(name);
                    if (returnValue != null)
                        return returnValue;
                }
            }

#if DEBUG
            throw new Exception($"Componate {name} not found");
#endif
            return null;
        }

        public static T FindComponateById<T>(string name) where T: Componate
        {
            Componate c = FindComponateById(name);

            if (c == null)
                return null;
            if (c is T)
                return c as T;

            throw new Exception("T is wrong type");
        }

        public static Componate Add(Componate componate)
        {
            Componates.Add(componate);
            return Componates[Componates.Count - 1];

        }


        private static int _DialogId = 0;

        /// <summary>
        /// shows a error dialog
        /// </summary>
        /// <param name="title">the title of the mesage box</param>
        /// <param name="description">the description</param>
        public static void ShowErrorDialog(string title, string description)
        {
            ShowErrorDialog(title, description, 0f, null);
        }

        /// <summary>
        /// shows a error dialog
        /// </summary>
        /// <param name="title">the title of the mesage box</param>
        /// <param name="description">the description</param>
        /// <param name="timeout">how long you want the box to be displayed before it dissapears</param>
        public static void ShowErrorDialog(string title, string description, float timeout)
        {
            ShowErrorDialog(title, description, timeout, null);
        }

        /// <summary>
        /// shows a error dialog
        /// </summary>
        /// <param name="title">the title of the mesage box</param>
        /// <param name="description">the description</param>
        /// <param name="timeout">how long you want the box to be displayed before it dissapears</param>
        /// <param name="_sender">a funcion to call when the box is closed</param>
        public static void ShowErrorDialog(string title, string description, float timeout, Action<object> _sender)
        {
            ShowDialog(title, description, new List<string>() { "Ok" }, timeout, _sender);
        }

        private static void ShowDialog(string title, string description, List<string> options, float timeout, Action<object> _sender)
        {
            if (options == null || options.Count == 0)
                throw new Exception("Not a valid amount of buttons");

            DialogBox db = new DialogBox(_sender, title, description, options, timeout);
            db.id = $"db_{_DialogId}[]#"; //set the id
            db.OnClosed = ((object sender) =>
            {
                Componate co = sender as Componate;
                for (int i = 0; i < Componates.Count; i++)
                    if (Componates[i].id == co.id)
                    {
                        Componates.RemoveAt(i);
                        break;
                    }
            });
            Componates.Add(db);
            _DialogId++;
        }

        /// <summary>
        /// shows a mesage dialog with buttons
        /// </summary>
        /// <param name="title">the title of the mesage box</param>
        /// <param name="description">the description</param>
        /// <param name="options">a list of button options</param>
        /// <param name="timeout">how long you want the box to be displayed before it dissapears</param>
        /// <param name="_sender">a funcion to call when the box is closed</param>
        public static void ShowMessageDialog(string title, string description, List<string> options, float timeout, Action<object> _sender)
        {
            ShowDialog(title, description, options, timeout, _sender);
        }

        /// <summary>
        /// shows a mesage dialog with buttons
        /// </summary>
        /// <param name="title">the title of the mesage box</param>
        /// <param name="description">the description</param>
        /// <param name="options">a list of button options</param>
        /// <param name="_sender">a funcion to call when the box is closed</param>
        public static void ShowMessageDialog(string title, string description, List<string> options, Action<object> _sender)
        {
            ShowDialog(title, description, options, 0f, _sender);
        }
    }
}
