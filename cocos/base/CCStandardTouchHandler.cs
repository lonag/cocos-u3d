using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cocos2d
{
    /// <summary>
    /// It forwardes each event to the delegate.
    /// </summary>
    public class CCStandardTouchHandler : CCTouchHandler
    {
        /// <summary>
        ///  initializes a TouchHandler with a delegate and a priority
        /// </summary>
        public virtual bool initWithDelegate(ICCStandardTouchDelegate pDelegate, int nPriority)
        {
            return base.initWithDelegate(pDelegate, nPriority);
        }

        /// <summary>
        /// allocates a TouchHandler with a delegate and a priority
        /// </summary>
        public static CCStandardTouchHandler handlerWithDelegate(ICCStandardTouchDelegate pDelegate, int nPriority)
        {
            CCStandardTouchHandler pHandler = new CCStandardTouchHandler();
            pHandler.initWithDelegate(pDelegate, nPriority);
            return pHandler;
        }
    }
}
