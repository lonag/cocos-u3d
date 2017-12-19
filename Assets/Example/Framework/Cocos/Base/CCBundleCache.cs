using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UObject = UnityEngine.Object;

namespace CocosFramework
{
    public class AssetBundleInfo {
        public AssetBundle m_AssetBundle;
        public int m_ReferencedCount;

        public AssetBundleInfo(AssetBundle assetBundle) {
            m_AssetBundle = assetBundle;
            m_ReferencedCount = 0;
        }
    }

    /// <summary>
    /// Singleton that handles the loading of textures
    /// Once the texture is loaded, the next time it will return
    /// a reference of the previously loaded texture reducing GPU & CPU memory
    /// </summary>
    public class CCBundleCache : MonoBehaviour
    {
        #region ASYNC_MODE
        string m_BaseDownloadingURL = "";
        string[] m_AllManifest = null;
        AssetBundleManifest m_AssetBundleManifest = null;
        Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();
        Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
        Dictionary<string, List<LoadAssetRequest>> m_LoadRequests = new Dictionary<string, List<LoadAssetRequest>>();
        public class LuaFunction
        {

        }

        class LoadAssetRequest {
            public Type assetType;
            public string[] assetNames;
            public LuaFunction luaFunc;
            public Action<UObject[]> sharpFunc;
        }
        public void LoadPrefabAsync(string abName, string assetName, Action<UObject[]> func) {
            LoadAssetAsync<GameObject>(abName, new string[] { assetName }, func);
        }

        public void LoadPrefabAsync(string abName, string[] assetNames, Action<UObject[]> func)
        {
            LoadAssetAsync<GameObject>(abName, assetNames, func);
        }

        public void LoadPrefabAsync(string abName, string[] assetNames, LuaFunction func)
        {
            LoadAssetAsync<GameObject>(abName, assetNames, null, func);
        }

        string GetRealAssetPath(string abName) {
            //if (abName.Equals(AppConst.AssetDir)) {
            //    return abName;
            //}
            abName = abName.ToLower();
            //if (!abName.EndsWith(AppConst.ExtName)) {
            //    abName += AppConst.ExtName;
            //}
            if (abName.Contains("/")) {
                return abName;
            }
            //string[] paths = m_AssetBundleManifest.GetAllAssetBundles();  
            for (int i = 0; i < m_AllManifest.Length; i++) {
                int index = m_AllManifest[i].LastIndexOf('/');  
                string path = m_AllManifest[i].Remove(0, index + 1);   
                if (path.Equals(abName)) {
                    return m_AllManifest[i];
                }
            }
            Debug.LogError("GetRealAssetPath Error:>>" + abName);
            return null;
        }


        void LoadAssetAsync<T>(string abName, string[] assetNames, Action<UObject[]> action = null, LuaFunction func = null) where T : UObject {
            abName = GetRealAssetPath(abName);

            LoadAssetRequest request = new LoadAssetRequest();
            request.assetType = typeof(T);
            request.assetNames = assetNames;
            request.luaFunc = func;
            request.sharpFunc = action;

            List<LoadAssetRequest> requests = null;
            if (!m_LoadRequests.TryGetValue(abName, out requests)) {
                requests = new List<LoadAssetRequest>();
                requests.Add(request);
                m_LoadRequests.Add(abName, requests);
                StartCoroutine(OnLoadAsset<T>(abName));
            } else {
                requests.Add(request);
            }
        }

        IEnumerator OnLoadAsset<T>(string abName) where T : UObject {
            AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);
            if (bundleInfo == null) {
                yield return StartCoroutine(OnLoadAssetBundle(abName, typeof(T)));

                bundleInfo = GetLoadedAssetBundle(abName);
                if (bundleInfo == null) {
                    m_LoadRequests.Remove(abName);
                    Debug.LogError("OnLoadAsset--->>>" + abName);
                    yield break;
                }
            }
            List<LoadAssetRequest> list = null;
            if (!m_LoadRequests.TryGetValue(abName, out list)) {
                m_LoadRequests.Remove(abName);
                yield break;
            }
            for (int i = 0; i < list.Count; i++) {
                string[] assetNames = list[i].assetNames;
                List<UObject> result = new List<UObject>();

                AssetBundle ab = bundleInfo.m_AssetBundle;
                for (int j = 0; j < assetNames.Length; j++) {
                    string assetPath = assetNames[j];
                    AssetBundleRequest request = ab.LoadAssetAsync(assetPath, list[i].assetType);
                    yield return request;
                    result.Add(request.asset);

                    //T assetObj = ab.LoadAsset<T>(assetPath);
                    //result.Add(assetObj);
                }
                if (list[i].sharpFunc != null) {
                    list[i].sharpFunc(result.ToArray());
                    list[i].sharpFunc = null;
                }
                if (list[i].luaFunc != null) {
                    //list[i].luaFunc.Call((object)result.ToArray());
                    //list[i].luaFunc.Dispose();
                    list[i].luaFunc = null;
                }
                bundleInfo.m_ReferencedCount++;
            }
            m_LoadRequests.Remove(abName);
        }

