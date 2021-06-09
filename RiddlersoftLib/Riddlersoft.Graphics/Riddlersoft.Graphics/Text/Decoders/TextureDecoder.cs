using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Riddlersoft.Graphics.Text.Decoders
{
    public class TextureDecoder : TextDecoder
    {

        //the effect also needs to enable to have textures within in
        //a texture with be add from an array
        //the encoder will use the format [x] where x is the number
        private static List<Texture2D> _textures = new List<Texture2D>();


        public static void AddTextures(List<Texture2D> textures)
        {
            _textures.AddRange(textures);
        }

        public static void AddTextures(Texture2D texture)
        {
            _textures.Add(texture);
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
            int startIndex = 0;
            while (startIndex < input.Length)
            {
                startIndex = input.IndexOf("[", startIndex);
                if (startIndex == -1)
                    break;

            //    startIndex++;

                int endIndex = input.IndexOf("]", startIndex + 1);
                if (endIndex == -1)
                    break;

                if (output == null)
                    output = new List<Decoders.CharData>();

                if (_textures.Count <= 0)
                    throw new Exception("you must set some textures");

                string indexS = input.Substring(startIndex + 1, endIndex - startIndex - 1);

                output.Add(new CharData()
                {
                    StartIndex = startIndex,
                    Sprite = _textures[Convert.ToInt32(indexS)],
                });
             
                input = input.Remove(startIndex, endIndex - startIndex + 1);

                startIndex++;
                
               
            }

            trimed = input;
            return output;
        }
    }
}
