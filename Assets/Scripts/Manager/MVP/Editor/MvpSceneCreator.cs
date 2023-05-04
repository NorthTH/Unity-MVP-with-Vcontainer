using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVP
{
    public class MvpSceneCreator
    {
        [MenuItem("Assets/Create/MVP/Generate MVP Scene", false, 1)]
        public static void CreateNewScene()
        {
            var window = ScriptableObject.CreateInstance<MvpSceneCreatorWindow>();

            window.maxSize = window.minSize = new Vector2(320, 300);
            window.ShowUtility();
        }

        private class MvpSceneCreatorWindow : EditorWindow
        {
            private string _sceneName;
            private int _sceneMode;
            private string _pathName;

            private bool _disabled;

            private string[] _sceneDirectories;

            private int _sceneSelectIndex;

            private void Awake()
            {
                _sceneDirectories = new DirectoryInfo("Assets/Scenes").GetDirectories()
                    .Select(d => d.Name)
                    .Prepend("なし")
                    .ToArray();
            }

            private void OnGUI()
            {
                using (new EditorGUI.DisabledScope(_disabled))
                {
                    EditorGUILayout.LabelField("Parent`s Directory");

                    _sceneSelectIndex = EditorGUILayout.Popup(_sceneSelectIndex, _sceneDirectories);

                    EditorGUILayout.LabelField("Input Scene`s Name:");
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        _sceneName = EditorGUILayout.TextField(_sceneName);
                        EditorGUILayout.LabelField("Scene");
                    }

                    EditorGUILayout.LabelField("Scene Mode:");
                    using (new EditorGUILayout.HorizontalScope())
                    {

                        string[] enum_name = { "Single", "Additive" };
                        GUIStyle style_radio = new GUIStyle(EditorStyles.miniButton);
                        _sceneMode = GUILayout.SelectionGrid(_sceneMode, enum_name, 1, style_radio);
                    }
                    if (_sceneSelectIndex != 0)
                    {
                        EditorGUILayout.LabelField("Path: Assets/Script/{Parent`s Directory}/{SceneName}");
                        EditorGUILayout.LabelField("Sample : Assets/Script/Sample/SampleScene");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Path");
                        _pathName = EditorGUILayout.TextField(_pathName);
                        EditorGUILayout.LabelField("Assets/Script/{Path}");
                    }

                    bool diseble = _sceneName == string.Empty || (_sceneSelectIndex == 0) ? _pathName == string.Empty : false;
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("MVP Scene Create:");
                    EditorGUI.BeginDisabledGroup(diseble);
                    if (GUILayout.Button("OK"))
                    {
                        _disabled = true;
                        CreateScene(_sceneName, _sceneMode, _pathName);
                        AssetDatabase.SaveAssets();
                        Close();
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }

            private void CreateScene(string name, int sceneMode, string pathName)
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                Camera mainCamera = null;

                var _name = _sceneSelectIndex switch
                {
                    0 => pathName,
                    _ => _sceneDirectories[_sceneSelectIndex]
                };
                var parentDirectory = _name + "/";
                var nameSpace = _name.Replace("/", ".");

                // create camera
                if (sceneMode == 0)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LocalData/Common/MainCamera.prefab");
                    var cameraGo = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    Debug.Assert(cameraGo != null, "cameraGo != null");
                    mainCamera = cameraGo.GetComponent<Camera>();
                }
                // Attach Manager
                {
                    _ = new GameObject { name = "SceneManager" };
                }
                // default canvas
                {
                    var prefab =
                        AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LocalData/Common/DefaultCanvas.prefab");
                    var canvas = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    if (sceneMode == 0)
                    {
                        Debug.Assert(canvas != null, "canvas != null");
                        Debug.Assert(mainCamera != null, "mainCamera != null");

                        canvas.GetComponent<Canvas>().worldCamera = mainCamera;
                    }
                }
                // event system
                if (sceneMode == 0)
                {
                    EditorApplication.ExecuteMenuItem("GameObject/UI/Event System");
                }
                // Create MVP
                {
                    var scriptDirectory = string.IsNullOrEmpty(parentDirectory)
                        ? $"Assets/Scripts/{name}Scene"
                        : $"Assets/Scripts/{parentDirectory}{name}";

                    if (Directory.Exists(scriptDirectory) == false)
                    {
                        Directory.CreateDirectory(scriptDirectory);
                    }

                    // Model
                    {
                        var model = string.Format(ModelTemplate, nameSpace, name);
                        File.WriteAllText($"{scriptDirectory}/{name}SceneModel.cs", model);
                    }

                    // View
                    {
                        var view = string.Format(ViewTemplate, nameSpace, name);
                        File.WriteAllText($"{scriptDirectory}/{name}SceneView.cs", view);
                    }

                    // Presenter
                    {
                        var presenter = string.Format(PresenterTemplate, nameSpace, name);
                        File.WriteAllText($"{scriptDirectory}/{name}ScenePresenter.cs", presenter);
                    }

                    // LifeTimeScope
                    {
                        var lifeTimeScope = string.Format(LifeTimeScopeTemplate, nameSpace, name);
                        File.WriteAllText($"{scriptDirectory}/{name}SceneLifeTimeScope.cs", lifeTimeScope);
                    }

                    AssetDatabase.Refresh();
                }

                if (!Directory.Exists($"Assets/Scenes/{parentDirectory}"))
                {
                    Directory.CreateDirectory($"Assets/Scenes/{parentDirectory}");
                }
                EditorSceneManager.SaveScene(scene, $"Assets/Scenes/{parentDirectory}{name}Scene.unity");
                EditorPrefs.SetBool("AttachComponents", true);
                EditorPrefs.SetString("NameSpace", nameSpace);
            }

            [DidReloadScripts]
            private static void OnBuildComplete()
            {
                if (EditorPrefs.GetBool("AttachComponents") == false)
                {
                    return;
                }

                try
                {
                    var scene = SceneManager.GetActiveScene();
                    var sceneName = scene.name;
                    var scenePrefix = EditorPrefs.GetString("NameSpace");

                    var sceneManager = GameObject.Find("SceneManager");
                    var view = sceneManager.AddComponent(GetType($"{scenePrefix}.{sceneName}View")) as View;

                    var lifeTimeScopeObj = GameObject.Find("LifeTimeScope");
                    lifeTimeScopeObj = (lifeTimeScopeObj == null) ? new GameObject("LifeTimeScope") : lifeTimeScopeObj;
                    var lifeTimeScope = lifeTimeScopeObj.AddComponent(GetType($"{scenePrefix}.{sceneName}LifeTimeScope")) as IMVPLifetimeScope;
                    lifeTimeScope.SetView(view);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }
                finally
                {
                    EditorPrefs.DeleteKey("AttachComponents");
                    EditorPrefs.DeleteKey("NameSpace");
                }
            }

            private static Type GetType(string type)
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .Select(a => a.GetType(type))
                    .FirstOrDefault(t => t != null);
            }

            private static string PresenterTemplate = @"using MVP;
using VContainer;

namespace {0}
{{
    public sealed class {1}ScenePresenter : Presenter<{1}SceneModel, {1}SceneView>
    {{
        public {1}ScenePresenter({1}SceneModel model, {1}SceneView view, IObjectResolver container) :
            base(model, view, container)
        {{
        }}

        protected override void Initialize() {{ }}
    }}
}}
";


            private static string ModelTemplate = @"using MVP;

namespace {0}
{{
    public sealed class {1}SceneModel : Model, I{1}SceneModel
    {{
        public {1}SceneModel() {{ }}

        public override void Initialize() 
        {{
            SceneDataPack?.SetSceneComplete();
        }}
    }}

    public interface I{1}SceneModel {{ }}
}}
";

            private static string ViewTemplate = @"using MVP;

namespace {0}
{{
    public class {1}SceneView : View, I{1}SceneView
    {{
        public override void Initialize() {{ }}
    }}

    public interface I{1}SceneView {{ }}
}}
";
            private static string LifeTimeScopeTemplate = @"using VContainer;
using MVP;

namespace {0}
{{
    public sealed class {1}SceneLifeTimeScope : MVPLifetimeScope<{1}ScenePresenter, {1}SceneModel, {1}SceneView>
    {{
        protected override void Configure(IContainerBuilder builder)
        {{
            base.Configure(builder);
        }}
    }}
}}
";
        }
    }
}
