using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace MVP
{
    public static class HistoryManager
    {
        private static readonly Stack<(string scene, LoadSceneMode loadSceneMode, SceneDataPack sceneDataPack)>
            _history = new Stack<(string, LoadSceneMode, SceneDataPack)>();

        private static string actualSingleSceneName = SceneManager.GetActiveScene().name;
        private static List<string> actualAdditiveSceneName = new List<string>();
        public static string GetActualSingleSceneName() => actualSingleSceneName;
        public static string GetLastLoadedSceneName() => _history.Peek().scene;
        public static string GetPreviousLoadedSceneName() => actualAdditiveSceneName.Count > 1 ? actualAdditiveSceneName[actualAdditiveSceneName.Count - 2] : actualSingleSceneName;

        static Action<bool> SetShowLoadingCurtain;
        static Func<string, LoadSceneMode, SceneDataPack, UniTask> LoadSceneAsync;

        public static void Initialize(Action<bool> setShowLoadingCurtain = null, Func<string, LoadSceneMode, SceneDataPack, UniTask> returnLoadSceneAsync = null)
        {
            SetShowLoadingCurtain = setShowLoadingCurtain;
            LoadSceneAsync = returnLoadSceneAsync;
        }

        public static void AddScene(string scene, LoadSceneMode loadSceneMode, SceneDataPack sceneDataPack = null)
        {
            AddSceneName(scene, loadSceneMode, sceneDataPack).Forget();
        }

        static async UniTaskVoid AddSceneName(string scene, LoadSceneMode loadSceneMode, SceneDataPack sceneDataPack)
        {
            if (loadSceneMode == LoadSceneMode.Single)
            {
                // 同じシーンの複数回の読み込みを回避するため、履歴に記録しない
                if (_history.Any() && _history.First().scene == scene && _history.First().loadSceneMode == loadSceneMode)
                    return;
                actualAdditiveSceneName.Clear();
                actualSingleSceneName = scene;
            }
            else
            {
                // 同じシーンの複数回の読み込みを回避するため、一度アンロード
                if (actualAdditiveSceneName.Contains(scene))
                    await RemoveAdditiveScene(scene);
                actualAdditiveSceneName.Add(scene);
            }
            _history.Push((scene, loadSceneMode, sceneDataPack));
        }

        public static async UniTask RemoveAllAdditiveScene()
        {
            var tempActualAdditiveSceneList = new List<string>(actualAdditiveSceneName);
            tempActualAdditiveSceneList.Reverse();
            foreach (var scene in tempActualAdditiveSceneList)
            {
                await RemoveAdditiveScene(scene);
            }
        }

        public static async UniTask RemoveAllAdditiveScene(params string[] sceneNameParams)
        {
            foreach (var scene in sceneNameParams)
            {
                await RemoveAdditiveScene(scene);
            }
        }

        /// <summary>
        /// 特定のサブシーンをアンロードするため、上から遡り見つけた場合、アンロードする。仕様：_historyの上からPopし仮Stackに保管し、
        /// 最後削除シーン以外を_historyに戻す
        /// </summary>
        public static async UniTask RemoveAdditiveScene(string scene)
        {
            Debug.Assert(_history.Count != 0, "_history.Count != 0");

            if (_history.Count <= 1)
            {
                Debug.LogWarning("No scenes in history");
                return;
            }

            var reversedStack = new Stack<(string, LoadSceneMode, SceneDataPack)>();

            while (_history.Count > 0)
            {
                var topItem = _history.Pop();
                if (topItem.loadSceneMode == LoadSceneMode.Single)
                {
                    reversedStack.Push(topItem);
                    Debug.LogWarning("No more additive scenes in Top of Stack");
                    break;
                }

                if (topItem.scene != scene)
                {
                    reversedStack.Push(topItem);
                }
                else
                {
                    var sceneName = topItem.scene;
                    CheckIsUniqueScene(sceneName);
                    actualAdditiveSceneName.Remove(sceneName);
                    await SceneManager.UnloadSceneAsync(sceneName);
                    break;
                }
            }

            while (reversedStack.Count > 0)
            {
                _history.Push(reversedStack.Pop());
            }
        }

        /// <summary>
        /// 履歴がある場合、ひとつ前のシーンに戻ります。
        /// </summary>
        /// <param name="scene">指定しない場合最後に開いたシーンの名前と同じ名前のシーンを削除します。</param>
        /// <returns></returns>
        public static async UniTask ReturnScene(bool returnAllCurrentScene = false)
        {
            Debug.Assert(_history.Count != 0, "_history.Count != 0");

            if (_history.Count <= 1)
            {
                Debug.LogWarning("No scenes in history");
                return;
            }

            if (returnAllCurrentScene)
            {
                SetShowLoadingCurtain?.Invoke(true);
                await RemoveAllAdditiveScene();
                if (_history.Count <= 1)
                {
                    SetShowLoadingCurtain?.Invoke(false);
                    return;
                }
            }

            // 現在表示しているシーンをスタックから取得
            var (currentScene, loadSceneMode, _) = _history.Pop();
            switch (loadSceneMode)
            {
                case LoadSceneMode.Single:
                    SetShowLoadingCurtain?.Invoke(true);
                    actualAdditiveSceneName.Remove(currentScene);

                    // 前の画面の状態を復元するため、Additiveだったシーンロードを再度再現
                    var additiveList =
                        new Stack<(string scene, LoadSceneMode loadSceneMode, SceneDataPack sceneDataPack)>();

                    while (_history.Peek().loadSceneMode == LoadSceneMode.Additive)
                    {
                        additiveList.Push(_history.Pop());
                    }

                    // Additiveが全てなくなったので、Singleに到達＝前の画面のルート
                    var previousRoot = _history.Pop();

                    // ルートをロード
                    await LoadSceneAsync.Invoke(
                        previousRoot.scene,
                        previousRoot.loadSceneMode,
                        previousRoot.sceneDataPack);

                    // Additiveされたシーンを全てロード
                    while (additiveList.Count > 0)
                    {
                        var additive = additiveList.Pop();
                        await LoadSceneAsync.Invoke(
                            additive.scene,
                            additive.loadSceneMode,
                            additive.sceneDataPack);
                    }
                    SetShowLoadingCurtain?.Invoke(false);
                    break;
                case LoadSceneMode.Additive:
                    CheckIsUniqueScene(currentScene);
                    actualAdditiveSceneName.Remove(currentScene);
                    await SceneManager.UnloadSceneAsync(currentScene);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        private static void CheckIsUniqueScene(string name)
        {
            var sceneCount = SceneManager.sceneCount;
            int foundCount = 0;
            for (int i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name == name)
                {
                    foundCount += 1;
                }
            }

            Debug.Assert(foundCount == 1, "foundCount == 1");
        }
    }
}
