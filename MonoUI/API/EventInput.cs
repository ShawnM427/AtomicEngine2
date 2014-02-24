using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OpenTK.Input;
using OpenTK;

using GameWindow = Microsoft.Xna.Framework.GameWindow;

namespace MonoUI.API
{
    /// <summary>
    /// Handles event-based keyboard input
    /// </summary>
    public static class EventInput
    {
        #region Events
        static KeyDownEventHandler _keyPressed;
        static CharPressedEventhandler _charPressed;
        static KeyUpEventHandler _keyUp;
        static MouseButtonEventHandler _mouseDown;
        static MouseButtonEventHandler _mouseUp;

        /// <summary>
        /// Gets or sets the event raised when a key is pressed
        /// </summary>
        public static KeyDownEventHandler KeyPressed
        {
            get { return _keyPressed; }
            set { _keyPressed = value; }
        }
        /// <summary>
        /// Gets or sets the event raised when a key is released
        /// </summary>
        public static KeyUpEventHandler KeyUp
        {
            get { return _keyUp; }
            set { _keyUp = value; }
        }
        /// <summary>
        /// Gets or sets the event raised when a character is pressed
        /// </summary>
        public static CharPressedEventhandler CharPressed
        {
            get { return _charPressed; }
            set { _charPressed = value; }
        }

        /// <summary>
        /// Gets or sets the event when the mouse is pressed
        /// </summary>
        public static MouseButtonEventHandler MouseDown
        {
            get { return _mouseDown; }
            set { _mouseDown = value; }
        }
        /// <summary>
        /// Gets or sets the event when the mouse is released
        /// </summary>
        public static MouseButtonEventHandler MouseUp
        {
            get { return _mouseUp; }
            set { _mouseUp = value; }
        }
        #endregion

        /// <summary>
        /// Hooks the text input to a game window
        /// </summary>
        /// <param name="window">The window to hook in to</param>
        public static void HookKeys(GameWindow window)
        {
            OpenTK.GameWindow OTKWindow = null;
            Type type = typeof(OpenTKGameWindow);

            System.Reflection.FieldInfo field = type.GetField("window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                OTKWindow = field.GetValue(window) as OpenTK.GameWindow;
            }
                        
            if (OTKWindow != null)
            {
                OTKWindow.KeyPress += _CharPress;
                OTKWindow.Keyboard.KeyUp += _KeyUp;
                OTKWindow.Keyboard.KeyDown += _KeyPressed;

                OTKWindow.Mouse.ButtonDown += _MousePressed;
                OTKWindow.Mouse.ButtonUp += _MouseReleased;
            }
        }

        private static void _KeyPressed(object sender, KeyboardKeyEventArgs e)
        {
            if (_keyPressed != null)
                _keyPressed.Invoke(new KeyDownEventArgs(e.Key));
        }

        private static void _KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (_keyUp != null)
                _keyUp.Invoke(new KeyUpEventArgs(e.Key));
        }

        private static void _CharPress(object sender, KeyPressEventArgs e)
        {
            if (_charPressed != null)
                _charPressed.Invoke(new CharPressedEventArgs(e.KeyChar));
        }

        private static void _MousePressed(object sender, MouseButtonEventArgs e)
        {
            if (_mouseDown != null)
                _mouseDown.Invoke(sender, e);
        }

        private static void _MouseReleased(object sender, MouseButtonEventArgs e)
        {
            if (_mouseUp != null)
                _mouseUp.Invoke(sender, e);
        }
    }

    #region Events
    #region Keyboard
    /// <summary>
    /// Represents a key that has been pressed
    /// </summary>
    public class KeyDownEventArgs 
    {
        Key _key;
        public Key Key{
            get{return _key;}
        }

        public KeyDownEventArgs(Key key)
        {
            _key = key;
        }
    }
    /// <summary>
    /// Handles when a key is pressed
    /// </summary>
    /// <param name="e">The KeyDownEventArgs to pass</param>
    public delegate void KeyDownEventHandler(KeyDownEventArgs e);

    /// <summary>
    /// Represents a key that has been released
    /// </summary>
    public class KeyUpEventArgs
    {
        Key _key;
        public Key Key
        {
            get { return _key; }
        }

        public KeyUpEventArgs(Key key)
        {
            _key = key;
        }
    }
    /// <summary>
    /// Handles when a key is released
    /// </summary>
    /// <param name="e">The KeyUpEventArgs to pass</param>
    public delegate void KeyUpEventHandler(KeyUpEventArgs e);

    /// <summary>
    /// Represents a char that has been pressed
    /// </summary>
    public class CharPressedEventArgs
    {
        char _char;
        public char KeyChar
        {
            get { return _char; }
        }

        public CharPressedEventArgs(char keyChar)
        {
            _char = keyChar;
        }
    }
    /// <summary>
    /// Handles when a character is pressed
    /// </summary>
    /// <param name="e">The CharPressedEventArgs to pass</param>
    public delegate void CharPressedEventhandler(CharPressedEventArgs e);
    #endregion

    #region Mouse
    public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);
    #endregion
    #endregion
}
