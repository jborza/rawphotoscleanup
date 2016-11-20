using System.Windows;

namespace rawphotoscleanup
{
    class WpfMessageService : IMessageBoxService
    {
        public bool ShowMessageYesNo(string text, string caption)
        {
            return MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}
