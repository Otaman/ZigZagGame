using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZigZagGame
{
    class Fraction
    {
        public Vector3 Velocity;
        public byte TTL = 15;
        public static float Size = 0.06f;

        private static Vector3 color = Color.Pink.ToVector3();

        public static Model Model;
        protected Vector3 _position;

        protected readonly Matrix _world;
        protected readonly Matrix _view;
        protected readonly Matrix _projection;

        protected readonly Matrix[] _transforms;

        public Fraction(Model model, Vector3 position, Matrix world, Matrix view, Matrix projection)
        {
            Model = model;
            _position = position;

            _world = world;
            _view = view;
            _projection = projection;

            _transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(_transforms);
        }

        public void Draw()
        {
            TTL--;

//            Velocity += new Vector3(0, -0.1f, 0);
            _position += Velocity;

            var w = _world * Matrix.CreateTranslation(_position);
            var difColor = new Vector3(0.2f);
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = color;

                    effect.DirectionalLight0.Direction = Vector3.Down;
                    effect.DirectionalLight0.DiffuseColor = difColor;
                    effect.DirectionalLight0.Enabled = true;

//                    effect.DirectionalLight1.Direction = Vector3.Left;
//                    effect.DirectionalLight1.DiffuseColor = difColor;
//                    effect.DirectionalLight1.Enabled = true;

                    effect.View = _view;
                    effect.Projection = _projection;
                    effect.World = _transforms[mesh.ParentBone.Index] * w;
                }
                mesh.Draw();
            }
        }

        public static Fraction GenerateRandom(Vector3 position, Matrix world, Matrix view, Matrix projection)
        {
            var rand = new Random();
            var res = new Fraction(Model, position, world, view, projection);
            res._position += new Vector3((float)(0.5 - rand.NextDouble()), (float)(0.5 - rand.NextDouble()), (float)(0.5 - rand.NextDouble()));
//            res.Velocity = new Vector3(0, 0, VelocityHelper.Coeficient);
            res.TTL += (byte)rand.Next(6);
//            res.Velocity = new Vector3((float)(0.5 - rand.NextDouble()), (float)(0.5 - rand.NextDouble()), (float)(0.5 - rand.NextDouble()));
            res.Velocity = new Vector3(0, 0.06f, 0);
            return res;
        }
    }
}
