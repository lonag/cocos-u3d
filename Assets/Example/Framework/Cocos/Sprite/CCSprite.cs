using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using CocosFramework;
//using Uobject = UnityEngine.Object;

namespace CocosFramework
{
    public class LuaFunction {

    } 

    public class CCSprite : CCNode {
        // image is flipped
        protected bool m_bFlipX;
        protected bool m_bFlipY;

        public static CCSprite create()
        {
            //
            CCSprite sprite = new CCSprite();
            if (sprite.init())
            {
                // sprite.autorelease();
                return sprite;
            }
            // CC_SAFE_DELETE(sprite);
            return null;
        }

        public static CCSprite create(string modelPath)
        {
            // CCASSERT(modelPath.length() >= 4, "invalid filename for Sprite");
            
            CCSprite sprite = new CCSprite();
            if (sprite.initWithFile(modelPath))
            {
                // sprite._contentSize = sprite.getBoundingBox().size;
                // sprite.autorelease();
                return sprite;
            }
            // CC_SAFE_DELETE(sprite);
            return null;
        }

        public static CCSprite create(string modelPath, string texturePath)
        {
            CCSprite sprite = create(modelPath);
            if (sprite != null)
            {
                //sprite.setTexture(texturePath);
            }
            
            return sprite;
        }

        public void createAsync(string modelPath, LuaFunction callback, object[] callbackparam)
        {
            createAsync(modelPath, "", callback, callbackparam);
        }

        public void createAsync(string modelPath, string texturePath, LuaFunction callback, object[] callbackparam)
        {
            // Sprite *sprite = new Sprite();
            // if (sprite.loadFromCache(modelPath))
            // {
            //     sprite.autorelease();
            //     if (!texturePath.empty())
            //         sprite.setTexture(texturePath);
            //     callback(sprite, callbackparam);
            //     return;
            // }
            
            // sprite._asyncLoadParam.afterLoadCallback = callback;
            // sprite._asyncLoadParam.texPath = texturePath;
            // sprite._asyncLoadParam.modlePath = modelPath;
            // sprite._asyncLoadParam.callbackParam = callbackparam;
            // sprite._asyncLoadParam.materialdatas = new (std::nothrow) MaterialDatas();
            // sprite._asyncLoadParam.meshdatas = new (std::nothrow) MeshDatas();
            // sprite._asyncLoadParam.nodeDatas = new (std::nothrow) NodeDatas();
            // AsyncTaskPool::getInstance().enqueue(AsyncTaskPool::TaskType::TASK_IO, CC_CALLBACK_1(Sprite::afterAsyncLoad, sprite), (void*)(&sprite._asyncLoadParam), [sprite]()
            // {
            //     sprite._asyncLoadParam.result = sprite.loadFromFile(sprite._asyncLoadParam.modlePath, sprite._asyncLoadParam.nodeDatas, sprite._asyncLoadParam.meshdatas, sprite._asyncLoadParam.materialdatas);
            // });
            
        }

        public void afterAsyncLoad(object[] param)
        {
            // AsyncLoadParam* asyncParam = (Sprite::AsyncLoadParam*)param;
            // autorelease();
            // if (asyncParam)
            // {
            //     if (asyncParam.result)
            //     {
            //         _meshes.clear();
            //         _meshVertexDatas.clear();
            //         CC_SAFE_RELEASE_NULL(_skeleton);
            //         removeAllAttachNode();
                    
            //         //create in the main thread
            //         auto& meshdatas = asyncParam.meshdatas;
            //         auto& materialdatas = asyncParam.materialdatas;
            //         auto&   nodeDatas = asyncParam.nodeDatas;
            //         if (initFrom(*nodeDatas, *meshdatas, *materialdatas))
            //         {
            //             auto spritedata = getInstance().getSpriteData(asyncParam.modlePath);
            //             if (spritedata == null)
            //             {
            //                 //add to cache
            //                 auto data = new (std::nothrow) SpriteData();
            //                 data.materialdatas = materialdatas;
            //                 data.nodedatas = nodeDatas;
            //                 data.meshVertexDatas = _meshVertexDatas;
            //                 for (const auto mesh : _meshes) {
            //                     data.glProgramStates.pushBack(mesh.getGLProgramState());
            //                 }
                            
            //                 getInstance().addSpriteData(asyncParam.modlePath, data);
                            
            //                 CC_SAFE_DELETE(meshdatas);
            //                 materialdatas = null;
            //                 nodeDatas = null;
            //             }
            //         }
            //         CC_SAFE_DELETE(meshdatas);
            //         CC_SAFE_DELETE(materialdatas);
            //         CC_SAFE_DELETE(nodeDatas);
                    
            //         if (asyncParam.texPath != "")
            //         {
            //             setTexture(asyncParam.texPath);
            //         }
            //     }
            //     else
            //     {
            //         CCLOG("file load failed: %s ", asyncParam.modlePath.c_str());
            //     }
            //     asyncParam.afterLoadCallback(this, asyncParam.callbackParam);
            // }
        }

        bool loadFromCache(string path)
        {
            
            return false;
        }

        bool loadFromFile(string path)
        {
            return false;
        }

        // Sprite::Sprite()
        // : _skeleton(null)
        // , _blend(BlendFunc::ALPHA_NON_PREMULTIPLIED)
        // , _aabbDirty(true)
        // , _lightMask(-1)
        // , _shaderUsingLight(false)
        // , _forceDepthWrite(false)
        // , _usingAutogeneratedGLProgram(true)
        // {
        // }

