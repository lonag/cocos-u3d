using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace CocosFramework
{
    public abstract class CCDirector : CCObject
    {
        static CCDirector s_sharedDirector = new CCDisplayLinkDirector();
        static bool s_bFirstRun = true;
        static int kDefaultFPS = 60;
        /// <summary>
        /// returns a shared instance of the director
        /// </summary>
        /// <returns></returns>
        public static CCDirector sharedDirector()
        {
            if (s_bFirstRun)
            {
                s_sharedDirector.init();
                s_bFirstRun = false;
            }

            return s_sharedDirector;
        }

        public virtual bool init()
        {
            //scene
            m_dOldAnimationInterval = m_dAnimationInterval = 1.0 / kDefaultFPS;

            // projection delegate if "Custom" projection is used
            //m_pProjectionDelegate = NULL;

            //FPS
            m_bDisplayFPS = false;
            m_uTotalFrames = m_uFrames = 0;
            m_pszFPS = "";

            m_bPaused = false;

            //paused?
            m_bPaused = false;
            m_fContentScaleFactor = 1;
            //purge?
            m_bPurgeDirecotorInNextLoop = false;
            return true;
        }

        public abstract void mainLoop();

        #region sceneManagement

        /// <summary>
        /// Draw the scene.
        /// This method is called every frame. Don't call it manually.
        /// </summary>
        protected void drawScene()
        {

            if (!m_bPaused)
            {
                //CCScheduler.sharedScheduler().tick((float)gameTime.ElapsedGameTime.TotalSeconds);
                //m_fDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            /* to avoid flickr, nextScene MUST be here: after tick and before draw.
             XXX: Which bug is this one. It seems that it can't be reproduced with v0.9 */
            if (m_pNextScene != null)
            {
                setNextScene();
            }

            //glPushMatrix();

            //applyOrientation();

            // By default enable VertexArray, ColorArray, TextureCoordArray and Texture2D
            // CC_ENABLE_DEFAULT_GL_STATES();

            // draw the scene
            if (m_pRunningScene != null)
            {
                m_pRunningScene.visit();
            }

            // draw the notifications node
            if (m_pNotificationNode != null)
            {
                m_pNotificationNode.visit();
            }

            if (m_bDisplayFPS)
            {
                showFPS();
            }

#if CC_ENABLE_PROFILERS
	showProfilers();
#endif

            m_uTotalFrames++;
        }

        protected void setNextScene()
        {
            // If it is not a transition, call onExit/cleanup
            /*if (! newIsTransition)*/
            //if (!(m_pNextScene is CCTransitionScene))
            //{
            //    if (m_pRunningScene != null)
            //    {
            //        m_pRunningScene.onExit();

            //        //CLEAR TOUCHES BEFORE LEAVING
            //        CCApplication.sharedApplication().ClearTouches();
            //    }

            //    // issue #709. the root node (scene) should receive the cleanup message too
            //    // otherwise it might be leaked.
            //    if (m_bSendCleanupToScene && m_pRunningScene != null)
            //    {
            //        m_pRunningScene.cleanup();
            //    }
            //}

            m_pRunningScene = m_pNextScene;
            // m_pNextScene.retain();
            m_pNextScene = null;

            if (m_pRunningScene != null)
            {
                m_pRunningScene.onEnter();
                //if (m_pRunningScene is CCTransitionScene)
                //{
                //    m_pRunningScene.onEnterTransitionDidFinish();
                //}
            }
        }

        /// <summary>
        /// Enters the Director's main loop with the given Scene. 
        /// Call it to run only your FIRST scene.
        /// Don't call it if there is already a running scene.
        /// </summary>
        /// <param name="pScene"></param>
        public void runWithScene(CCScene pScene)
        {
            Debug.Assert(pScene != null, "pScene cannot be null");
            Debug.Assert(m_pRunningScene == null, "m_pRunningScene cannot be null");

            pushScene(pScene);
            startAnimation();
        }

        /// <summary>
        /// Suspends the execution of the running scene, pushing it on the stack of suspended scenes.
        /// The new scene will be executed.
        /// Try to avoid big stacks of pushed scenes to reduce memory allocation. 
        /// ONLY call it if there is a running scene.
        /// </summary>
        /// <param name="pScene"></param>
        public void pushScene(CCScene pScene)
        {
            Debug.Assert(pScene != null, "pScene cannot be null");

            m_bSendCleanupToScene = false;

            m_pobScenesStack.Add(pScene);
            m_pNextScene = pScene;
        }

        /// <summary>
        /// Pops out a scene from the queue.
        /// This scene will replace the running one.
        /// The running scene will be deleted. If there are no more scenes in the stack the execution is terminated.
        /// ONLY call it if there is a running scene.
        /// </summary>
        public void popScene()
        {
            Debug.Assert(m_pRunningScene != null, "m_pRunningScene cannot be null");

            if (m_pobScenesStack.Count > 0)
            {
                m_pobScenesStack.RemoveAt(m_pobScenesStack.Count - 1);
            }
            int c = m_pobScenesStack.Count;

            if (c == 0)
            {
                //CCApplication.sharedApplication().Game.Exit();
                end();
            }
            else
            {
                m_bSendCleanupToScene = true;
                m_pNextScene = m_pobScenesStack[c - 1];
            }
        }

        public CCScene getLastScene()
        {
            if (m_pobScenesStack.Count > 1)
                return m_pobScenesStack[m_pobScenesStack.Count - 2];
            else
                return null;

        }

        /// <summary>
        /// Replaces the running scene with a new one. The running scene is terminated.
        /// ONLY call it if there is a running scene.
        /// </summary>
        /// <param name="pScene"></param>
        public void replaceScene(CCScene pScene)
        {
            Debug.Assert(pScene != null, "pScene cannot be null");

            int index = m_pobScenesStack.Count;

            m_bSendCleanupToScene = true;
            m_pobScenesStack[index - 1] = pScene;

            m_pNextScene = pScene;
        }

        ///<summary>
        /// Whether or not the replaced scene will receive the cleanup message.
        /// If the new scene is pushed, then the old scene won't receive the "cleanup" message.
        /// If the new scene replaces the old one, the it will receive the "cleanup" message.
        /// @since v0.99.0
        /// </summary>
        public bool isSendCleanupToScene()
        {
            return m_bSendCleanupToScene;
        }

        #endregion

        #region Protected

        protected void purgeDirector()
        {
            if (m_pRunningScene != null)
            {
                //m_pRunningScene->onExit();
                //m_pRunningScene->cleanup();
                //m_pRunningScene->release();
            }

            m_pRunningScene = null;
            m_pNextScene = null;

            // remove all objects, but don't release it.
            // runWithScene might be executed after 'end'.
            m_pobScenesStack.Clear();

            stopAnimation();
        }

        protected bool m_bPurgeDirecotorInNextLoop; // this flag will be set to true in end()

        protected void updateContentScaleFactor()
        {

        }

        protected void showFPS()
        {
            throw new NotImplementedException();
        }

        protected double m_dAnimationInterval;
        protected double m_dOldAnimationInterval;

        /* landscape mode ? */
        bool m_bLandscape;

        bool m_bDisplayFPS;
        float m_fAccumDt;
        float m_fFrameRate;

        /* is the running scene paused */
        bool m_bPaused;

        /* How many frames were called since the director started */
        uint m_uTotalFrames;
        uint m_uFrames;

        float m_fDeltaTime;

        /* The running scene */
        CCScene m_pRunningScene;

        /* will be the next 'runningScene' in the next frame
         nextScene is a weak reference. */
        CCScene m_pNextScene;

        /// <summary>
        /// If YES, then "old" scene will receive the cleanup message
        /// </summary>
        bool m_bSendCleanupToScene;

        /// <summary>
        /// scheduled scenes
        /// </summary>
        List<CCScene> m_pobScenesStack = new List<CCScene>();

        /* window size in pixels */
        //CCSize m_obWinSizeInPixels;

        /* content scale factor */
        float m_fContentScaleFactor = 1;

        /* store the fps string */
        string m_pszFPS;

        /* This object will be visited after the scene. Useful to hook a notification node */
        CCNode m_pNotificationNode;

        #endregion

        #region attribute

        /// <summary>
        ///  Get current running Scene. Director can only run one Scene at the time 
        /// </summary>
        public CCScene runningScene
        {
            get
            {
                return m_pRunningScene;
            }
        }

        /// <summary>
        /// the FPS value
        /// </summary>
        public virtual double animationInterval
        {
            get { return m_dAnimationInterval; }
            set { }
        }

        /// <summary>
        /// Whether or not to display the FPS on the bottom-left corner 
        /// </summary>
        /// <returns></returns>
        public bool DisplayFPS
        {
            get { return m_bDisplayFPS; }
            set { m_bDisplayFPS = value; }
        }

        /// <summary>
        /// Whether or not the Director is paused
        /// </summary>
        public bool isPaused
        {
            get { return m_bPaused; }
        }

        /// <summary>
        /// How many frames were called since the director started
        /// </summary>
        public uint getFrames()
        {
            return m_uFrames;
        }

        /** Ends the execution, releases the running scene.
         It doesn't remove the OpenGL view from its parent. You have to do it manually.
         */

        /* end is key word of lua, use other name to export to lua. */
        public void endToLua()
        {
            end();
        }

        public void end()
        {
            m_bPurgeDirecotorInNextLoop = true;
        }

        /// <summary>
        /// Pauses the running scene.
        /// The running scene will be _drawed_ but all scheduled timers will be paused
        /// While paused, the draw rate will be 4 FPS to reduce CPU consumption
        /// </summary>
        public void pause()
        {
            if (m_bPaused)
            {
                return;
            }

            m_dOldAnimationInterval = m_dAnimationInterval;

            // when paused, don't consume CPU
            animationInterval = 1 / 4.0;
            m_bPaused = true;
        }

        /// <summary>
        /// Resumes the paused scene
        /// The scheduled timers will be activated again.
        /// The "delta time" will be 0 (as if the game wasn't paused)
        /// </summary>
        public void resume()
        {
            if (!m_bPaused)
            {
                return;
            }

            animationInterval = m_dOldAnimationInterval;

            m_bPaused = false;
        }

        /// <summary>
        ///  Stops the animation. Nothing will be drawn. The main loop won't be triggered anymore.
        ///  If you don't want to pause your animation call [pause] instead.
        /// </summary>
        public abstract void stopAnimation();

        /// <summary>
        /// The main loop is triggered again.
        /// Call this function only if [stopAnimation] was called earlier
        /// warning Don't call this function to start the main loop. To run the main loop call runWithScene
        /// </summary>
        public abstract void startAnimation();

        // Memory Helper

        /// <summary>
        /// Removes cached all cocos2d cached data.
        /// It will purge the CCTextureCache, CCSpriteFrameCache, CCLabelBMFont cache
        /// @since v0.99.3
        /// </summary>
        public void purgeCachedData()
        {

        }

        #endregion

        // Profiler
        public void showProfilers()
        {

        }
        /// <summary>
        /// returns the size of the OpenGL view in points.
        /// It takes into account any possible rotation (device orientation) of the window
        /// </summary>
        /// <returns></returns>
        public CCSize getWinSize()
        {
            CCSize s = new CCSize(640,960);//new CCSize(m_obWinSizeInPoints.width, m_obWinSizeInPoints.height);

            //it's different from cocos2d-win32. 

            //if (m_eDeviceOrientation == ccDeviceOrientation.CCDeviceOrientationLandscapeLeft
            //    || m_eDeviceOrientation == ccDeviceOrientation.CCDeviceOrientationLandscapeRight)
            //{
            //    // swap x,y in landspace mode
            //    CCSize tmp = s;
            //    s.width = tmp.width;
            //    s.height = tmp.height;
            //}

            return s;
        }
        public float ContentScaleFactor
        {
            get { return m_fContentScaleFactor; }
            set
            {
                if (value != m_fContentScaleFactor)
                {
                    m_fContentScaleFactor = value;

                }
            }
        }
        /// <summary>
        /// converts a UIKit coordinate to an OpenGL coordinate
        /// Useful to convert (multi) touches coordinates to the current layout (portrait or landscape)
        /// </summary>
        public CCPoint convertToGL(CCPoint obPoint)
        {
            CCSize s = new CCSize(640, 960);//m_obWinSizeInPoints;

            //this is different from cocos2d-win32
            return new CCPoint(obPoint.x, s.height - obPoint.y);

            //CCSize s = m_obWinSizeInPoints;
            //float newY = s.height - obPoint.y;
            //float newX = s.width - obPoint.x;

            //CCPoint ret = new CCPoint(0, 0);
            //switch (m_eDeviceOrientation)
            //{
            //    case ccDeviceOrientation.CCDeviceOrientationPortrait:
            //        ret = new CCPoint(obPoint.x, newY);
            //        break;
            //    case ccDeviceOrientation.CCDeviceOrientationPortraitUpsideDown:
            //        ret = new CCPoint(newX, obPoint.y);
            //        break;
            //    case ccDeviceOrientation.CCDeviceOrientationLandscapeLeft:
            //        ret.x = obPoint.y;
            //        ret.y = obPoint.x;
            //        break;
            //    case ccDeviceOrientation.CCDeviceOrientationLandscapeRight:
            //        ret.x = newY;
            //        ret.y = newX;
            //        break;
            //}

            //return ret;
        }
        /// <summary>
        /// converts an OpenGL coordinate to a UIKit coordinate
        /// Useful to convert node points to window points for calls such as glScissor
        /// </summary>
        /// <param name="obPoint"></param>
        /// <returns></returns>
        public CCPoint convertToUI(CCPoint obPoint)
        {
            CCSize winSize = new CCSize(640, 960); //m_obWinSizeInPoints;

            //this is different from cocos2d-win32
            return new CCPoint(obPoint.x, winSize.height - obPoint.y);

            //float oppositeX = winSize.width - obPoint.x;
            //float oppositeY = winSize.height - obPoint.y;
            //CCPoint uiPoint = new CCPoint();

            //switch (m_eDeviceOrientation)
            //{
            //    case ccDeviceOrientation.CCDeviceOrientationPortrait:
            //        uiPoint = new CCPoint(obPoint.x, oppositeY);
            //        break;
            //    case ccDeviceOrientation.CCDeviceOrientationPortraitUpsideDown:
            //        uiPoint = new CCPoint(oppositeX, obPoint.y);
            //        break;
            //    case ccDeviceOrientation.CCDeviceOrientationLandscapeLeft:
            //        uiPoint = new CCPoint(obPoint.y, obPoint.x);
            //        break;
            //    case ccDeviceOrientation.CCDeviceOrientationLandscapeRight:
            //        // Can't use oppositeX/Y because x/y are flipped
            //        uiPoint = new CCPoint(winSize.width - obPoint.y, winSize.height - obPoint.x);
            //        break;
            //}

            //return uiPoint;
        }
    }
}
