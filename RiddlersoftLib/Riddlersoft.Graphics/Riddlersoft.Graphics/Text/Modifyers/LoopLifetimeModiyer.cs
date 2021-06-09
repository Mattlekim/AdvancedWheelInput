using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Graphics.Text.Modifyers
{
    public class LoopLifetimeModiyer : TextModifyer
    {
        private float _startLoopTime, _endLoopTime;

        private bool _disableNextUpdate = false;

        private int _charToDisable = 0;
        private bool _disabledChar = false;       
        
        public LoopLifetimeModiyer(float startTime, float endTime)
        {
            if (startTime >= endTime)
                throw new Exception("start time must be less then endtime");

            if (startTime < 0 || endTime < 0)
                throw new Exception("times must be larger than 0");

            _startLoopTime = startTime;
            _endLoopTime = endTime;
        }

        protected override void ParentApply(TextChar c, StringEffect _effect, int index)
        {
          
        }

        protected override void ParentUpdate(TextChar c, int index)
        {
            if (_disableNextUpdate)
            {
                if (c.LifeTime > _endLoopTime)
                {
                    if (_charToDisable == index)
                    {
                        _disabledChar = true;
                    }
                    if (index <= _charToDisable)
                        return;
                }
            }

            if (c.LifeTime > _endLoopTime)
                c.LifeTime -= (_endLoopTime - _startLoopTime);
        }

        public override void UpdateModifyer(StringEffect effect)
        {
            if (_disableNextUpdate)
            {
                if (_disabledChar)
                    _charToDisable++;
                if (_charToDisable >= effect._chars.Count)
                    Enabled = false;
            }

            _disabledChar = false;
        }

        public override void Disable()
        {
            _disableNextUpdate = true;
        }
    }
}