        IEnumerator OnLoadAssetBundle(string abName, Type type) {
            string url = m_BaseDownloadingURL + abName;

            WWW download = null;
            if (type == typeof(AssetBundleManifest))
                download = new WWW(url);
            else {
                string[] dependencies = m_AssetBundleManifest.GetAllDependencies(abName);
                if (dependencies.Length > 0) {
                    m_Dependencies.Add(abName, dependencies);
                    for (int i = 0; i < dependencies.Length; i++) {
                        string depName = dependencies[i];
                        AssetBundleInfo bundleInfo = null;
                        if (m_LoadedAssetBundles.TryGetValue(depName, out bundleInfo)) {
                            bundleInfo.m_ReferencedCount++;
                        } else if (!m_LoadRequests.ContainsKey(depName)) {
                            yield return StartCoroutine(OnLoadAssetBundle(depName, type));
                        }
                    }
                }
                download = WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(abName), 0);
            }
            yield return download;

            AssetBundle assetObj = download.assetBundle;
            if (assetObj != null) {
                m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(assetObj));
            }
        }

        AssetBundleInfo GetLoadedAssetBundle(string abName) {
            AssetBundleInfo bundle = null;
            m_LoadedAssetBundles.TryGetValue(abName, out bundle);
            if (bundle == null) return null;

            // No dependencies are recorded, only the bundle itself is required.
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
                return bundle;

            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies) {
                AssetBundleInfo dependentBundle;
                m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null) return null;
            }
            return bundle;
        }

        /// <summary>
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isThorough"></param>
        public void UnloadAssetBundle(string abName, bool isThorough = false) {
            abName = GetRealAssetPath(abName);
            Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + abName);
            UnloadAssetBundleInternal(abName, isThorough);
            UnloadDependencies(abName, isThorough);
            Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + abName);
        }

        void UnloadDependencies(string abName, bool isThorough) {
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
                return;

            // Loop dependencies.
            foreach (var dependency in dependencies) {
                UnloadAssetBundleInternal(dependency, isThorough);
            }
            m_Dependencies.Remove(abName);
        }

        void UnloadAssetBundleInternal(string abName, bool isThorough) {
            AssetBundleInfo bundle = GetLoadedAssetBundle(abName);
            if (bundle == null) return;

            if (--bundle.m_ReferencedCount <= 0) {
                if (m_LoadRequests.ContainsKey(abName)) {
                    return;     
                }
                bundle.m_AssetBundle.Unload(isThorough);
                m_LoadedAssetBundles.Remove(abName);
                Debug.Log(abName + " has been unloaded successfully");
            }
        }

        #endregion
        
        private string[] m_Variants = { };
        private AssetBundleManifest manifest;
        private AssetBundle shared, assetbundle;

        protected Dictionary<string, AssetBundle> bundles;
        object m_pDictLock;
        object m_pContextLock;

        #region Singleton

        private static CCBundleCache g_sharedTextureCache;

        private CCBundleCache()
        {
            //Debug.Assert(g_sharedTextureCache == null, "Attempted to allocate a second instance of a singleton.");

            bundles = new Dictionary<string, AssetBundle>();

            m_pDictLock = new object();
            m_pContextLock = new object();

        }

        ~CCBundleCache()
        {
            CCLog.Log("cocos2d: deallocing CCBundleCache.");

            bundles.Clear();
        }

        /// <summary>
        /// Retruns ths shared instance of the cache
        /// </summary>
        public static CCBundleCache sharedTextureCache()
        {
            if (g_sharedTextureCache == null)
            {
                g_sharedTextureCache = new CCBundleCache();
            }

            return g_sharedTextureCache;
        }

        #endregion

        /// <summary>
        /// purges the cache. It releases the retained instance.
        /// @since v0.99.0
        /// </summary>
        public static void purgeSharedTextureCache()
        {
            g_sharedTextureCache = null;
        }

        /// <summary>
        /// Returns a Texture2D object given an file image
        /// If the file image was not previously loaded, it will create a new AssetBundle
        /// object and it will return it. It will use the filename as a key.
        /// Otherwise it will return a reference of a previosly loaded image.
        /// Supported image extensions: .png, .bmp, .tiff, .jpeg, .pvr, .gif
        /// </summary>
        public AssetBundle addImage(string fileimage)
        {
            return null;
            
        }

        /** Returns a Texture2D object given an UIImage image
	    * If the image was not previously loaded, it will create a new AssetBundle object and it will return it.
	    * Otherwise it will return a reference of a previously loaded image
	    * The "key" parameter will be used as the "key" for the cache.
	    * If "key" is nil, then a new texture will be created each time.
	    */
        //public AssetBundle addUIImage(CCImage image, string key)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Returns an already created texture. Returns nil if the texture doesn't exist.
        /// @since v0.99.5
        /// </summary>
        public AssetBundle textureForKey(string key)
        {
            //@todo
            //std::string strKey = CCFileUtils::fullPathFromRelativePath(key);
            AssetBundle texture = null;

            try
            {
                bundles.TryGetValue(key, out texture);
            }
            catch (ArgumentNullException)
            {
                CCLog.Log("Texture of key {0} is not exist.", key);
            }

            return texture;
        }

        /// <summary>
        /// Purges the dictionary of loaded textures.
        /// Call this method if you receive the "Memory Warning"
        /// In the short term: it will free some resources preventing your app from being killed
        /// In the medium term: it will allocate more resources
        /// In the long term: it will be the same
        /// </summary>
        public void removeAllTextures()
        {
            bundles.Clear();
        }

        /// <summary>
        /// Removes unused textures
        /// Textures that have a retain count of 1 will be deleted
        /// It is convinient to call this method after when starting a new Scene
        /// @since v0.8
        /// </summary>
        public void removeUnusedTextures()
        {
            //GAC to handle
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a texture from the cache given a texture
        /// </summary>
        public void removeTexture(AssetBundle texture)
        {
            if (texture == null)
            {
                return;
            }

            string key = null;
            foreach (KeyValuePair<string, AssetBundle> kvp in bundles)
            {
                if (kvp.Value == texture)
                {
                    key = kvp.Key;
                    break;
                }
            }
            if (key != null)
            {
                bundles.Remove(key);
            }
        }

        /// <summary>
        /// Deletes a texture from the cache given a its key name
        /// @since v0.99.4
        /// </summary>
        public void removeTextureForKey(string textureKeyName)
        {
            if (textureKeyName == null)
            {
                return;
            }

            //string fullPath = CCFileUtils::fullPathFromRelativePath(textureKeyName);
            bundles.Remove(textureKeyName);
        }

        /// <summary>
        /// Output to CCLOG the current contents of this CCBundleCache
        /// This will attempt to calculate the size of each texture, and the total texture memory in use
        /// @since v1.0
        /// </summary>
        public void dumpCachedTextureInfo()
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        /// 载入素材
        /// </summary>
        public T LoadAsset<T>(string abname, string assetname) where T : UnityEngine.Object {
            abname = abname.ToLower();
            AssetBundle bundle = LoadAssetBundle(abname);
            return bundle.LoadAsset<T>(assetname);
        }

        public void LoadPrefab(string abName, string[] assetNames, LuaFunction func) {
            //abName = abName.ToLower();
            //List<UObject> result = new List<UObject>();
            //for (int i = 0; i < assetNames.Length; i++) {
            //    UObject go = LoadAsset<UObject>(abName, assetNames[i]);
            //    if (go != null) result.Add(go);
            //}
            //if (func != null) func.Call((object)result.ToArray());
        }

        /// <summary>
        /// 载入AssetBundle
        /// </summary>
        /// <param name="abname"></param>
        /// <returns></returns>
        public AssetBundle LoadAssetBundle(string abname) {
            //if (!abname.EndsWith(AppConst.ExtName)) {
            //    abname += AppConst.ExtName;
            //}
            //AssetBundle bundle = null;
            //if (!bundles.ContainsKey(abname)) {
            //    byte[] stream = null;
            //    string uri = Util.DataPath + abname;
            //    Debug.LogWarning("LoadFile::>> " + uri);
            //    LoadDependencies(abname);

            //    stream = File.ReadAllBytes(uri);
            //    bundle = AssetBundle.LoadFromMemory(stream);
            //    bundles.Add(abname, bundle);
            //} else {
            //    bundles.TryGetValue(abname, out bundle);
            //}
            //return bundle;
            return null;
        }

        /// <summary>
        /// 载入依赖
        /// </summary>
        /// <param name="name"></param>
        void LoadDependencies(string name) {
            if (manifest == null) {
                Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
            }
            // Get dependecies from the AssetBundleManifest object..
            string[] dependencies = manifest.GetAllDependencies(name);
            if (dependencies.Length == 0) return;

            for (int i = 0; i < dependencies.Length; i++)
                dependencies[i] = RemapVariantName(dependencies[i]);

            // Record and load all dependencies.
            for (int i = 0; i < dependencies.Length; i++) {
                LoadAssetBundle(dependencies[i]);
            }
        }

        // Remaps the asset bundle name to the best fitting asset bundle variant.
        string RemapVariantName(string assetBundleName) {
            string[] bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();

            // If the asset bundle doesn't have variant, simply return.
            if (System.Array.IndexOf(bundlesWithVariant, assetBundleName) < 0)
                return assetBundleName;

            string[] split = assetBundleName.Split('.');

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++) {
                string[] curSplit = bundlesWithVariant[i].Split('.');
                if (curSplit[0] != split[0])
                    continue;

                int found = System.Array.IndexOf(m_Variants, curSplit[1]);
                if (found != -1 && found < bestFit) {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }
            if (bestFitIndex != -1)
                return bundlesWithVariant[bestFitIndex];
            else
                return assetBundleName;
        }
    }
}
