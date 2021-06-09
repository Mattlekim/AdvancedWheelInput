using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Riddlersoft.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Modules.Chat
{
    public struct Snippit
    {
        public string WhosTalking;
        public string Text;
        public bool AutoEndSnippit;
        public Responces Responces { get; private set; }
        public bool Pause;
        public Func<bool> Requirment;

        public List<TexturePosition> Textures;

        public object data;

        public Snippit(string who, StringPlus text)
        {
            data = null;
            WhosTalking = who;
            Text = text;
            Responces = Responces.Ok();
            _hasRappedText = false;
            AutoEndSnippit = false;
            Requirment = null;
            Pause = false;
            Textures = new List<Chat.TexturePosition>();
        }

        /// <summary>
        /// creates a new snippit
        /// </summary>
        /// <param name="who">the person who is taling</param>
        /// <param name="text">what the person is saying</param>
        /// <param name="requirment">the requements for the conversation to progress with</param>
        public Snippit(string who, string text, Func<bool> requirment)
        {
            data = null;
            WhosTalking = who;
            Text = text;
            Responces = null;
            _hasRappedText = false;
            _hasRappedText = false;
            Pause = false;
            AutoEndSnippit = false;
            Requirment = requirment;
            Textures = new List<Chat.TexturePosition>();
        }

        /// <summary>
        /// creates a new snippit
        /// </summary>
        /// <param name="who">the person who is taling</param>
        /// <param name="text">what the person is saying</param>
        /// <param name="responces">the options the player has</param>
        public Snippit(string who, string text, Responces responces)
        {
            data = null;
            WhosTalking = who;
            Text = text;
            Responces = responces;
            _hasRappedText = false;
            _hasRappedText = false;
            AutoEndSnippit = false;
            Pause = false;
            Requirment = null;
            Textures = new List<Chat.TexturePosition>();
        }

        private bool _hasRappedText;
        public void WrapText(SpriteFont font, int width)
        {
            if (_hasRappedText)
                return;

            Text = TextHelper.Wrap(font, Text, width);

            _hasRappedText = true;
        }

    }
}
