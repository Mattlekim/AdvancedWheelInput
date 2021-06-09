using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Riddlersoft.Core;
using Riddlersoft.Core.Input;
using Riddlersoft.Core.Screen;
namespace WinForms
{

    public enum MesurementUnit { Px, Percentage };
    /// <summary>
    /// the base componate of all winforms
    /// all WinForms class will inherite from this one class
    /// </summary>
    public class Componate
    {
        public static Rectangle GetPercentageForRequiredPixels(Rectangle requiredPixels, Componate componate)
        {
            throw new Exception("Not implmented");
        }

        public bool AllowToRunWhenNotInFocuse = false;

        public static float MasterScale = 1;

        public Anchor _Anchor = Anchor.None;

        protected Vector2 ScrollOffset = Vector2.Zero;

        protected static ContentManager _content;
        /// <summary>
        /// used for creating instant drawing to screen
        /// </summary>
        protected static SpriteBatch InstantBatch;
        protected static Texture2D bg;
        protected static Texture2D _arrow;
        protected static GraphicsDevice Device;
        public MesurementUnit Units = MesurementUnit.Px;
        internal static void Initalize(Texture2D backGround, Texture2D arrow ,GraphicsDevice device, ContentManager content)
        {
            _arrow = arrow;
            bg = backGround;
            InstantBatch = new SpriteBatch(device);
            _content = content;
            Device = device;
            
        }

        public virtual void ClickReset()
        {
            foreach (Componate c in Componates)
                c.ClickReset();

        }

        protected virtual void LoadContent()
        {

        }
        /// <summary>
        /// the parent of this componate
        /// all locations will be relative to the parent
        /// </summary>
        protected Componate _parent = null;

        protected bool _verticalScrole = false;
        protected bool _horizontalScrole = false;

        public delegate void _PostRender(ref SpriteBatch sb, object sender);
        public _PostRender PostRender;

        public delegate void _postUpdate(Componate componate);
        public _postUpdate PostUpdate;

        /// <summary>
        /// retuns the center of the componate
        /// PERFORMANCE CAN BE GAINED HERE BY CHANGING THIS CODE
        /// </summary>
        protected Vector2 _center
        {
            get
            {
                return new Vector2(_areaOriginal.Width / 2, _areaOriginal.Height / 2); //NOT EFFICENT
            }
        }
        

        /// <summary>
        /// returns the current position of the window as a Vector2
        /// </summary>
        public Vector2 Position {  get { return new Vector2(Area.X, Area.Y); } }

        protected float Scale = 1;
        protected float ScaleTimer; 
        /// <summary>
        /// the current area for the componate base on the parent
        /// </summary>
        protected Rectangle _areaCurrent;
        protected Rectangle _areaOriginal;
        public Rectangle Area { get { return _areaCurrent; }}

        public Rectangle AreaOriginal { get { return _areaOriginal; } set { _areaOriginal = value; } }

        public Color BgColourPrimary = Controler.PrimaryBG;
        public Color BgColourSecondry = Controler.SecondryBG;

        public Color TextColourPrimary = Controler.PrimaryText;
        public Color TextColourSecondry = Controler.SecondryText;

        protected int _tabIndex = -1;
        public int TabIndex {  get { return _tabIndex; } }

        public List<Componate> Componates = new List<Componate>();
        public delegate void InternalEvent(object componate);

        /// <summary>
        /// the function to call when the dialog box is closed
        /// public void functionname(string id);
        /// </summary>
        /// <param name="id">the text of the button clicked</param>
        public Action<object, Point> OnLeftClick;

        public Action<object , Point> OnLeftMouseDown;


        public bool UseParentClickEvent = false;

        public Action<object, Point> OnRightClick;

        

        public InternalEvent OnClosed;

        public InternalEvent OnShowen;

        public InternalEvent MouseExit, MouseEnter;


        public bool Active = true;
        protected bool _visible = true;

        public bool DeactivateNextUpdateCycle = false;

        public bool _internalRender = true;

        public bool Boarderless = false;

