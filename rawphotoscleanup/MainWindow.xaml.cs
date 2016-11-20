﻿using Microsoft.Practices.ServiceLocation;
using rawphotoscleanup.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using Microsoft.VisualBasic.FileIO;

namespace rawphotoscleanup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private MainViewModel vm { get { return ServiceLocator.Current.GetInstance<MainViewModel>(); } }

        //as per http://stackoverflow.com/questions/688990/reading-metadata-from-images-in-wpf
        private int GetOrientation(string filename)
        {
            BitmapFrame frame = BitmapFrame.Create(new Uri(filename), BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default);
            var bmData = (BitmapMetadata)frame.Metadata;

            if (bmData != null)
            {
                object val = bmData.GetQuery("/app1/ifd/exif:{uint=274}");
                switch ((ushort)val)
                {
                    case 6:
                        return 90;
                    case 3:
                        return 180;
                    case 8:
                        return 270;
                    default:
                        return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (vm == null)
                return;
            vm.HandleKeyPressed(e.Key);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //open a directory
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //collect all items to delete
            var caption = "Do you want to delete the following items?";
            var filesToDelete = vm.FileItems.Where(p => p.IsChecked);
            var content = string.Join(Environment.NewLine, filesToDelete.Select(p => p.Name));
            if (MessageBox.Show(content, caption, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            foreach (var f in filesToDelete)
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(f.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            vm.RefreshDirectory();
        }
    }
}
