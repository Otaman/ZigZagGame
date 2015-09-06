using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZigZagGame
{
    class Block
    {
        public virtual float Size { get; set; }

        public static Vector3 HorizontalVelocity = Vector3.Zero;
        public static Vector3 VerticalVelocity = new Vector3(0, -0.5f, 0);
        protected static readonly Vector3 DiffuseColor = new Vector3(0.3f);
        
        public static Vector3 CurrentColor = Color.DeepSkyBlue.ToVector3();

        protected readonly Model _model;
        protected Vector3 _position;

        protected readonly Matrix _world;
        protected readonly Matrix _view;
        protected readonly Matrix _projection;

        protected readonly Matrix[] _transforms;

        public Block(Model model, Vector3 position, Matrix world, Matrix view, Matrix projection)
        {
            Size = 1;
            _model = model;
            _position = position;

            _world = world;
            _view = view;
            _projection = projection;

            _transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(_transforms);
        }

        public virtual void Draw()
        {
            if (this.GetPositionZ() > 10)
            {
                _position += VerticalVelocity;
            }
            _position += HorizontalVelocity;

            var w = _world*Matrix.CreateTranslation(_position);

            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = CurrentColor;

                    effect.DirectionalLight0.Direction = Vector3.Down;
                    effect.DirectionalLight0.DiffuseColor = DiffuseColor;
                    effect.DirectionalLight0.Enabled = true;

                    effect.View = _view;
                    effect.Projection = _projection;
                    effect.World = _transforms[mesh.ParentBone.Index] * w;
                }
                mesh.Draw();
            }
        }

        public virtual float GetPositionZ()
        {
            return _position.Z;
        }

        public virtual Vector3 GetPosition()
        {
            return _position;
        }

        public Vector3 GetRealPosition()
        {
            return _position;
        }
    }
}
