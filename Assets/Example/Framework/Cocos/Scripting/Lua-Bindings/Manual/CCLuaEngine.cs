namespace cocos2d {
    public class LuaEngine {
        LuaEngine* LuaEngine::_defaultEngine = nullptr;

        public static LuaEngine getInstance(void)
        {
            if (!_defaultEngine)
            {
                _defaultEngine = new LuaEngine();
                _defaultEngine.init();
            }
            return _defaultEngine;
        }

        private Destroy()
        {
            // CC_SAFE_RELEASE(_stack);
            _defaultEngine = nullptr;
        }

        public bool init(void)
        {
            _stack = LuaStack.create();
            // _stack.retain();
            return true;
        }

        public void addSearchPath(const char* path)
        {
            _stack.addSearchPath(path);
        }

        public void addLuaLoader(lua_CFunction func)
        {
            _stack.addLuaLoader(func);
        }

        public void removeScriptObjectByObject(Ref* pObj)
        {
            _stack.removeScriptObjectByObject(pObj);
            ScriptHandlerMgr::getInstance().removeObjectAllHandlers(pObj);
        }

        public void removeScriptHandler(int nHandler)
        {
            _stack.removeScriptHandler(nHandler);
        }

        public int executeString(const char *codes)
        {
            int ret = _stack.executeString(codes);
            _stack.clean();
            return ret;
        }

        public int executeScriptFile(const char* filename)
        {
            int ret = _stack.executeScriptFile(filename);
            _stack.clean();
            return ret;
        }

        public int executeGlobalFunction(const char* functionName)
        {
            int ret = _stack.executeGlobalFunction(functionName);
            _stack.clean();
            return ret;
        }

        public int executeNodeEvent(Node* pNode, int nAction)
        {
            return 0;
        }

        public int executeMenuItemEvent(MenuItem* pMenuItem)
        {
            return 0;
        }

        public int executeNotificationEvent(__NotificationCenter* pNotificationCenter, const char* pszName)
        {
            int nHandler = pNotificationCenter.getObserverHandlerByName(pszName);
            if (!nHandler) return 0;
            
            _stack.pushString(pszName);
            int ret = _stack.executeFunctionByHandler(nHandler, 1);
            _stack.clean();
            return ret;
        }

        public int executeCallFuncActionEvent(CallFunc* pAction, Ref* pTarget/* = NULL*/)
        {
            return 0;
        }

        public int executeSchedule(int nHandler, float dt, Node* pNode/* = NULL*/)
        {
            if (!nHandler) return 0;
            _stack.pushFloat(dt);
            int ret = _stack.executeFunctionByHandler(nHandler, 1);
            _stack.clean();
            return ret;
        }
    }
}
