using System;

namespace CoffeeLogger
{
    public class GrindDial
    {
        public bool isValid { get; private set; }
        private int? z, zMax;
        private int? y, yMax;
        private int? x, xMax;

        public GrindDial(int zMax, int xMax, int yMax)
        {
            this.zMax = zMax;
            this.xMax = xMax;
            this.yMax = yMax;
            z = 0;
            x = 0;
            y = 0;
            isValid = false;
        }

        public GrindDial(string maxes)
        {
            maxes = maxes.Trim().Replace(" ", string.Empty);
            if (string.IsNullOrWhiteSpace(maxes) || 
                !maxes.All(c => char.IsAsciiDigit(c) || c == ',') ||
                maxes.Contains(",,") ||
                !char.IsAsciiDigit(maxes[0]) ||
                !char.IsAsciiDigit(maxes[maxes.Length - 1]))
            {
                Console.WriteLine("Invalid dial format");
                isValid = false;
                return;
            }

            int[] maxVals = Array.ConvertAll(maxes.Split(','), int.Parse);
            int length = maxVals.Length - 1;
            isValid = true;

            if (length > 2)
            {
                Console.WriteLine("\nDial format too long, using only first 3 values");
                length = 2;
            }
            xMax = maxVals[length--];
            if (length < 0) { return; }
            yMax = maxVals[length--];
            if (length < 0) { return; }
            zMax = maxVals[length];
            return;
        }
    /*
        public int ZUp()
        {
            if (++z > zMax) { z = 0; }
            return z;
        }

        public int ZDown()
        {
            if (--z < 0) { z = zMax; }
            return z;
        }

        public int yUp()
        {
            if (++y > yMax) { y = 0; }
            return y;
        }

        public int YDown()
        {
            if (--y < 0) { y = yMax; }
            return y;
        }

        public int XUp()
        {
            if (++x > xMax) { x = 0; }
            return x;
        }

        public int XDown()
        {
            if (--x < 0) { x = xMax; } 
            return x;
        }
    */
        public string? GetDialFormatString()
        {
            if (isValid == false)
            { return null; }
            int?[] all = { zMax, yMax, xMax };
            all = all.Where(x => x != null).ToArray();
            return string.Join(",", all);
        }
        
        public int?[] GetDialFormatIntArr()
        {
            if (isValid == false)
            { return null; }
            int?[] val = {zMax, yMax, xMax};
            return val.Where(x => x != null).ToArray();
        }
    }
}