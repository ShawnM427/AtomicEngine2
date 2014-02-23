using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace MonoUI.API
{
    /// <summary>
    /// Handles keyboard input
    /// </summary>
    public class KeyboardManager
    {
        static KeyboardState _prevState = Keyboard.GetState();
        static KeyboardState _currState;

        /// <summary>
        /// Begins the current update for the keyboard
        /// </summary>
        public static void Begin()
        {
            _currState = Keyboard.GetState();
        }

        /// <summary>
        /// Checks if a key is currently down
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key is down</returns>
        public static bool IsKeyDown(Keys key)
        {
            return _currState.IsKeyDown(key);
        }

        /// <summary>
        /// Checks if a key is currently up
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key is up</returns>
        public static bool IsKeyUp(Keys key)
        {
            return _currState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if a key has been pressed on this tick
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key has been pressed</returns>
        public static bool IsPressed(Keys key)
        {
            return _currState.IsKeyDown(key) & _prevState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if a key has been released on this tick
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key has been released</returns>
        public static bool IsReleased(Keys key)
        {
            return _currState.IsKeyUp(key) & _prevState.IsKeyDown(key);
        }

        /// <summary>
        /// Ends the current update for the keyboard
        /// </summary>
        public static void End()
        {
            _prevState = _currState;
        }
    }
}
