using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace Riddlersoft.Core.Storage
{
    public static class UDirectory
    {
        public static DirectoryInfo Create(string path)
        {
            path = UFile.PlatformSpeciffAccess(path);
            return Directory.CreateDirectory(path);
        }

        public static string[] GetFiles(string path)
        {
            path = UFile.PlatformSpeciffAccess(path);
            return Directory.GetFiles(path);
        }
        public static string[] GetFiles(string path, string searchPatten)
        {
            path = UFile.PlatformSpeciffAccess(path);
            return Directory.GetFiles(path, searchPatten);
        }
        public static string[] GetFiles(string path, string searchPatten, SearchOption searchOption)
        {
            path = UFile.PlatformSpeciffAccess(path);
            return Directory.GetFiles(path, searchPatten, searchOption);
        }

    }
}
