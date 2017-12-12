namespace cocos2d {
    public class LuaValue {
        public LuaValue intValue(const int intValue)
        {
            LuaValue value;
            value._type = LuaValueTypeInt;
            value._field.intValue = intValue;
            return value;
        }

        public LuaValue floatValue(const float floatValue)
        {
            LuaValue value;
            value._type = LuaValueTypeFloat;
            value._field.floatValue = floatValue;
            return value;
        }

        public LuaValue booleanValue(const bool booleanValue)
        {
            LuaValue value;
            value._type = LuaValueTypeBoolean;
            value._field.booleanValue = booleanValue;
            return value;
        }

        public LuaValue stringValue(const char* stringValue)
        {
            LuaValue value;
            value._type = LuaValueTypeString;
            value._field.stringValue = new std::string(stringValue ? stringValue : "");
            return value;
        }

        public LuaValue stringValue(const std::string& stringValue)
        {
            LuaValue value;
            value._type = LuaValueTypeString;
            value._field.stringValue = new std::string(stringValue);
            return value;
        }

        public LuaValue dictValue(const LuaValueDict& dictValue)
        {
            LuaValue value;
            value._type = LuaValueTypeDict;
            value._field.dictValue = new (std::nothrow) LuaValueDict(dictValue);
            return value;
        }

        public LuaValue arrayValue(const LuaValueArray& arrayValue)
        {
            LuaValue value;
            value._type = LuaValueTypeArray;
            value._field.arrayValue = new (std::nothrow) LuaValueArray(arrayValue);
            return value;
        }

        public LuaValue ccobjectValue(Ref* ccobjectValue, const char* objectTypename)
        {
            LuaValue value;
            value._type = LuaValueTypeObject;
            value._field.ccobjectValue = ccobjectValue;
            ccobjectValue->retain();
            value._ccobjectType = new std::string(objectTypename);
            return value;
        }

        public LuaValue ccobjectValue(Ref* ccobjectValue, const std::string& objectTypename)
        {
            return LuaValue::ccobjectValue(ccobjectValue, objectTypename.c_str());
        }

        LuaValue::LuaValue(const LuaValue& rhs)
        {
            copy(rhs);
        }

        LuaValue& LuaValue::operator=(const LuaValue& rhs)
        {
            if (this != &rhs) copy(rhs);
            return *this;
        }

        LuaValue::~LuaValue(void)
        {
            if (_type == LuaValueTypeString)
            {
                delete _field.stringValue;
            }
            else if (_type == LuaValueTypeDict)
            {
                delete _field.dictValue;
            }
            else if (_type == LuaValueTypeArray)
            {
                delete _field.arrayValue;
            }
            else if (_type == LuaValueTypeObject)
            {
                _field.ccobjectValue->release();
                delete _ccobjectType;
            }
        }

        public void copy(const LuaValue& rhs)
        {
            memcpy(&_field, &rhs._field, sizeof(_field));
            _type = rhs._type;
            if (_type == LuaValueTypeString)
            {
                _field.stringValue = new std::string(*rhs._field.stringValue);
            }
            else if (_type == LuaValueTypeDict)
            {
                _field.dictValue = new (std::nothrow) LuaValueDict(*rhs._field.dictValue);
            }
            else if (_type == LuaValueTypeArray)
            {
                _field.arrayValue = new (std::nothrow) LuaValueArray(*rhs._field.arrayValue);
            }
            else if (_type == LuaValueTypeObject)
            {
                _field.ccobjectValue = rhs._field.ccobjectValue;
                _field.ccobjectValue->retain();
                _ccobjectType = new std::string(*rhs._ccobjectType);
            }
        }
            
    } 
} 
