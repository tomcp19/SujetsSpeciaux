using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace FileScanner.Models
{
    public class Files : INotifyPropertyChanged
    {
        private string filePath;
        private string fileIconPath;
        private string folderIconPath;
        public string FilePath { 
            get => filePath;
            set
            {
                filePath = value;
                OnPropertyChanged();
            } 
        }
        public string FileIconPath
        {
            get => fileIconPath;
            set
            {
                fileIconPath = value;
                OnPropertyChanged();
            }
        }

        public string FolderIconPath
        {
            get => folderIconPath;
            set
            {
                folderIconPath = value;
                OnPropertyChanged();
            }
        }

        public Files(string _filePath, string _folderIconPath, string _fileIconPath)
        {
            filePath = _filePath;
            fileIconPath = _fileIconPath;
            folderIconPath = _folderIconPath;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
