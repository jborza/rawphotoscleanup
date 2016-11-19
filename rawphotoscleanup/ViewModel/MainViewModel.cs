using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using rawphotoscleanup.ImageProcessing;

namespace rawphotoscleanup.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            fileItems = new ObservableCollection<ViewModel.FileItem>();
        }

        private string lastDirectory;
        public int Maximum { get { return FileItems.Count; } }
        public int Progress => CurrentImageIndex + 1;

        public string CurrentFileName { get { return FileItems[CurrentImageIndex].FullName; } }
        public string SelectedMessage
        {
            get
            {
                var selected = fileItems.Count(p => p.IsChecked);
                return $"{selected} of {ImageCount} selected";
            }
        }

        public int ImageCount => FileItems.Count;
        public string CurrentProgressMessage { get { return $"{CurrentImageIndex + 1} / {ImageCount}"; } }

        private ObservableCollection<FileItem> fileItems;
        public ObservableCollection<FileItem> FileItems
        {
            get { return fileItems; }
        }

        public void LoadDirectory(string directoryName)
        {
            lastDirectory = directoryName;
            const string EXTENSION = "*.nef";
            var files = Directory.GetFiles(directoryName, EXTENSION);
            var items = files.Select(name => new FileItem(name));
            fileItems.Clear();
            foreach (var item in items)
                fileItems.Add(item);
            CurrentImageIndex = 0;
            RaisePropertyChanged(() => Maximum);
            RaisePropertyChanged(() => SelectedMessage);
        }

        internal void RefreshDirectory()
        {
            LoadDirectory(lastDirectory);
        }

        private int currentImageIndex;
        public int CurrentImageIndex
        {
            get { return currentImageIndex; }
            set
            {
                Set<int>(ref currentImageIndex, value);
                RaisePropertyChanged(() => CurrentItem);
                RaisePropertyChanged(() => CurrentFileName);
                RaisePropertyChanged(() => CurrentProgressMessage);
                RaisePropertyChanged(() => Progress);
                LoadImage();
            }
        }

        private void LoadImage()
        {
            var bitmapImage = BitmapImageTools.GetBitmapImage(CurrentItem.FullName, Properties.Settings.Default.DcrawExeName);
            ImageSource = bitmapImage;
        }

        public void SelectPressed()
        {
            CurrentItem.ToggleChecked();
            RaisePropertyChanged(() => SelectedMessage);
        }

        public FileItem CurrentItem => FileItems[CurrentImageIndex];

        internal void RightPressed()
        {
            if (CurrentImageIndex < ImageCount - 1)
                CurrentImageIndex++;
        }

        internal void LeftPressed()
        {
            if (CurrentImageIndex > 0)
                CurrentImageIndex--;
        }

        private ImageSource imageSource;
        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                Set(ref imageSource, value);
            }
        }

       
    }
}