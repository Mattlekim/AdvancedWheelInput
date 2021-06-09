using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Riddlersoft.Core;
using Riddlersoft.Core.Input;

namespace Riddlersoft.Modules.Chat
{
    public class Conversation
    {
        public Color ScreenColour = Color.CornflowerBlue;
        public Color BackgroundColour = Color.Black;
        public Texture2D ScreenTexture = null;

        internal bool AllowChatToDraw = true;

        internal Rectangle _area;

        public Vector2 TextOffset = Vector2.Zero;

        internal Vector2 _TextDrawPos { get { return new Vector2(_area.X + _chat.TextPadding.X, _area.Y + _chat.TextPadding.Y); } }
        internal int _ChatTextWidth { get { return _area.Width - (int)_chat.TextPadding.X * 2; } }

        public ConversationType TypeOfConversation { get; set; } = ConversationType.Active;

        public static float TimeForAutoEndSnippit = 1f;
        protected float _autoEndTimer = 0;


        /// <summary>
        /// weather this conversation is current speed up
        /// this could be moved to the chat modual
        /// </summary>
        public bool SpeedUpText = false;

        protected List<Snippit> _snippits = new List<Snippit>();

        protected int _activeSnippit = -1;

        protected float _textProgress = 0;

        /// <summary>
        /// weather this conversation can be speed up or not
        /// </summary>
        public bool CanSpeedUpText = true;

        public bool SnippitFinished { get { return _textProgress >= 1; } } //returns true if the current snippit has finished

        /// <summary>
        /// the chat modual that this is attached to
        /// </summary>
        protected ChatModule _chat;

        public float TextSpeed { get; set; } = 4f;

        public bool ConversationEnded { get; private set; } = false;

        protected int _responceSelected = 0;
        /// <summary>
        /// the height of the body of text
        /// </summary>
        internal float _textHeight;
        /// <summary>
        /// hid the paramitless constructor
        /// </summary>
        internal Conversation() { }

        public Conversation(ChatModule parent)
        {
            _chat = parent;
        }

        public void ClearSnippits()
        {
            _snippits.Clear();
            ConversationEnded = false;
            _activeSnippit = -1;
            _textProgress = 0;
            SpeedUpText = false;
        }

        public void AddSnippit(string who, string text, bool autoEnd = false)
        {
            // text.InsertSpacing(_chat._FontSmall);
            Snippit sn = new Chat.Snippit(who, text);
            sn.AutoEndSnippit = autoEnd;
            AddSnippit(sn);
        }

        public void AddSnippit(string who, string text, Responce responce, bool autoEnd = false)
        {
            //     text.InsertSpacing(_chat._FontSmall);
            Responces r = new Chat.Responces();
            r.Add(responce);
            Snippit sn = new Chat.Snippit(who, text, r);
            sn.AutoEndSnippit = true;
            AddSnippit(sn);
        }

        public void AddSnippit(string who, string text, Responces responce, bool autoEnd = false)
        {
            //    text.InsertSpacing(_chat._FontSmall);
            Snippit sn = new Chat.Snippit(who, text, responce);
            sn.AutoEndSnippit = true;
            AddSnippit(sn);
        }



        public void AddSnippit(Snippit snippit)
        {
            //       snippit.Text.InsertSpacing(_chat._FontSmall);
            _snippits.Add(snippit);
            _chat._WriteLog($"Snippit Added {snippit.Text}");
        }

        /// <summary>
        /// starts the conversation  
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            if (_snippits.Count <= 0)
                return false;
            SpeedUpText = false;
            _activeSnippit = 0;
            _textProgress = 0;
            ConversationEnded = false;
            WrapActiveSnippit();
            return true;
        }

        public void SetSize(Rectangle size)
        {
            _area = size;
        }
        protected List<Responce> _responces;

        protected virtual void WrapActiveSnippit()
        {
            _autoEndTimer = 0f;//reset timer
            Snippit snippit = _snippits[_activeSnippit];
            snippit.WrapText(_chat.FontSmall, _ChatTextWidth);
            _snippits[_activeSnippit] = snippit;
            if (snippit.Responces == null)
                _responces = new List<Responce>();
            else
                _responces = snippit.Responces.GetResponces(); //get responces
            _textHeight = _chat.FontSmall.MeasureString(snippit.Text).Y; //get the height;

            _responceSelected = -1; //reset the responces flag
            if (_responces.Count > 0)
                _responceSelected = 0; //set to default responces if we have responces

            SetBubbleSize();

            _currentChar = 0;
            _lastChar = 0;
        }

