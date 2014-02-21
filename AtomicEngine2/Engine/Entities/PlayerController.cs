using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtomicEngine2.Engine.Entities
{
    public class PlayerController : EntityController
    {
        KeyboardState _prevKeyState;
        GamePadState _prevGPState;

        Keys _moveLeft = Keys.A;
        Keys _moveRight = Keys.D;
        Keys _jump = Keys.Space;
        Keys _crouch;

        Buttons _gp_jump = Buttons.A;
        Buttons _gp_crouch;

        float _xAcc = 5F;
        float _jumpSpeed = 1.0F;

        public PlayerController()
        {
            _prevGPState = GamePad.GetState(PlayerIndex.One);
            _prevKeyState = Keyboard.GetState();
        }

        public override EntityState Apply(EntityState entityState)
        {
            GamePadState currentGPState = GamePad.GetState(PlayerIndex.One);
            KeyboardState currentKeyState = Keyboard.GetState();

            if (currentKeyState.IsKeyDown(_moveRight))
                entityState.ReqX += _xAcc;

            if (currentKeyState.IsKeyDown(_moveLeft))
                entityState.ReqX -= _xAcc;

            if (currentKeyState.IsKeyDown(_jump) & _prevKeyState.IsKeyUp(_jump) 
                & entityState.IsOnGround)
            {
                entityState.YAcc -= _jumpSpeed;
            }

            if (currentGPState.IsButtonDown(_gp_jump) & _prevGPState.IsButtonUp(_gp_jump) 
                & entityState.IsOnGround)
            {
                entityState.YAcc -= _jumpSpeed;
            }

            entityState.ReqX += currentGPState.ThumbSticks.Left.X * _xAcc;

            _prevKeyState = currentKeyState;
            _prevGPState = currentGPState;

            return entityState;
        }
    }
}
