using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rawphotoscleanup
{
    public interface IFolderBrowserService
    {
        bool ShowDialog(string lastPath, out string selectedPath);
    }
}
