using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Core
{
    public struct ComplexString
    {
        public struct TexturePart
        {
            public Vector2 Position;
            public int ImgId;
        }

        public struct StringPart
        {
            public string Text;
            public Vector2 Pos;

            public static implicit operator StringPart(string input)
            {
                return new Core.ComplexString.StringPart()
                {
                    Text = input,
                    Pos = Vector2.Zero,
                };
            }
        }

        private List<StringPart> _parts;
        private List<ComplexTexture> _textures;
        private List<TexturePart> _texturesToDraw;
        private float? _textureScale;
        private string _text;
        private float _scale;

        public static SpriteFont DefaultFont;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public float Scale
        {
            get { return _scale; }
            set { _scale = value;  CalculateLines(); }
        }
        public SpriteFont Font;
        public ComplexString(string text, SpriteFont font)
        {
            _text = text;
            _textures = new List<Core.ComplexTexture>();
            _parts = new List<StringPart>();
            Font = font;
            _texturesToDraw = new List<TexturePart>();
            _scale = 1;
            _textureScale = 1;
            Width = 0;
            Height = 0;
            CalculateLines();
        }

        private void CalculateLines()
        {
            if (_textureScale != null)
            {
                //set the texture scales
                foreach (ComplexTexture ct in _textures)
                {
                    ct.Scale = (float)_textureScale;
                }
            }
            _parts = GetLines(_text, 40);
        }

        public ComplexString(string text, SpriteFont font, List<ComplexTexture> textures, float? textureScale = null)
        {
            _text = text;
            _textures = textures;
            _parts = new List<StringPart>();
            Font = font;
            _texturesToDraw = new List<TexturePart>();
            _scale = 1;
            _textureScale = textureScale;
            Width = 0;
            Height = 0;
            _parts = GetLines(text, 40);
        }


        public List<TexturePart> GetTextureLocations()
        {
            List<TexturePart> output = new List<TexturePart>();

            return output;
        }

        private List<StringPart> GetLines(string input, float lineSpacing)
        {
            List<StringPart> output = new List<StringPart>();
            input += "\n";
            int index = input.IndexOf("\n", 0);
            int start = 0;
            Vector2 textPos = Vector2.Zero;
            _texturesToDraw.Clear();
            while (index != -1)
            {
                //look for textures in the string
                if (_textures.Count > 0 && start != index) //if we have some textures
                {
                    //first get texture
                    int startIndex = input.IndexOf("[", start, index - start);
                    int endIndex = index;

                    int stringStart = start;
                    while (startIndex != -1) //if a texture to draw
                    {
                        endIndex = input.IndexOf("]", startIndex, index - startIndex); //get end
                        if (endIndex == -1)
                            throw new Exception("invalid string format");

                        startIndex += 1;
                        //get the texture
                        int tex = Convert.ToInt32(input.Substring(startIndex, endIndex - startIndex));
                        //get width
                        float width = (_textures[tex].Texture.Width) * _textures[tex].Scale;

                        string text = input.Substring(stringStart, startIndex - stringStart - 1);
                        //break string up before the texture
                        output.Add(new StringPart()
                        {
                            Text = text,
                            Pos = textPos
                        });

                        //get position at end of text
                        textPos.X += Font.MeasureString(text).X * _scale;

                        //now add the texture
                        _texturesToDraw.Add(new TexturePart()
                        {
                            Position = textPos + new Vector2(0,5),
                            ImgId = tex,
                        });

                        //set text position for after texture
                        textPos.X += width;
                        if (textPos.X > Width)
                            Width = Convert.ToInt32(textPos.X);
                        stringStart = endIndex + 1;
                        startIndex = input.IndexOf("[", endIndex, index - startIndex);


                    }
                    
                    output.Add(new StringPart()
                    {
                        Text = input.Substring(stringStart, index - stringStart),
                        Pos = textPos
                    });
                    textPos.X += Font.MeasureString(input.Substring(stringStart, index - stringStart)).X * Scale;
                    if (textPos.X > Width)
                        Width = Convert.ToInt32(textPos.X);
                }
                else
                {
                    if (start != index) //make sure we dont add a black line
                        output.Add(new StringPart()
                        {
                            Text = input.Substring(start, index - start),
                            Pos = textPos
                        });
                    textPos.X += Font.MeasureString(input.Substring(start, index - start)).X * Scale;
                    if (textPos.X > Width)
                        Width = Convert.ToInt32(textPos.X);

                }
                start = index + 1;
                index = input.IndexOf("\n", start);
                 //reset for new line
                textPos.Y += lineSpacing;
                textPos.X = 0;
            }
            output.Add(new StringPart()
            {
                Text = input.Substring(start),
                Pos = textPos,
            });
            textPos.X += Font.MeasureString(input.Substring(start)).X * Scale;
            if (textPos.X > Width)
                Width = Convert.ToInt32(textPos.X);
            textPos.X = 0;
            Height = Convert.ToInt32(output.Count * lineSpacing);
            
            
            return output;
        }


        public static implicit operator ComplexString(string text)
        {
            return new ComplexString(text, DefaultFont);
        }


        public void Draw(SpriteBatch sb, Vector2 pos, Color color, float fade)
        {
            foreach(StringPart s in _parts)
                sb.DrawString(Font, s.Text, pos + s.Pos, color, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);

            foreach (TexturePart t in _texturesToDraw)
                _textures[t.ImgId].Draw(sb, t.Position + pos, fade);
        }

    }
}
