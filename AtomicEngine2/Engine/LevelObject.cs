using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AtomicEngine2.Utils;

namespace AtomicEngine2.Engine
{
    /// <summary>
    /// Represents a static object in a level
    /// </summary>
    public abstract class LevelObject
    {
        protected Vector2 _pos;
        protected Vector2 _max;

        /// <summary>
        /// Gets or sets the top-left X-coords of the bounds
        /// </summary>
        public float X
        {
            get { return _pos.X; }
            set { _pos.X = value; _bounds.X = value; }
        }
        /// <summary>
        /// Gets or sets the top-left Y-coords of the bounds
        /// </summary>
        public float Y
        {
            get { return _pos.Y; }
            set { _pos.Y = value; _bounds.Y = value; }
        }
        /// <summary>
        /// Gets or sets the top-left corner of the bounds
        /// </summary>
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; _bounds.X = value.X; _bounds.Y = value.Y; }
        }

        /// <summary>
        /// Gets the maximum x co-ord of the bounds
        /// </summary>
        public virtual float MaxX
        {
            get { return _max.X; }
            protected set { _max.X = value; _bounds.Width = value - X; }
        }
        /// <summary>
        /// Gets the maximum y co-ord of the bounds
        /// </summary>
        public virtual float MaxY
        {
            get { return _max.Y; }
            protected set { _max.Y = value; _bounds.Height = value - Y; }
        }
        /// <summary>
        /// Gets the maximum co-ords of the bounds
        /// </summary>
        public virtual Vector2 Max
        {
            get { return _max; }
            protected set
            {
                _max = value; _bounds.Width = value.X - X; _bounds.Height = value.Y - Y;
            }
        }

        /// <summary>
        /// Gets the bounds of this object
        /// </summary>
        public virtual RectangleF Bounds
        {
            get { return _bounds; }
            protected set
            {
                _bounds = value;
                X = value.X; Y = value.Y; MaxX = value.X + value.Width; MaxY = value.Y + value.Height;
            }
        }
        RectangleF _bounds;

        /// <summary>
        /// Adds this object to the level's geometry
        /// </summary>
        /// <param name="geometry">The level geometry to add to</param>
        public abstract void AddToGeometry(LevelGeometry geometry, TextureManager texManager);

        /// <summary>
        /// Checks if this level object contains a point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if this level object contains the point</returns>
        public abstract bool ContainsPoint(Vector2 point);
    }
}
