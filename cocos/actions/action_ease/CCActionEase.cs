/****************************************************************************
Copyright (c) 2010-2012 cocos2d-x.org
Copyright (c) 2008-2009 Jason Booth
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
using System.Text;

namespace cocos2d
{
    public class CCActionEase : CCActionInterval
    {
        /// <summary>
        /// initializes the action
        /// </summary>
        /// <param name="pAction"></param>
        /// <returns></returns>
        public bool initWithAction(CCActionInterval pAction)
        {
		    if (base.initWithDuration(pAction.duration))
		    {
			    m_pOther = pAction;
			    return true;
		    }
		    return false;
		
        }

        public override CCObject copyWithZone(CCZone pZone)
        {
            CCZone pNewZone = null;
		    CCActionEase pCopy = null;

		    if(pZone != null && pZone.m_pCopyObject != null) 
		    {
			    //in case of being called at sub class
			    pCopy = pZone.m_pCopyObject as CCActionEase;
		    }
		    else
		    {
			    pCopy = new CCActionEase();
			    pZone = pNewZone = new CCZone(pCopy);
		    }

		    base.copyWithZone(pZone);

		    pCopy.initWithAction((CCActionInterval)(m_pOther.copy()));
		
		    return pCopy;
        }

        public override void startWithTarget(CCNode pTarget)
        {
            base.startWithTarget(pTarget);
		    m_pOther.startWithTarget(m_pTarget);
        }

        public override void stop()
        {
            m_pOther.stop();
		    base.stop();
        }

        public override void update(float time)
        {
            m_pOther.update(time);
        }

        public override CCFiniteTimeAction reverse()
        {
            return CCActionEase.actionWithAction((CCActionInterval)m_pOther.reverse());
        }

        /// <summary>
        /// creates the action
        /// </summary>
        /// <param name="pAction"></param>
        /// <returns></returns>
        public static CCActionEase actionWithAction(CCActionInterval pAction)
        {
            CCActionEase pRet = new CCActionEase();

            if (pRet!=null)
            {
                if (pRet.initWithAction(pAction))
                {
                    //pRet->autorelease();
                }
                else
                {
                    //CC_SAFE_RELEASE_NULL(pRet);
                }
            }

            return pRet;
        }

        protected CCActionInterval m_pOther;

    }
}
