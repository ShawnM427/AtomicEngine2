using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AtomicEngine2.Engine.Entities
{
    public abstract class EntityController
    {
        public virtual EntityState Apply(EntityState entityState)
        {
            return entityState;
        }
    }

    /// <summary>
    /// Represents the state of an instance
    /// </summary>
    public struct EntityState
    {
        /// <summary>
        /// Gets whether the entity is on the ground
        /// </summary>
        public readonly bool IsOnGround;
        /// <summary>
        /// Gets the current position of the entity
        /// </summary>
        public readonly Vector2 Pos;
        /// <summary>
        /// Gets the current X position of the entity
        /// </summary>
        public float X
        {
            get { return Pos.X; }
        }
        /// <summary>
        /// Gets the current y position of the entity
        /// </summary>
        public float Y
        {
            get { return Pos.Y; }
        }

        /// <summary>
        /// Gets or sets the requested X acceleration
        /// </summary>
        public float ReqX;
        /// <summary>
        /// Gets or sets the requested Y acceleration
        /// </summary>
        public float ReqY;

        public EntityState(bool onGround, Vector2 pos, float reqX, float reqY)
        {
            this.IsOnGround = onGround;
            this.Pos = pos;
            this.ReqX = reqX;
            this.ReqY = reqY;
        }
    }
}
