using System;

namespace CoffeeLogger
{
    public class GrindDial
    {
        private int z, zMax;
        private int y, yMax;
        private int x, xMax;
        public GrindDial(int zMax, int xMax, int yMax) 
        {
            this.zMax = zMax;
            this.xMax = xMax;
            this.yMax = yMax;
            z = 0;
            x = 0;
            y = 0;
        }
        public GrindDial(string maxes)
        {
            maxes = maxes.Trim().Replace(" ", string.Empty);
            if (string.IsNullOrWhiteSpace(maxes) || !maxes.All(c => char.IsAsciiDigit(c) || c == ','))
            {
                throw new ArgumentException("Invalid dial format");
            }
            int[] maxVals = Array.ConvertAll(maxes.Split(','), int.Parse);
            int length = maxVals.Length - 1;

            if (length > 2)
            {
                Console.WriteLine("Dial format too long, using only first 3 vals");
                length = 2;
            }
            xMax = maxVals[length--];
            if (length < 0) { return; }
            yMax = maxVals[length--];
            if (length < 0) { return; }
            zMax = maxVals[length];
            return;
        }
        public int ZUp()
        {
            z++;
            if (z > zMax)
            {
                z = 0;
            }
            return z;
        }
        public int ZDown()
        {
            if (z == 0)
            {
                z = zMax;
            }
            else
            {
                z--;
            }
            return z;
        }
        public int yUp()
        {
            y++;
            if (y > yMax)
            {
                y = 0;
            }
            return y;
        }
        public int YDown()
        {
            if (y == 0)
            {
                y = yMax;
            }
            else
            {
                y--;
            }
            return y;
        }
        public int XUp()
        {
            x++;
            if (x > xMax)
            {
                x = 0;
            }
            return x;
        }
        public int XDown()
        {
            if (x == 0)
            {
                x = xMax;
            }
            else 
            {
                x--;
            }
            return x;
        }
        public int Result()
        {
            return z * 100 + y * 10 + x;
        }
    }
}
