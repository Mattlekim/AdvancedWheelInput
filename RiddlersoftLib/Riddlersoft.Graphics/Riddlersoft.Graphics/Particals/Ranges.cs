using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Particals
{
    public static class RandomClass
    {
        public static Random Random { get; private set; } = new Random();
    }

    public struct ColourRange
    {
        public Color One { get; set; }
        public Color Two { get; set; }

        public ColourRange(Color col)
        {
            One = col;
            Two = col;
        }

        public ColourRange(Color one, Color two)
        {
            One = one;
            Two = two;
        }

        public void ReplaceOne(Color col)
        {
            One = col;
        }

        public void ReplaceTwo(Color col)
        {
            Two = col;
        }

        public Color GetValue(float amount)
        {
            return Color.Lerp(One, Two, amount);
        }
    }

    public struct FloatRange
    {
        public float One { get; set; }
        public float Two { get; set; }


        public FloatRange(float one, float two)
        {
            One = one;
            Two = two;
        }

        public float GetValue(float amount)
        {
            return (Two - One) * amount + One;
        }

        public float GetValue()
        {
            float amount = (float)RandomClass.Random.NextDouble();
            return (Two - One) * amount + One;
        }

        public static implicit operator FloatRange(float input)
        {
            return new Particals.FloatRange(input, input);
        }
    }

    public struct Vector2Range
    {
        public Vector2 One { get; set; }
        public Vector2 Two { get; set; }

        public Vector2Range(Vector2 one, Vector2 two)
        {
            One = one;
            Two = two;
        }

        public Vector2 LerpVector2(Vector2 one, Vector2 two, float amount, float amount2)
        {
            return new Vector2(MathHelper.Lerp(one.X, two.X, amount), MathHelper.Lerp(one.Y, two.Y, amount2));
        }

        public Vector2 GetValue(float amount, float amount2)
        {
            return LerpVector2(One, Two, amount, amount2);
        }
    }
}
