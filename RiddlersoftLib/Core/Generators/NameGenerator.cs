using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Core.Generators
{
    public class NameGenerator
    {
        private List<string> _nameParts;

        private Random _random = new Random();
        public NameGenerator()
        {
            _nameParts = new List<string>() { "mat", "jim", "ric", "vet", "pet", "dav", "fin", "cra", "loz", "lyz", "jes", "amb",
                "eli", "kei", "han", "car","mor", "kel","woo", "wod","bri", "kim", "gre", "rot", "mex", "sha", "tle", "ith", "use", "flt",
                "ood", "Ton", "jon", "che", "tom" };
        }

        public string Generate()
        {
            return $"{_nameParts[_random.Next(_nameParts.Count-1)]}{_nameParts[_random.Next(_nameParts.Count - 1)]}{_nameParts[_random.Next(_nameParts.Count - 1)]}";
        }
    }
}
