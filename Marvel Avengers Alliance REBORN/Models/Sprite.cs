﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marvel_Avengers_Alliance_REBORN.Models
{
    class Sprite
    {
        #region UpdateFrame var
        protected int _cur_frame;
        protected int _cur_column;
        protected int _cur_row;
        protected int _frame_width;
        protected int _frame_height;

        protected int _frame_per_sec = 15;
        protected int _time_cast;

        protected float _timePerFrame;
        protected float _TotalElapsed;
        
        protected bool _Paused;
        #endregion

        #region Graphic Field
        protected bool isPointed;
        protected bool isFocus;
        protected Texture2D _cur_texture;
        protected MouseState _currentMouse;
        protected MouseState _previousMouse;
        public event EventHandler Click;
        protected SpriteSortMode _loc;
        
        #endregion

        #region Motion var
        protected float _Rotation = 0;
        protected float _Scale = 1;
        protected float _Depth = 0;
        protected Vector2 _cur_position;
        protected Vector2 _rotate_point;
        protected Vector2 _velocity;
        #endregion

        #region Action var
        protected Texture2D _main_texture;
        protected Texture2D _was_hit_texture;
        protected List<Texture2D> _skill_texture;
        protected bool isAttacking;
        protected bool isDead;
        protected bool hasTarget;
        protected bool wasAttacked;
        protected bool isReachTarget;
        protected List<Sprite> _targets;
        #endregion

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)_cur_position.X + (_frame_width / 2) - 15, (int)_cur_position.Y + ((_frame_height / 3)) + 80, _frame_width / 2, (_frame_height / 4) - 5);
            }
        }

        public Vector2 Position { get; set; }

        //public Vector2 Position;
        
        public Sprite(ContentManager content, string hero_name, string uniform_name)
        {
            _targets = new List<Sprite>();
            _skill_texture = new List<Texture2D>();

            _timePerFrame = (float)1 / _frame_per_sec;

            ChangeTexture(content.Load<Texture2D>("Character/" + hero_name + "/" + uniform_name + "/" + ("Sprite_" + uniform_name + "_Main")), 15, 4);

            _main_texture = _cur_texture;

            _was_hit_texture = content.Load<Texture2D>("Character/" + hero_name + "/" + uniform_name + "/" + (uniform_name + "_was_Hit"));

            for (int i = 1; i <= 4; i++)
            {
                _skill_texture.Add(content.Load<Texture2D>("Character/" + hero_name + "/" + uniform_name + "/Skill_" + i));
            }
        }

        public void ChangeTexture(Texture2D texture, int frame_per_sec, int time_cast)
        {
            _frame_per_sec = frame_per_sec;
            _time_cast = time_cast;
            _cur_column = 0;
            _cur_row = 0;
           /* _frame_width = _cur_texture.Width / frame_per_sec;
            _frame_height = _cur_texture.Height / time_cast;
            _timePerFrame = (float)1 / frame_per_sec;*/
            _cur_texture = texture;
            _TotalElapsed = 0;
            _Paused = false;
        }

        #region Set Function
        public void Set_IsFocus(bool logic)
        {
            isFocus = logic;
        }

        public void Set_IsAttacking(bool logic)
        {
            isAttacking = logic;
        }

        public void Set_IsDead(bool logic)
        {
            isDead = logic;
        }

        public void Set_WasAttacked(bool logic)
        {
            wasAttacked = logic;
        }

        public void Set_Targets(List<Sprite> targets)
        {
            _targets = targets;
        }
        #endregion

        #region Get Function

        public int Get_Sprite_Height()
        {
            return _frame_height;
        }

        public  int Get_Sprite_Width()
        {
            return _frame_width;
        }
        #endregion

        #region FrameControl Function
        public bool IsPaused
        {
            get { return _Paused; }
        }

        public void Reset()
        {
            _cur_frame = 0;
            _TotalElapsed = 0f;
        }

        public void Stop()
        {
            Pause();
            Reset();
        }

        public void Play()
        {
            _Paused = false;
        }

        public void Pause()
        {
            _Paused = true;
        }
        #endregion

        #region Animation Editor
        /*public abstract void Animating_Skill_1st();
        public abstract void Animating_Skill_2nd();
        public abstract void Animating_Skill_3rd();
        public abstract void Animating_Skill_4th();*/
        #endregion
        
        public void UpdateFrame(float elapsed)
        {
            //_Depth = -1 + ((Position.Y + _frame_height + 100) / 100);

            #region Mouse Cast
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            isPointed = false;

            if (mouseRectangle.Intersects(Rectangle))
            {
                isPointed = true;

                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
            #endregion

            #region ReachTarget

            wasAttacked = false;

            isReachTarget = false;

            if (hasTarget)
            {
                var targetRectangle = new Rectangle((int)_targets[0].Position.X, (int)_targets[0].Position.Y, _frame_width / 2, _frame_height / 2);
                
                if (targetRectangle.Intersects(Rectangle) && isAttacking)
                {
                    foreach (var target in _targets)
                    {
                        target.Set_WasAttacked(true);
                        isReachTarget = true;
                    }
                }
            }
            #endregion

            if (_Paused)
                return;

            _TotalElapsed += elapsed;

            _cur_frame = (_frame_per_sec * _cur_row) + _cur_column + 1;

            if (_TotalElapsed > _timePerFrame)
            {
                _cur_column++;
                if (_cur_column == _frame_per_sec) _cur_row++;

                if (!isAttacking) _cur_position = Position;

                foreach (var target in _targets)
                {
                    if (wasAttacked) ChangeTexture(_was_hit_texture, 1, 1);
                    else ChangeTexture(_main_texture, 15, 4);
                }

                //Set Back Main
                if (isAttacking && _cur_column == _frame_per_sec * _time_cast)
                {
                    ChangeTexture(_main_texture, 15, 4);
                    isAttacking = false;
                    foreach (var target in _targets)
                        target.ChangeTexture(_main_texture, 15, 4);
                    _targets = new List<Sprite>();
                    isFocus = false;
                }
                // Keep the Frame between 0 and the total frames, minus one.
                _cur_column = _cur_column % _frame_per_sec;
                _cur_row = _cur_row % _time_cast;
                _TotalElapsed -= _timePerFrame;
            }
        

        /*_TotalElapsed += elapsed;

        _cur_frame = (_frame_per_sec * _cur_row) + _cur_column;

        if (_TotalElapsed > _timePerFrame)
        {
            if (!isAttacking)
            {
                _cur_position = Position;
            }

            if (wasAttacked) _cur_texture = _was_hit_texture;

            //Set Back Main
            if (!isAttacking && (_cur_column * _cur_row) == (_frame_per_sec * _time_cast))
            {
                _cur_texture = _main_texture;
            }

            //Matrix Frame
            if (_cur_column == _frame_per_sec) _cur_row++;
            _cur_column++;

            _cur_column %= _frame_per_sec;
            _cur_row %= _time_cast;

            _TotalElapsed -= _timePerFrame;
        }*/
    }

        public void DrawFrame(SpriteBatch spriteBatch)
        {
            if (Position.X < MAAGame.SCREEN_WIDTH / 2) DrawFrame(spriteBatch, _cur_column, _cur_row);
            else DrawFrameFlip(spriteBatch);
        }

        public void DrawFrame(SpriteBatch spriteBatch,int cur_column, int cur_row, SpriteEffects effect = SpriteEffects.None)
        {
            var color = Color.Gray;

            if (isPointed || isFocus)
                color = Color.White;

            _frame_width = _cur_texture.Width / _frame_per_sec;
            _frame_height = _cur_texture.Height / _time_cast;
            Rectangle source_rect = new Rectangle(_frame_width * _cur_column, _frame_height * _cur_row, _frame_width, _frame_height);
            spriteBatch.Draw(_cur_texture, _cur_position, source_rect, color, _Rotation, _rotate_point, _Scale, effect, _Depth);
        }

        public void DrawFrameFlip(SpriteBatch spriteBatch)
        {
            DrawFrame(spriteBatch, _cur_column, _cur_row, SpriteEffects.FlipHorizontally);
        }

        #region Motion Fuction
        public void Transition(Vector2 initial, Vector2 final, int startframe, int number_of_frame)
        {
            if (isAttacking && _cur_frame > startframe && _cur_frame < startframe + number_of_frame)
            {
                _velocity.X = (final.X - initial.X) / number_of_frame;
                _velocity.Y = (final.Y - initial.Y) / number_of_frame;

                _cur_position.X += _velocity.X;
                _cur_position.Y += _velocity.Y;
            }
        }
        #endregion
    }
}
