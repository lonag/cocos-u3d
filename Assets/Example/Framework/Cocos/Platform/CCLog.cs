using System;
using System.Diagnostics;
using UnityEngine;

namespace CocosFramework
{
    public class CCLog
    { 
        public static void Log(string message)
        {
            //Debug.WriteLine(message);
        }

        public static void Log(string format, params object[] args)
        {
            //Debug.log(format, args);
        }
    }
}
