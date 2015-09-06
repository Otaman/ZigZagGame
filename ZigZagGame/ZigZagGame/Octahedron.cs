using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZigZagGame
{
    class Octahedron : Block
    {
        public Octahedron(Model model, Vector3 position, Matrix world, Matrix view, Matrix projection) : base(model, position, world, view, projection)
        {
            Size = 0.666f;
        }

        public override void Draw()
        {
            if (this.GetPositionZ() > 10)
            {
                _position += VerticalVelocity;
            }
            _position += HorizontalVelocity;

            var w = _world * Matrix.CreateTranslation(_position);
            var color = Color.Pink.ToVector3();
            var difColor = new Vector3(0.2f);
            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = color;

                    effect.DirectionalLight0.Direction = Vector3.Down;
                    effect.DirectionalLight0.DiffuseColor = difColor;
                    effect.DirectionalLight0.Enabled = true;

                    effect.DirectionalLight1.Direction = Vector3.Left;
                    effect.DirectionalLight1.DiffuseColor = difColor;
                    effect.DirectionalLight1.Enabled = true;

                    effect.View = _view;
                    effect.Projection = _projection;
                    effect.World = _transforms[mesh.ParentBone.Index] * w;
                }
                mesh.Draw();
            }
        }

        public void Explode(FractionHelper fractionHelper)
        {
            Game1.Counter += 2;

            var count = 4 + new Random().Next(5);
            var pos = _position + new Vector3(0, 0.5f, 0);

            for (int i = 0; i < count; i++)
            {
                fractionHelper.CreateRandomFracion(pos);
//                var fraction = Fraction.GenerateRandom(pos, _world, _view, _projection);

            }
        }
    }
}
