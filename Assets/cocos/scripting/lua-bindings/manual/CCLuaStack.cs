/****************************************************************************
 Copyright (c) 2011-2012 cocos2d-x.org
 Copyright (c) 2013-2014 Chukong Technologies Inc.

 http://www.cocos2d-x.org

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

namespace {
    int get_string_for_print(IntPtr   * L, std::string* out)
    {
        int n = lua_gettop(L);  /* number of arguments */
        int i;

        lua_getglobal(L, "tostring");
        for (i=1; i<=n; i++) {
            const char *s;
            lua_pushvalue(L, -1);  /* function to be called */
            lua_pushvalue(L, i);   /* value to print */
            lua_call(L, 1, 1);
            size_t sz;
            s = lua_tolstring(L, -1, &sz);  /* get result */
            if (s == NULL)
                return luaL_error(L, LUA_QL("tostring") " must return a string to "
                                  LUA_QL("print"));
            if (i>1) out->append("\t");
            out->append(s, sz);
            lua_pop(L, 1);  /* pop result */
        }
        return 0;
    }

    int lua_print(IntPtr   * L)
    {
        std::string t;
        get_string_for_print(L, &t);
        CCLOG("[LUA-print] %s", t.c_str());

        return 0;
    }

    int lua_release_print(IntPtr   * L)
    {
        std::string t;
        get_string_for_print(L, &t);
        log("[LUA-print] %s", t.c_str());

        return 0;
    }
}

namespace cocos2d

// LuaStack::~LuaStack()
// {
//     if (nullptr != _state)
//     {
//         lua_close(_state);
//     }
// }

class LuaStack {
    public static LuaStack create(void)
    {
        LuaStack *stack = new  LuaStack();
        stack.init();
        // stack->autorelease();
        return stack;
    }

    public static LuaStack attach(IntPtr L)
    {
        LuaStack *stack = new (std::nothrow) LuaStack();
        stack->initWithLuaState(L);
        stack->autorelease();
        return stack;
    }

    public bool init(void)
    {
    //     _state = lua_open();
    //     luaL_openlibs(_state);
    //     toluafix_open(_state);

    //     // Register our version of the global "print" function
    //     const luaL_reg global_functions [] = {
    //         {"print", lua_print},
    //         {"release_print",lua_release_print},
    //         {nullptr, nullptr}
    //     };
    //     luaL_register(_state, "_G", global_functions);

    //     g_luaType.clear();
    //     register_all_cocos2dx(_state);
    //     tolua_opengl_open(_state);
    //     register_all_cocos2dx_manual(_state);
    //     register_all_cocos2dx_module_manual(_state);
    //     register_all_cocos2dx_math_manual(_state);
    //     register_all_cocos2dx_experimental(_state);
    //     register_all_cocos2dx_experimental_manual(_state);

    //     register_glnode_manual(_state);
    // #if CC_USE_PHYSICS
    //     register_all_cocos2dx_physics(_state);
    //     register_all_cocos2dx_physics_manual(_state);
    // #endif

    // #if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS || CC_TARGET_PLATFORM == CC_PLATFORM_MAC)
    //     LuaObjcBridge::luaopen_luaoc(_state);
    // #endif

    // #if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
    //     LuaJavaBridge::luaopen_luaj(_state);
    // #endif
    //     register_all_cocos2dx_deprecated(_state);
    //     register_all_cocos2dx_manual_deprecated(_state);

    //     tolua_script_handler_mgr_open(_state);

    //     // add cocos2dx loader
    //     addLuaLoader(cocos2dx_lua_loader);

        return true;
    }

    public bool initWithLuaState(IntPtr *L)
    {
        _state = L;
        return true;
    }

    public void addSearchPath(const char* path)
    {
        lua_getglobal(_state, "package");                                  /* L: package */
        lua_getfield(_state, -1, "path");                /* get package.path, L: package path */
        const char* cur_path =  lua_tostring(_state, -1);
        lua_pushfstring(_state, "%s;%s/?.lua", cur_path, path);            /* L: package path newpath */
        lua_setfield(_state, -3, "path");          /* package.path = newpath, L: package path */
        lua_pop(_state, 2);                                                /* L: - */
    }

