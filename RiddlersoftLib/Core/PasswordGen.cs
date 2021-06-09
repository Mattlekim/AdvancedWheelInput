using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Core
{
    public class PasswordGen
    {
        /// <summary>
        /// the current seed for the generator
        /// </summary>
        public int Seed { get; private set; }

        private Random rd;

        private List<char> chars = new List<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
        'm','n','o','p','q','r','s','t','u','v','w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S',
        'R','U','V','W','X','Y','Z','0','1','2','3','4','5','6','7','8','9','0','+','_','(',')' };

        public PasswordGen()
        {
            DateTime time = DateTime.UtcNow;
            Seed = time.Millisecond * 6000 + time.Second * 60 + time.Hour;
            rd = new Random(Seed);
        }

        public PasswordGen(int seed)
        {
            Seed = seed;
            rd = new Random(Seed);
        }

        public string GeneratePassword(int lenght)
        {
              string output = string.Empty;
              for (int i=0;i < lenght; i++)
              {
                  output += chars[rd.Next(0, chars.Count - 1)];
              }
              return output;
        }
    }
}
