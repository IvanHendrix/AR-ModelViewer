using System;
using System.Threading.Tasks;
using AR.Models;
using GLTFast;
using UnityEngine;

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

            Task<bool> task;
            try
            {
                task = _gltf.Load(new Uri(url));
            }
            catch (Exception e)
            {
                Debug.LogError($"[ModelLoader] Exception while starting load: {e.Message}");
                OnSendMessage?.Invoke("Failed");
                return false;
            }

            float fakeProgress = 0f;
            while (!task.IsCompleted || !_gltf.LoadingDone)
            {
                await Task.Delay(200);
                fakeProgress = Mathf.Min(fakeProgress + 0.05f, 0.9f);
                OnSendLoadProgress?.Invoke(fakeProgress);
            }

            bool success = task.IsCompletedSuccessfully && task.Result && _gltf.LoadingDone;

            if (!success)
            {
                Debug.LogError("[ModelLoader] Failed to load model from task result.");
                Debug.LogError($"IsCompletedSuccessfully: {task.IsCompletedSuccessfully}, Result: {task.Result}, LoadingDone: {_gltf.LoadingDone}");
                OnSendMessage?.Invoke("Failed");
                return false;
            }

            Debug.Log("[ModelLoader] Model loaded successfully and ready to instantiate later.");
            OnSendMessage?.Invoke("Success");

            if (_loadedModel != null)
            {
                Destroy(_loadedModel);
                Debug.Log("[ModelLoader] Destroyed previous model.");
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