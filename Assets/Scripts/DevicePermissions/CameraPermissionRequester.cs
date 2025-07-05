using UnityEngine;
using UnityEngine.Android;

namespace DevicePermissions
{
    public class CameraPermissionRequester : MonoBehaviour
    {
        private void Start()
        {
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }
#endif
        }
    }
}