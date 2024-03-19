namespace Settings
{
    [System.Serializable]
    public class SettingsData
    {
        public int gameWidth;
        public int gameHeight;
        public int clientWidth;
        public int clientHeight;
        public float music;
        public float sound;
        public int gameResolutionDropDownValue;
        public int clientResolutionDropDownValue;

        public SettingsData(SettingManager settings)
        {
            gameWidth = settings.gameWidth;
            gameHeight = settings.gameHeight;
            clientWidth = settings.clientWidth;
            clientHeight = settings.clientHeight;
            music = settings.music;
            sound = settings.sound;
            gameResolutionDropDownValue = settings.gameResolutionDropDownValue;
            clientResolutionDropDownValue = settings.clientResolutionDropDownValue;
        }
    }
}