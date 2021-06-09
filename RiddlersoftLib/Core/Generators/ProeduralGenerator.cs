using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Core.Generators
{
    public class ProeduralGenerator
    {
        private Random _rd;


        private List<byte> _chances = new List<byte>();

        public ProeduralGenerator(int seed)
        {
            _rd = new Random(seed);

            List<byte> c = new List<byte>();
            for (int i = 0; i <= 255; i++)
                c.Add((byte)i);

            int index = 0;
            while (c.Count > 0)
            {
                index = _rd.Next(c.Count);
                _chances.Add(c[index]);
                c.RemoveAt(index);
            }

        }

        public bool Chance(float per, int data)
        {
            if (per <= 0)
            {
                return false;
            }
            int i = data & 254;

            if (per * 255 >= _chances[i])
                return true;
            return false;
        }
    }
}
