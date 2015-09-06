using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZigZagGame
{
    class OctahedronHelper
    {
        private readonly Model _octahedron;
        private readonly List<Octahedron> _activeOctahedrons = new List<Octahedron>();

        private readonly Matrix _world;
        private readonly Matrix _view;
        private readonly Matrix _projection;

        public OctahedronHelper(Model octahedron, Matrix world, Matrix view, Matrix projection)
        {
            _octahedron = octahedron;
            _world = world;
            _view = view;
            _projection = projection;
        }


        public void CreateOctahedron(Vector3 pos)
        {
            _activeOctahedrons.Add(new Octahedron(_octahedron, pos, _world, _view, _projection));
        }

        public void UpdateElements()
        {
            var list = _activeOctahedrons.Where(o => o.GetRealPosition().Y < -6).ToList();
            foreach (var octahedron in list)
            {
                _activeOctahedrons.Remove(octahedron);
            }
        }

        public void DrawElements()
        {
            foreach (var activeOctahedron in _activeOctahedrons)
            {
                activeOctahedron.Draw();
            }
        }

        public void InitBeginState()
        {
            _activeOctahedrons.Clear();
        }

        public List<Octahedron> GetOctahedronsOnZeroZ()
        {
            return _activeOctahedrons.Where(o =>
                    Math.Abs((Decimal)o.GetRealPosition().Z) < (Decimal)(MHelper.Sqrt2 * o.Size)).ToList();
        }

        public void Remove(Octahedron octahedron)
        {
            _activeOctahedrons.Remove(octahedron);
        }
    }
}
