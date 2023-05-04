using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Linq;

public static class ScenesInBuildEditor
{
    const string SCENES_FOLDER_PATH = "Assets/Scenes";
    const string FIRST_SCENE_FILE_NAME = "Sample01Scene.unity";

    [MenuItem("Tools/InitializeScenesInBuild")]
    private static void InitializedScenesList()
    {
        InitializeEditorBuildSettingsScenes();
    }

    private static void InitializeEditorBuildSettingsScenes()
    {
        List<EditorBuildSettingsScene> scenesList = new List<EditorBuildSettingsScene>();

        List<string> filePathList = Directory
            .GetFiles(SCENES_FOLDER_PATH, "*.unity", SearchOption.AllDirectories).ToList();

        for (int i = 0; i < filePathList.Count; i++)
        {
            var scenePath = filePathList[i];
            if (Path.GetFileName(scenePath) == FIRST_SCENE_FILE_NAME)
            {
                filePathList.Remove(scenePath);
                filePathList.Insert(0, scenePath);
                scenesList.Insert(0, new EditorBuildSettingsScene(scenePath, true));
            }
            else
            {
                scenesList.Add(new EditorBuildSettingsScene(scenePath, true));
            }
        }

        EditorBuildSettings.scenes = scenesList.ToArray();
    }
}