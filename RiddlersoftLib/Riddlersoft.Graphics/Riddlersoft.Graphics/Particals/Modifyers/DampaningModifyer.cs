using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riddlersoft.Core.Xml;
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

        protected override void _prosses(Partical input, float dt)
        {
            if (DampenVelocity)
                input.Velocity *= _dampaningMultiplyer;

            if (DampenAngulorVelocity)
                input.AngulorRotation *= _angulorDampaningMultiplyer;

        }

        public override void WriteToFile(CustomXmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void ReadFromFile(CustomXmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override CustomModifyer _create()
        {
            throw new NotImplementedException();
        }
    }
}
