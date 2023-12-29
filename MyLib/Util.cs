using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kek
{
    public class Util
    {
        public static UInt64 _Get_Closest_Power(UInt64 val)
        {
            UInt64 _k = 0;
            while (true)
            {
                val = val >> 1;
                _k++;
                if (val == 1)
                {
                    break;
                }
            }
            return _k;
        }

        //возвести число в степень
        public static UInt64 _Power(UInt64 a, UInt64 b)
        {
            if (b == 0) return 1;

            UInt64 _res = 1;
            while (b > 0)
            {
                _res *= a;
                b--;
            }
            return _res;
        }
    }
}
