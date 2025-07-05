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
                    Debug.Log("[ARPlacementManager] Hit something: " + hit.transform.name);

                    if (hit.transform.TryGetComponent<ModelSelectable>(out var selectable))
                    {
                        selectable.HandleSelection();
                    }

                    return;
                }

                Vector3 fakePos = ray.GetPoint(Distance);
                TryPlaceModel(fakePos, Quaternion.identity);
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
            }
        }
        
        private void OnDestroy()
        {
            _placeButton.onClick.RemoveListener(OnPlaceModeToggle);
        }
    }
}