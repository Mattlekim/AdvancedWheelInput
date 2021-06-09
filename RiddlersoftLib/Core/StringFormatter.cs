using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Core
{
    public static class StringFormatter
    {
        public static string FromTimeSpan(TimeSpan time, bool showSeconds)
        {
            if (time.Ticks == 0)
                return $"0s";

            if (showSeconds)
            {
                if (time.Days >= 7)
                {
                    int weeks = (int)(time.Days / 7);
                    return $"{weeks}w {time.Days - weeks * 7}d {time.Hours}h {time.Minutes}m {time.Seconds}s";
                }

                if (time.Days != 0)
                    return $"{time.Days}d {time.Hours}h {time.Minutes}m {time.Seconds}s";

                if (time.Hours != 0)
                    return $"{time.Hours}h {time.Minutes}m {time.Seconds}s";

                if (time.Minutes != 0)
                    return $"{time.Minutes}m {time.Seconds}s";

                return $"{time.Seconds}s";
            }
            if (time.Days >= 7)
            {
                int weeks = (int)(time.Days / 7);
                return $"{weeks}w {time.Days - weeks * 7}d {time.Hours}h {time.Minutes}m";
            }

            if (time.Days != 0)
                return $"{time.Days}d {time.Hours}h {time.Minutes}m";

            if (time.Hours != 0)
                return $"{time.Hours}h {time.Minutes}m";

            return $"{time.Minutes}m";
        }


        public static string FromFloat(float time, bool showSeconds = false)
        {
            if (time == 0)
                return $"0s";

            int m = (int)(time / 60f);
            int h = (int)(m / 60f);
            string mil = Convert.ToString((int)((time - (int)time) * 1000));
            string sec = Convert.ToString(((int)time) - m * 60);
            string mins = Convert.ToString(m);
            string hr = Convert.ToString(h);

            if (sec.Length == 0)
                sec = $"{00}";
            else
            if (sec.Length == 1)
                sec = $"{0}{sec}";

            if (mins.Length == 0)
                mins = $"{00}";
            else
                if (mins.Length == 1)
                mins = $"{0}{mins}";
            return $"{hr}:{mins}:{sec}:{mil}";
        }
    }
}
