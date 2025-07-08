using System;
using System.Threading.Tasks;
using AR.Models;
using GLTFast;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets
{
    public class ModelLoader : MonoBehaviour
    {
        public event Action<string> OnSendMessage;
        public event Action<float> OnSendLoadProgress;

        public static ModelLoader Instance { get; private set; }

        public bool IsModelReady => _loadedModel != null;

        private GltfImport _gltf;
        private GameObject _loadedModel;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async Task<bool> LoadModelFromURL(string url)
        {
            Debug.Log($"[ModelLoader] Starting to load model from URL: {url}");
            OnSendMessage?.Invoke("Loading...");

            _gltf = new GltfImport();

            byte[] data;

            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.SendWebRequest();

                while (!www.isDone)
                {
                    OnSendLoadProgress?.Invoke(www.downloadProgress); 
                    await Task.Yield();
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[ModelLoader] Download failed: {www.error}");
                    OnSendMessage?.Invoke("Failed");
                    return false;
                }

                data = www.downloadHandler.data;
            }
            
            bool success = await _gltf.Load(data);

            if (!success || !_gltf.LoadingDone)
            {
                Debug.LogError("[ModelLoader] Import failed");
                OnSendMessage?.Invoke("Failed");
                return false;
            }

            OnSendMessage?.Invoke("Success");

            if (_loadedModel != null)
            {
                Destroy(_loadedModel);
            }

            _loadedModel = new GameObject("GLB Model Holder");
            DontDestroyOnLoad(_loadedModel);
            OnSendLoadProgress?.Invoke(1f);

            return true;
        }
        
        public GameObject InstantiateModel(Vector3 position, Quaternion rotation)
        {
            if (_gltf == null)
            {
                Debug.LogWarning("[ModelLoader] No GLTF asset loaded to instantiate.");
                return null;
            }

            GameObject root = new GameObject("GLB Model");
            bool success = _gltf.InstantiateMainScene(root.transform);

            if (!success || root.transform.childCount == 0)
            {
                Debug.LogError("[ModelLoader] InstantiateMainScene failed or empty model.");
                Destroy(root);
                return null;
            }

            root.transform.position = position;
            root.transform.rotation = rotation;

            Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds combinedBounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    combinedBounds.Encapsulate(renderers[i].bounds);
                }

                BoxCollider collider = root.AddComponent<BoxCollider>();
                collider.center = root.transform.InverseTransformPoint(combinedBounds.center);
                collider.size = root.transform.InverseTransformVector(combinedBounds.size);
            }

            root.AddComponent<ModelSelectable>();

            Debug.Log("[ModelLoader] Instantiated model at position: " + position);
            return root;
        }
    }
}