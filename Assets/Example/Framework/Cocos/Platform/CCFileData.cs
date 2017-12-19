using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosFramework
{
    public class CCFileData
    {
        protected byte[] m_pBuffer;
        public byte[] Buffer
        {
            get { return m_pBuffer; }
        }

        protected UInt64 m_uSize;
        public UInt64 Size
        {
            get { return m_uSize; }
        }

        public CCFileData(string pszFileName, string pszMode)
        {
            //m_pBuffer = CCFileUtils.getFileData(pszFileName, pszMode, m_uSize);
        }

        public bool reset(string pszFileName, string pszMode)
        {
            m_pBuffer = null;
            m_uSize = 0;
            //m_pBuffer = CCFileUtils.getFileData(pszFileName, pszMode, m_uSize);
            return (m_pBuffer != null) ? true : false;
        }
    }
}
