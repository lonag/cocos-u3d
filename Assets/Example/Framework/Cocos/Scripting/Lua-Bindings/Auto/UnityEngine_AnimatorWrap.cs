using System;
using UnityEngine;
using CocosFramework;

public class UnityEngine_AnimatorWrap {
    public static void Register(SharpLua L)
    {
        //L.BeginClass(typeof(UnityEngine.Animator), typeof(UnityEngine.Behaviour));
        //L.RegFunction("GetFloat", GetFloat);
        //L.RegFunction("SetFloat", SetFloat);
        //L.RegFunction("GetBool", GetBool);
        //L.EndClass();
    }

    static int GetFloat(int n, SharpLua.var[] argv)
    {
        return 0;
    }
    
    static void SetFloat(int n, SharpLua.var[] argv)
    {

    }
    static bool GetBool(int n, SharpLua.var[] argv){
        return false;
    }

}

// SharpLua.LuaObject gc = l.GetFunction("collectgarbage");
// l.CallFunction(gc, "collect");
// l.CollectGarbage();

// SharpLua.LuaObject regFunction = l.GetFunction("init");
// SharpLua.SharpFunction func = FuncCallByLua;
// l.CallFunction(regFunction, "GetComponet",GetComponet);

// static GameOjbect GetComponet(int n, SharpLua.var[] argv) {
//     Console.WriteLine("I'm in lua :");
//     for (int i=1; i<n; i++) {
//         Console.WriteLine("Args {0} type {1}", i, argv[i].type);
//     }

//     argv[0].type = SharpLua.var_type.INTEGER;
//     argv[0].d = 123;
//     return null;
// }

