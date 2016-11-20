using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rawphotoscleanup
{
    class FolderBrowserService : IFolderBrowserService
    {
        public bool ShowDialog(string lastPath, out string selectedPath)
        {
            selectedPath = null;
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.SelectedPath = lastPath;
            if (fd.ShowDialog() != DialogResult.OK)
                return false;
            selectedPath = fd.SelectedPath;
            return true;
        }
    }
}
