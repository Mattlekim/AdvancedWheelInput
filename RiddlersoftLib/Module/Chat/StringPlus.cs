using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Modules.Chat
{
    public class StringPlus
    {
        public string Text;
        public List<Texture2D> Textures = new List<Texture2D>();
        public List<TexturePosition> TexturePos = new List<TexturePosition>();

        private StringPlus(string s)
        {
            Text = s;
        }

        public StringPlus(string text, Texture2D tex = null, Texture2D tex1 = null, Texture2D tex2 = null, Texture2D tex3 = null, Texture2D tex4= null, Texture2D tex5 = null)
        {
            Text = text;

            if (tex != null)
                Textures.Add(tex);
            if (tex1 != null)
                Textures.Add(tex1);
            if (tex2 != null)
                Textures.Add(tex2);
            if (tex3 != null)
                Textures.Add(tex3);
            if (tex4 != null)
                Textures.Add(tex4);
            if (tex5 != null)
                Textures.Add(tex5);

           
        }

        /// <summary>
        /// replace the textuers with spaces
        /// </summary>
        public void InsertSpacing(SpriteFont font)
        {
            string tmp = Text;
            float space = font.MeasureString(" ").X;

            int i = tmp.IndexOf('`');
            int last = 0;
            while (i != -1)
            {
                int tex = Convert.ToInt32(tmp[i + 1].ToString());

                int number = (int)Math.Ceiling((float)Textures[tex].Width / space);

                string sp = "";
                for (int c = 0; c < number; c++)
                    sp = $"{sp} ";
                Text = Text.Replace($"`{tex}", sp);

                string cur = tmp.Substring(0, i);
                int tpos = Convert.ToInt32(font.MeasureString($"{cur}").X);
                TexturePos.Add(new Chat.TexturePosition()
                {
                    //TextureId = tex,
                    Pos = new Vector2(tpos + last, 0)
                });
                tmp = tmp.Substring(i + 1);
                i = tmp.IndexOf('`');
                last += tpos + Convert.ToInt32(font.MeasureString(sp).X);
            }
        }

        public static implicit operator StringPlus (string s) 
        {
            return new StringPlus(s);
        }

        public static implicit operator string(StringPlus sp)
        {
            return sp.Text;
        }


        public static StringPlus Wrap(SpriteFont font, StringPlus text, int width)
        {
            StringPlus output = string.Empty;
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
            output.TexturePos = text.TexturePos;
            output.Textures = text.Textures;
            return output;
        }
    }
}
