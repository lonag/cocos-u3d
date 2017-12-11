/****************************************************************************
Copyright (c) 2010-2012 cocos2d-x.org
Copyright (c) 2008-2010 Ricardo Quesada
Copyright (c) 2009      Valentin Milea
Copyright (c) 2011      Zynga Inc.
Copyright (c) 2011-2012 openxlive.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace cocos2d
{

    public abstract class CCApplication : MonoBehavior
    {
        // CCTouch m_pTouch;
        internal GraphicsDeviceManager graphics;
        protected Rectangle m_rcViewPort;
        protected IEGLTouchDelegate m_pDelegate;
        // List<CCTouch> m_pSet;

        protected bool m_bCaptured;
        protected float m_fScreenScaleFactor;

        private readonly LinkedList<CCTouch> m_pTouches;
        private readonly Dictionary<int, LinkedListNode<CCTouch>> m_pTouchMap;

        public CCApplication(Game game)
            : base(game)
        {
            TouchPanel.EnabledGestures = GestureType.Tap;

            //m_pTouch = new CCTouch();
            //m_pSet = new List<CCTouch>();
            m_pTouches = new LinkedList<CCTouch>();
            m_pTouchMap = new Dictionary<int, LinkedListNode<CCTouch>>();

            m_fScreenScaleFactor = 1.0f;
        }

        #endregion

        // http://www.cocos2d-x.org/boards/17/topics/10777
        public void ClearTouches()
        {
            m_pTouches.Clear();
            m_pTouchMap.Clear();
            // m_pSet.Clear();
        }

        #region GameComponent

        public override void Initialize()
        {
            sm_pSharedApplication = this;

            // PVRFrameEnableControlWindow(false);

            initInstance();

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update its non-visible state. This method is invoked continuously
        /// in a stream of state updates.
        /// </summary>
        /// <param name="gameTime">This is the current game time.</param>
        public override void Update(GameTime gameTime)
        {
            // Process touch events 
            ProcessTouch();
            if (!CCDirector.sharedDirector().isPaused)
            {
                CCScheduler.sharedScheduler().tick((float)gameTime.ElapsedGameTime.TotalSeconds);
                //m_fDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Invoked by the render engine when the game components need to be refreshed.
        /// </summary>
        /// <param name="gameTime">This is the current game time.</param>
        public override void Draw(GameTime gameTime)
        {
            basicEffect.View = viewMatrix;
            basicEffect.World = worldMatrix;
            basicEffect.Projection = projectionMatrix;

            CCDirector.sharedDirector().mainLoop(gameTime); // TODO: Split into two parts - the update and the draw

            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            // spriteBatch = new SpriteBatch(GraphicsDevice);
            // basicEffect = new BasicEffect(GraphicsDevice);

            // GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            base.LoadContent();

            applicationDidFinishLaunching();
        }

        #endregion

        // #region Touch Methods

        // public IEGLTouchDelegate TouchDelegate
        // {
        //     set { m_pDelegate = value; }
        // }

        // private void ProcessTouch()
        // {
        //     if (m_pDelegate != null)
        //     {
        //         TouchCollection touchCollection = TouchPanel.GetState();

        //         List<CCTouch> newTouches = new List<CCTouch>();
        //         List<CCTouch> movedTouches = new List<CCTouch>();
        //         List<CCTouch> endedTouches = new List<CCTouch>();

        //         foreach (TouchLocation touch in touchCollection)
        //         {
        //             switch (touch.State)
        //             {
        //                 case TouchLocationState.Pressed:
        //                     if (m_rcViewPort.Contains((int)touch.Position.X, (int)touch.Position.Y))
        //                     {
        //                         m_pTouches.AddLast(new CCTouch(touch.Id, touch.Position.X - m_rcViewPort.Left / m_fScreenScaleFactor, touch.Position.Y - m_rcViewPort.Top / m_fScreenScaleFactor));
        //                         m_pTouchMap[touch.Id] = m_pTouches.Last;
        //                         newTouches.Add(m_pTouches.Last.Value);
        //                     }
        //                     break;

        //                 case TouchLocationState.Moved:
        //                     if (m_pTouchMap.ContainsKey(touch.Id))
        //                     {
        //                         movedTouches.Add(m_pTouchMap[touch.Id].Value);
        //                         m_pTouchMap[touch.Id].Value.SetTouchInfo(touch.Id,
        //                             touch.Position.X - m_rcViewPort.Left / m_fScreenScaleFactor,
        //                             touch.Position.Y - m_rcViewPort.Top / m_fScreenScaleFactor);
        //                     }
        //                     break;

        //                 case TouchLocationState.Released:
        //                     if (m_pTouchMap.ContainsKey(touch.Id))
        //                     {
        //                         endedTouches.Add(m_pTouchMap[touch.Id].Value);
        //                         m_pTouches.Remove(m_pTouchMap[touch.Id]);
        //                         m_pTouchMap.Remove(touch.Id);
        //                     }
        //                     break;

        //                 default:
        //                     throw new ArgumentOutOfRangeException();
        //             }

        //         }
        //         if (newTouches.Count > 0)
        //         {
        //             m_pDelegate.touchesBegan(newTouches, null);
        //         }

        //         if (movedTouches.Count > 0)
        //         {
        //             m_pDelegate.touchesMoved(movedTouches, null);
        //         }

        //         if (endedTouches.Count > 0)
        //         {
        //             m_pDelegate.touchesEnded(endedTouches, null);
        //         }
        //     }
        // }

        // private CCTouch getTouchBasedOnID(int nID)
        // {
        //     if (m_pTouchMap.ContainsKey(nID))
        //     {
        //         LinkedListNode<CCTouch> curTouch = m_pTouchMap[nID];
        //         //If ID's match...
        //         if (curTouch.Value.view() == nID)
        //         {
        //             //return the corresponding touch
        //             return curTouch.Value;
        //         }
        //     }
        //     //If we reached here, we found no touches
        //     //matching the specified id.
        //     return null;
        // }

        #endregion

        // sharedApplication pointer
        protected static CCApplication sm_pSharedApplication;

        /// <summary>
        /// Get current applicaiton instance.
        /// </summary>
        /// <returns>Current application instance pointer.</returns>
        public static CCApplication sharedApplication()
        {
            return sm_pSharedApplication;
        }

        #region virtual Method

        /// <summary>
        /// Implement for initialize OpenGL instance, set source path, etc...
        /// </summary>
        public virtual bool initInstance()
        {
            return true;
        }

        /// <summary>
        /// Implement CCDirector and CCScene init code here.
        /// </summary>
        /// <returns>
        ///     return true    Initialize success, app continue.
        ///     return false   Initialize failed, app terminate.
        /// </returns>
        public virtual bool applicationDidFinishLaunching()
        {
            return false;
        }

        /// <summary>
        ///  The function be called when the application enter background
        /// </summary>
        public virtual void applicationDidEnterBackground()
        {

        }

        /// <summary>
        /// The function be called when the application enter foreground
        /// </summary>
        public virtual void applicationWillEnterForeground()
        {

        }

        #endregion

        /// <summary>
        /// Callback by CCDirector for limit FPS
        /// </summary>
        /// <param name="interval">The time, which expressed in second in second, between current frame and next. </param>
        public double animationInterval
        {
            set
            {
                game.TargetElapsedTime = TimeSpan.FromSeconds(value);
            }
        }

    }
}
