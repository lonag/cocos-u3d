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
using System.Text;

namespace CocosFramework
{
    /***************************************************
    * implementation of DisplayLinkDirector
    **************************************************/

    // should we afford 4 types of director ??
    // I think DisplayLinkDirector is enough
    // so we now only support DisplayLinkDirector

    public class CCDisplayLinkDirector : CCDirector
    {
        bool m_bInvalid;

        public override void stopAnimation()
        {
            m_bInvalid = true;
        }

        public override void startAnimation()
        {
            m_bInvalid = false;
            //sharedDirector().animationInterval = m_dAnimationInterval;
        }

        public override void mainLoop()
        {
            if (m_bPurgeDirecotorInNextLoop)
            {
                purgeDirector();
                m_bPurgeDirecotorInNextLoop = false;
            }
            else if (!m_bInvalid)
            {
                drawScene(gameTime);
            }
        }

        public override double animationInterval
        {
            get
            {
                return base.animationInterval;
            }
            set
            {
                m_dAnimationInterval = value;
                if (!m_bInvalid)
                {
                    stopAnimation();
                    startAnimation();
                }
            }
        }
    }
}