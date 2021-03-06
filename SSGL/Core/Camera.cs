﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Core
{
    public class Camera : GameComponent
    {
        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }
        public float Far { get; protected set; }
        public float Near { get; protected set; }
        public Vector3 Position { get; protected set; }
        public BoundingFrustum Frustrum { get; set; }
        
        private Vector3 _direction;
        private Vector3 _up;
        private float _speed;
        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;
        private const int TIME_PER_TICK = 60;

        public Camera(Game game, Vector3 position, Vector3 target, Vector3 up, float near = 1.0f, float far = 1500f)
            : base(game)
        {
            Far = far;
            Near = near;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)(Game.Window.ClientBounds.Width / Game.Window.ClientBounds.Height), Near, Far);
            Position = position;
            this._direction = target - position;
            this._direction.Normalize();
            this._up = up;

            //Pitch camera a bit to get a more top-down view
            this._direction = Vector3.Transform(this._direction, Matrix.CreateFromAxisAngle(Vector3.Cross(this._up, this._direction), (MathHelper.PiOver4 / 100) * 50));

            this._speed = 0.25f;
            this.CreateLookAt();
            this.Frustrum = new BoundingFrustum(this.View * this.Projection);
        }

        public override void Initialize()
        {
            // Set mouse position and do initial get state
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
            this._previousMouseState = Mouse.GetState();
            this._previousKeyboardState = Keyboard.GetState();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //Keyboard
            // Move side to side
            if (Keyboard.GetState( ).IsKeyDown(Keys.A))
            {
                Vector3 diff = Vector3.Cross(this._up, this._direction) * (this._speed * gameTime.ElapsedGameTime.Milliseconds);
                diff.Y = 0;
                Position += diff;
            }
            if (Keyboard.GetState( ).IsKeyDown(Keys.D))
            {
                Vector3 diff = Vector3.Cross(this._up, this._direction) * (this._speed * gameTime.ElapsedGameTime.Milliseconds);
                diff.Y = 0;
                Position -= diff;
            }
            // Move forward/backward
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Vector3 diff = Vector3.Cross(Vector3.Cross(this._up, this._direction), this._up) * (this._speed * gameTime.ElapsedGameTime.Milliseconds);
                diff.Y = 0;
                Position += diff;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Vector3 diff = Vector3.Cross(Vector3.Cross(this._up, this._direction), this._up) * (this._speed * gameTime.ElapsedGameTime.Milliseconds);
                diff.Y = 0;
                Position -= diff;
            }

            // Zoom in stepwize
            KeyboardState NewState = Keyboard.GetState();
            if (NewState.IsKeyDown(Keys.E) && _previousKeyboardState.IsKeyUp(Keys.E))
            {
                if(Position.Y > 25)
                    Position /= 2;
            }

            if (NewState.IsKeyDown(Keys.Q) && _previousKeyboardState.IsKeyUp(Keys.Q))
            {
                if (Position.Y < 200)
                    Position *= 2;
            }

            //Mouse
            // Zoom Out/In
            //if (Mouse.GetState().ScrollWheelValue > this._previousMouseState.ScrollWheelValue)
            //    Position += this._direction * 5.0f;
            //if (Mouse.GetState().ScrollWheelValue < this._previousMouseState.ScrollWheelValue)
            //    Position -= this._direction * 5.0f;
            
            
            if (Mouse.GetState( ).RightButton == ButtonState.Pressed)
            {
            }

            if(Mouse.GetState( ).MiddleButton == ButtonState.Pressed){
                //Yaw
                float yawAngle = (-MathHelper.PiOver4 / 150) * (Mouse.GetState().X - this._previousMouseState.X);
                this._direction = Vector3.Transform(this._direction, Matrix.CreateFromAxisAngle(this._up, yawAngle));

                //Pitch
                this._direction = Vector3.Transform(this._direction, Matrix.CreateFromAxisAngle(Vector3.Cross(this._up, this._direction), (MathHelper.PiOver4 / 100) * (Mouse.GetState().Y - this._previousMouseState.Y)));
            }
            else
            {
                ////Edge movement
                //if (Mouse.GetState().Y < (Game.Window.ClientBounds.Height * 0.05))
                //{
                //    Vector3 diff = Vector3.Cross(Vector3.Cross(this._up, this._direction), this._up) * this._speed;
                //    diff.Y = 0;
                //    Position += diff;
                //}
                //if (Mouse.GetState().Y > (Game.Window.ClientBounds.Height * 0.95))
                //{
                //    Vector3 diff = Vector3.Cross(Vector3.Cross(this._up, this._direction), this._up) * this._speed;
                //    diff.Y = 0;
                //    Position -= diff;
                //}
                //if (Mouse.GetState().X < (Game.Window.ClientBounds.Width * 0.05))
                //    Position += Vector3.Cross(this._up, this._direction) * this._speed;
                //if (Mouse.GetState().X > (Game.Window.ClientBounds.Width * 0.95))
                //    Position -= Vector3.Cross(this._up, this._direction) * this._speed;
            }

            // Reset prevMouseState
            this._previousMouseState = Mouse.GetState( );

            //Recreate camera-view matrix
            this.CreateLookAt();

            _previousKeyboardState = NewState;

            base.Update(gameTime);
        }

        private void CreateLookAt()
        {
            View = Matrix.CreateLookAt(Position, Position + this._direction, this._up);
        }
    }
}
