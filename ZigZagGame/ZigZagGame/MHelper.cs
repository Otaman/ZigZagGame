using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ZigZagGame
{
    static class MHelper
    {
        public static readonly float Sqrt2 = (float) Math.Sqrt(2);

        public static bool BallOnBlock(Vector3 ballPosition, List<Block> blocks)
        {
//            Vector2 ball = new Vector2(ballPosition.X, ballPosition.Y);

            

            foreach (var block in blocks)
            {
//                var pos = block.GetRealPosition();
//
//                Vector2 blockC = new Vector2(pos.X, pos.Y);
//                Vector2 blockA = new Vector2(pos.X + block.Size, pos.Y);
//                Vector2 blockB = new Vector2(pos.X, pos.Y + block.Size);
//
//
//
//                if ((TriangleCompositionArea(blockA, blockB, blockC, ball) - TriangleArea(blockA, blockB, blockC)) <
//                    0.01)
//                {
//                    return true;
//                }
                if (Math.Abs((decimal) (ballPosition.X - block.GetRealPosition().X)) +
                    Math.Abs((decimal) (ballPosition.Z - block.GetRealPosition().Z)) < ((decimal) (block.Size*Sqrt2)))
                {
                    return true;
                }
            }
            return false;
        }

        private static float TriangleCompositionArea(Vector2 A, Vector2 B, Vector2 C, Vector2 M)
        {
            return (TriangleArea(A, B, M) + TriangleArea(C, B, M) + TriangleArea(A, C, M));
        }

        private static float TriangleArea(Vector2 A, Vector2 B, Vector2 C)
        {
            float ab = (float)Math.Sqrt(Math.Pow((A.X - B.X), 2) + Math.Pow((A.Y - B.Y), 2));
            float bc = (float)Math.Sqrt(Math.Pow((C.X - B.X), 2) + Math.Pow((C.Y - B.Y), 2));
            float ac = (float)Math.Sqrt(Math.Pow((A.X - C.X), 2) + Math.Pow((A.Y - C.Y), 2));

            float p = (ab + bc + ac) / 2;

            return (float)Math.Sqrt(p * (p - ab) * (p - bc) * (p - ac));
        }


        public static void ExplodeOctahedrons(Vector3 ball, OctahedronHelper octahedronHelper, FractionHelper fractionHelper)
        {
            var octahedrons = octahedronHelper.GetOctahedronsOnZeroZ();
            foreach (var octahedron in octahedrons)
            {
                if (Math.Abs((decimal)(ball.X - octahedron.GetRealPosition().X)) +
                    Math.Abs((decimal)(ball.Z - octahedron.GetRealPosition().Z)) < (decimal)(octahedron.Size * Sqrt2))
                {
                    octahedron.Explode(fractionHelper);
                    octahedronHelper.Remove(octahedron);
                }
            }
        }
    }
}
