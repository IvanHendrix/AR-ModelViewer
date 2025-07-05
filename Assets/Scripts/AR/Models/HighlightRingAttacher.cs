using UnityEngine;

namespace AR.Models
{
    public class HighlightRingAttacher : MonoBehaviour
    {
        [SerializeField] private GameObject _ring;

        public void AttachRing(GameObject model)
        {
            if (model == null)
            {
                return;
            }

            _ring.transform.SetParent(null);
            
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                return;
            }

            Bounds bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            Vector3 center = bounds.center;
            Vector3 bottom = new Vector3(center.x, bounds.min.y, center.z);
            _ring.transform.position = bottom;

            float diameter = Mathf.Max(bounds.size.x, bounds.size.z) * 1.1f;
            _ring.transform.localScale = new Vector3(diameter, 0.01f, diameter);

            _ring.SetActive(true);
            _ring.transform.SetParent(model.transform);
        }
    }
}