        /// <summary>
        /// a variable that can be used to store custom data
        /// </summary>
        public object CustomData;

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                if (value == false)
                    Active = false;
            }
        }

        /// <summary>
        /// sets the position relative to any parent compoanteas,
        /// not relative to the screen
        /// </summary>
        /// <param name="position">the relative position to set this compoante to</param>
        public void SetPosition(Vector2 position)
        {
            _areaOriginal.X = (int)position.X;
            _areaOriginal.Y = (int)position.Y;
        }

        /// <summary>
        /// overides the current partent with a new one
        /// </summary>
        /// <param name="newParent">the new parent class</param>
        public void SetParent(Componate newParent)
        {
            _parent = newParent;
        }

        public void Resize(int newWidth, int newHeight)
        {
            _areaOriginal.Width = newWidth;
            _areaOriginal.Height = newHeight;
        }

        public bool Hidden = false;

        private bool ShowDescription = false;
        public string Description;
        private float MouseOverTimer = 0;
        
        public SpriteFont Font;
        public SpriteFont FontLarge;
        public string id;



        public bool IsFocuse { get; private set; } = false;
        public Componate(Rectangle area, Componate parentcomponate)
        {
            _areaOriginal = area;
            _areaCurrent = area;
            _parent = parentcomponate;
            Font = Fonts.VerySmall;
            FontLarge = Fonts.Small;
        }

        public virtual void Activate(bool isfocuse, object customdata)
        {
            Activate(isfocuse);
        }

        public virtual void Activate(bool isfocuse)
        {
            if (isfocuse)
            {
                if (Controler.ComponateInFocase != null)
                {
                    Controler.ComponateInFocase.IsFocuse = false;
                }

                IsFocuse = isfocuse;

                Controler.ComponateInFocase = this;
            }
            Activate();
        }

        public virtual void Activate()
        {
            if (!Hidden)
                _visible = true;
            Active = true;

            if (OnShowen != null)
                OnShowen(this);
        }

        public virtual void OnMouseEnter()
        {
            if (MouseEnter != null)
                MouseEnter(this);
        }

        public virtual void OnMouseExit()
        {
            if (MouseExit != null)
                MouseExit(this);
        }

        public virtual void DeActivate()
        {
            if (!Active)
                return;

            _visible = false;
            Active = false;
            if (IsFocuse)
                Controler.ComponateInFocase = null;
            IsFocuse = false;
            OnClose();
        }

        public Point LastMouseClickedPostion { get; protected set; }

        public virtual bool OnLeftMouseButtonDown(Point p)
        {
            LastMouseClickedPostion = p;
            
            if (OnLeftMouseDown != null)
            {
                OnLeftMouseDown(this, p);
                
            }
            return true;
        }


        public virtual bool OnLeftMouseClick(Point p)
        {
           
            LastMouseClickedPostion = p;
            Controler.TriggerComponateClicked(this);
            if (OnLeftClick != null)
            {
                OnLeftClick(this, p);
                return true;
            }
            return false;
        }

        public virtual bool OnRightMouseClick(Point p)
        {
            if (OnRightClick != null)
            {
                OnRightClick(this, p);
                return true;
            }
            return false;
        }

        /// <summary>
        /// fires when the form is closed
        /// </summary>
        protected virtual void OnClose()
        {
            if (OnClosed != null)
                OnClosed(this);

        }

        private bool _loadedContent = false;

        private void Resize()
        {
            if (!Visible)
                return;

            if (_parent == null)
            {
                if (Units == MesurementUnit.Px)
                {
                    _areaCurrent = _areaOriginal;

                    switch (_Anchor)
                    {
                        case Anchor.Left:
                            _areaCurrent.X = 0;
                            break;

                        case Anchor.Right:
                            _areaCurrent.X = ScreenManiger.Format.VirtualWidth - _areaOriginal.Width;
                            break;

                        case Anchor.Top:
                            _areaCurrent.Y = 0;
                            break;

                        case Anchor.Bottom:
                            _areaCurrent.Y = ScreenManiger.Format.VirtualHeight - _areaOriginal.Height;
                            break;
                    }
                }
                else
                {
                    float width, height;
                    width = ScreenManiger.Format.VirtualWidth;
                    height = ScreenManiger.Format.VirtualHeight;

                    _areaCurrent.X = Convert.ToInt32(width * (float)_areaOriginal.X / 100f);
                    _areaCurrent.Y = Convert.ToInt32(height * (float)_areaOriginal.Y / 100f);
                    _areaCurrent.Width = Convert.ToInt32(width * (float)_areaOriginal.Width / 100f);
                    _areaCurrent.Height = Convert.ToInt32(height * (float)_areaOriginal.Height / 100f);
                }
            }
            else
            {
                if (Units == MesurementUnit.Px)
                {
                    _areaCurrent.X = _areaOriginal.X;
                    _areaCurrent.Y = _areaOriginal.Y;
                    switch (_Anchor)
                    {
                        case Anchor.Left:
                            _areaCurrent.X = 0;
                            break;

                        case Anchor.Right:
                            _areaCurrent.X = _parent.Area.Width - _areaOriginal.Width;
                            break;

                        case Anchor.Top:
                            _areaCurrent.Y = 0;
                            break;

                        case Anchor.Bottom:
                            _areaCurrent.Y = _parent.Area.Height - _areaOriginal.Height;
                            break;
                    }

                    _areaCurrent.X += _parent.Area.X;
                    _areaCurrent.Y += _parent.Area.Y;
                    _areaCurrent.Width = _areaOriginal.Width;
                    _areaCurrent.Height = _areaOriginal.Height;
                }
                else
                {
                    float width, height;
                    width = _parent._areaCurrent.Width;
                    height = _parent._areaCurrent.Height;
                    _areaCurrent.X = Convert.ToInt32(width * ((float)_areaOriginal.X / 100f)) + _parent._areaCurrent.X;
                    _areaCurrent.Y = Convert.ToInt32(height * ((float)_areaOriginal.Y / 100f)) + _parent._areaCurrent.Y;
                    _areaCurrent.X += Convert.ToInt32(_parent.ScrollOffset.X);
                    _areaCurrent.Y += Convert.ToInt32(_parent.ScrollOffset.Y);
                    _areaCurrent.Width = Convert.ToInt32(width * ((float)_areaOriginal.Width / 100f));
                    _areaCurrent.Height = Convert.ToInt32(height * ((float)_areaOriginal.Height / 100f));
                }
            }
        }

        protected virtual void PreUpdate() { }

        public bool InternalUpdate(float dt)
        {
            if (this.AllowToRunWhenNotInFocuse || IsFocuse)
                for (int i = 0; i < Componates.Count; i++)
                    Componates[i].AllowToRunWhenNotInFocuse = true;

            if (id == "gs_EditorScreen")
            {

            }

            PreUpdate();
            if (!_loadedContent)
            {
                _loadedContent = true;
                LoadContent();
            }

            Resize(); //resize this compoante

            if (!Active)
            {
                for (int i = 0; i < Componates.Count; i++)
                    Componates[i].Resize(); //have to resize componates if they are visible
                return false;
            }

            if (DeactivateNextUpdateCycle)
            {
                DeactivateNextUpdateCycle = false;
                DeActivate();
                return false;
            }

            for (int i = 0; i < Componates.Count; i++)
                Componates[i].InternalUpdate(dt);

            if (Controler.ComponateInFocase != null)
                if (!IsFocuse && !AllowToRunWhenNotInFocuse)
                    return true;

            if (PostUpdate != null)
                PostUpdate(this);
            

            Point p = new Point((int)MouseTouch.Position.X, (int)MouseTouch.Position.Y);
            if (_areaCurrent.Contains(p))
            {
                if (MouseOverTimer == 0)
                    OnMouseEnter();
                MouseOverTimer += dt;

            }
            else
            {
                if (MouseOverTimer > 0)
                    OnMouseExit();
                MouseOverTimer = 0;
            }

            

            Update(dt);
            return true;
        }

        protected virtual void Update(float dt) { }

        /// <summary>
        /// check each sub componate for a click
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual bool ComponatsCheckLeftClick(Point p)
        {
            for (int i = Componates.Count - 1; i >= 0; i--) //we start at the last record because that one will be the topmost drawn
                if (Componates[i].Active && !Componates[i].IsFocuse)
                {
                   
                    if (!Componates[i].ComponatsCheckLeftClick(p)) //check the componate for a click
                    {
                        if (Componates[i].DetectClick())
                            if (Componates[i].Area.Contains(p)) //if the current mouse click is in the bounds of the componate
                            {
                                if (!Componates[i].OnLeftMouseClick(p)) //if the componate does not have an event on click
                                    if (OnLeftClick != null && Componates[i].UseParentClickEvent) //if parent has an event
                                        OnLeftClick(Componates[i], p); //use the parents event
                                return true;
                            }
                    }
                    else
                        return true;
                }
            return false;
        }

        private bool DetectClick()
        {
            if (!Active)
                return false;

            if (Controler.ComponateInFocase != null)
                if (!this.AllowToRunWhenNotInFocuse)
                    return false;
            return true;
        }

        public virtual bool ComponatsCheckLeftMouseDown(Point p)
        {
            for (int i = Componates.Count - 1; i >= 0; i--) //we start at the last record because that one will be the topmost drawn
            {
                if (Componates[i].Active && !Componates[i].IsFocuse)
                {
                    if (!Componates[i].ComponatsCheckLeftMouseDown(p)) //check the componate for a click
                    {
                        if (Componates[i].DetectClick())
                            if (Componates[i].Area.Contains(p)) //if the current mouse click is in the bounds of the componate

                            {
                                if (!Componates[i].OnLeftMouseButtonDown(p)) //if the componate does not have an event on click
                                    if (OnLeftMouseDown != null)
                                        OnLeftMouseDown(this, p); //use the parents event
                                return true;
                            }
                    }
                    else
                        return true;
                }
            }
            return false;
        }

        public bool ComponatsCheckRightClick(Point p)
        {
            for (int i = Componates.Count - 1; i >= 0; i--) //we start at the last record because that one will be the topmost drawn
                {
                if (Componates[i].Active && !Componates[i].IsFocuse)
                    if (!Componates[i].ComponatsCheckRightClick(p)) //check the componate for a click
                    {
                        if (Componates[i].DetectClick())
                            if (Componates[i]._areaCurrent.Contains(p)) //if the current mouse click is in the bounds of the componate
                            {
                                if (!Componates[i].OnRightMouseClick(p)) //if the componate does not have an event on click
                                    if (OnRightClick != null && UseParentClickEvent) //if parent has an event
                                        OnRightClick(Componates[i], p); //use the parents event
                                return true;
                            }
                    }
                    else
                        return true; //no need to check for any more componates
                }
            return false;
        }

        protected void RenderDescription(ref SpriteBatch sb)
        {
            if (MouseOverTimer > 1 && Description != null)
            {
                Vector2 size = Fonts.VerySmall.MeasureString(Description);
                size.X += 2;
                size.Y += 2;
                Point p = new Point(_areaCurrent.X, _areaCurrent.Y);
                sb.Draw(bg, new Rectangle(p.X, p.Y - 2 - (int)size.Y, (int)size.X, (int)size.Y), Color.White);
                sb.DrawString(Fonts.VerySmall, Description, new Vector2(p.X + 2, p.Y + 2 - size.Y), Color.Black);

                sb.Draw(bg, new Rectangle(p.X, p.Y - (int)size.Y - 2, 1, (int)size.Y), BgColourSecondry);
                sb.Draw(bg, new Rectangle(p.X, p.Y - (int)size.Y - 2, (int)size.X, 1), BgColourSecondry);
                sb.Draw(bg, new Rectangle(p.X + (int)size.X, p.Y - (int)size.Y - 2, 1, (int)size.Y), BgColourSecondry);
                sb.Draw(bg, new Rectangle(p.X, p.Y - 2, (int)size.X, 1), BgColourSecondry);

            }
        }

     
        public bool InternalRender(ref SpriteBatch sb, bool fadeout = false)
        {
            if (id == "debugger")
            {

            }

            if (!Visible)
                return false;
            
            if (_internalRender)
            {
                sb.Draw(bg, _areaCurrent, BgColourPrimary);

                if (!Boarderless)
                {
                    sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, _areaCurrent.Width, 2), BgColourSecondry);
                    sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, 2, _areaCurrent.Height), BgColourSecondry);
                    sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y + _areaCurrent.Height - 2, _areaCurrent.Width, 2), BgColourSecondry);
                    sb.Draw(bg, new Rectangle(_areaCurrent.X + _areaCurrent.Width - 2, _areaCurrent.Y, 2, _areaCurrent.Height), BgColourSecondry);
                }
            }

            Render(ref sb);

            if (this as GameScreen != null)
            {
                RenderComponates(ref sb, fadeout);
                RenderDescription(ref sb);
                if (PostRender != null)
                    PostRender(ref sb, this);
            }
            else
            {
                RenderComponates(ref sb);
                RenderDescription(ref sb);
                if (PostRender != null)
                    PostRender(ref sb, this);
                if (fadeout || !Active)
                    sb.Draw(bg, _areaCurrent, Color.Black * .7f);
            }

            
            return true;
        }

        protected virtual void Render(ref SpriteBatch sb) { }

        public bool InternalRenderPrimitives(GraphicsDevice device, BasicCamera camera)
        {
            if (!Visible)
                return false;

            for (int i = 0; i < Componates.Count; i++)
                Componates[i].InternalRenderPrimitives(device, camera);
            RenderPrimitives(device, camera);
            return true;
        }

        protected virtual void RenderPrimitives(GraphicsDevice device, BasicCamera camera) { }

        protected void RenderComponates(ref SpriteBatch sb, bool fadeout = false)
        {
            for (int i = 0; i < Componates.Count; i++)
                Componates[i].InternalRender(ref sb, fadeout);
        }
        

        /// <summary>
        /// add a new componate to this current componate
        /// </summary>
        /// <param name="componate"></param>
        public Componate AddComponate(Componate componate)
        {
            Componates.Add(componate);
            return componate;
        }

        /// <summary>
        /// finds a compoante using the given id
        /// </summary>
        /// <param name="name">the id of the compoant to show</param>
        /// <returns></returns>
        public virtual Componate FindComponateById(string name)
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

            return null;
        }

        /// <summary>
        /// try and show the componate with the given id
        /// </summary>
        /// <param name="id">the id to show</param>
        /// <returns></returns>
     /*   public bool ShowComponate(string cid)
        {
            string d = id;
            for (int i = 0; i < Componates.Count; i++)
            {
                if (Componates[i].id == cid)
                {
                    Componates[i].Activate();
                    return true;
                }
                else
                    if (Componates[i].ShowComponate(cid))
                    return true;
            }
            return false;
        }
        */
        /// <summary>
        /// try and show the componate with the given id
        /// </summary>
        /// <param name="id">the id to show</param>
        /// <returns></returns>
        public bool ShowComponate(string cid, object ob = null, bool isFocuse = false)
        {
            string d = id;
            for (int i = 0; i < Componates.Count; i++)
            {
                if (Componates[i].id == cid)
                {
                    Componates[i].Activate(isFocuse, ob);
                    return true;
                }
                else
                    if (Componates[i].ShowComponate(cid, ob, isFocuse))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// try and hide the componate with the given id
        /// </summary>
        /// <param name="id">the id to hide</param>
        /// <returns></returns>
        public bool HideComponate(string cid)
        {
            string d = id;
            for (int i = 0; i < Componates.Count; i++)
            {
                if (Componates[i].id == cid)
                {
                    Componates[i].DeActivate();
                    return true;
                }
                else
                    if (Componates[i].HideComponate(cid))
                    return true;
            }
            return false;
        }

        protected Rectangle ScaleRectangle(Rectangle input, float scale)
        {
         

            Vector2 center = new Vector2(input.Width / 2f + input.X, input.Height / 2f + input.Y);
            Vector2 scaled = new Vector2(input.Width / 2f, input.Height / 2f) * scale;
            
            return new Rectangle(Convert.ToInt32(center.X - scaled.X), Convert.ToInt32(center.Y - scaled.Y),
                Convert.ToInt32(scaled.X * 2f), Convert.ToInt32(scaled.Y * 2f));

        }
    }
}
