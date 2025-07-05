using Services.Input;
using UnityEngine;

namespace AR.Models
{
    public class ModelSelectable : MonoBehaviour
    {
        private IInputProvider _input;
        private Camera _camera;

        private void Awake()
        {
            _input = InputProviderResolver.InputProvider;
        }

        private void Start()
        {
            _camera  = Camera.main;
        }

        private void Update()
        {
            if (_input.TryGetTap(out Vector2 pos))
            {
                Ray ray = _camera.ScreenPointToRay(pos);
                if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
                {
                    HandleSelection();
                }
            }
        }

        public void HandleSelection()
        {
            ModelInteractionController.Instance.SetTarget(transform);
        }
    }
}