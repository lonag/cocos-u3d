local sharplua = require "sharplua"

local UnityEngine = class("UnityEngine")

function UnityEngine:ctor()

end

function UnityEngineRegFunction(func,funcName)
	UnityEngine[funcName] = function(self, ...)
		sharplua.call(func, ...)
	end 
end

return UnityEngine

-- local cocos = class("cocos")
-- function cocos:ctor()

-- end

-- cocos["func"] = function(self, a, b)
-- 	print(a,b)
-- end

-- cocos:func(1,2)