    public void addLuaLoader(lua_CFunction func)
    {
        if (!func) return;

        // stack content after the invoking of the function
        // get loader table
        lua_getglobal(_state, "package");                                  /* L: package */
        lua_getfield(_state, -1, "loaders");                               /* L: package, loaders */

        // insert loader into index 2
        lua_pushcfunction(_state, func);                                   /* L: package, loaders, func */
        for (int i = (int)(lua_objlen(_state, -2) + 1); i > 2; --i)
        {
            lua_rawgeti(_state, -2, i - 1);                                /* L: package, loaders, func, function */
            // we call lua_rawgeti, so the loader table now is at -3
            lua_rawseti(_state, -3, i);                                    /* L: package, loaders, func */
        }
        lua_rawseti(_state, -2, 2);                                        /* L: package, loaders */

        // set loaders into package
        lua_setfield(_state, -2, "loaders");                               /* L: package */

        lua_pop(_state, 1);
    }


    public void removeScriptObjectByObject(Ref* pObj)
    {
        toluafix_remove_ccobject_by_refid(_state, pObj->_luaID);
    }
    public void removeScriptHandler(int nHandler)
    {
        toluafix_remove_function_by_refid(_state, nHandler);
    }

    public int executeString(const char *codes)
    {
        luaL_loadstring(_state, codes);
        return executeFunction(0);
    }

    static const std::string BYTECODE_FILE_EXT    = ".luac";
    static const std::string NOT_BYTECODE_FILE_EXT = ".lua";

    public int executeScriptFile(const char* filename)
    {
        CCAssert(filename, "CCLuaStack::executeScriptFile() - invalid filename");

        std::string buf(filename);
        //
        // remove .lua or .luac
        //
        size_t pos = buf.rfind(BYTECODE_FILE_EXT);
        if (pos != std::string::npos)
        {
            buf = buf.substr(0, pos);
        }
        else
        {
            pos = buf.rfind(NOT_BYTECODE_FILE_EXT);
            if (pos == buf.length() - NOT_BYTECODE_FILE_EXT.length())
            {
                buf = buf.substr(0, pos);
            }
        }

        FileUtils *utils = FileUtils::getInstance();
        //
        // 1. check .lua suffix
        // 2. check .luac suffix
        //
        std::string tmpfilename = buf + NOT_BYTECODE_FILE_EXT;
        if (utils->isFileExist(tmpfilename))
        {
            buf = tmpfilename;
        }
        else
        {
            tmpfilename = buf + BYTECODE_FILE_EXT;
            if (utils->isFileExist(tmpfilename))
            {
                buf = tmpfilename;
            }
        }

        std::string fullPath = utils->fullPathForFilename(buf);
        Data data = utils->getDataFromFile(fullPath);
        int rn = 0;
        if (!data.isNull())
        {
            if (luaLoadBuffer(_state, (const char*)data.getBytes(), (int)data.getSize(), fullPath.c_str()) == 0)
            {
                rn = executeFunction(0);
            }
        }
        return rn;
    }

    public int executeGlobalFunction(const char* functionName)
    {
        lua_getglobal(_state, functionName);       /* query function by name, stack: function */
        if (!lua_isfunction(_state, -1))
        {
            CCLOG("[LUA ERROR] name '%s' does not represent a Lua function", functionName);
            lua_pop(_state, 1);
            return 0;
        }
        return executeFunction(0);
    }

    public void clean(void)
    {
        lua_settop(_state, 0);
    }

    public void pushInt(int intValue)
    {
        lua_pushinteger(_state, intValue);
    }

    public void pushFloat(float floatValue)
    {
        lua_pushnumber(_state, floatValue);
    }

    public void pushLong(long longValue)
    {
        lua_pushnumber(_state, longValue);
    }

    public void pushBoolean(bool boolValue)
    {
        lua_pushboolean(_state, boolValue);
    }

    public void pushString(const char* stringValue)
    {
        lua_pushstring(_state, stringValue);
    }

    public void pushString(const char* stringValue, int length)
    {
        lua_pushlstring(_state, stringValue, length);
    }

    public void pushNil(void)
    {
        lua_pushnil(_state);
    }

    public void pushObject(Ref* objectValue, const char* typeName)
    {
        toluafix_pushusertype_ccobject(_state, objectValue->_ID, &objectValue->_luaID, objectValue, typeName);
    }

