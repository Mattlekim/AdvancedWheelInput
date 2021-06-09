using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

#if SWITCH
using Riddlersoft.Core.Switch;
#endif

namespace Riddlersoft.Core.Storage
{
    /// <summary>
    /// stands for universal file.
    /// This will automaticly mount a file system if required on switch
    /// it will also work for all platforms
    /// </summary>
    public class UFile
    {
        public static bool AutoUnmountAfterCommit = true;
        public static bool AutoMountOnFileAccess = true;

        private static bool _HaveInitializedStorage = false;
        public static void MountStorage()
        {
#if SWITCH
            if (!_HaveInitializedStorage)
            {
                NSwitch.Init();
                _HaveInitializedStorage = true;
            }

            NSwitch.MountRom();
#endif
        }

        internal static string PlatformSpeciffAccess(string path)
        {
            if (AutoMountOnFileAccess)
                MountStorage();
#if SWITCH
            return $"{NSwitch.SavePath}{path}";
#endif

            return path;
        }

        public static bool Exits(string path)
        {
            path = PlatformSpeciffAccess(path);
            return File.Exists(path);
        }

        public static FileStream Create(string path)
        {
            path = PlatformSpeciffAccess(path);
            return File.Create(path);
        }

        public static FileStream Open(string path, FileMode mode)
        {
            path = PlatformSpeciffAccess(path);
            return File.Open(path, mode);
        }

        public static void Commit()
        {
#if SWITCH
            NSwitch.CommitSaveData();
#endif
            if (AutoUnmountAfterCommit)
                UnMountStorage();
        }

        public static void UnMountStorage()
        {
#if SWITCH
            NSwitch.UnmountRom();
#endif

        }

    }
}
