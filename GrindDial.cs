using System;

namespace CoffeeLogger
{
    public class GrindDial
    {
        private byte z, zMax;
        private byte y, yMax;
        private byte x, xMax;
        public GrindDial(byte zMax, byte xMax, byte yMax) 
        {
            this.zMax = zMax;
            this.xMax = xMax;
            this.yMax = yMax;
            z = 0;
            x = 0;
            y = 0;
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
