using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZigZagGame
{
    class BallHelper
    {
        private readonly Matrix _world;
        private readonly Matrix _view;
        private readonly Matrix _projection;

        private Ball _ball;
        private bool _isFalling;

        public BallHelper(Matrix world, Matrix view, Matrix projection)
        {
            _world = world;
            _view = view;
            _projection = projection;
        }

        public void InitBeginState(Model ball, Vector3 position)
        {
            _isFalling = false;
            _ball = new Ball(ball, position, _world, _view, _projection);
        }

        public void DrawElement()
        {
            _ball.Draw();
        }

        public void ShangeVelocity()
        {
            if(!_isFalling)
                Ball.Velocity = -Ball.Velocity;
        }

        public Vector3 GetBallPosition()
        {
            return _ball.Position;
        }

        public void Fall()
        {
            _isFalling = true;
            Ball.Velocity += new Vector3(0, -0.1f, 0);
        }
    }
}
