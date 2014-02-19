using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AtomicEngine2.Utils;

namespace AtomicEngine2.Engine
{
    /// <summary>
    /// Represents a level colllider totem for a bipedal entity
    /// </summary>
    public class BipedalCollisionTotem
    {
        RectCollider<BipedalColliderState>[] _relativeChecks;

        private RectCollider<BipedalColliderState> this[int index]
        {
            get { return _relativeChecks[index];}
        }

        public BipedalCollisionTotem(float xOffset, float totemWidth,float height = 48 )
        {
            float tH = height / 3;
            _relativeChecks = new RectCollider<BipedalColliderState>[3];

            _relativeChecks[0] = new RectCollider<BipedalColliderState>(
                new RectangleF(xOffset - totemWidth, -tH, totemWidth, tH), BipedalColliderState.Lower);
            _relativeChecks[1] = new RectCollider<BipedalColliderState>(
                new RectangleF(xOffset - totemWidth, -2 * tH, totemWidth, tH), BipedalColliderState.Middle);
            _relativeChecks[2] = new RectCollider<BipedalColliderState>(
                new RectangleF(xOffset - totemWidth, -3 * tH, totemWidth, tH), BipedalColliderState.Top); 
        }

        public BipedalColliderState Check(LevelCollider collider, Vector2 pos)
        {
            BipedalColliderState State = BipedalColliderState.None;

            for (int i = 0; i < _relativeChecks.Length; i++)
            {
                if (collider.GetIntersect(this[i].Rect) != Vector2.Zero)
                    State = State == BipedalColliderState.None ? 
                        (BipedalColliderState)this[i].Tag :
                        State | (BipedalColliderState)this[i].Tag;
            }

            return State;
        }

        /// <summary>
        /// Gets the intersect stack, ordered from bottom to top
        /// </summary>
        /// <param name="collider">The collider to check against</param>
        /// <param name="pos">The position of the stack's orgin</param>
        /// <returns>A Vector2[] that represents the intersects</returns>
        public Vector2[] GetIntersectStack(LevelCollider collider, Vector2 pos)
        {
            Vector2[] ret = new Vector2[3];

            for (int i = 0; i < _relativeChecks.Length; i++)
            {
                ret[i] = collider.GetIntersect(this[i].Rect, pos);
            }

            return ret;
        }
    }

    /// <summary>
    /// Represents a single point that has an object tag
    /// </summary>
    public class RectCollider<T>
    {
        RectangleF _rect;
        T _tag;

        public T Tag
        {
            get { return _tag; }
        }
        public RectangleF Rect
        {
            get { return _rect; }
        }

        public RectCollider(RectangleF rect, T tag)
        {
            _rect = rect;
            _tag = tag;
        }
    }

    /// <summary>
    /// Represents the state of a bipedal totem collision
    /// </summary>
    [Flags]
    public enum BipedalColliderState
    {
        Lower,
        Middle,
        Top,
        None
    }
}
