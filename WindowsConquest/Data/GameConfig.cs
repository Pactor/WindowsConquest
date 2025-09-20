using System.IO;

namespace WindowsConquest.Data
{
    public static class GameConfig
    {
        private static string BaseDir
        {
            get
            {
                // exe runs in bin/Debug, so go up 2 levels to project root
                var dir = Directory.GetCurrentDirectory();
                return Path.GetFullPath(Path.Combine(dir, @"..\..\"));
            }
        }

        public static readonly string GameDataPath =
            Path.Combine(BaseDir, "gamedata");

        public static readonly string AssetsPath =
            Path.Combine(BaseDir, "assets");

        public static string GetGameDataFile(string fileName)
        {
            return Path.Combine(GameDataPath, fileName);
        }

        public static string GetAssetFile(string relativePath)
        {
            return Path.Combine(AssetsPath, relativePath.Replace("/", "\\"));
        }
    }
}
