namespace cocos2d
{
    public class CCTouch : CCObject
    {
        int m_nViewId;
        CCPoint m_point;
        CCPoint m_prevPoint;

        public CCTouch()
            : this(0, 0, 0)
        {

        }

        public CCTouch(int nViewId, float x, float y)
        {
            m_nViewId = nViewId;
            m_point = new CCPoint(x, y);
            m_prevPoint = new CCPoint(x, y);
        }

        public CCPoint locationInView(int nViewId)
        {
            //CC_UNUSED_PARAM(nViewId); 
            return m_point;
        }

        public CCPoint previousLocationInView(int nViewId)
        {
            //CC_UNUSED_PARAM(nViewId); 
            return m_prevPoint;
        }

        public int view()
        {
            return m_nViewId;
        }

        public void SetTouchInfo(int nViewId, float x, float y)
        {
            m_nViewId = nViewId;
            m_prevPoint = new CCPoint(m_point.x, m_point.y);
            m_point.x = x;
            m_point.y = y;
        }
    }

    public class CCEvent : CCObject
    {

    }
}