    public void pushLuaValue(const LuaValue& value)
    {
        const LuaValueType type = value.getType();
        if (type == LuaValueTypeInt)
        {
            return pushInt(value.intValue());
        }
        else if (type == LuaValueTypeFloat)
        {
            return pushFloat(value.floatValue());
        }
        else if (type == LuaValueTypeBoolean)
        {
            return pushBoolean(value.booleanValue());
        }
        else if (type == LuaValueTypeString)
        {
            return pushString(value.stringValue().c_str());
        }
        else if (type == LuaValueTypeDict)
        {
            pushLuaValueDict(value.dictValue());
        }
        else if (type == LuaValueTypeArray)
        {
            pushLuaValueArray(value.arrayValue());
        }
        else if (type == LuaValueTypeObject)
        {
            pushObject(value.ccobjectValue(), value.getObjectTypename().c_str());
        }
    }

    public void pushLuaValueDict(const LuaValueDict& dict)
    {
        lua_newtable(_state);                                              /* L: table */
        for (LuaValueDictIterator it = dict.begin(); it != dict.end(); ++it)
        {
            lua_pushstring(_state, it->first.c_str());                     /* L: table key */
            pushLuaValue(it->second);                                     /* L: table key value */
            lua_rawset(_state, -3);                     /* table.key = value, L: table */
        }
    }

    public void pushLuaValueArray(const LuaValueArray& array)
    {
        lua_newtable(_state);                                              /* L: table */
        int index = 1;
        for (LuaValueArrayIterator it = array.begin(); it != array.end(); ++it)
        {
            pushLuaValue(*it);                                            /* L: table value */
            lua_rawseti(_state, -2, index);          /* table[index] = value, L: table */
            ++index;
        }
    }

    public bool pushFunctionByHandler(int nHandler)
    {
        toluafix_get_function_by_refid(_state, nHandler);                  /* L: ... func */
        if (!lua_isfunction(_state, -1))
        {
            CCLOG("[LUA ERROR] function refid '%d' does not reference a Lua function", nHandler);
            lua_pop(_state, 1);
            return false;
        }
        return true;
    }

    public int executeFunction(int numArgs)
    {
        int functionIndex = -(numArgs + 1);
        if (!lua_isfunction(_state, functionIndex))
        {
            CCLOG("value at stack [%d] is not function", functionIndex);
            lua_pop(_state, numArgs + 1); // remove function and arguments
            return 0;
        }

        int traceback = 0;
        lua_getglobal(_state, "__G__TRACKBACK__");                         /* L: ... func arg1 arg2 ... G */
        if (!lua_isfunction(_state, -1))
        {
            lua_pop(_state, 1);                                            /* L: ... func arg1 arg2 ... */
        }
        else
        {
            lua_insert(_state, functionIndex - 1);                         /* L: ... G func arg1 arg2 ... */
            traceback = functionIndex - 1;
        }

        int error = 0;
        ++_callFromLua;
        error = lua_pcall(_state, numArgs, 1, traceback);                  /* L: ... [G] ret */
        --_callFromLua;
        if (error)
        {
            if (traceback == 0)
            {
                CCLOG("[LUA ERROR] %s", lua_tostring(_state, - 1));        /* L: ... error */
                lua_pop(_state, 1); // remove error message from stack
            }
            else                                                            /* L: ... G error */
            {
                lua_pop(_state, 2); // remove __G__TRACKBACK__ and error message from stack
            }
            return 0;
        }

        // get return value
        int ret = 0;
        if (lua_isnumber(_state, -1))
        {
            ret = (int)lua_tointeger(_state, -1);
        }
        else if (lua_isboolean(_state, -1))
        {
            ret = (int)lua_toboolean(_state, -1);
        }
        // remove return value from stack
        lua_pop(_state, 1);                                                /* L: ... [G] */

        if (traceback)
        {
            lua_pop(_state, 1); // remove __G__TRACKBACK__ from stack      /* L: ... */
        }

        return ret;
    }

    public int executeFunctionByHandler(int nHandler, int numArgs)
    {
        int ret = 0;
        if (pushFunctionByHandler(nHandler))                                /* L: ... arg1 arg2 ... func */
        {
            if (numArgs > 0)
            {
                lua_insert(_state, -(numArgs + 1));                        /* L: ... func arg1 arg2 ... */
            }
            ret = executeFunction(numArgs);
        }
        lua_settop(_state, 0);
        return ret;
    }

    public bool handleAssert(const char *msg)
    {
        if (_callFromLua == 0) return false;

        lua_pushfstring(_state, "ASSERT FAILED ON LUA EXECUTE: %s", msg ? msg : "unknown");
        lua_error(_state);
        return true;
    }

