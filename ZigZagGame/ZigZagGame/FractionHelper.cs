using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZigZagGame
{
    class FractionHelper
    {
        private readonly Model _fraction;
        private readonly List<Fraction> _activeFractions = new List<Fraction>();

        private readonly Matrix _world;
        private readonly Matrix _view;
        private readonly Matrix _projection;

        public FractionHelper(Model fraction, Matrix world, Matrix view, Matrix projection)
        {
            _fraction = fraction;
            _world = world;
            _view = view;
            _projection = projection;
            Fraction.Model = _fraction;
        }

        public void InitBeginState()
        {
            _activeFractions.Clear();
        }

        public void UpdateElements()
        {
            var list = _activeFractions.Where(o => o.TTL == 0).ToList();
            foreach (var octahedron in list)
            {
                _activeFractions.Remove(octahedron);
            }
        }

        public void DrawElements()
        {
            Debug.WriteLine("FractionHelper elements count: " + _activeFractions.Count);
            foreach (var activeFraction in _activeFractions)
            {
                activeFraction.Draw();
            }
        }

        public void CreateRandomFracion(Vector3 pos)
        {
            _activeFractions.Add(Fraction.GenerateRandom(pos, _world, _view, _projection));
        }
    }
}
