using System;

namespace gMKVToolNix
{
    public static class PlatformExtensions
    {
        private static bool? _isOnLinux = null;

        /// <summary>
        /// Returns if the running Platform is Linux Or MacOSX
        /// </summary>
        public static bool IsOnLinux
        {
            get
            {
                if (_isOnLinux.HasValue)
                {
                    return _isOnLinux.Value;
                }

                PlatformID myPlatform = Environment.OSVersion.Platform;

                // 128 is Mono 1.x specific value for Linux systems, so it's there to provide compatibility
                _isOnLinux = (myPlatform == PlatformID.Unix) || (myPlatform == PlatformID.MacOSX) || ((int)myPlatform == 128);

                return _isOnLinux.Value;
            }
        }
    }
}
