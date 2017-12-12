using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cocos2d
{
    public class CCTouchDelegate : ICCTouchDelegate
    {
        protected Dictionary<int, string> m_pEventTypeFuncMap;

        /// <summary>
        /// ! call the release() in child(layer or menu)
        /// </summary>
        public virtual void destroy()
        {
        }

        /// <summary>
        /// ! call the retain() in child (layer or menu)
        /// </summary>
        public virtual void keep()
        { }

        /// <summary>
        /// functions for script call back
        /// </summary>
        public void registerScriptTouchHandler(int eventType, string pszScriptFunctionName)
        {
            if (m_pEventTypeFuncMap == null)
            {
                m_pEventTypeFuncMap = new Dictionary<int, string>();
            }

            (m_pEventTypeFuncMap)[eventType] = pszScriptFunctionName;
        }

        public bool isScriptHandlerExist(int eventType)
        {
            if (m_pEventTypeFuncMap != null)
            {
                return (m_pEventTypeFuncMap)[eventType].Count() != 0;
            }

            return false;
        }

        public void excuteScriptTouchHandler(int eventType, CCTouch pTouch)
        {
            if (m_pEventTypeFuncMap != null && CCScriptEngineManager.sharedScriptEngineManager().ScriptEngine != null)
            {
                CCScriptEngineManager.sharedScriptEngineManager().ScriptEngine.executeTouchEvent((m_pEventTypeFuncMap)[eventType].ToString(),
                                                                                                         pTouch);
            }

        }

        public void excuteScriptTouchesHandler(int eventType, List<CCTouch> pTouches)
        {
            if (m_pEventTypeFuncMap != null && CCScriptEngineManager.sharedScriptEngineManager().ScriptEngine != null)
            {
                CCScriptEngineManager.sharedScriptEngineManager().ScriptEngine.executeTouchesEvent((m_pEventTypeFuncMap)[eventType].ToString(),
                                                                                                            pTouches);
            }
        }
    }
}
