using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AtomicEngine2.Engine.Render
{
    public class BasicCameraTracker : IFocusable
    {
        Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public float X
        {
            get { return _position.X; }
            set { _position.X = value; }
        }
        public float Y
        {
            get { return _position.Y; }
            set { _position.Y = value; }
        }

        public BasicCameraTracker(Vector2 pos)
        {
            Position = pos;
        }
    }
}
