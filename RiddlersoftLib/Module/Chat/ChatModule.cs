using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Riddlersoft.Core.Input;

namespace Riddlersoft.Modules.Chat
{
    public class ChatModule: GameComponent
    {
        public SpriteFont FontLarge { get; internal set; }
        public SpriteFont FontSmall { get; internal set; }

        public Vector2 TextPadding { get; set; } = new Vector2(50, 50);

        internal Random Rd = new Random();

        public bool Pause = false;

        public int ResponceSpacing = 80;

        public Color TextResponceColour = Color.White;
        public Color TextResponceSelecteColour = Color.OrangeRed;
       

      
        public Color TextColour = Color.White;

        private Dictionary<string, Conversation> _conversations = new Dictionary<string, Conversation>();

        private Conversation _activeConversation;
        private string _activeConversationName;

        private float fade = 0;

        internal Texture2D _pixel;

        public Action<string> Log;

        public Action<int> OnNextChar;

        public Action<int> OnConversationOver;

        public Action<int> OnConversationStart;

        internal void _WriteLog(string log)
        {
            if (Log != null)
                Log(log);
        }

        public ChatModule(Game game) : base(game)
        {
        }

      

        public override void Initialize()
        {
            _pixel = new Texture2D(Game.GraphicsDevice, 1, 1);
            _pixel.SetData<Color>(new Color[] { Color.White });
            _WriteLog("Chat Initialized");
            base.Initialize();
        }

        public void SetFonts(SpriteFont large, SpriteFont small)
        {
            FontLarge = large;
            FontSmall = small;
        }

        public float ConversationHeight
        {
            get
            {
                if (_activeConversation == null)
                    return 0;
                return _activeConversation._textHeight;
            }
        }


        public ConversationState State { get; private set; }

        public void AddNewConversation(string name, Conversation conversation)
        {
            _conversations.Add(name, conversation);
        }

        public string CurrentConversation()
        {
            return _activeConversationName;
        }

        public bool ConversationExist(String name)
        {
            if (_conversations.ContainsKey(name))
                return true;
            return false;
        }

        public Conversation GetConversation(string name)
        {
            if (!_conversations.ContainsKey(name))
                return null;

            return _conversations[name];
        }

        public bool DeleteConversation(string name)
        {
            if (!_conversations.ContainsKey(name))
                return false;

            _conversations.Remove(name);
            return true;
        }

        public bool Replace(string name, Conversation conversation)
        {
            if (!_conversations.ContainsKey(name))
                return false;

            _conversations[name] = conversation;
            return true;
        }

        public void StartConversation(string name, bool imidiate = false)
        {
            

            if (State != ConversationState.None) //if conversation already active
            {
                _WriteLog("Conversation is alread active");
                return;
            }

            if (!_conversations.ContainsKey(name))
            {
                _WriteLog("[r]Conversation not found");
                return;
            }

            _WriteLog($"[g]Starting Conversation '{name}'");
            _activeConversation = _conversations[name]; //get the conversation to start
            _activeConversation.Start(); //start the conversation
            _activeConversationName = name; //log the name
            if (_activeConversation.TypeOfConversation == ConversationType.Active)
                State = ConversationState.Active; //set flag
            else
                State = ConversationState.Passive;
        
            if (imidiate)
                fade = 1;
            else
                fade = 0;

            _bgColour = _activeConversation.BackgroundColour;
            _screenColour = _activeConversation.ScreenColour;
            _area = _activeConversation._area;

            if (OnConversationStart != null)
                OnConversationStart(0);
        }

        public void Reset()
        {
            _activeConversation = null;
            _activeConversationName = null;
            State =  ConversationState.None;
        }

        private bool _imediateTerminate;
        public void TerminateConversation(bool _imdediate)
        {
            _imediateTerminate = _imdediate;
            _activeConversation = null;
            _activeConversationName = null;
            State =  ConversationState.None;
            if (OnConversationOver != null)
                OnConversationOver(0);
        }

        float fadeSpd = 4f;
        public override void Update(GameTime gameTime)
        {
            if (Pause)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (State == ConversationState.None)
            {
                if (fade > 0)
                    fade -= dt * fadeSpd;
                return;
            }

            if (fade < 1)
            {
                fade += dt * fadeSpd;
                return;
            }

            if (_activeConversation.TypeOfConversation == ConversationType.Active)
            {
                if (GamePadApi.IsButtonClick(Buttons.A) || KeyboardAPI.IsKeyPressed(Keys.Enter))
                {
                    _activeConversation.Confirm();
                    return;
                }

                if (GamePadApi.IsButtonClick(Buttons.B) || KeyboardAPI.IsKeyPressed(Keys.Escape))
                    _activeConversation.JumpToEnd();
            }
            //fade = 0;
            _activeConversation.Update(dt);
            base.Update(gameTime);
        }


        Color _screenColour = Color.White, _bgColour = Color.White;
        Rectangle _area;
        public void Draw(SpriteBatch sb)
        {
            if (_imediateTerminate)
            {
                _imediateTerminate = false;
                fade = 0;
                return;
            }
            if (State == ConversationState.None)
            {
                sb.Draw(_pixel, new Rectangle(0, 0, 1280, 720), _screenColour * fade);
                sb.Draw(_pixel, _area, _bgColour * fade);
                return;
            }
            else
            if (_activeConversation.AllowChatToDraw)
            {
                sb.Draw(_pixel, new Rectangle(0, 0, 1280, 720), _screenColour * fade);
                sb.Draw(_pixel, _area, _bgColour * fade);
            }

            if (_activeConversation.ScreenTexture != null)
                sb.Draw(_activeConversation.ScreenTexture, Vector2.Zero, _screenColour);
            _activeConversation.Draw(sb);
        }

        public void DrawLightMap(SpriteBatch sb)
        {
            if (_imediateTerminate)
            {
                _imediateTerminate = false;
                fade = 0;
                return;
            }
            if (State == ConversationState.None)
            {
                sb.Draw(_pixel, new Rectangle(0, 0, 1280, 720), _screenColour * fade);
                sb.Draw(_pixel, _area, _bgColour * fade);
                return;
            }
            else
            if (_activeConversation.AllowChatToDraw)
            {
                sb.Draw(_pixel, new Rectangle(0, 0, 1280, 720), _screenColour * fade);
                sb.Draw(_pixel, _area, _bgColour * fade);
            }

            if (_activeConversation.ScreenTexture != null)
                sb.Draw(_activeConversation.ScreenTexture, Vector2.Zero, _screenColour);
            _activeConversation.DrawLightMap(sb);
        }

    }
}
