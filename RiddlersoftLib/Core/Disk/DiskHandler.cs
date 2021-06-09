
using System;

using Microsoft.Xna.Framework;

using System.IO;
using System.Runtime.Serialization;

namespace Riddlersoft.Core.Disk
{
    
    public class DiskHandler: GameComponent
    {
        /// <summary>
        /// if the disk hander has been initalized or not
        /// </summary>
        public static bool Initalized { get; private set; } = false;
        public static GameComponent Gc { get; private set; }

        public DiskHandler(Game game) : base(game)
        {
            if (Initalized)
                throw new Exception("You can only have one instance of the disk hander running");
            
            Initalized = true;
            Gc = this;
        }
        
        private string SetUpPath(FileFormat file, string loc)
        {
            if (!RDirectory.Exists(file.Path))
                RDirectory.CreateDirectory(file.Path);

            if (!loc.Contains(file.Extention))
                loc = $"{loc}.{file.Extention}";
            if (!loc.Contains(file.Path))
                loc = $"{file.Path}\\{loc}";
            return loc;
        }

        public bool Save(FileFormat file, string location)
        {
            if (!Initalized)
                throw new Exception("You must initalized the disk hander to be able to save");
            location = SetUpPath(file, location);
            try
            {
                using (Stream disk = RFile.Create(location))
                {

                    DataContractSerializer writer = new DataContractSerializer(file.GetType());
                    writer.WriteObject(disk, file);
                }
                return true;
            }
            catch (Exception x)
            {
                return false;
            }
        }

        public object Load(FileFormat file, string location)
        {
            if (!Initalized)
                throw new Exception("You must initalized the disk hander to be able to load");

            location = SetUpPath(file, location);
            try
            {
                using (Stream disk = RFile.OpenRead(location))
                {
                    Type t = file.GetType();
                    DataContractSerializer writer = new DataContractSerializer(file.GetType());
                    return writer.ReadObject(disk);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