        protected virtual void SetBubbleSize()
        { }

        public void JumpToEnd()
        {
            _textProgress = 2;
        }

        private void EndConversation()
        {
            _activeSnippit = -1;
            ConversationEnded = true;
            if (this is SpeachBubbles)
                _chat.TerminateConversation(true);
            else
                _chat.TerminateConversation(false);
        }

        private void NextSnippit()
        {
            _autoEndTimer = 0;
            if (_responces.Count > 0) //if we have responces
            {
                Responce r = _responces[_responceSelected];
                if (r.Next >= 0)
                    _activeSnippit = r.Next;
                else
                    _activeSnippit = -1;

                if (r._CallBack != null) //if there is a call back
                    r._CallBack(r);
            }
            else
                _activeSnippit++;

            if (_activeSnippit < 0 || _activeSnippit >= _snippits.Count) //if there is no snippits left
                EndConversation();
            else
            {
                _textProgress = 0;
                WrapActiveSnippit();

            }
        }

        public void Confirm()
        {
            if (_textProgress < 1)
            {
                if (CanSpeedUpText)
                    SpeedUpText = true;
            }
            else
                NextSnippit();
        }

        public void Update(float dt)
        {
            if (_activeSnippit == -1)
                return;

            if (delay > 0)
            {
                delay -= dt;
                return;
            }

            if (_currentChar != _lastChar)
            {
                if (_chat.OnNextChar != null)
                    _chat.OnNextChar(_currentChar);
                _lastChar = _currentChar;
            }
            float spd = TextSpeed * dt;

            if (SpeedUpText)
                spd *= 4f;

            _textProgress += dt / ((float)_snippits[_activeSnippit].Text.Length / spd);

            if (_responces.Count > 0) //make sure we have responces
            {
                if (TypeOfConversation == ConversationType.Passive)
                    throw new Exception("Cannot have a passive conversation with responces");
                if (KeyboardAPI.IsKeyPressed(Keys.Left) || GamePadApi.DPadDirectionClicked(DPadDirection.Left) ||
                    GamePadApi.LeftThumbStickFlickLeft)
                    if (_responceSelected > 0)
                        _responceSelected--;

                if (KeyboardAPI.IsKeyPressed(Keys.Right) || GamePadApi.DPadDirectionClicked(DPadDirection.Right) ||
                    GamePadApi.LeftThumbStickFlickRight)
                    if (_responceSelected < _responces.Count - 1)
                        _responceSelected++;


            }

            if (_textProgress >= 1)
            {
                _flasher += dt;
                if (_flasher > 1)
                    _flasher -= 2;
            }

            if (_responces.Count <= 1 && _textProgress > 1)
            {
                if (TypeOfConversation == ConversationType.Passive)
                    if (_snippits[_activeSnippit].Requirment != null)
                    {
                        if (_snippits[_activeSnippit].Requirment()) //check to see if requirment met
                        {
                            if (_snippits[_activeSnippit].Pause)
                                return;
                            Confirm();
                            return;
                        }
                    }
                    else
                    {
                        _autoEndTimer += dt;
                        if (_autoEndTimer >= 1)
                        {
                            if (_snippits[_activeSnippit].Pause)
                                return;
                            Confirm();
                            return;
                        }
                    }

                if (_snippits[_activeSnippit].AutoEndSnippit) //if we are going to auto end a snippit
                {
                    _autoEndTimer += dt;
                    if (_autoEndTimer >= 1)
                        Confirm();
                }
            }




        }

        protected int _currentChar, _lastChar;
        protected float _flasher = 0;
        Vector2 pos;
        private float delay = 0;

