namespace Settings
{
    [System.Serializable]
    public class SettingsData
    {
        public int gameWidth;
        public int gameHeight;
        public int clientWidth;
        public int clientHeight;

        public SettingsData(SettingManager settings)
        {
            gameWidth = settings.gameWidth;
            gameHeight = settings.gameHeight;
            clientWidth = settings.clientWidth;
            clientHeight = settings.clientHeight;
        }
    }
}