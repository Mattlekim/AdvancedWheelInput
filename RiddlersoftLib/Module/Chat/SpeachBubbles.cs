using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Riddlersoft.Core;

namespace Riddlersoft.Modules.Chat
{
    public class SpeachBubbleAcher
    {
        public Vector2 Postion;
        public string name;

     
        public SpeachBubbleAcher()
        {
            Postion = Vector2.Zero;
            name = string.Empty;
        }

        public SpeachBubbleAcher(Vector2 pos)
        {
            Postion = pos;
            name = string.Empty;
        }


        public SpeachBubbleAcher(float x, float y)
        {
            Postion = new Vector2(x, y);
            name = string.Empty;
        }
    }

    public class SpeachBubbles : Conversation
    {
        public Color TextColour = Color.White;

        private readonly Rectangle[] _parts = new Rectangle[10] 
        {
            new Rectangle(0,0,56 / 2,50 / 2),
            new Rectangle(264 / 2,0,56 / 2,50 / 2),
            new Rectangle(0,100 / 2, 56 / 2, 50 / 2),
            new Rectangle(264 / 2, 100 / 2, 56 / 2, 50 / 2),
            new Rectangle(100 / 2, 0, 64 / 2, 50 / 2),
            new Rectangle(264 / 2, 56 / 2, 56 / 2, 44 / 2),
            new Rectangle(56 / 2, 100 / 2, 50 / 2, 50 / 2),
            new Rectangle(0, 56 / 2, 56 / 2, 44 / 2),
            new Rectangle(45,30,1,1),
            new Rectangle(165 / 2, 139 / 2, 59 / 2, 59 / 2)
        };

        private SpriteFont _font;

        public SpeachBubbleAcher _sb1, _sb2;

        public byte Data;

        private Texture2D _background;
        private Texture2D _backgroundLight;

        public void SetTexture(Texture2D tex, Texture2D light)
        {
            _background = tex;
            _backgroundLight = light;
        }
        
        public void SetAcher(int id, SpeachBubbleAcher ancher)
        {
            if (id == 0)
                _sb1 = ancher;
            else
                _sb2 = ancher;
        }

        public SpeachBubbles(ChatModule parent, SpeachBubbleAcher p1, SpeachBubbleAcher p2, SpriteFont font) : base(parent)
        {
            _font = font;
            _background = parent.Game.Content.Load<Texture2D>("imgs\\GUI\\speachbubble");
            _backgroundLight = parent.Game.Content.Load<Texture2D>("imgs\\GUI\\speachbubblelight");
            TypeOfConversation = ConversationType.Passive;
            ScreenColour = Color.Transparent;
            BackgroundColour = Color.White;
            AllowChatToDraw = false;
            TextSpeed = 40;

            _sb1 = p1;
            _sb2 = p2;
        }
        protected override void WrapActiveSnippit()
        {
            _autoEndTimer = 0f;//reset timer
            Snippit snippit = _snippits[_activeSnippit];
            snippit.WrapText(_font, _ChatTextWidth);
            _snippits[_activeSnippit] = snippit;
            if (snippit.Responces == null)
                _responces = new List<Responce>();
            else
                _responces = snippit.Responces.GetResponces(); //get responces
            _textHeight = _font.MeasureString(snippit.Text).Y; //get the height;

            _responceSelected = -1; //reset the responces flag
            if (_responces.Count > 0)
                _responceSelected = 0; //set to default responces if we have responces

            SetBubbleSize();

            _currentChar = 0;
            _lastChar = 0;
        }

