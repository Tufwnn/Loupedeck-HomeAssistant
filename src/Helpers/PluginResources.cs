namespace Loupedeck.HomeAssistantPlugin
{
    using System;
    using System.IO;
    using System.Reflection;

    internal static class PluginResources
    {
        private static Assembly _assembly;

        public static void Init(Assembly assembly)
        {
            assembly.CheckNullArgument(nameof(assembly));
            _assembly = assembly;
        }

        public static String[] GetFilesInFolder(String folderName) => _assembly.GetFilesInFolder(folderName);
        public static String FindFile(String fileName) => _assembly.FindFileOrThrow(fileName);
        public static String[] FindFiles(String regexPattern) => _assembly.FindFiles(regexPattern);
        public static Stream GetStream(String resourceName) => _assembly.GetStream(FindFile(resourceName));
        public static String ReadTextFile(String resourceName) => _assembly.ReadTextFile(FindFile(resourceName));
        public static Byte[] ReadBinaryFile(String resourceName) => _assembly.ReadBinaryFile(FindFile(resourceName));
        public static BitmapImage ReadImage(String resourceName) => _assembly.ReadImage(FindFile(resourceName));

        public static void ExtractFile(String resourceName, String filePathName)
            => _assembly.ExtractFile(FindFile(resourceName), filePathName);
    }
}
