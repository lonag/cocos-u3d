using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cocos2d
{
    /// <summary>
    ///  Object than contains the delegate and priority of the event handler.
    /// </summary>
    public class CCTouchHandler
    {
        /// <summary>
        /// delegate
        /// </summary>
        public ICCTouchDelegate Delegate
        {
            get { return m_pDelegate; }
            set { m_pDelegate = value; }
        }

        /// <summary>
        /// priority
        /// </summary>
        public int Priority
        {
            get { return m_nPriority; }
            set { m_nPriority = value; }
        }

        /// <summary>
        /// enabled selectors 
        /// </summary>
        public int getEnabledSelectors 
        {
            get { return m_nEnabledSelectors; }
            set { m_nEnabledSelectors = value; }
        }

        /// <summary>
        /// initializes a TouchHandler with a delegate and a priority 
        /// </summary>
        public virtual bool initWithDelegate(ICCTouchDelegate pDelegate, int nPriority)
        {
            m_pDelegate = pDelegate;
            m_nPriority = nPriority;
            m_nEnabledSelectors = 0;

            return true;
        }

        /// <summary>
        /// allocates a TouchHandler with a delegate and a priority 
        /// </summary>
        public static CCTouchHandler handlerWithDelegate(ICCTouchDelegate pDelegate, int nPriority)
        {
            CCTouchHandler pHandler = new CCTouchHandler();

            if (pHandler.initWithDelegate(pDelegate, nPriority))
            {
                pHandler = null;
            }
            else
            {
                pHandler = null;
            }

            return pHandler;
        }

        protected ICCTouchDelegate m_pDelegate;
        protected int m_nPriority;
        protected int m_nEnabledSelectors;
    }
}
