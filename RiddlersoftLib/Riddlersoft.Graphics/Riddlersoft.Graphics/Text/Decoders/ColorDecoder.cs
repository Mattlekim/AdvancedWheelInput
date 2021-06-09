using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
namespace Riddlersoft.Graphics.Text.Decoders
{
    public class ColorDecoder : TextDecoder
    {
        private static List<Color> _pallet = new List<Color>();

        public static void AddToPallet(List<Color> colors)
        {
            _pallet.AddRange(colors);
        }

        public static void AddToPallet(Color color)
        {
            _pallet.Add(color);
        }
        /// <summary>
        /// decodes colore data from a stirng
        /// #C.3-04
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<CharData> Decode(string input, out string trimed)
        {


            List<CharData> output = null;
            int point = 0;
            while (true)
            {
                int index = input.IndexOf("#c", point);
                if (index == -1)
                    break;

                if (_pallet.Count <= 0)
                    throw new Exception("you must set a colour pallet");

                index += 2;
                int endIndex = 3;
                try
                {
                    int i = Convert.ToInt32(input.Substring(index, 1));
                    int l = 99;
                    if (input[index + 1] == '-') //now look for lenght
                    {
                        l = Convert.ToInt32(input.Substring(index + 2, 2));
                        endIndex += 3;
                    }
                    if (output == null)
                        output = new List<CharData>();
                    output.Add(new CharData()
                    {
                        Colour = _pallet[i],
                        Count = l,
                        StartIndex = index - 2,
                    });

                    input = input.Remove(index - 2, endIndex);
                }
                catch
                {
                    trimed = input;
                    return output;
                }
               
            }

            trimed = input;
            return output;
        }
    }
}
