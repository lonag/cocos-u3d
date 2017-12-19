using UnityEngine;
using System.Collections.Generic;
using System;

public class CocosLuaNode<T>
{
    public List<CocosLuaNode<T>> childs = new List<CocosLuaNode<T>>();
    public CocosLuaNode<T> parent = null;
    public T value;
    //添加命名空间节点所在位置，解决A.B.C/A.C存在相同名称却在不同命名空间所造成的Wrap问题
    public int layer;
}

public class CocosLuaTree<T> 
{       
    public CocosLuaNode<T> _root = null;
    private List<CocosLuaNode<T>> _list = null;

    public CocosLuaTree()
    {
        _root = new CocosLuaNode<T>();
        _list = new List<CocosLuaNode<T>>();
    }

    //加入pos跟root里的pos比较，只有位置相同才是统一命名空间节点
    void FindParent(List<CocosLuaNode<T>> list, List<CocosLuaNode<T>> root, Predicate<T> match, int layer)
    {
        if (list == null || root == null)
        {
            return;
        }

        for (int i = 0; i < root.Count; i++)
        {
            // 加入layer跟root里的pos比较，只有位置相同才是统一命名空间节点
            if (match(root[i].value) && root[i].layer == layer)
            {
                list.Add(root[i]);
            }

            FindParent(list, root[i].childs, match, layer);
        }
    }

    /*public void BreadthFirstTraversal(Action<CocosLuaNode<T>> action)
    {
        List<CocosLuaNode<T>> root = _root.childs;        
        Queue<CocosLuaNode<T>> queue = new Queue<CocosLuaNode<T>>();

        for (int i = 0; i < root.Count; i++)
        {
            queue.Enqueue(root[i]);
        }

        while (queue.Count > 0)
        {
            CocosLuaNode<T> node = queue.Dequeue();
            action(node);

            if (node.childs != null)
            {
                for (int i = 0; i < node.childs.Count; i++)
                {
                    queue.Enqueue(node.childs[i]);
                }
            }
        }
    }*/

    public void DepthFirstTraversal(Action<CocosLuaNode<T>> begin, Action<CocosLuaNode<T>> end, CocosLuaNode<T> node)
    {
        begin(node);

        for (int i = 0; i < node.childs.Count; i++)
        {            
            DepthFirstTraversal(begin, end, node.childs[i]);
        }

        end(node);
    }

    //只有位置相同才是统一命名空间节点
    public List<CocosLuaNode<T>> Find(Predicate<T> match, int layer)
    {
        _list.Clear();
        FindParent(_list, _root.childs, match, layer);
        return _list;
    }

    public CocosLuaNode<T> GetRoot()
    {
        return _root;
    }
}
