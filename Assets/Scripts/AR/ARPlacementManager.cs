using System;
using System.Collections.Generic;
using AR.Models;
using Assets;
using Services.Input;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace AR
{
    public class ARPlacementManager : MonoBehaviour
    {
        [Header("AR Components")]
        [SerializeField] private ARRaycastManager _raycastManager;
        [SerializeField] private ARPlaneManager _planeManager;

        [Header("UI")]
        [SerializeField] private Button _placeButton;

        private bool _isPlacementMode = false;
        private static List<ARRaycastHit> hits = new();

        private IInputProvider _input;

        private void Awake()
        {
            _input = InputProviderResolver.InputProvider;
        }

        private void Start()
        {
            _placeButton.onClick.AddListener(OnPlaceModeToggle);
        }

        private void Update()
        {
            if (!_isPlacementMode)
            {
                if (_planeManager.trackables.count > 0)
                {
                    _placeButton.interactable = true; 
                }
                
                return;
            }

            if (_input.TryGetTap(out Vector2 position))
            {
                Ray ray = Camera.main.ScreenPointToRay(position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Debug.Log("[ARPlacementManager] Hit something: " + hit.transform.name);
                    
                    if (hit.transform.TryGetComponent<ModelSelectable>(out var selectable))
                    {
                        selectable.HandleSelection();
                    }
                    return;
                }

                if (_raycastManager.Raycast(position, hits, TrackableType.PlaneWithinPolygon))
                {
                    var hitPose = hits[0].pose;
                    TryPlaceModel(hitPose.position, hitPose.rotation);
                }
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