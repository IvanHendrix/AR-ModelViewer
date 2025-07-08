using System.Collections.Generic;
using AR.Models;
using Assets;
using Services.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class TestPlacementManager : MonoBehaviour
    {
        private const float Distance = 5f;

        [SerializeField] private Button _placeButton;

        private bool _isPlacementMode = false;

        private IInputProvider _input;
        private Camera _camera;

        private List<GameObject> _models = new List<GameObject>();

        private void Awake()
        {
            _input = InputProviderResolver.InputProvider;
        }

        private void Start()
        {
            _camera = Camera.main;
            _placeButton.onClick.AddListener(OnPlaceModeToggle);
        }

        private void Update()
        {
            if (!_isPlacementMode)
            {
                return;
            }

            if (_input.TryGetTap(out Vector2 position))
            {
                Ray ray = _camera.ScreenPointToRay(position);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Debug.Log($"[PlacementManager] Hit: {hit.transform.name} at {hit.point}");

                    if (hit.transform.TryGetComponent<ModelSelectable>(out var selectable))
                    {
                        selectable.HandleSelection();
                        return;
                    }

                    if (hit.collider != null)
                    {
                        TryPlaceModel(hit.point, Quaternion.identity);
                        return;
                    }
                }

                Vector3 fallback = ray.GetPoint(Distance);
                TryPlaceModel(fallback, Quaternion.identity);
            }
        }

        private void OnPlaceModeToggle()
        {
            _isPlacementMode = true;
            _placeButton.gameObject.SetActive(false);
        }

        private void TryPlaceModel(Vector3 pos, Quaternion rot)
        {
            if (ModelLoader.Instance.IsModelReady)
            {
                GameObject model = ModelLoader.Instance.InstantiateModel(pos, rot);

                ModelSelectable selectable = model.GetComponent<ModelSelectable>();
                
                selectable.HandleSelection();
                _models.Add(model);
            }
        }
        
        private void OnDestroy()
        {
            foreach (GameObject item in _models)
            {
                Destroy(item);
            }
            
            _models.Clear();
            
            _placeButton.onClick.RemoveListener(OnPlaceModeToggle);
        }
    }
}