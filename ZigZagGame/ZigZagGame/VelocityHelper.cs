using Microsoft.Xna.Framework;

namespace ZigZagGame
{
    static class VelocityHelper
    {
        public static float Coeficient = 0.18f;

        public static void InitVelocity()
        {
            Block.HorizontalVelocity = new Vector3(0, 0, Coeficient);
            Block.VerticalVelocity = new Vector3(0, -2.2f*Coeficient, 0);
            Ball.Velocity = new Vector3(Coeficient, 0, 0);
        }

        public static void ClearVelocity()
        {
            Block.HorizontalVelocity = Vector3.Zero;
        }
    }
}
