using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CocosFramework
{
    public enum Orientation
    {
        /// Device oriented vertically, home button on the bottom
        kOrientationPortrait = 0,
        /// Device oriented vertically, home button on the top
        kOrientationPortraitUpsideDown = 1,
        /// Device oriented horizontally, home button on the right
        kOrientationLandscapeLeft = 2,
        /// Device oriented horizontally, home button on the left
        kOrientationLandscapeRight = 3,
    } ;

    public abstract class CCApplication : MonoBehaviour
    {

        #region Fields and Construct Method

        protected bool m_bCaptured;
        protected float m_fScreenScaleFactor;

        private readonly LinkedList<CCTouch> m_pTouches;
        private readonly Dictionary<int, LinkedListNode<CCTouch>> m_pTouchMap;

        public CCApplication()
        {
            
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

        public void Initialize()
        {
            sm_pSharedApplication = this;

            PVRFrameEnableControlWindow(false);

            initInstance();

            //base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update its non-visible state. This method is invoked continuously
        /// in a stream of state updates.
        /// </summary>
        /// <param name="gameTime">This is the current game time.</param>
        public void Update()
        {
            // Process touch events 
            //ProcessTouch();
            if (!CCDirector.sharedDirector().isPaused)
            {
                //CCScheduler.sharedScheduler().tick((float)gameTime.ElapsedGameTime.TotalSeconds);
                //m_fDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            //base.Update(gameTime);
        }

        /// <summary>
        /// Invoked by the render engine when the game components need to be refreshed.
        /// </summary>
        /// <param name="gameTime">This is the current game time.</param>
        public void Draw()
        {
            //basicEffect.View = viewMatrix;
            //basicEffect.World = worldMatrix;
            //basicEffect.Projection = projectionMatrix;

            CCDirector.sharedDirector().mainLoop(); // TODO: Split into two parts - the update and the draw

            //base.Draw(gameTime);
        }

        protected void LoadContent()
        {
            //spriteBatch = new SpriteBatch(GraphicsDevice);
            //basicEffect = new BasicEffect(GraphicsDevice);

            //GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            //base.LoadContent();

            applicationDidFinishLaunching();
        }

        #endregion

        private CCTouch getTouchBasedOnID(int nID)
        {
            if (m_pTouchMap.ContainsKey(nID))
            {
                LinkedListNode<CCTouch> curTouch = m_pTouchMap[nID];
                //If ID's match...
                if (curTouch.Value.view() == nID)
                {
                    //return the corresponding touch
                    return curTouch.Value;
                }
            }
            //If we reached here, we found no touches
            //matching the specified id.
            return null;
        }


        /// <summary>
        /// This function change the PVRFrame show/hide setting in register.
        /// </summary>
        /// <param name="bEnable"> If true show the PVRFrame window, otherwise hide.</param>
        static void PVRFrameEnableControlWindow(bool bEnable)
        { }

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
                //game.TargetElapsedTime = TimeSpan.FromSeconds(value);
            }
        }

        /// <summary>
        /// Callback by CCDirector for change devic e orientation. 
        /// </summary>
        /// <param name="orientation">The defination of orientation which CCDirector want change to.</param>
        /// <returns>The actual orientation of the application.</returns>
        //public Orientation setOrientation(Orientation orientation)
        //{

        //}

        /// <summary>
        /// Get status bar rectangle in EGLView window.
        /// </summary>
        /// <param name="rect"></param>
        public void statusBarFrame(out CCRect rect)
        {
            // Windows doesn't have status bar.
            rect = new CCRect(0, 0, 0, 0);
        }
        
        bool m_bOrientationReverted;
        CCPoint m_tSizeInPoints;

        public bool canSetContentScaleFactor
        {
            get { return true; }
        }

        private CCSize _size = new CCSize(640, 960);
        public CCSize getSize()
        {
            return new CCSize(_size.width, _size.height);
        }

        public void setContentScaleFactor(float contentScaleFactor)
        {
            m_fScreenScaleFactor = contentScaleFactor;
            if (m_bOrientationReverted)
            {
                //resize((int)(m_tSizeInPoints.cy * contentScaleFactor), (int)(m_tSizeInPoints.cx * contentScaleFactor));
            }
            else
            {
                //resize((int)(m_tSizeInPoints.cx * contentScaleFactor), (int)(m_tSizeInPoints.cy * contentScaleFactor));
            }
            centerWindow();
        }

        void resize(int width, int height)
        { }

        void centerWindow()
        { }

    }
}