    public int reallocateScriptHandler(int nHandler)
    {
        LUA_FUNCTION  nNewHandle = -1;

        if (pushFunctionByHandler(nHandler))
        {
           nNewHandle = toluafix_ref_function(_state,lua_gettop(_state),0);
        }
    /*
        toluafix_get_function_by_refid(_state,nNewHandle);
        if (!lua_isfunction(_state, -1))
        {
            CCLOG("Error!");
        }
        lua_settop(_state, 0);
    */
        return nNewHandle;

    }

    public int executeFunctionReturnArray(int handler,int numArgs,int numResults,__Array& resultArray)
    {
        int top = lua_gettop(_state);
        if (pushFunctionByHandler(handler))                 /* L: ... arg1 arg2 ... func */
        {
            if (numArgs > 0)
            {
                lua_insert(_state, -(numArgs + 1));         /* L: ... func arg1 arg2 ... */
            }
            int functionIndex = -(numArgs + 1);
            if (!lua_isfunction(_state, functionIndex))
            {
                CCLOG("value at stack [%d] is not function", functionIndex);
                lua_pop(_state, numArgs + 1); // remove function and arguments
                lua_settop(_state,top);
                return 0;
            }

            int traceback = 0;
            lua_getglobal(_state, "__G__TRACKBACK__");                         /* L: ... func arg1 arg2 ... G */
            if (!lua_isfunction(_state, -1))
            {
                lua_pop(_state, 1);                                            /* L: ... func arg1 arg2 ... */
            }
            else
            {
                lua_insert(_state, functionIndex - 1);                         /* L: ... G func arg1 arg2 ... */
                traceback = functionIndex - 1;
            }

            int error = 0;
            ++_callFromLua;
            error = lua_pcall(_state, numArgs, numResults, traceback);                  /* L: ... [G] ret1 ret2 ... retResults*/
            --_callFromLua;
            if (error)
            {
                if (traceback == 0)
                {
                    CCLOG("[LUA ERROR] %s", lua_tostring(_state, - 1));        /* L: ... error */
                    lua_pop(_state, 1); // remove error message from stack
                }
                else                                                            /* L: ... G error */
                {
                    lua_pop(_state, 2); // remove __G__TRACKBACK__ and error message from stack
                }
                lua_settop(_state,top);
                return 0;
            }

            // get return value,don't pass LUA_MULTRET to numResults,
            if (numResults <= 0)
            {
                lua_settop(_state,top);
                return 0;
            }

            for (int i = 0 ; i < numResults; i++)
            {
                if (lua_type(_state, -1) == LUA_TBOOLEAN) {

                    bool value = lua_toboolean(_state, -1);
                    resultArray.addObject(__Bool::create(value));

                }else if (lua_type(_state, -1) == LUA_TNUMBER) {

                    double value = lua_tonumber(_state, -1);
                    resultArray.addObject(__Double::create(value));

                }else if (lua_type(_state, -1) == LUA_TSTRING) {

                    const char* value = lua_tostring(_state, -1);
                    resultArray.addObject(__String::create(value));

                }else{

                    resultArray.addObject(static_cast<Ref*>(tolua_tousertype(_state, -1, nullptr)));
                }
                // remove return value from stack
                lua_pop(_state, 1);                                                /* L: ... [G] ret1 ret2 ... ret*/
            }
            /* L: ... [G]*/

            if (traceback)
            {
                lua_pop(_state, 1); // remove __G__TRACKBACK__ from stack      /* L: ... */
            }
        }
        lua_settop(_state,top);
        return 1;
    }

