using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace rawphotoscleanup.ViewModel
{
    class MainWindowViewModel : ObservableObject
    {
        private const string EXTENSION = "*.nef";
        public MainWindowViewModel()
        {
            fileItems = new ObservableCollection<FileItem>();
        }

        private ObservableCollection<FileItem> fileItems;
        public ObservableCollection<FileItem> FileItems
        {
            get { return fileItems; }
        }

        public void LoadDirectory(string directoryName)
        {
            var files = Directory.GetFiles(directoryName, EXTENSION);
            var items = files.Select(name => new FileItem(name));
            fileItems.Clear();
            foreach (var item in items)
                fileItems.Add(item);
        }
    }

    class FileItem : ObservableObject
    {
        public FileItem(string name)
        {
            this.Name = name;
            this.isChecked = false;
        }

        public string Name { get; }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                Set<bool>(() => this.IsChecked, ref isChecked, value);
            }
        }
    }
}
