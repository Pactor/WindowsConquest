using System.IO;

namespace WindowsConquest.Data
{
    public static class GameConfig
    {
        public static readonly string GameDataPath =
            Path.Combine(Directory.GetCurrentDirectory(), "gamedata");

        public static readonly string AssetsPath =
            Path.Combine(Directory.GetCurrentDirectory(), "assets");

        public static string GetGameDataFile(string fileName)
        {
            return Path.Combine(GameDataPath, fileName);
        }

        public static string GetAssetFile(string relativePath)
        {
            return Path.Combine(AssetsPath, relativePath);
        }
    }
}
