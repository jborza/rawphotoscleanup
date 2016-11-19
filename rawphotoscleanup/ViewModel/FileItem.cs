using GalaSoft.MvvmLight;
using System.IO;
using System;

namespace rawphotoscleanup.ViewModel
{
    public class FileItem : ObservableObject
    {
        public FileItem(string name)
        {
            var fi = new FileInfo(name);
            this.FullName = fi.FullName;
            this.Name = fi.Name;
            this.isChecked = false;
        }

        public string FullName { get; }
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

        internal void ToggleChecked()
        {
            IsChecked = !IsChecked;
        }
    }
}
