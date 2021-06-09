using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Runtime.Serialization;

namespace Riddlersoft.Core.Disk
{
    /// <summary>
    /// the file format that we will use to save the data
    /// </summary>
    [DataContract]
    public abstract class FileFormat
    {
        public string Path { get; protected set; }
        public string Extention { get; protected set; }

        //the main disk hander
        protected DiskHandler _hander;

        protected void TryAttachHander()
        {
            if (_hander == null)
            {
                if (DiskHandler.Gc == null)
                    throw new Exception("Disk hander must be enabled before any saving or loading takes place");

                _hander = DiskHandler.Gc as DiskHandler; //set the disk handler
            }
        }

        public void Save()
        {
            Save(null);
        }

        public void Load()
        {
            Load(null);
        }

        public async void LoadAsync()
        {

        }

        public void Save(object data)
        {
            TryAttachHander();
            SaveData(data);
        }

        public void Load(object data)
        {
            TryAttachHander();
            LoadData(data);
        }

        protected abstract void SaveData(object data);

        protected abstract void LoadData(object data);

    }
}