        // Sprite::~Sprite()
        // {
        //     _meshes.clear();
        //     _meshVertexDatas.clear();
        //     CC_SAFE_RELEASE_NULL(_skeleton);
        //     removeAllAttachNode();
        // }

        public bool init()
        {
            // if(Node::init())
            // {
            //     return true;
            // }
            return false;
        }

        bool initWithFile(string path)
        {
            return false;
        }

        public static Sprite createSpriteNode()
        {
           return null;
        }

        //public void setMaterial(Material *material)
        //{
        //    setMaterial(material, -1); 
        //}

        //public void setMaterial(Material *material, int meshIndex)
        //{
        //    // CCASSERT(material, "Invalid Material");
        //    // CCASSERT(meshIndex == -1 || (meshIndex >=0 && meshIndex < _meshes.size()), "Invalid meshIndex");


        //    if (meshIndex == -1)
        //    {
        //        for (ssize_t i = 0; i < _meshes.size(); i++)
        //        {
        //            _meshes.at(i).setMaterial(i == 0 ? material : material.clone());
        //        }
        //    }
        //    else
        //    {
        //        auto mesh = _meshes.at(meshIndex);
        //        mesh.setMaterial(material);
        //    }

        //    _usingAutogeneratedGLProgram = false;
        //}

        //// Material* getMaterial(int meshIndex) const
        //// {
        ////     CCASSERT(meshIndex >=0 && meshIndex < _meshes.size(), "Invalid meshIndex");
        ////     return _meshes.at(meshIndex).getMaterial();
        //// }


        //public void genMaterial(bool useLight)
        //{
            
        //}


        //public void  addMesh(Mesh* mesh)
        //{

        //}

        // public void setTexture(string texFile)
        // {
        //     auto tex = Director::getInstance().getTextureCache().addImage(texFile);
        //     setTexture(tex);
        // }

        // public void setTexture(Texture2D* texture)
        // {
        //     for (auto mesh: _meshes) {
        //         mesh.setTexture(texture);
        //     }
        // }

        CCAction runAction(CCAction action)
        {
            //setForceDepthWrite(true);
            return base.runAction(action);
        }

        // Rect getBoundingBox() const
        // {
        //     AABB aabb = getAABB();
        //     Rect ret(aabb._min.x, aabb._min.y, (aabb._max.x - aabb._min.x), (aabb._max.y - aabb._min.y));
        //     return ret;
        // }
        /// <summary>
        /// whether or not the sprite is flipped horizontally. 
        /// It only flips the texture of the sprite, and not the texture of the sprite's children.
        /// Also, flipping the texture doesn't alter the anchorPoint.
        /// If you want to flip the anchorPoint too, and/or to flip the children too use:
        /// sprite->setScaleX(sprite->getScaleX() * -1);
        /// </summary>
        public bool IsFlipX
        {
            set
            {
                if (m_bFlipX != value)
                {
                    m_bFlipX = value;
                    //setTextureRectInPixels(m_obRectInPixels, m_bRectRotated, m_tContentSizeInPixels);
                }
            }
            get { return m_bFlipX; }
        }

        /// <summary>
        /// whether or not the sprite is flipped vertically.
        /// It only flips the texture of the sprite, and not the texture of the sprite's children.
        /// Also, flipping the texture doesn't alter the anchorPoint.
        /// If you want to flip the anchorPoint too, and/or to flip the children too use:
        /// sprite->setScaleY(sprite->getScaleY() * -1);
        /// </summary>
        public bool IsFlipY
        {
            set
            {
                if (m_bFlipY != value)
                {
                    m_bFlipY = value;
                    //setTextureRectInPixels(m_obRectInPixels, m_bRectRotated, m_tContentSizeInPixels);
                }
            }
            get { return m_bFlipY; }
        }
        
    }
    

    public class SpriteCache {
        public class SpriteData {

        }

        /////////////////////////////////////////////////////////////////////////////////
        protected static SpriteCache _cacheInstance;
        protected Dictionary<string, SpriteData> _spriteDatas;
        public static SpriteCache getInstance()
        {
            if (_cacheInstance == null)
                _cacheInstance = new SpriteCache();
            return _cacheInstance;
        }
        public void destroyInstance()
        {
            //if (_cacheInstance )
            //{
            //    delete _cacheInstance;
            //    _cacheInstance = null;
            //}
            _cacheInstance = null;
        }

        public SpriteData getSpriteData(string key)
        {
            //object it = _spriteDatas.find(key);
            //if (it != _spriteDatas.end())
            //    return it.second;
            return null;
        }

        public bool addSpriteData(string key, SpriteData spritedata)
        {
            // object it = _spriteDatas.find(key);
            // if (it == _spriteDatas.end())
            // {
            //     _spriteDatas[key] = spritedata;
            //     return true;
            // }
            return false;
        }

        public void removeSpriteData(string key)
        {
            //if (key == null)
            //{
            //    return;
            //}

            //string key = null;
            //foreach (KeyValuePair<string, SpriteData> kvp in _spriteDatas)
            //{
            //    if (kvp.Key == key)
            //    {
            //        key = kvp.Key;
            //        break;
            //    }
            //}
            //if (key != null)
            //{
            //    bundles.Remove(key);
            //}

        }

        public void removeAllSpriteData()
        {
            _spriteDatas.Clear();
        }

        // SpriteCache()
        // {
            
        // }
        // ~SpriteCache()
        // {
        //     removeAllSpriteData();
        // }
    }
    
}
