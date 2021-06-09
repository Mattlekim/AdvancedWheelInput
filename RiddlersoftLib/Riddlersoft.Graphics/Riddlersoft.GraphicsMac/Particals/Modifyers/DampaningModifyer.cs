using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riddlersoft.Graphics.Particals.Emitters;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public class DampaningModifyer : Modifyer
    {

        public bool DampenVelocity = true, DampenAngulorVelocity = false;
        private float _dampaningAmount;
        private float _dampaningMultiplyer;
        public float DampaningAmount
        {
            get
            { return _dampaningAmount; }
            set
            {
                _dampaningAmount = value;
                _dampaningMultiplyer = 1 - _dampaningAmount;
            }
        }

        private float _angulorDampaningAmount;
        private float _angulorDampaningMultiplyer;

        public float AngulorDampaningAmount
        {
            get
            { return _angulorDampaningAmount; }
            set
            {
                _angulorDampaningAmount = value;
                _angulorDampaningMultiplyer = 1 - _angulorDampaningAmount;
            }
        }
      

        public DampaningModifyer(float amount, Emitter parent) : base()
        {
            DampaningAmount = amount;
        }

        public override void Processes(Partical input, float dt)
        {
            if (DampenVelocity)
                input.Velocity *= _dampaningMultiplyer;

            if (DampenAngulorVelocity)
                input.AngulorRotation *= _angulorDampaningMultiplyer;

        }
    }
}
