using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZigZagGame
{
    class Ball
    {
        public static Vector3 Velocity = Vector3.Zero;

        private readonly Model _model;
        public Vector3 Position;

        private readonly Matrix _world;
        private readonly Matrix _view;
        private readonly Matrix _projection;

        public Ball(Model model, Vector3 position, Matrix world, Matrix view, Matrix projection)
        {
            _model = model;
            Position = position;

            _world = world;
            _view = view;
            _projection = projection;
        }

        public void Draw()
        {
            Position += Velocity;
            var transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = new Vector3(0.4f);

                    effect.DirectionalLight0.Direction = Vector3.Down;
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.3f);
                    effect.DirectionalLight0.Enabled = true;

                    effect.View = _view;
                    effect.Projection = _projection;
                    effect.World = transforms[mesh.ParentBone.Index] * _world * Matrix.CreateTranslation(Position);
                }
                mesh.Draw();
            }
        }
    }
}
