using System.Collections.Generic;
using System.Diagnostics;

namespace CocosFramework
{
    /** Singleton that manages the Animations.
    It saves in a cache the animations. You should use this class if you want to save your animations in a cache.

    Before v0.99.5, the recommend way was to save them on the CCSprite. Since v0.99.5, you should use this class instead.

    @since v0.99.5
    */
    public class CCAnimationCache : CCObject
    {
        public CCAnimationCache()
        {
        }

        ~CCAnimationCache()
        { 
        }
		
		/** Retruns ths shared instance of the Animation cache */
        public static CCAnimationCache sharedAnimationCache()
        {
            if (null == s_pSharedAnimationCache)
            {
                s_pSharedAnimationCache = new CCAnimationCache();
                s_pSharedAnimationCache.init();
            }

            return s_pSharedAnimationCache;
        }

		/** Purges the cache. It releases all the CCAnimation objects and the shared instance.
		*/
        public static void purgeSharedAnimationCache()
        {
            //CC_SAFE_RELEASE_NULL(s_pSharedAnimationCache);
            s_pSharedAnimationCache = null;
        }

		/** Adds a CCAnimation with a name.
		*/
        public void addAnimation(CCAnimation animation, string name)
        { 
            m_pAnimations.Add(name, animation);
        }

		/** Deletes a CCAnimation from the cache.
		*/
        public void removeAnimationByName(string name)
        {
            if (null == name)
            {
                return;
            }

            m_pAnimations.Remove(name);
        }

		/** Returns a CCAnimation that was previously added.
		If the name is not found it will return nil.
		You should retain the returned copy if you are going to use it.
		*/
        public CCAnimation animationByName(string name)
        { 
            CCAnimation animation = new CCAnimation();
            if (m_pAnimations.TryGetValue(name, out animation))
            {
                return animation;
            }
            else
            {
                return null;
            }
        }

        public bool init()
        { 
            m_pAnimations = new Dictionary<string, CCAnimation>();
		    return true;
        }

		Dictionary<string, CCAnimation> m_pAnimations;
		static CCAnimationCache s_pSharedAnimationCache;


    }
}