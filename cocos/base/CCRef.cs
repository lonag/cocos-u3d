using System;
using System.Runtime.CompilerServices;
using UnityEngine;


NS_CC_BEGIN

public abstract class CCRef : IDisposable
{
    public string name = null;
    protected int reference = -1;
    protected LuaState luaState;
    protected ObjectTranslator translator = null;

    protected volatile bool beDisposed;
    protected int count = 0;

    public CCRef()
    {
        IsAlive = true;
        count = 1;
    }

    ~CCRef()
    {
        IsAlive = false;
        Dispose(false);
    }

    public virtual void Dispose()
    {
        --count;

        if (count > 0)
        {
            return;
        }

        IsAlive = false;
        Dispose(true);            
    }

    public void AddRef()
    {
        ++count;            
    }

    public virtual void Dispose(bool disposeManagedResources)
    {
        if (!beDisposed)
        {
            beDisposed = true;   

            if (reference > 0 && luaState != null)
            {
                luaState.CollectRef(reference, name, !disposeManagedResources);
            }
            
            reference = -1;
            luaState = null;
            count = 0;
        }            
    }

    //慎用
    public void Dispose(int generation)
    {                         
        if (count > generation)
        {
            return;
        }

        Dispose(true);
    }

    public LuaState GetLuaState()
    {
        return luaState;
    }

    public void Push()
    {
        luaState.Push(this);
    }

    public override int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(this);            
    }

    public virtual int GetReference()
    {
        return reference;
    }

    public override bool Equals(object o)
    {
        if (o == null) return reference <= 0;
        CCRef lr = o as CCRef;      
        
        if (lr == null || lr.reference != reference)
        {
            return false;
        }

        return reference > 0;
    }

    static bool CompareRef(CCRef a, CCRef b)
    {
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        object l = a;
        object r = b;

        if (l == null && r != null)
        {
            return b.reference <= 0;
        }

        if (l != null && r == null)
        {
            return a.reference <= 0;
        }

        if (a.reference != b.reference)
        {
            return false;
        }

        return a.reference > 0;
    }

    public static bool operator == (CCRef a, CCRef b)
    {
        return CompareRef(a, b);
    }

    public static bool operator != (CCRef a, CCRef b)
    {
        return !CompareRef(a, b);
    }

    public volatile bool IsAlive = true;
}

NS_CC_END
