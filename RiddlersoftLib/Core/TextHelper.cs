using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace Riddlersoft.Core
{
    public static class TextHelper
    {
        public static string Wrap(SpriteFont font, string text, int width)
        {
            if (width < 100)
                throw new Exception("Width must be more than 100");
            string output = string.Empty;
            int newLinePoint = 0;
            string line = text;
            for (int i = 0; i < line.Length; i++)
            {
                int lenght = (int)font.MeasureString(line.Substring(0, i)).X;
                if (line[i] == '\n') //detect a new line
                {
                    output += line.Substring(0, i);
                    line = line.Substring(i);
                    lenght = 0;
                    continue;
                }

                if (lenght >= width)
                {
                    if (line[0] == ' ' && line.Length > 1) //if first charitor is a space
                        line = line.Substring(1);
                    string tmp = line.Substring(0, i);
                    int dotPoint = tmp.LastIndexOf('.') + 1;
                    newLinePoint = tmp.LastIndexOf(' ') + 1;
                    if (dotPoint == 0)
                        dotPoint = int.MaxValue;
                    newLinePoint = MathHelper.Min(dotPoint, newLinePoint);
                    if (newLinePoint == -1)
                    {
                        output += line.Substring(0, i);
                        line = line.Substring(i);
                        i = 0;
                    }
                    else
                    {
                        output += line.Substring(0, newLinePoint);
                        line = line.Substring(newLinePoint);
                        i = 0;
                    }
                        output += '\n';
                    
                }
            }
            output += line;
            return output;
        }

        public static string Lerp(string text, float amount)
        {
            amount = MathHelper.Clamp(amount, 0, 1);
            int upTo = Convert.ToInt32((float)text.Length * amount);
            return text.Substring(0, upTo);
        }
    }
}
