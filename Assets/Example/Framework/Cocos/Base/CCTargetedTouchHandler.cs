using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosFramework
{
    public class CCTargetedTouchHandler : CCTouchHandler
    {
        /// <summary>
        /// whether or not the touches are swallowed
        /// </summary>
        public bool IsSwallowsTouches 
        {
            get { return m_bSwallowsTouches; }
            set { m_bSwallowsTouches = value; }
        }

        /// <summary>
        /// MutableSet that contains the claimed touches 
        /// </summary>
        public List<CCTouch> ClaimedTouches
        {
            get
            {
                return m_pClaimedTouches;
            }
        }

        /// <summary>
        ///  initializes a TargetedTouchHandler with a delegate, a priority and whether or not it swallows touches or not
        /// </summary>
        public bool initWithDelegate(ICCTargetedTouchDelegate pDelegate, int nPriority, bool bSwallow)
        {
            if (base.initWithDelegate(pDelegate, nPriority))
            {
                m_pClaimedTouches = new List<CCTouch>();
                m_bSwallowsTouches = bSwallow;

                return true;
            }

            return false;
        }

        /// <summary>
        /// allocates a TargetedTouchHandler with a delegate, a priority and whether or not it swallows touches or not 
        /// </summary>
        public static CCTargetedTouchHandler handlerWithDelegate(ICCTargetedTouchDelegate pDelegate, int nPriority, bool bSwallow)
        {
            CCTargetedTouchHandler pHandler = new CCTargetedTouchHandler();
            pHandler.initWithDelegate(pDelegate, nPriority, bSwallow);
            return pHandler;
        }

        protected bool m_bSwallowsTouches;
        protected List<CCTouch> m_pClaimedTouches;
    }
}