        public int BubbleHeight { get; private set; }
        public void DrawSpeachBubble(SpriteBatch sb, Vector2 pos, int width, int height)
        {
            pos.X = Convert.ToInt32(pos.X);
            pos.Y = Convert.ToInt32(pos.Y);
            //draw coners
            sb.Draw(_background, pos, _parts[0], Color.White);
            sb.Draw(_background, pos + new Vector2(width,0), _parts[1], Color.White);

            sb.Draw(_background, pos + new Vector2(0, height), _parts[2], Color.White);
            sb.Draw(_background, pos + new Vector2(width, height), _parts[3], Color.White);

            Point p = pos.ToPoint();
            //Draw sides
            sb.Draw(_background, new Rectangle(p.X + _parts[0].Width, p.Y, width - _parts[0].Width, _parts[4].Height), _parts[4], Color.White);
            sb.Draw(_background, new Rectangle(p.X + width, p.Y + _parts[0].Height, _parts[5].Width, height - _parts[0].Height), _parts[5], Color.White);
            

            sb.Draw(_background, new Rectangle(p.X + _parts[0].Width, p.Y + height, width - _parts[0].Width, _parts[6].Height), _parts[6], Color.White);
            sb.Draw(_background, new Rectangle(p.X, p.Y + _parts[0].Height, _parts[7].Width, height - _parts[0].Height), _parts[7], Color.White);

            sb.Draw(_background, new Rectangle(p.X + _parts[0].Width, p.Y + _parts[0].Height, width - _parts[0].Width, height - _parts[0].Height), _parts[8], Color.White);

            sb.Draw(_background, new Rectangle(p.X + _parts[0].Width + 50, p.Y + height + 18, _parts[9].Width, _parts[9].Height), _parts[9], Color.White);

            BubbleHeight = height;
        }

        public void DrawSpeachBubbleLight(SpriteBatch sb, Vector2 pos, int width, int height)
        {
            pos.X = Convert.ToInt32(pos.X);
            pos.Y = Convert.ToInt32(pos.Y);
            //draw coners
            sb.Draw(_background, pos, _parts[0], Color.White);
            sb.Draw(_background, pos + new Vector2(width, 0), _parts[1], Color.White);

            sb.Draw(_background, pos + new Vector2(0, height), _parts[2], Color.White);
            sb.Draw(_background, pos + new Vector2(width, height), _parts[3], Color.White);

            Point p = pos.ToPoint();
            //Draw sides
            sb.Draw(_background, new Rectangle(p.X + _parts[0].Width, p.Y, width - _parts[0].Width, _parts[4].Height), _parts[4], Color.White);
            sb.Draw(_background, new Rectangle(p.X + width, p.Y + _parts[0].Height, _parts[5].Width, height - _parts[0].Height), _parts[5], Color.White);


            sb.Draw(_background, new Rectangle(p.X + _parts[0].Width, p.Y + height, width - _parts[0].Width, _parts[6].Height), _parts[6], Color.White);
            sb.Draw(_background, new Rectangle(p.X, p.Y + _parts[0].Height, _parts[7].Width, height - _parts[0].Height), _parts[7], Color.White);

            sb.Draw(_background, new Rectangle(p.X + _parts[0].Width, p.Y + _parts[0].Height, width - _parts[0].Width, height - _parts[0].Height), _parts[8], Color.White);

            sb.Draw(_background, new Rectangle(p.X + _parts[0].Width + 50, p.Y + height + 18, _parts[9].Width, _parts[9].Height), _parts[9], Color.White);

            BubbleHeight = height;
        }

        public void DrawNewSpeachBubble(SpriteBatch sb, Vector2 pos, float scale, Color col)
        {
            pos.X = Convert.ToInt32(pos.X);
            pos.Y = Convert.ToInt32(pos.Y);

            sb.Draw(_background, pos, null, col, 0f, new Vector2(_background.Width * .5f, _background.Height), scale, SpriteEffects.None, 0f);
        }

        public void DrawSpeachBubble(SpriteBatch sb, Vector2 pos, int width, int height, Color col, float scale)
        {

            DrawNewSpeachBubble(sb, pos + new Vector2(_background.Width * .5f, _background.Height + 30), scale, col);
            return;
            pos.X = Convert.ToInt32(pos.X);
            pos.Y = Convert.ToInt32(pos.Y);
            //draw coners
            sb.Draw(_background, pos, _parts[0], Color.White);
            sb.Draw(_background, pos + new Vector2(width, 0), _parts[1], Color.White);

            sb.Draw(_background, pos + new Vector2(0, height), _parts[2], Color.White);
            sb.Draw(_background, pos + new Vector2(width, height), _parts[3], Color.White);

            Point p = pos.ToPoint();
            //Draw sides
            sb.Draw(_background, new Rectangle(p.X + _parts[0].Width, p.Y, width - _parts[0].Width, _parts[4].Height), _parts[4], col);
            sb.Draw(_background, new Rectangle(p.X + width, p.Y + _parts[0].Height, _parts[5].Width, height - _parts[0].Height), _parts[5], col);


            sb.Draw(_background, new Rectangle(p.X + _parts[0].Width, p.Y + height, width - _parts[0].Width, _parts[6].Height), _parts[6], col);
            sb.Draw(_background, new Rectangle(p.X, p.Y + _parts[0].Height, _parts[7].Width, height - _parts[0].Height), _parts[7], col);

            sb.Draw(_background, new Rectangle(p.X + _parts[0].Width, p.Y + _parts[0].Height, width - _parts[0].Width, height - _parts[0].Height), _parts[8], col);

            sb.Draw(_background, new Rectangle(p.X + _parts[0].Width + 50, p.Y + height + 18, _parts[9].Width, _parts[9].Height), _parts[9], col);

            BubbleHeight = height;
        }

