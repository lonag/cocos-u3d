using System;

namespace CocosFramework
{
    //public class UnityEngine_Wrap {
    //    static string FuncCallByLua(int n, SharpLua.var[] argv) {
    //        Console.WriteLine("I'm in lua :");
    //        for (int i=1; i<n; i++) {
    //            Console.WriteLine("Args {0} type {1}", i, argv[i].type);
    //        }

    //        argv[0].type = SharpLua.var_type.INTEGER;
    //        argv[0].d = 123;
    //        return null;
    //    }

    //    public static void Register(SharpLua l) {
    //        SharpLua.LuaObject gc = l.GetFunction("collectgarbage");
    //        l.CallFunction(gc, "collect");
    //        l.CollectGarbage();

    //        SharpLua.LuaObject regFunction = l.GetFunction("UnityEngineRegFunction");
    //        SharpLua.SharpFunction func = FuncCallByLua;
    //        l.CallFunction(regFunction, "GetComponet",FuncCallByLua);
    //    }
    //}
}