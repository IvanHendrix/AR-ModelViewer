using System;
using Services.Input;
using UnityEngine;

namespace AR.Models
{
    public class ModelInteractionController : MonoBehaviour
    {
        [SerializeField] private HighlightRingAttacher _highlightRingAttacher;
        
        public static ModelInteractionController Instance { get; private set; }
        
        private static Transform _currentTarget;
        
        private IInputProvider _input;

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
            
            _input = InputProviderResolver.InputProvider;
        }

        private void Update()
        {
            if (_currentTarget == null) return;

            if (_input.IsPinching(out float pinch))
            {
                float scale = 1 + pinch * 0.001f;
                _currentTarget.localScale *= scale;
            }

            if (_input.TryGetSwipeDelta(out Vector2 delta))
            {
                _currentTarget.Rotate(Vector3.up, -delta.x * 0.2f, Space.World);
            }
        }

        public void SetTarget(Transform target)
        {
            _currentTarget = target;

            _highlightRingAttacher.AttachRing(target.gameObject);
        }
    }
}