    public int executeFunction(int handler, int numArgs, int numResults, const std::function<void(IntPtr  *,int)>& func)
    {
        if (pushFunctionByHandler(handler))                 /* L: ... arg1 arg2 ... func */
        {
            if (numArgs > 0)
            {
                lua_insert(_state, -(numArgs + 1));                        /* L: ... func arg1 arg2 ... */
            }

            int functionIndex = -(numArgs + 1);

            if (!lua_isfunction(_state, functionIndex))
            {
                CCLOG("value at stack [%d] is not function", functionIndex);
                lua_pop(_state, numArgs + 1); // remove function and arguments
                return 0;
            }

            int traceCallback = 0;
            lua_getglobal(_state, "__G__TRACKBACK__");                        /* L: ... func arg1 arg2 ... G */
            if (!lua_isfunction(_state, -1))
            {
                lua_pop(_state, 1);                                           /* L: ... func arg1 arg2 ... */
            }
            else
            {
                lua_insert(_state, functionIndex - 1);                         /* L: ... G func arg1 arg2 ... */
                traceCallback = functionIndex - 1;
            }

            int error = 0;
            ++_callFromLua;
            error = lua_pcall(_state, numArgs, numResults, traceCallback);     /* L: ... [G] ret1 ret2 ... retResults*/
            --_callFromLua;

            if (error)
            {
                if (traceCallback == 0)
                {
                    CCLOG("[LUA ERROR] %s", lua_tostring(_state, - 1));        /* L: ... error */
                    lua_pop(_state, 1);                                        // remove error message from stack
                }
                else                                                           /* L: ... G error */
                {
                    lua_pop(_state, 2);                                        // remove __G__TRACKBACK__ and error message from stack
                }
                return 0;
            }

            // get return value,don't pass LUA_MULTRET to numResults,
            do {

                if (numResults <= 0 || nullptr == func)
                    break;

                func(_state, numResults);

            } while (0);

            if (traceCallback)
            {
                lua_pop(_state, 1);                                          // remove __G__TRACKBACK__ from stack      /* L: ... */
            }
        }

        return 1;
    }

    public int reload(const char* moduleFileName)
    {
        if (nullptr == moduleFileName || strlen(moduleFileName) == 0)
        {
            CCLOG("moudulFileName is null");
            return 1;
        }

        lua_getglobal(_state, "package");                         /* L: package */
        lua_getfield(_state, -1, "loaded");                       /* L: package loaded */
        lua_pushstring(_state, moduleFileName);
        lua_gettable(_state, -2);                                 /* L:package loaded module */
        if (!lua_isnil(_state, -1))
        {
            lua_pushstring(_state, moduleFileName);               /* L:package loaded module name */
            lua_pushnil(_state);                                  /* L:package loaded module name nil*/
            lua_settable(_state, -4);                             /* L:package loaded module */
        }
        lua_pop(_state, 3);

        std::string name = moduleFileName;
        std::string require = "require \'" + name + "\'";
        return executeString(require.c_str());
    }

    public void setXXTEAKeyAndSign(const char *key, int keyLen, const char *sign, int signLen)
    {
        cleanupXXTEAKeyAndSign();

        if (key && keyLen && sign && signLen)
        {
            _xxteaKey = (char*)malloc(keyLen);
            memcpy(_xxteaKey, key, keyLen);
            _xxteaKeyLen = keyLen;

            _xxteaSign = (char*)malloc(signLen);
            memcpy(_xxteaSign, sign, signLen);
            _xxteaSignLen = signLen;

            _xxteaEnabled = true;
        }
        else
        {
            _xxteaEnabled = false;
        }
    }

    public void cleanupXXTEAKeyAndSign()
    {
        if (_xxteaKey)
        {
            free(_xxteaKey);
            _xxteaKey = nullptr;
            _xxteaKeyLen = 0;
        }
        if (_xxteaSign)
        {
            free(_xxteaSign);
            _xxteaSign = nullptr;
            _xxteaSignLen = 0;
        }
    }

    public int loadChunksFromZIP(const char *zipFilePath)
    {
        pushString(zipFilePath);
        luaLoadChunksFromZIP(_state);
        int ret = lua_toboolean(_state, -1);
        lua_pop(_state, 1);
        return ret;
    }