        public virtual void DrawLightMap(SpriteBatch sb)
        {

            if (_activeSnippit == -1)
                return;


            Snippit sn = _snippits[_activeSnippit];
            string text = _snippits[_activeSnippit].WhosTalking;

            if (TypeOfConversation == ConversationType.Active)
            {
                sb.DrawString(_chat.FontSmall, text, new Vector2(640, 126), Color.Black, 0f, _chat.FontSmall.MeasureString(text) * .5f,
                    1f, SpriteEffects.None, 0f);
                sb.DrawString(_chat.FontSmall, text, new Vector2(640, 120), _chat.TextColour, 0f, _chat.FontSmall.MeasureString(text) * .5f,
                    1f, SpriteEffects.None, 0f);
                text = "=============================================";
                sb.DrawString(_chat.FontSmall, text, new Vector2(640, 166), Color.Black, 0f, _chat.FontSmall.MeasureString(text) * .5f,
                    1f, SpriteEffects.None, 0f);
                sb.DrawString(_chat.FontSmall, text, new Vector2(640, 160), _chat.TextColour, 0f, _chat.FontSmall.MeasureString(text) * .5f,
                    1f, SpriteEffects.None, 0f);
            }
            text = TextHelper.Lerp(_snippits[_activeSnippit].Text, _textProgress);

            _lastChar = _currentChar;
            _currentChar = text.Length;

            if (text.Length > 0 && text[text.Length - 1] == ' ' && _lastChar != _currentChar)
            {
                delay = (float)_chat.Rd.NextDouble() * .2f;
                text += " ";
            }
            else
            if (text.Length > 1 && text[text.Length - 1] == '\n' && _lastChar != _currentChar)
            {
                delay = 1f * (1200f / TextSpeed);
                text = text.Insert(text.Length - 1, "_");
            }
            else
                if (text.Length > 0 && text[text.Length - 1] == '\n')
                text = text.Insert(text.Length - 1, "_");
            else
                text += "_";

            sb.DrawString(_chat.FontSmall, text, TextOffset + _TextDrawPos + new Vector2(0, 6), Color.Black);
            sb.DrawString(_chat.FontSmall, text, TextOffset + _TextDrawPos, _chat.TextColour);



            foreach (TexturePosition tp in sn.Textures)
            {
                if (tp.Percentage < _textProgress)
                    sb.Draw(tp.Sprite, tp.Pos + _TextDrawPos, null, Color.White, 0f, Vector2.Zero, tp.Scale, SpriteEffects.None, 0f);
            }

            if (_textProgress >= 1)
            {
                float drawX = _TextDrawPos.X + 20;
                float delay = 1 / (float)_responces.Count;
                Color tmp = _chat.TextResponceColour;
                for (int i = 0; i < _responces.Count; i++)
                {
                    if (i == _responceSelected)
                        tmp = _chat.TextResponceSelecteColour;
                    else
                        tmp = _chat.TextResponceColour;

                    float fade = MathHelper.Clamp(((_textProgress - 1) - (delay * i) * delay), 0, 1);
                    sb.DrawString(_chat.FontSmall, _responces[i].Text, new Vector2(drawX, _TextDrawPos.Y + _textHeight + 26), Color.Black);
                    sb.DrawString(_chat.FontSmall, _responces[i].Text, new Vector2(drawX, _TextDrawPos.Y + _textHeight + 20), tmp);

                    if (i == _responceSelected)
                        fade *= Math.Abs(_flasher);
                    else
                        fade = 0;

                    if (_responces.Count == 1)
                        fade = 0;

                    sb.DrawString(_chat.FontSmall, ">", new Vector2(drawX - 20, _TextDrawPos.Y + _textHeight + 20), tmp * fade);
                    drawX += _chat.FontSmall.MeasureString(_responces[i].Text).X + _chat.ResponceSpacing;

                    sb.DrawString(_chat.FontSmall, "<", new Vector2(drawX - _chat.ResponceSpacing, _TextDrawPos.Y + _textHeight + 20), tmp * fade);
                }
                if (TypeOfConversation == ConversationType.Active)
                    if (_responces.Count == 0)
                    {

                        sb.DrawString(_chat.FontSmall, "CONTINUE_", new Vector2(_area.X + _area.Width - _chat.TextPadding.X -
                            _chat.FontSmall.MeasureString("CONTINUE_").X, _area.Y + _area.Height - 144), Color.Black * Math.Abs(_flasher));

                        sb.DrawString(_chat.FontSmall, "CONTINUE_", new Vector2(_area.X + _area.Width - _chat.TextPadding.X -
                            _chat.FontSmall.MeasureString("CONTINUE_").X, _area.Y + _area.Height - 150), _chat.TextColour * Math.Abs(_flasher));

                    }
            }


        }

