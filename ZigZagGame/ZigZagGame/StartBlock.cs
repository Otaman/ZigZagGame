using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZigZagGame
{
    sealed class StartBlock : Block
    {
        public override float Size { get; set; }

        public StartBlock(Model model, Vector3 position, Matrix world, Matrix view, Matrix projection) : base(model, position, world, view, projection)
        {
            Size = 8;
        }

        public override float GetPositionZ()
        {
            return base.GetPositionZ() - 7f*MHelper.Sqrt2;
        }

        public override Vector3 GetPosition()
        {
            return base.GetPosition() + new Vector3(0, 0, -7*MHelper.Sqrt2);
        }
    }
}
