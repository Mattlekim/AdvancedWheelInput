using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Core
{
    public struct Range
    {
        /// <summary>
        /// the minumum value
        /// </summary>
        public float Min;
        /// <summary>
        /// the maximum value
        /// </summary>
        public float Max;

        /// <summary>
        /// weather the range has been set or not
        /// </summary>
        public bool isSet;

        /// <summary>
        /// clears the current min and max and resets the isset flag
        /// </summary>
        public void Clear()
        {
            Min = 0;
            Max = 0;
            isSet = false;
        }
        /// <summary>
        /// creates a new range instance
        /// </summary>
        /// <param name="min">the minimum value</param>
        /// <param name="max">the maximum value</param>
        public Range(float min, float max)
        {
            if (min > max)
                throw new Exception("Input value invalid, Min cannot be bigger the max");
            Min = min;
            Max = max;
            isSet = true; //set the flag
        }


        /// <summary>
        /// returns true if the input value is within the min and max
        /// </summary>
        /// <param name="value">the input value</param>
        /// <returns>bool</returns>
        public bool Contains(float value)
        {
            return value >= Min && value <= Max;
        }
    }
}
