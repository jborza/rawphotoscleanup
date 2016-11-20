namespace rawphotoscleanup.UserSettings
{
    class LastSelectedPathManager : ILastSelectedPathManager
    {
        public string LastSelectedPath
        {
            get
            {
                return Properties.Settings.Default.LastPath;
            }

            set
            {
                if (LastSelectedPath == value)
                    return;
                Properties.Settings.Default.LastPath = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
