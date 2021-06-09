#define WINDOWS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

#if WINDOWS_UWP
using Windows.Storage;
using System.Threading;
#endif

namespace Riddlersoft.Core.Disk
{
    public class RFile: RStructor
    {
        public static bool Exists(string filename)
        {
            filename = Valadate(filename);
#if WINDOWS || WINDOWS_UWP
            return File.Exists(filename);
#elif WINDOWS_UWP
            bool exist = false;
            bool ready = false;
            ManualResetEvent mre = new ManualResetEvent(false);
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                    exist = true;
                }
                catch { }
                ready = true;
            });
            while(!ready)
            {

            }

            return exist;
#endif
        }

        public static Stream Create(string filename)
        {
            filename = Valadate(filename);
#if WINDOWS || WINDOWS_UWP
            return File.Create(filename);
#elif WINDOWS_UWP
            ManualResetEvent mre = new ManualResetEvent(false);
            Exception ex = null;
            Stream fs = null;
            bool ready = false;
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                    fs = await storageFile.OpenStreamForWriteAsync();
                    
                }
                catch (Exception x)
                {
                    ex = x;
                }
                ready = true;
            });
            while (!ready)
            {
            }
            if (ex != null)
                throw ex;
            return fs;
#endif

        }

        public static Stream OpenRead(string filename)
        {
            filename = Valadate(filename);
#if WINDOWS || WINDOWS_UWP
            return File.OpenRead(filename);
#elif WINDOWS_UWP
            return File.OpenRead(filename);
            Exception ex = null;
            Stream fs = null;
            bool ready = false;
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    fs = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filename);
                }
                catch (Exception x)
                {
                    ex = x;
                }
                ready = true; 
            });

            while (!ready)
            {
            }
            if (ex != null)
                throw ex;
            return fs;
#endif

        }

        public static async Task<Stream> OpenReadAsync(string filename)
        {
            filename = Valadate(filename);
#if WINDOWS
            return File.OpenRead(filename);
#elif WINDOWS_UWP
            Exception ex = null;
            Stream fs = null;
            try
            {
                fs = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filename);
            }
            catch (Exception x)
            {
                ex = x;
            }
            if (ex != null)
                throw ex;
            return fs;
#endif

        }
    }
}