        public virtual void Draw(SpriteBatch sb)
        {
            
            if (_activeSnippit == -1)
                return;


            Snippit sn = _snippits[_activeSnippit];
            string text = _snippits[_activeSnippit].WhosTalking;

            if (TypeOfConversation == ConversationType.Active)
            {
                sb.DrawString(_chat.FontSmall, text, new Vector2(640, 126), Color.Black, 0f, _chat.FontSmall.MeasureString(text) * .5f,
                    1f, SpriteEffects.None, 0f);
                sb.DrawString(_chat.FontSmall, text, new Vector2(640, 120), _chat.TextColour, 0f, _chat.FontSmall.MeasureString(text) * .5f,
                    1f, SpriteEffects.None, 0f);
                text = "=============================================";
                sb.DrawString(_chat.FontSmall, text, new Vector2(640, 166), Color.Black, 0f, _chat.FontSmall.MeasureString(text) * .5f,
                    1f, SpriteEffects.None, 0f);
                sb.DrawString(_chat.FontSmall, text, new Vector2(640, 160), _chat.TextColour, 0f, _chat.FontSmall.MeasureString(text) * .5f,
                    1f, SpriteEffects.None, 0f);
            }
            text = TextHelper.Lerp(_snippits[_activeSnippit].Text, _textProgress);

            _lastChar = _currentChar;
            _currentChar = text.Length;

            if (text.Length > 0 && text[text.Length - 1] == ' ' && _lastChar != _currentChar)
            {
                delay = (float)_chat.Rd.NextDouble() * .2f;
                text += " ";
            }
            else
            if (text.Length > 1 && text[text.Length - 1] == '\n' && _lastChar != _currentChar)
            {
                delay = 1f * (1200f / TextSpeed);
                text = text.Insert(text.Length - 1, "_");
            }
            else
                if (text.Length > 0 && text[text.Length - 1] == '\n')
                text = text.Insert(text.Length - 1, "_");
            else
                text += "_";

            sb.DrawString(_chat.FontSmall, text, TextOffset + _TextDrawPos + new Vector2(0, 6), Color.Black);
            sb.DrawString(_chat.FontSmall, text, TextOffset + _TextDrawPos, _chat.TextColour);

           

            foreach (TexturePosition tp in sn.Textures)
            {
                if (tp.Percentage < _textProgress)
                    sb.Draw(tp.Sprite, tp.Pos + _TextDrawPos, null, Color.White, 0f, Vector2.Zero, tp.Scale, SpriteEffects.None, 0f);
            }

            if (_textProgress >= 1)
            {
                float drawX = _TextDrawPos.X + 20;
                float delay = 1 / (float)_responces.Count;
                Color tmp = _chat.TextResponceColour;
                for (int i = 0; i < _responces.Count; i++)
                {
                    if (i == _responceSelected)
                        tmp = _chat.TextResponceSelecteColour;
                    else
                        tmp = _chat.TextResponceColour;

                    float fade = MathHelper.Clamp(((_textProgress - 1) - (delay * i) * delay), 0, 1);
                    sb.DrawString(_chat.FontSmall, _responces[i].Text, new Vector2(drawX, _TextDrawPos.Y + _textHeight + 26), Color.Black);
                    sb.DrawString(_chat.FontSmall, _responces[i].Text, new Vector2(drawX, _TextDrawPos.Y + _textHeight + 20), tmp);

                    if (i == _responceSelected)
                        fade *= Math.Abs(_flasher);
                    else
                        fade = 0;

                    if (_responces.Count == 1)
                        fade = 0;

                    sb.DrawString(_chat.FontSmall, ">", new Vector2(drawX - 20, _TextDrawPos.Y + _textHeight + 20), tmp * fade);
                    drawX += _chat.FontSmall.MeasureString(_responces[i].Text).X + _chat.ResponceSpacing;

                    sb.DrawString(_chat.FontSmall, "<", new Vector2(drawX - _chat.ResponceSpacing, _TextDrawPos.Y + _textHeight + 20), tmp * fade);
                }
                if (TypeOfConversation == ConversationType.Active)
                    if (_responces.Count == 0)
                    {

                        sb.DrawString(_chat.FontSmall, "CONTINUE_", new Vector2(_area.X + _area.Width - _chat.TextPadding.X -
                            _chat.FontSmall.MeasureString("CONTINUE_").X, _area.Y + _area.Height - 144), Color.Black * Math.Abs(_flasher));

                        sb.DrawString(_chat.FontSmall, "CONTINUE_", new Vector2(_area.X + _area.Width - _chat.TextPadding.X -
                            _chat.FontSmall.MeasureString("CONTINUE_").X, _area.Y + _area.Height - 150), _chat.TextColour * Math.Abs(_flasher));

                    }
            }


        }
    }
}
