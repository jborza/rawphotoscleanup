using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using rawphotoscleanup.ImageProcessing;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System;
using rawphotoscleanup.UserSettings;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

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
        private ILastSelectedPathManager lastSelectedPathManager;
        private IMessageBoxService messageBoxService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ILastSelectedPathManager lastSelectedPathManager, IMessageBoxService messageBoxService)
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            this.lastSelectedPathManager = lastSelectedPathManager;
            this.messageBoxService = messageBoxService;
            fileItems = new ObservableCollection<ViewModel.FileItem>();
            OpenDirectoryCommand = new RelayCommand(OpenDirectory);
            DeleteFilesCommand = new RelayCommand(DeleteFiles);
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

        internal void HandleKeyPressed(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    LeftPressed();
                    return;
                case Key.Right:
                    RightPressed();
                    return;
                case Key.Home:
                    HomePressed();
                    return;
                case Key.End:
                    EndPressed();
                    return;
                case Key.X:
                    SelectPressed();
                    return;
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

        public FileItem CurrentItem => FileItems[CurrentImageIndex];

        public void SelectPressed()
        {
            CurrentItem.ToggleChecked();
            RaisePropertyChanged(() => SelectedMessage);
        }

        private void EndPressed()
        {
            CurrentImageIndex = ImageCount - 1;
        }

        private void HomePressed()
        {
            CurrentImageIndex = 0;
        }

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

        public ICommand OpenDirectoryCommand { get; private set; }
        public ICommand DeleteFilesCommand { get; private set; }

        private void OpenDirectory()
        {
            FolderBrowserDialog fd = new FolderBrowserDialog()
            {
                SelectedPath = lastSelectedPathManager.LastSelectedPath
            };
            if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            lastSelectedPathManager.LastSelectedPath = fd.SelectedPath;
            LoadDirectory(fd.SelectedPath);
        }

        private void DeleteFiles()
        {
            var caption = "Do you want to delete the following items?";
            var filesToDelete = FileItems.Where(p => p.IsChecked);
            var content = string.Join(Environment.NewLine, filesToDelete.Select(p => p.Name));
            if (messageBoxService.ShowMessageYesNo(content, caption) == false)
                return;
            foreach (var f in filesToDelete)
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(f.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            RefreshDirectory();
        }
    }
}