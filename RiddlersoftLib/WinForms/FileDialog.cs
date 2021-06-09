using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace WinForms
{
    public enum FileSystemType { Folder, File}

    public struct FileSystem
    {
        public FileSystemType Type;
        public string Name;
        public string Path;

        public FileSystem(FileSystemType type, string name, string path)
        {
            Type = type;
            Name = name;
            Path = path;
        }
    }

    public enum FileDialogType { Save, Load, Picker}
   

    public class FileDialog : Form
    {
        private FileDialogType _type = FileDialogType.Load;
        public static string DefaultDirectory = ".\\";

        private string _selectedFile = null;

        public List<string> AllowedFileExtentions = new List<string>() { "*.*" };

        /// <summary>
        /// this even first when the users clicks on a file
        /// </summary>
        /// <param name="filepath">the path of the selected file</param>
        /// <param name="isSaveDialog">if this is a save or open dialog</param>
        public delegate void _onFileSelected(string filepath, FileDialogType fdType);
        public _onFileSelected OnFileSelected;

        public FileDialog(Rectangle area, Componate parentcomponate) : base(area, parentcomponate)
        {
            Visible = false;
            Active = false;
            
            AddComponate(new ListBox(new Rectangle(1, 11, 98, 77), this, null)
            {
                Units = MesurementUnit.Percentage,
                id = "filelist",
                OnItemSelected = FileList_OnItemClicked
            });

            AddComponate(new Button(new Rectangle(85, 89, 14, 10), this)
            {
                Units = MesurementUnit.Percentage,
                id = "ok",
                Text = "Load",
                OnLeftClick = Bnt_On_Ok_Clicked,
            });

            AddComponate(new Button(new Rectangle(69, 89, 14, 10), this)
            {
                Units = MesurementUnit.Percentage,
                id = "cancel",
                Text = "Cancel",
                CloseParent = true,
            });

            AddComponate(new TextBox(new Rectangle(1, 89, 66, 10), this)
            {
                Units = MesurementUnit.Percentage,
                id = "txtfile",
                Text = "",
                OnKeyPressed = Txt_On_Key_Pressed,
            });

        }

        public override bool OnLeftMouseClick(Point p)
        {
            return base.OnLeftMouseClick(p);
                    }

        public void Txt_On_Key_Pressed(object componate)
        {
            TextBox tb = componate as TextBox;
            if (tb.Text.Length == 0)
                FindComponateById("ok").Active = false;
            else
            {
                Button bnt = FindComponateById("ok") as Button;
                bnt.Active = true;
            }

            _selectedFile = CurrentDirectory + "\\" + tb.Text;
            if (AllowedFileExtentions.Count == 1)
                if (!_selectedFile.Contains(AllowedFileExtentions[0]))
                    _selectedFile += AllowedFileExtentions[0];

        }

        private void Bnt_On_Ok_Clicked(object sender, Point p)
        {
            TextBox tb = (TextBox)this.FindComponateById("txtfile");
            if (_selectedFile == null || _selectedFile == string.Empty)
                if (tb.Text.Length == 0)
                    return;
                else
                    _selectedFile = CurrentDirectory + "\\" + tb.Text;

            if (_type == FileDialogType.Load || _type == FileDialogType.Picker) //if load dialog box
            {
                //now make sure that the file exists
                if (!File.Exists(_selectedFile)) 
                {
                    Controler.MessageBox.ShowErrorDialog("File Not Found", "The file does not exist");
                    return; //exit because file does not exits
                }
            }

            if (OnFileSelected != null)
                OnFileSelected(_selectedFile, _type);

            if (_type == FileDialogType.Picker)
                if (_returnFunction != null)
                    _returnFunction(this, _selectedFile);

            DeActivate();
        }

        private void FileList_OnItemClicked(ListBox.ListBoxItem item, int selection)
        {
            TextBox tb = (TextBox)this.FindComponateById("txtfile");
            Button bntOk = (Button)this.FindComponateById("ok");


            if (_fileSystemEntrys[selection].Name == "..\\") //if go up one directory
            {
                string tmp = CurrentDirectory;
                if (tmp.Length < 3)
                    return;
                int ld = tmp.LastIndexOf("\\");
                if (ld == -1)
                    return;
                if (ld <= 2)
                    tmp = tmp.Substring(0, 3);
                else
                    tmp = tmp.Substring(0, ld);
                BuildDirectoryListing(tmp);
                tb.Text = string.Empty;
                _selectedFile = null;
                bntOk.Active = false;
                return;
            }

            if (_fileSystemEntrys[selection].Type == FileSystemType.Folder) //if folder
            {
                BuildDirectoryListing(_fileSystemEntrys[selection].Path);
                tb.Text = string.Empty;
                _selectedFile = null;
                bntOk.Active = false;
            }
            else
            { //if file
                tb.Text = _fileSystemEntrys[selection].Name;
                _selectedFile = _fileSystemEntrys[selection].Path;
                bntOk.Active = true;
             //   DeActivate(); //close dialog
            }
        }

        private List<string> _directoryHistory = new List<string>();

        private List<FileSystem> _fileSystemEntrys = new List<FileSystem>();
        public string CurrentDirectory { get; set; } = null;

        private void BuildDirectoryListing(string currentDirectory)
        {
         
            CurrentDirectory = currentDirectory;
            Title = currentDirectory;
            ListBox lb = FindComponateById("filelist") as ListBox;
            lb.Font = Font;
            lb.ClearList();
            
            string[] files = Directory.GetFiles(currentDirectory);
            string[] directorys = Directory.GetDirectories(currentDirectory);
            
            _fileSystemEntrys.Clear();
            _fileSystemEntrys.Add(new FileSystem(FileSystemType.Folder, "..\\", "..\\"));
            string name;
            foreach (string str in directorys)
            {
                name = str.Replace(currentDirectory, string.Empty);
                _fileSystemEntrys.Add(new FileSystem(FileSystemType.Folder,".\\" + name, str));
            }
            
            foreach (string str in files)
            {
                name = str.Replace(currentDirectory, string.Empty);
                name = name.Replace("\\", string.Empty);

                bool add = false;
                if (AllowedFileExtentions.Count == 0 || AllowedFileExtentions.Contains("*.*"))
                    add = true;
                else
                {
                    foreach (string ext in AllowedFileExtentions)
                    {
                       
                        if (str.Contains(ext.Replace("*", "")))
                        {
                            add = true;
                            break;
                        }
                    }
                }
                if (add)
                    _fileSystemEntrys.Add(new FileSystem(FileSystemType.File, name, str));
            }
          
            foreach (FileSystem fs in _fileSystemEntrys)
                lb.AddListItem(fs.Name);
            
        }

        private Action<Componate, string> _returnFunction;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isfocuse">if its in focuse or not</param>
        /// <param name="data">true for save dialog false for load dialog</param>
        public override void Activate(bool isfocuse, object data)
        {
            _type = (FileDialogType)data;
            
            Button bnt = FindComponateById("ok") as Button;
            switch (_type)
            {
                case FileDialogType.Load:
                    bnt.Text = "Load";
                    FindComponateById("txtfile").Active = false;
                    FindComponateById("txtfile").Visible = true;
                    break;

                case FileDialogType.Picker:
                    FindComponateById("txtfile").Visible = false;
                    FindComponateById("txtfile").Active = false;
                    break;

                case FileDialogType.Save:
                    bnt.Text = "Save";
                    FindComponateById("txtfile").Active = true;
                    FindComponateById("txtfile").Visible = true;
                    break;
            }
          

            if (CurrentDirectory == null)
                CurrentDirectory = DefaultDirectory;
            DirectoryInfo parentInfo = Directory.GetParent(CurrentDirectory);
            BuildDirectoryListing(parentInfo.FullName);
            
            base.Activate(isfocuse);
        }


        protected override void Render(ref SpriteBatch sb)
        {
            
            base.Render(ref sb);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isfocuse">if its in focuse or not</param>
        public void ShowSaveDialog(bool isfocuse, string currentFile)
        {
          
            if (currentFile == null)
                currentFile = string.Empty;

            TextBox tb = FindComponateById("txtfile") as TextBox;
            tb.Text = currentFile;
            tb.Visible = true;
            tb.Active = true;

            if (tb.Text == string.Empty)
                FindComponateById("ok").Active = false;
            else
                FindComponateById("ok").Active = true;

            _type = FileDialogType.Save;
            

            Button bnt = FindComponateById("ok") as Button;
            bnt.Text = "Save";


            BuildDirectoryTree();

            base.Activate(isfocuse);
        }

        private void BuildDirectoryTree()
        {
           
            if (CurrentDirectory == null)
                CurrentDirectory = DefaultDirectory;
            if (CurrentDirectory.Contains(":\\"))
            {
                BuildDirectoryListing(CurrentDirectory);
            }
            else
            {
                DirectoryInfo parentInfo = Directory.GetParent(CurrentDirectory);
                if (CurrentDirectory == ".\\")
                    CurrentDirectory = string.Empty;
                BuildDirectoryListing(parentInfo.FullName + CurrentDirectory);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isfocuse">if its in focuse or not</param>
        public void ShowLoadDialog(bool isfocuse, string currentFile)
        {
            if (currentFile == null)
                currentFile = string.Empty;

            TextBox tb = (TextBox)this.FindComponateById("txtfile");
            tb.Text = currentFile;
            tb.Visible = true;

            _type = FileDialogType.Load;
            Button bnt = FindComponateById("ok") as Button;
            bnt.Text = "Load";

            BuildDirectoryTree();

            base.Activate(isfocuse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isfocuse">if its in focuse or not</param>
        public void ShowFilePicker(bool isfocuse, string currentFile, Action<Componate, string> returnFunction = null)
        {
            _returnFunction = returnFunction;
            if (currentFile == null)
                currentFile = string.Empty;

            TextBox tb = (TextBox)this.FindComponateById("txtfile");
            tb.Visible = false;
            tb.Active = false;
            _type = FileDialogType.Picker;
            Button bnt = FindComponateById("ok") as Button;
            bnt.Text = "Select";

            BuildDirectoryTree();

            base.Activate(isfocuse);
        }
    }
}
