using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace AtomicEngine2.Engine
{
    public class PlayerController : EntityController
    {
        KeyboardState _prevKeyState;
        GamePadState _prevGamepadState;

        Keys _moveLeft = Keys.A;
        Keys _moveRight = Keys.D;
        Keys _jump;
        Keys _crouch;

        GamePadThumbSticks _gp_moveStick;
        GamePadButtons _gp_jump;
        GamePadButtons _gp_crouch;

        float _xAcc = 2F;

        public PlayerController()
        {
            //_prevGamepadState = GamePad.
            _prevKeyState = Keyboard.GetState();
        }

        public override EntityState Apply(EntityState entityState)
        {
            //GamePadState currentGPState = GamePad.GetState(PlayerIndex.One);
            KeyboardState currentKeyState = Keyboard.GetState();

            if (currentKeyState.IsKeyDown(_moveRight))
                entityState.ReqX += _xAcc;

            if (currentKeyState.IsKeyDown(_moveLeft))
                entityState.ReqX -= _xAcc;

            //entityState.ReqX += currentGPState.ThumbSticks.Left.X * _xAcc;

            _prevKeyState = currentKeyState;
            //_prevGamepadState = currentGPState;

            return entityState;
        }
    }
}
