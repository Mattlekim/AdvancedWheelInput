using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Graphics.Text.Modifyers
{
    public abstract class TextModifyer
    {
        public bool LimitRange = false;
        public int LowerLimit = 0;
        public int UpperLimit = 0;

        private bool _enabled = true;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value;
                if (!_enabled && _wasEnabled)
                {
                    _wasEnabled = false;
                    OnDisabled(Tag);
                }
            }
        }
        public bool ApplyOnStartup = true;

        public string Tag = string.Empty;

        internal Action<string> OnDisabled;

        private bool _wasEnabled = false;

        protected abstract void ParentUpdate(TextChar c, int index);

        protected abstract void ParentApply(TextChar c, StringEffect _effect, int index);

        public void Apply(TextChar c, StringEffect _effect, int index)
        {
            if (!ApplyOnStartup)
                return;

            if (LimitRange)
            {
                if (index < LowerLimit || index > UpperLimit)
                    return;
            }
            ParentApply(c, _effect, index);
        }

        public void Update(TextChar c, int index)
        {
            if (Enabled)
            {
                _wasEnabled = true;
                if (LimitRange)
                {
                    if (index < LowerLimit || index > UpperLimit)
                        return;
                }
                ParentUpdate(c, index);
            }
        }

        public virtual void UpdateModifyer(StringEffect effect)
        {
            
        }

        public virtual void Disable()
        {
            Enabled = false;
        }

    }
}
