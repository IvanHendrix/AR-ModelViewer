using System;

namespace Utility
{
    public static class URLValidator
    {
        public static bool IsValidGLB(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute) && url.EndsWith(".glb");
        }
    }
}