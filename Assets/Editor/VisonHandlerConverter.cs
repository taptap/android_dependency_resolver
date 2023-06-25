using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TapTap.Internal.Editor
{
    public class VisonHandlerConverter
    {
        private const string EDM4U_LABEL = "gvh";
        private const string EDM4U_VERSION_LABEL = "gvh_version-";
        private const string EDM4U_PATH_LABEL = "gvhp-exportPath-";
        private const string EDM4U_MANIFEST_LABEL = "gvh_manifest";
        private const string EDM4U_EDITOR_DLL_LABEL = "gvh-targets-editor";

        [MenuItem("TapTap/VersionHandler/Refresh")]
        public static void Refresh()
        {
            string pluginName = "TapTap-Android-Dependency-Resolver";
            string rootFolder = "Assets/TapTap/AndroidDependencyResolver";
            string version = "100.0.9";
            ConvertToVersionHandler(pluginName, rootFolder, version);
        }
        public static void ConvertToVersionHandler(string pluginName, string rootFolder, string version)
        {
            // delete old manifest file
            var manifestUnityPath = $"{rootFolder}/{pluginName}_{version}.txt";
            var rootFolderAbsolutePath = UnityPath2AbsolutePath($"{rootFolder}");
            StringBuilder manifestContent = new StringBuilder();
            var txtFiles = Directory.GetFiles(rootFolderAbsolutePath, "*.txt");
            var manifestFilePath = UnityPath2AbsolutePath(manifestUnityPath);
            if (txtFiles != null)
            {
                foreach (var txtFile in txtFiles)
                {
                    if (txtFile == manifestFilePath) continue;
                    File.Delete(txtFile);
                    File.Delete($"{txtFile}.meta");
                }
            }
            
            // 1. 更改所有文件夹的 meta 信息
            var allFilesPath = GetAllFiles(rootFolder);
            foreach (var filePath in allFilesPath)
            {
                UpdateFileMetaInfo(filePath, version);
                manifestContent.AppendLine(filePath);
                
            }
            // 2. 生成 VersionHandler 需要的 manifest 文件
            if (!File.Exists(manifestFilePath))
            {
                // create new manifest file
                File.Create(manifestFilePath);
                manifestContent.AppendLine(manifestUnityPath);
                AssetDatabase.Refresh();
            }
            else
            {
                // clear manifest file content
                File.WriteAllText(manifestFilePath, "");
            }
            
            
            // write content to manifest file which path is manifestFilePath
            File.WriteAllText(manifestFilePath, manifestContent.ToString());
            UpdateFileMetaInfo(manifestUnityPath, version, true);

        }

        private static string UnityPath2AbsolutePath(string unityPath)
        {
            // convert relative path to absolute path
            return Application.dataPath.Replace("Assets", unityPath);
        }
        
        private static string AbsolutePath2UnityPath(string absolutePath)
        {
            // convert absolute path to relative path
            return absolutePath.Replace(Application.dataPath, "Assets");
        }
        
        private static List<string> GetAllFiles(string rootFolder)
        {
            //check rootFolder is a valid folder under Unity Assets folder
            if (!rootFolder.StartsWith("Assets/"))
            {
                return null;
            }
            // get all files except meta files and subfolder under rootFolder path
            string[] allFiles = AssetDatabase.FindAssets("", new[] { rootFolder });
            List<string> result = new List<string>();
            foreach (string guid in allFiles)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                result.Add(path);
            }
            return result;
        }

        private static void UpdateFileMetaInfo(string filePath, string version, bool isManifest = false)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(filePath);
            if (asset == null) return;
            List<string> labels = new List<string>();
            labels.Add(EDM4U_LABEL);
            labels.Add($"{EDM4U_VERSION_LABEL}{version}");
            filePath = filePath.TrimStart("Assets/".ToArray());
            labels.Add($"{EDM4U_PATH_LABEL}{filePath}");
            // get AssetImporter from filePath
            var pluginImporter = AssetImporter.GetAtPath(filePath) as PluginImporter;
            if (pluginImporter != null &&
                (pluginImporter.GetCompatibleWithAnyPlatform() || pluginImporter.GetCompatibleWithEditor()))
            {
                labels.Add(EDM4U_EDITOR_DLL_LABEL);
            }

            if (isManifest)
            {
                labels.Add(EDM4U_MANIFEST_LABEL);
            }
            AssetDatabase.SetLabels(asset, labels.ToArray());
        }
    }
}