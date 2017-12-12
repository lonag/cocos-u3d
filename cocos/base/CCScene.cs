using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cocos2d
{
    public enum ccSceneFlag
    {
        ccNormalScene = 1 << 0,
        ccTransitionScene = 1 << 1
    }

    /// <summary>
    /// brief CCScene is a subclass of CCNode that is used only as an abstract concept.
    /// CCScene an CCNode are almost identical with the difference that CCScene has it's
    /// anchor point (by default) at the center of the screen.
    /// For the moment CCScene has no other logic than that, but in future releases it might have
    /// additional logic.
    ///  It is a good practice to use and CCScene as the parent of all your nodes.
    /// </summary>
    public class CCScene : CCNode
    {
        protected ccSceneFlag m_eSceneType;
        public ccSceneFlag SceneType
        {
            get { return m_eSceneType; }
        }

        public CCScene()
        {
            isRelativeAnchorPoint = false;
            anchorPoint = CCPointExtension.ccp(0.5f, 0.5f);
            m_eSceneType = ccSceneFlag.ccNormalScene;
        }

        public bool init()
        {
            bool bRet = false;
            do
            {
                CCDirector director = CCDirector.sharedDirector();
                if (director == null)
                {
                    break;
                }

                contentSize = director.getWinSize();
                // success
                bRet = true;
            } while (false);
            return bRet;
        }

        public static new CCScene node()
        {
            CCScene pRet = new CCScene();
            if (pRet.init())
            {
                return pRet;
            }
            else
            {
                return null;
            }
        }
    }

    public class CCNormalScene : CCScene
    {
        public CCNormalScene()
        {
        }

        ~CCNormalScene()
        {
        }

        public static new CCNormalScene node()
        {
            CCNormalScene pRet = new CCNormalScene();
            if (pRet.init())
            {
                return pRet;
            }
            else
            {
                return null;
            }
        }
    }
}
