using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZigZagGame
{
    class BlockHelper
    {
        private Model _block;
        private readonly List<Block> _activeBlocks = new List<Block>();

        private readonly Matrix _world;
        private readonly Matrix _view;
        private readonly Matrix _projection;
        private readonly OctahedronHelper _octahedronHelper;

        private readonly Color[] _colors =
        {
            Color.DeepSkyBlue, 
            Color.BlueViolet, 
            Color.Coral, 
            Color.Chocolate, 
            Color.BurlyWood, 
            Color.DarkGoldenrod, 
            Color.LimeGreen,  
            Color.IndianRed, 
            Color.YellowGreen,
            new Color(213, 0, 0),
            Color.MidnightBlue
        };
        private Color _previousColor;
        private Color _currentColor;
        private Color _nextColor;

        private int _colorChangeFrequency = 400; //12 seconds
        private int _colorChangingTime = 33; //one second
        private int _colorChangeTimer;
        private int _colorChangingPhase;

        private bool _colorChanging;

        public BlockHelper(Matrix world, Matrix view, Matrix projection, OctahedronHelper octahedronHelper)
        {
            _world = world;
            _view = view;
            _projection = projection;
            _octahedronHelper = octahedronHelper;
        }

        public void InitBeginState(Model block, Model startBlock)
        {
            _block = block;

            _previousColor = _colors[0];
            _currentColor = _previousColor;
            Block.CurrentColor = _currentColor.ToVector3();

            _activeBlocks.Clear();
            _activeBlocks.Add(new StartBlock(startBlock, Vector3.Zero, _world, _view, _projection));

            while (_activeBlocks.Last().GetPositionZ() > -16*MHelper.Sqrt2)
            {
                CreateNextBlock();
            }
        }

        private void CreateNextBlock()
        {
            var x = (new Random().Next(2) < 1) ? -MHelper.Sqrt2 : MHelper.Sqrt2;
            var pos = _activeBlocks.Last().GetPosition() + new Vector3(x, 0, -MHelper.Sqrt2);
            if (pos.X > 5)
            {
                pos.X -= 2 * MHelper.Sqrt2;
            }
            if (pos.X < -5)
            {
                pos.X += 2 * MHelper.Sqrt2;
            }

            _activeBlocks.Add(new Block(_block, pos, _world, _view, _projection));
            if (new Random().NextDouble() < 0.15)
            {
                pos.Y += 3.1f;
                _octahedronHelper.CreateOctahedron(pos);
            }
        }

        public void DrawElements()
        {
            foreach (var activeBlock in _activeBlocks)
            {
                activeBlock.Draw();
            }
        }

        public void UpdateElements()
        {
            _colorChangeTimer++;
            if (_colorChangeTimer == _colorChangeFrequency)
            {
                _colorChanging = true;
                do
                {
                    _nextColor = _colors[new Random().Next(_colors.Length)];
                } while (_nextColor == _previousColor);
                
                _colorChangeTimer = 0;
            }

            if (_colorChanging)
            {
                _colorChangingPhase++;
                _currentColor = Color.Lerp(_previousColor, _nextColor, (float)_colorChangingPhase / _colorChangingTime);
                if (_colorChangingPhase == _colorChangingTime)
                {
                    _previousColor = _nextColor;
                    _colorChangingPhase = 0;
                    _colorChanging = false;
                }
            }

            Block.CurrentColor = _currentColor.ToVector3();

            if (_activeBlocks.Last().GetPositionZ() > -16*MHelper.Sqrt2)
            {
                CreateNextBlock();
            }

            if (_activeBlocks.Count > 28)
            {
                _activeBlocks.RemoveAt(0);
            }
//            var toRemoveList = _activeBlocks.Where(b => b.GetPosition().Y < -6).ToList();
//            foreach (var b in toRemoveList)
//            {
//                _activeBlocks.Remove(b);
//            }

//            Debug.WriteLine("Blocks count: " + _activeBlocks.Count);
        }

        public List<Block> GetBlocksOnZeroZ()
        {
            return _activeBlocks.Where(b => 
                    Math.Abs((Decimal) b.GetRealPosition().Z) < (Decimal) (MHelper.Sqrt2 * b.Size)).ToList();
        }
    }
}
