using System;
using System.Runtime.InteropServices;

namespace CocosFramework {
	class MonoPInvokeCallbackAttribute : System.Attribute {
		public MonoPInvokeCallbackAttribute( Type t ) {}
	}

	class SharpLua {
		public enum var_type {
			NIL = 0,
			INTEGER = 1,
			INT64 = 2,
			REAL = 3,
			BOOLEAN = 4,
			STRING = 5,
			POINTER = 6,
			LUAOBJ = 7,
			SHARPOBJ = 8,
		};
		public struct var {
			public var_type type;
			public int d;
			public long d64;
			public double f;
			public IntPtr ptr;
		}
		public struct LuaObject {
			public int id;
		}
		const string DLL = "sharplua.dll";
		const int max_args = 256;	// Also defined in sharplua.c MAXRET

		IntPtr L;
		var[] args = new var[max_args];
		string[] strs = new string[max_args];
		static SharpObject objects = new SharpObject();	// All the SharpLua class share one one objects map

		public delegate string SharpFunction(int n, var[] argv);

		[DllImport (DLL, CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr c_newvm([MarshalAs(UnmanagedType.LPStr)] string name,  Callback cb, out IntPtr err);
		[DllImport (DLL, CallingConvention=CallingConvention.Cdecl)]
		static extern void c_closevm(IntPtr L);
		[DllImport (DLL, CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr c_getglobal(IntPtr L, [MarshalAs(UnmanagedType.LPStr)] string key, out int id);
		[DllImport (DLL, CallingConvention=CallingConvention.Cdecl)]
		static extern int c_callfunction(IntPtr L, int argc, [In, Out, MarshalAs(UnmanagedType.LPArray, SizeConst=max_args)] var[] argv,
			int strc, [In, MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.LPStr, SizeParamIndex=3)] string[] strs);
		[DllImport (DLL, CallingConvention=CallingConvention.Cdecl)]
		static extern int c_collectgarbage(IntPtr L, int n, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] int[] result);
		[DllImport (DLL, CallingConvention=CallingConvention.Cdecl)]
		static extern int c_pushstring(IntPtr ud, [MarshalAs(UnmanagedType.LPStr)] string str);

		delegate int Callback(int argc, [In, Out, MarshalAs(UnmanagedType.LPArray, SizeConst=max_args)] var[] argv, IntPtr sud);
		[MonoPInvokeCallback (typeof (Callback))]
		static int CallSharp(int argc, [In, Out, MarshalAs(UnmanagedType.LPArray, SizeConst=max_args)] var[] argv, IntPtr sud) {
			try {
				SharpFunction f = (SharpFunction)objects.Get(argv[0].d);
				string ret = f(argc, argv);
				if (ret != null) {
					// push string into L for passing C sharp string to lua.
					if (c_pushstring(sud, ret) == 0) {
						throw new ArgumentException("Push string failed");
					}
				}
				return (int)argv[0].type;
			} catch (Exception ex) {
				c_pushstring(sud, ex.ToString());
				return -1;
			}
		}

		public SharpLua(string name) {
			IntPtr err;
			IntPtr tmp = c_newvm(name, CallSharp, out err);
			if (err != IntPtr.Zero) {
				string msg = Marshal.PtrToStringAnsi(err);
				c_closevm(tmp);
				throw new ArgumentException(msg);
			} else {
				L = tmp;
			}
		}

		~SharpLua() {
			c_closevm(L);
		}

		public void Close() {
			c_closevm(L);
			L = IntPtr.Zero;
		}

		public LuaObject GetFunction(string name) {
			int id;
			IntPtr err = c_getglobal(L, name, out id);
			if (id != 0) {
				// succ 
				return new LuaObject { id = id };
			} else {
				if (err == IntPtr.Zero) {
					return new LuaObject();	// nil
				} else {
					throw new ArgumentException(Marshal.PtrToStringAnsi(err));
				}
			}
		}

		int pushvalue(ref var v, object arg) {
			if (arg == null) {
				v.type = (int)var_type.NIL;
			} else {
				Type t = arg.GetType();
				if (t == typeof(int)) {
					v.type = var_type.INTEGER;
					v.d = (int)arg;
				} else if ( t == typeof(long)) {
					v.type = var_type.INTEGER;
					v.d64 = (long)arg;
				} else if (t == typeof(float)) {
					v.type = var_type.REAL;
					v.f = (float)arg;
				} else if (t == typeof(double)) {
					v.type = var_type.REAL;
					v.f = (double)arg;
				} else if (t == typeof(bool)) {
					v.type = var_type.BOOLEAN;
					v.d = (bool)arg ? 1 : 0;
				} else if (t == typeof(string)) {
					v.type = var_type.STRING;
					return 2;	// string
				} else if (t == typeof(LuaObject)) {
					v.type = var_type.LUAOBJ;
					v.d = ((LuaObject)arg).id;
				} else if (t.IsClass) {
					v.type = var_type.SHARPOBJ;
					v.d = objects.Query(arg);
				} else {
					return 0;	// error
				}
			}
			return 1;
		}

		public void CollectGarbage() {
			const int cap = 256;
			int[] result = new int[cap];	// 256 per cycle
			int n;
			do {
				n = c_collectgarbage(L, cap, result);
				for (int i=0;i<n;i++) {
					objects.Remove(result[i]);
				}
			} while(n<cap && n > 0);
		}

		public object[] CallFunction(LuaObject func, params object[] arg) {
			int n = arg.Length;
			if (n+1 > max_args) {
				throw new ArgumentException("Too many args");
			}
			args[0].type = var_type.LUAOBJ;
			args[0].d = func.id;

			int sn = 0;
			for (int i = 0; i < n; i++) {
				int r = pushvalue(ref args[i+1], arg[i]);
				switch(r) {
				case 0:
					throw new ArgumentException(String.Format("Unsupport type : {1} at {0}", i, arg[i].GetType()));
				case 1:
					break;
				case 2:
					// string
					args[i+1].d = sn;
					strs[sn] = (string)arg[i];
					++sn;
					break;
				}
			}
			int retn = c_callfunction(L, n+1, args, sn, strs);
			if (retn < 0) {
				throw new ArgumentException(Marshal.PtrToStringAnsi(args[0].ptr));
			}
			if (retn == 0) {
				return null;
			}
			object[] ret = new object[retn];
			for (int i = 0; i < retn; i++) {
				switch(args[i].type) {
				case var_type.NIL :
					ret[i] = null;
					break;
				case var_type.INTEGER :
					ret[i] = args[i].d;
					break;
				case var_type.INT64 :
					ret[i] = args[i].d64;
					break;
				case var_type.REAL :
					ret[i] = args[i].f;
					break;
				case var_type.BOOLEAN :
					ret[i] = (args[i].d != 0) ? true : false;
					break;
				case var_type.STRING :
					// todo: encoding
					ret[i] = Marshal.PtrToStringAnsi(args[i].ptr);
					break;
				case var_type.POINTER :
					ret[i] = args[i].ptr;
					break;
				case var_type.LUAOBJ :
					ret[i] = new LuaObject { id = args[i].d };
					break;
				case var_type.SHARPOBJ :
					ret[i] = objects.Get(args[i].d);
					if (ret[i] == null) {
						throw new ArgumentException("Invalid sharp object");
					}
					break;
				}
			}
			
			return ret;
		}
		public void BeginPreLoad()
        {
            LuaGetGlobal("package");
            LuaGetField(-1, "preload");
            moduleSet = new HashSet<string>();
        }

        public void EndPreLoad()
        {
            LuaPop(2);
            moduleSet = null;
        }

        public void AddPreLoad(string name, SharpFunction func, Type type)
        {            
            if (!preLoadMap.ContainsKey(type))
            {
                LuaDLL.tolua_pushcfunction(L, func);
                LuaSetField(-2, name);
                preLoadMap[type] = func;
                string module = type.Namespace;

                if (!string.IsNullOrEmpty(module) && !moduleSet.Contains(module))
                {
                    LuaDLL.tolua_addpreload(L, module);
                    moduleSet.Add(module);
                }
            }            
        }

        public int BeginPreModule(string name)
        {
            // int top = LuaGetTop();

            // if (string.IsNullOrEmpty(name))
            // {
            //     LuaDLL.lua_pushvalue(L, LuaIndexes.LUA_GLOBALSINDEX);
            //     ++beginCount;
            //     return top;
            // }
            // else if (LuaDLL.tolua_beginpremodule(L, name))
            // {
            //     ++beginCount;
            //     return top;
            // }
            
            // throw new LuaException(string.Format("create table {0} fail", name));            
        }

        public void EndPreModule(int reference)
        {
            // --beginCount;            
            // LuaDLL.tolua_endpremodule(L, reference);
        }

        public void EndPreModule(IntPtr L, int reference)
        {
            // --beginCount;
            // LuaDLL.tolua_endpremodule(L, reference);
        }

        public void BindPreModule(Type t, SharpFunction func)
        {
            // preLoadMap[t] = func;
        }

        // public SharpFunction GetPreModule(Type t)
        // {
        //     SharpFunction func = null;
        //     preLoadMap.TryGetValue(t, out func);
        //     return func;
        // }

        public bool BeginModule(string name)
        {
// #if UNITY_EDITOR
//             if (name != null)
//             {                
//                 LuaTypes type = LuaType(-1);

//                 if (type != LuaTypes.LUA_TTABLE)
//                 {                    
//                     throw new LuaException("open global module first");
//                 }
//             }
// #endif
//             if (LuaDLL.tolua_beginmodule(L, name))
//             {
//                 ++beginCount;
//                 return true;
//             }

//             LuaSetTop(0);
//             throw new LuaException(string.Format("create table {0} fail", name));            
        }

        public void EndModule()
        {
            // --beginCount;            
            // LuaDLL.tolua_endmodule(L);
        }

        void BindTypeRef(int reference, Type t)
        {
            // metaMap.Add(t, reference);
            // typeMap.Add(reference, t);

            // if (t.IsGenericTypeDefinition)
            // {
            //     genericSet.Add(t);
            // }
        }

        // public Type GetClassType(int reference)
        // {
        //     Type t = null;
        //     typeMap.TryGetValue(reference, out t);
        //     return t;
        // }

        // string GetToLuaTypeName(Type t)
        // {
        //     if (t.IsGenericType)
        //     {
        //         string str = t.Name;
        //         int pos = str.IndexOf('`');

        //         if (pos > 0)
        //         {
        //             str = str.Substring(0, pos);
        //         }

        //         return str;
        //     }

        //     return t.Name;
        // }

        public int BeginClass(Type t, Type baseType, string name = null)
        {
            // if (beginCount == 0)
            // {
            //     throw new LuaException("must call BeginModule first");
            // }

            // int baseMetaRef = 0;
            // int reference = 0;            

            // if (name == null)
            // {
            //     name = GetToLuaTypeName(t);
            // }

            // if (baseType != null && !metaMap.TryGetValue(baseType, out baseMetaRef))
            // {
            //     LuaCreateTable();
            //     baseMetaRef = LuaRef(LuaIndexes.LUA_REGISTRYINDEX);                
            //     BindTypeRef(baseMetaRef, baseType);
            // }

            // if (metaMap.TryGetValue(t, out reference))
            // {
            //     LuaDLL.tolua_beginclass(L, name, baseMetaRef, reference);
            //     RegFunction("__gc", Collect);
            // }
            // else
            // {
            //     reference = LuaDLL.tolua_beginclass(L, name, baseMetaRef);
            //     RegFunction("__gc", Collect);                
            //     BindTypeRef(reference, t);
            // }

            // return reference;
            return 0;
        }

        public void EndClass()
        {
            // LuaDLL.tolua_endclass(L);
        }

        public int BeginEnum(Type t)
        {
            // if (beginCount == 0)
            // {
            //     throw new LuaException("must call BeginModule first");
            // }

            // int reference = LuaDLL.tolua_beginenum(L, t.Name);
            // RegFunction("__gc", Collect);            
            // BindTypeRef(reference, t);
            // return reference;
            return 0;
        }

        public void EndEnum()
        {
            // LuaDLL.tolua_endenum(L);
        }

        public void BeginStaticLibs(string name)
        {
            // if (beginCount == 0)
            // {
            //     throw new LuaException("must call BeginModule first");
            // }

            // LuaDLL.tolua_beginstaticclass(L, name);
        }

        public void EndStaticLibs()
        {
            // LuaDLL.tolua_endstaticclass(L);
        }

        public void RegFunction(string name, SharpFunction func)
        {
            // IntPtr fn = Marshal.GetFunctionPointerForDelegate(func);
            // LuaDLL.tolua_function(L, name, fn);            
        }

        public void RegVar(string name, SharpFunction get, SharpFunction set)
        {            
            // IntPtr fget = IntPtr.Zero;
            // IntPtr fset = IntPtr.Zero;

            // if (get != null)
            // {
            //     fget = Marshal.GetFunctionPointerForDelegate(get);
            // }

            // if (set != null)
            // {
            //     fset = Marshal.GetFunctionPointerForDelegate(set);
            // }

            // LuaDLL.tolua_variable(L, name, fget, fset);
        }

        public void RegConstant(string name, double d)
        {
            // LuaDLL.tolua_constant(L, name, d);
        }

        public void RegConstant(string name, bool flag)
        {
            // LuaDLL.lua_pushstring(L, name);
            // LuaDLL.lua_pushboolean(L, flag);
            // LuaDLL.lua_rawset(L, -3);
        }
	}
}