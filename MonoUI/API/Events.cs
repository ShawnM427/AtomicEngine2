using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonoUI.API
{
    /// <summary>
    /// The event arguments raised when a control's focus changes
    /// </summary>
    public class FocusChangedEventArgs
    {
        bool _focused;
        /// <summary>
        /// The new focus of the control
        /// </summary>
        public bool Focused
        {
            get { return _focused; }
        }

        /// <summary>
        /// Creates a new FocusChangedEventArgs
        /// </summary>
        /// <param name="focus">The controls new focus</param>
        public FocusChangedEventArgs(bool focus)
        {
            _focused = focus;
        }
    }

    /// <summary>
    /// Handles events for when a control's focus changes
    /// </summary>
    /// <param name="e">The focus changed event args</param>
    public delegate void FocusChangedEventHandler(object sender, FocusChangedEventArgs e);

    /// <summary>
    /// The event arguments raised when a control's focus changes
    /// </summary>
    public class BoundsChangedEventArgs
    {
        Rectangle _prevBounds;
        Rectangle _newBounds;

        /// <summary>
        /// The old bounds of the control
        /// </summary>
        public Rectangle OldBounds
        {
            get { return _prevBounds; }
        }
        /// <summary>
        /// The new bounds of the control
        /// </summary>
        public Rectangle NewBounds
        {
            get { return _newBounds; }
        }

        /// <summary>
        /// Creates a new bounds changed event args
        /// </summary>
        /// <param name="oldBounds">The old bounds of the control</param>
        /// <param name="newBounds">The new bounds of the control</param>
        public BoundsChangedEventArgs(Rectangle oldBounds, Rectangle newBounds)
        {
            _prevBounds = oldBounds;
            _newBounds = newBounds;
        }
    }

    /// <summary>
    /// Handles events for when a control's bounds changes
    /// </summary>
    /// <param name="e">The bounds changed event args</param>
    public delegate void BoundsChangedEventHandler(object sender, BoundsChangedEventArgs e);
}
