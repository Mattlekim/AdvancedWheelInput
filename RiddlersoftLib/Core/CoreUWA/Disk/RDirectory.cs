using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS
using System.IO;
#elif WINDOWS_UWP
using Windows.Storage;
using System.Threading;
#endif
namespace CoreUWA.Disk
{
    public class RDirectory: RStructor
    {
      

        public static bool Exists(string path)
        {
            path = Valadate(path);
#if WINDOWS
            return Directory.Exists(path);
#elif WINDOWS_UWP
            bool exist = false;
            bool ready = false;
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await ApplicationData.Current.LocalFolder.GetFolderAsync(path);
                    exist = true;
                }
                catch { }
                ready = true;
            });
            while (!ready)
            { }
            
            return exist;
#endif
        }

        public static void CreateDirectory(string path)
        {
            path = Valadate(path);
#if WINDOWS
            Directory.CreateDirectory(path);
#elif WINDOWS_UWP
            bool ready = false;
            Exception ex = null;
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await ApplicationData.Current.LocalFolder.CreateFolderAsync(path);
                }
                catch (Exception x)
                {
                    ex = x;
                }
                ready = true;
            });
            while (!ready)
            { }
            if (ex != null)
                throw ex;
#endif
        }

    }
}
