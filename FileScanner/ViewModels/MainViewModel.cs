using FileScanner.Commands;
using FileScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;


//référence du datagrid / datatemplate pour le xaml https://www.codeproject.com/Questions/204314/How-do-I-add-an-image-and-data-to-a-WPF-Data-Grid + lab de l'hiver passé

namespace FileScanner.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string selectedFolder;
        private ObservableCollection<string> folderItems = new ObservableCollection<string>();
        private ObservableCollection<Files> itemsList = new ObservableCollection<Files>();

        public DelegateCommand<string> OpenFolderCommand { get; private set; }
        public DelegateCommand<string> ScanFolderCommand { get; private set; }

        public ObservableCollection<string> FolderItems { 
            get => folderItems;
            set 
            { 
                folderItems = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Files> ItemsList
        {
            get => itemsList;
            set
            {
                itemsList = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFolder
        {
            get => selectedFolder;
            set
            {
                selectedFolder = value;
                OnPropertyChanged();
                ScanFolderCommand.RaiseCanExecuteChanged();
            }
        }

        public MainViewModel()
        {
            OpenFolderCommand = new DelegateCommand<string>(OpenFolder);
            ScanFolderCommand = new DelegateCommand<string>(ScanFolderAsync, CanExecuteScanFolder);
        }

        private bool CanExecuteScanFolder(string obj)
        {
            return !string.IsNullOrEmpty(SelectedFolder);
        }

        private void OpenFolder(string obj)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SelectedFolder = fbd.SelectedPath;
                }
            }
        }

        //références
        //https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-enumerate-directories-and-files
        //https://docs.microsoft.com/fr-fr/dotnet/csharp/programming-guide/concepts/async/
        //https://docs.microsoft.com/en-us/dotnet/standard/exceptions/how-to-use-the-try-catch-block-to-catch-exceptions

        private async void ScanFolderAsync(string dir) 
        {
            ItemsList.Clear(); //vider la liste entre 2 scans
            await Task.Run(() =>
            {
                FolderItems = new ObservableCollection<string>(GetDirFiles(dir));
                try 
                {//try


                    foreach (var folder in Directory.EnumerateDirectories(dir, "*", SearchOption.AllDirectories))
                    {//fe1
                        var newFolder = new Files(folder, "/images/folder.png", ""); //https://icon-icons.com/
                        //j'ai voulu m'amuser avec l'idendation si folder ou file, j'ai donc mis un icon par colonne... voir xaml)


                        //https://stackoverflow.com/questions/2137769/where-do-i-get-a-thread-safe-collectionview?fbclid=IwAR1NAWrwFGoLUq9wkZCgdUL-2qSKdu56-gHTbk1nSwwSe2xN05slv-as2-g
                        System.Windows.Application.Current.Dispatcher.BeginInvoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        (Action)delegate ()
                        {
                            ItemsList.Add(newFolder);
                        });
                        /*
                         https://docs.microsoft.com/en-us/dotnet/api/system.io.directory.enumeratefiles?view=netcore-3.1
                         En placant le foeach avec le folder en cours, ca nous permet de lister les fichiers sous chaque dossiers qui les contient, au lieu de tous
                         les dossiers et ensuite tous les fichiers
                         
                         */
                        foreach (var files in Directory.EnumerateFiles(folder, "*"))
                        {//fe2
                            var newFiles = new Files(files, "", "/images/file.png"); //https://icon-icons.com/
                             System.Windows.Application.Current.Dispatcher.BeginInvoke(
                             System.Windows.Threading.DispatcherPriority.Normal,
                             (Action)delegate ()
                             {
                                 ItemsList.Add(newFiles);
                             });
                        }//fe2
                    }//fe1

                }//try
                catch (System.UnauthorizedAccessException)
                {

                    System.Windows.MessageBox.Show("Vous n'avez pas les accès nécessaires pour scanner tout cet emplacement" );
                }
            });
        }

            IEnumerable<string> GetDirFiles(string dir)//envoi file et dir
        {
            foreach (var folder in Directory.EnumerateDirectories(dir, "*"/*, SearchOption.AllDirectories*/))
            {
                 yield return folder;            
            }
        }
    }
}