        Vector2 drawpos;

        public override void DrawLightMap(SpriteBatch sb)
        {
            Snippit sn = _snippits[_activeSnippit];
            string text = _snippits[_activeSnippit].WhosTalking;

            int p1;
            if (_snippits[_activeSnippit].WhosTalking == string.Empty)
                drawpos = Vector2.Zero;
            else
            {
                p1 = Convert.ToInt32(_snippits[_activeSnippit].WhosTalking);
                if (p1 == 0)
                    drawpos = _sb1.Postion;
                else
                    drawpos = _sb2.Postion;
            }
            Vector2 pos = _font.MeasureString(_snippits[_activeSnippit].Text) + new Vector2(40, 10);
            DrawSpeachBubbleLight(sb, drawpos - new Vector2(30, 24), (int)pos.X, (int)pos.Y);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (_activeSnippit == -1)
                return;

            Snippit sn = _snippits[_activeSnippit];
            string text = _snippits[_activeSnippit].WhosTalking;
           
            int p1;
            if (_snippits[_activeSnippit].WhosTalking == string.Empty)
                drawpos = Vector2.Zero;
            else
            {
                p1 = Convert.ToInt32(_snippits[_activeSnippit].WhosTalking);
                if (p1 == 0)
                    drawpos = _sb1.Postion;
                else
                    drawpos = _sb2.Postion;
            }
            Vector2 pos = _font.MeasureString(_snippits[_activeSnippit].Text) + new Vector2(40,10);
            DrawSpeachBubble(sb, drawpos - new Vector2(30,24), (int)pos.X, (int)pos.Y);

            text = TextHelper.Lerp(_snippits[_activeSnippit].Text, _textProgress);
            _lastChar = _currentChar;
            _currentChar = text.Length;
          //  sb.DrawString(_font, text , drawpos + new Vector2(0, 2), Color.Black);
            sb.DrawString(_font, text , drawpos, TextColour);

            foreach (TexturePosition tp in sn.Textures)
            {
                if (tp.Percentage < _textProgress)
                    sb.Draw(tp.Sprite, tp.Pos + drawpos, null, Color.White, 0f, Vector2.Zero, tp.Scale, SpriteEffects.None, 0f);
            }




        }

        public void Draw(SpriteBatch sb, Vector2 camera)
        {
            if (_activeSnippit == -1)
                return;


            Snippit sn = _snippits[_activeSnippit];
            string text = _snippits[_activeSnippit].WhosTalking;

            int p1;
            p1 = Convert.ToInt32(_snippits[_activeSnippit].WhosTalking);
            if (p1 == 0)
                drawpos = _sb1.Postion;
            else
                drawpos = _sb2.Postion;
            Vector2 pos = _font.MeasureString(_snippits[_activeSnippit].Text) + new Vector2(40, 10);
            DrawSpeachBubble(sb, drawpos - new Vector2(30, 24) - camera, (int)pos.X, (int)pos.Y);

            //sb.DrawString(_font, TextHelper.Lerp(_snippits[_activeSnippit].Text, _textProgress), drawpos + new Vector2(0, 2) - camera, Color.Black);
            sb.DrawString(_font, TextHelper.Lerp(_snippits[_activeSnippit].Text, _textProgress), drawpos - camera, _chat.TextColour);

            foreach (TexturePosition tp in sn.Textures)
            {
                if (tp.Percentage < _textProgress)
                    sb.Draw(tp.Sprite, tp.Pos + drawpos - camera, null, Color.White, 0f, Vector2.Zero, tp.Scale, SpriteEffects.None, 0f);
            }




        }

    }
}
