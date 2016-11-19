using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace rawphotoscleanup.ImageProcessing
{
    class BitmapImageTools
    {
        public static BitmapImage GetBitmapImage(string inputFile, string dcRawExe)
        {
            var startInfo = new ProcessStartInfo(dcRawExe)
            {
                Arguments = $"-c -e \"{inputFile}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var process = Process.Start(startInfo);
            var stream = process.StandardOutput.BaseStream;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            return image;
        }
    }
}
