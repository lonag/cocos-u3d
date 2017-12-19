using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosFramework
{
    public interface ICCSAXDelegator
    {
        void startElement(object ctx, string name, string[] atts);
        void endElement(object ctx, string name);
        void textHandler(object ctx, byte[] ch, int len);
    }
}