    public int luaLoadChunksFromZIP(IntPtr *L)
    {
        if (lua_gettop(L) < 1) {
            CCLOG("luaLoadChunksFromZIP() - invalid arguments");
            return 0;
        }

        const char *zipFilename = lua_tostring(L, -1);
        lua_settop(L, 0);
        FileUtils *utils = FileUtils::getInstance();
        std::string zipFilePath = utils->fullPathForFilename(zipFilename);

        LuaStack *stack = this;

        do {
            void *buffer = nullptr;
            ZipFile *zip = nullptr;
            Data zipFileData(utils->getDataFromFile(zipFilePath));
            unsigned char* bytes = zipFileData.getBytes();
            ssize_t size = zipFileData.getSize();

            bool isXXTEA = stack && stack->_xxteaEnabled && size >= stack->_xxteaSignLen
                && memcmp(stack->_xxteaSign, bytes, stack->_xxteaSignLen) == 0;


            if (isXXTEA) { // decrypt XXTEA
                xxtea_long len = 0;
                buffer = xxtea_decrypt(bytes + stack->_xxteaSignLen,
                                       (xxtea_long)size - (xxtea_long)stack->_xxteaSignLen,
                                       (unsigned char*)stack->_xxteaKey,
                                       (xxtea_long)stack->_xxteaKeyLen,
                                       &len);
                zip = ZipFile::createWithBuffer(buffer, len);
            } else {
                if (size > 0) {
                    zip = ZipFile::createWithBuffer(bytes, (unsigned long)size);
                }
            }

            if (zip) {
                CCLOG("lua_loadChunksFromZIP() - load zip file: %s%s", zipFilePath.c_str(), isXXTEA ? "*" : "");
                lua_getglobal(L, "package");
                lua_getfield(L, -1, "preload");

                int count = 0;
                std::string filename = zip->getFirstFilename();
                while (filename.length()) {
                    ssize_t bufferSize = 0;
                    unsigned char *zbuffer = zip->getFileData(filename.c_str(), &bufferSize);
                    if (bufferSize) {
                        // remove .lua or .luac extension
                        size_t pos = filename.find_last_of('.');
                        if (pos != std::string::npos)
                        {
                            std::string suffix = filename.substr(pos, filename.length());
                            if (suffix == NOT_BYTECODE_FILE_EXT || suffix == BYTECODE_FILE_EXT) {
                                filename.erase(pos);
                            }
                        }
                        // replace path separator '/' '\' to '.'
                        for (int i=0; i<filename.size(); i++) {
                            if (filename[i] == '/' || filename[i] == '\\') {
                                filename[i] = '.';
                            }
                        }
                        CCLOG("[luaLoadChunksFromZIP] add %s to preload", filename.c_str());
                        if (stack->luaLoadBuffer(L, (char*)zbuffer, (int)bufferSize, filename.c_str()) == 0) {
                            lua_setfield(L, -2, filename.c_str());
                            ++count;
                        }
                        free(zbuffer);
                    }
                    filename = zip->getNextFilename();
                }
                CCLOG("lua_loadChunksFromZIP() - loaded chunks count: %d", count);
                lua_pop(L, 2);
                lua_pushboolean(L, 1);

                delete zip;
            } else {
                CCLOG("lua_loadChunksFromZIP() - not found or invalid zip file: %s", zipFilePath.c_str());
                lua_pushboolean(L, 0);
            }


            if (buffer) {
                free(buffer);
            }
        } while (0);

        return 1;
    }

    public int luaLoadBuffer(IntPtr *L, const char *chunk, int chunkSize, const char *chunkName)
    {
        int r = 0;
        
        auto skipBOM = [](const char*& chunk, int& chunkSize){
            // UTF-8 BOM? skip
            if (static_cast<unsigned char>(chunk[0]) == 0xEF &&
                static_cast<unsigned char>(chunk[1]) == 0xBB &&
                static_cast<unsigned char>(chunk[2]) == 0xBF)
            {
                chunk += 3;
                chunkSize -= 3;
            }
        };

        if (_xxteaEnabled && strncmp(chunk, _xxteaSign, _xxteaSignLen) == 0)
        {
            // decrypt XXTEA
            xxtea_long len = 0;
            unsigned char* result = xxtea_decrypt((unsigned char*)chunk + _xxteaSignLen,
                                                  (xxtea_long)chunkSize - _xxteaSignLen,
                                                  (unsigned char*)_xxteaKey,
                                                  (xxtea_long)_xxteaKeyLen,
                                                  &len);
            skipBOM((const char*&)result, (int&)len);
            r = luaL_loadbuffer(L, (char*)result, len, chunkName);
            free(result);
        }
        else
        {
            skipBOM(chunk, chunkSize);
            r = luaL_loadbuffer(L, chunk, chunkSize, chunkName);
        }

    #if defined(COCOS2D_DEBUG) && COCOS2D_DEBUG > 0
        if (r)
        {
            switch (r)
            {
                case LUA_ERRSYNTAX:
                    CCLOG("[LUA ERROR] load \"%s\", error: syntax error during pre-compilation.", chunkName);
                    break;

                case LUA_ERRMEM:
                    CCLOG("[LUA ERROR] load \"%s\", error: memory allocation error.", chunkName);
                    break;

                case LUA_ERRFILE:
                    CCLOG("[LUA ERROR] load \"%s\", error: cannot open/read file.", chunkName);
                    break;

                default:
                    CCLOG("[LUA ERROR] load \"%s\", error: unknown.", chunkName);
            }
        }
    #endif
        return r;
    }

}