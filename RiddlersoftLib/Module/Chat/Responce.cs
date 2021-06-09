using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Modules.Chat
{
    public struct Responce
    {
        internal Action<Responce> _CallBack;
        public string Text { get; private set; }
        public int Next { get; private set; }

        public Responce(string text)
        {
            Text = text;
            Next = -1;
            _CallBack = null;
        }

        public Responce(string text, int next, Action<Responce> result = null)
        {
            Text = text;
            Next = next;
            _CallBack = result;
        }

        public Responce(string text, Action<Responce> result = null)
        {
            Text = text;
            Next = -1;
            _CallBack = result;
        }
    }
}
