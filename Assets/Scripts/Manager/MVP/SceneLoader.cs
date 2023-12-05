using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

namespace MVP
{
    public interface ISceneLoader
    {
        public void SetUp(Action<string> PreAction, Action<string> PostAction, Func<bool, UniTask> SetShowCurtain);
    }

    public partial class SceneLoader : ISceneLoader
    {
        public static SceneLoader Instance;

        readonly LifetimeScope rootLifetimeScope;
        OrderedDictionary lifetimeScopeDic;

        Action<string> PreAction;
        Action<string> PostAction;
        Func<bool, UniTask> SetShowCurtain;

        public SceneLoader(LifetimeScope rootLifetimeScope)
        {
            this.rootLifetimeScope = rootLifetimeScope;
            Instance = this;

            Initialize();
        }

        void Initialize()
        {
            Debug.Log("Initialize SceneLoader");
            lifetimeScopeDic = new OrderedDictionary();

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                var lifetimeScope = LifetimeScope.Find<LifetimeScope>(scene);
                lifetimeScopeDic.Add(scene.name, lifetimeScope);

                var mainCamera = Camera.main;
                var gameObjects = scene.GetRootGameObjects();
                foreach (var gameObject in gameObjects)
                {
                    var canvas = gameObject.GetComponent<Canvas>();
                    if (canvas)
                    {
                        canvas.worldCamera = mainCamera;
                    }
                }
            };
            SceneManager.sceneUnloaded += (scene) =>
            {
                if (lifetimeScopeDic.Contains(scene.name))
                    lifetimeScopeDic.Remove(scene.name);
            };
        }

        public void SetUp(Action<string> PreAction, Action<string> PostAction, Func<bool, UniTask> SetShowCurtain)
        {
            this.PreAction = PreAction;
            this.PostAction = PostAction;
            this.SetShowCurtain = SetShowCurtain;

            HistoryManager.Initialize(SetShowCurtain, ReturnLoadScene);

            var scene = SceneManager.GetActiveScene();
            HistoryManager.AddScene(scene.name, LoadSceneMode.Single, new SceneDataPack(null));
        }

        public UniTask LoadScene(string sceneName, object data = null)
        {
            return LoadScene(sceneName, LoadSceneMode.Single, data);
        }

        public UniTask LoadScene(string sceneName, LoadSceneMode mode, object data = null)
        {
            return InternalLoadScene(sceneName, mode, new SceneDataPack(data), mode == LoadSceneMode.Single);
        }

        UniTask InternalLoadScene(string sceneName, LoadSceneMode mode, SceneDataPack sceceDataPack, bool isShowCurtain)
        {
            var utcs = new UniTaskCompletionSource();
            sceceDataPack.SetUniTaskCompletionSource(utcs);

            var showCurtainTask = UniTask.CompletedTask;
            if (isShowCurtain)
            {
                showCurtainTask = SetShowCurtain?.Invoke(true) ?? UniTask.CompletedTask;
            }

            PreAction?.Invoke(sceneName);
            HistoryManager.AddScene(sceneName, mode, sceceDataPack);

            LoadScene().Forget();
            async UniTaskVoid LoadScene()
            {
                await showCurtainTask;

                InternalLoadScene(sceneName, mode, sceceDataPack, async () =>
                {
                    PostAction?.Invoke(sceneName);
                    await utcs.Task;
                    if (mode == LoadSceneMode.Single)
                    {
                        await (SetShowCurtain?.Invoke(false) ?? UniTask.CompletedTask);
                    }
                }).Forget();
            }

            return utcs.Task;
        }

        UniTask ReturnLoadScene(string sceneName, LoadSceneMode mode, SceneDataPack sceceDataPack)
        {
            var utcs = new UniTaskCompletionSource();
            sceceDataPack.SetUniTaskCompletionSource(utcs);

            PreAction?.Invoke(sceneName);
            HistoryManager.AddScene(sceneName, mode, sceceDataPack);

            InternalLoadScene(sceneName, mode, sceceDataPack, async () =>
            {
                PostAction?.Invoke(sceneName);
                await utcs.Task;
            }).Forget();

            return utcs.Task;
        }

        /// <summary>
        /// 今からロードされるシーンと一番手前の現在のシーンを親子関係を繋ぐ
        /// </summary>
        async UniTask InternalLoadScene(string sceneName, LoadSceneMode mode, SceneDataPack sceneDataPack, Action OnComplete)
        {
            if (mode == LoadSceneMode.Single)
            {
                lifetimeScopeDic.Clear();
                using (LifetimeScope.EnqueueParent(rootLifetimeScope))
                using (LifetimeScope.Enqueue(builder =>
                {
                    // 今からロードするシーンにシーンデータの参照を渡す
                    builder.Register<SceneDataPack>(container =>
                    {
                        return sceneDataPack;
                    }, Lifetime.Scoped).AsImplementedInterfaces();
                }))
                {
                    await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                }
            }
            else if (mode == LoadSceneMode.Additive)
            {
                var parent = (LifetimeScope)lifetimeScopeDic[lifetimeScopeDic.Count - 1];
                using (LifetimeScope.EnqueueParent(parent))
                using (LifetimeScope.Enqueue(builder =>
                {
                    // 今からロードするシーンに親のPresenterの参照を渡す
                    builder.Register<ParentPresenter>(container =>
                    {
                        var parentContainer = new ParentPresenter(parent.Container.Resolve<IPresenter>());
                        return parentContainer;
                    }, Lifetime.Scoped);

                    // 今からロードするシーンにシーンデータの参照を渡す
                    builder.Register<SceneDataPack>(container =>
                    {
                        return sceneDataPack;
                    }, Lifetime.Scoped).AsImplementedInterfaces();
                }))
                {
                    await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                }
            }
            OnComplete.Invoke();
        }

        /// <summary>
        /// LastScopeの親を辿って逆検索し、指定した(T)のLifetimeScopeのContainerを取得する
        /// </summary>
        public bool FindContainer<T>(out IObjectResolver? container) where T : LifetimeScope
        {
            container = default;
            var currentScope = (lifetimeScopeDic.Count == 0) ? rootLifetimeScope : (LifetimeScope)lifetimeScopeDic[lifetimeScopeDic.Count - 1];
            while (currentScope != null)
            {
                Debug.Log("Scope: " + currentScope.GetHashCode());
                if (currentScope is T)
                {
                    container = currentScope.Container;
                    return true;
                }
                currentScope = currentScope.Parent;
            }

            return false;
        }
    }


    /// <summary>
    /// 親シーンのPresenterの参照を子シーンに引き渡すため、使用するためのClass。引き渡しの流れはWaterFall図です。
    /// </summary>
    public class ParentPresenter
    {
        public IPresenter Presenter { get; private set; }

        public ParentPresenter(IPresenter presenter)
        {
            this.Presenter = presenter;
        }
    }

    public class SceneDataPack : ISceneDataPack
    {
        object data;
        public UniTaskCompletionSource SceneUtcs { get; private set; }

        public SceneDataPack(object data)
        {
            this.data = data;
        }

        /// <summary>
        /// シーンロード完了時にシーンロード非同期操作の完了通知を登録する
        /// </summary>
        public void SetUniTaskCompletionSource(UniTaskCompletionSource sceneUtcs)
        {
            this.SceneUtcs = sceneUtcs;
        }

        public T GetData<T>() where T : notnull, new()
        {
            // シーンデータがnullの場合、新しく生成する。
            data ??= new T();

            // 指定されたシーンデータの型と一致しない場合、例外を投げる
            if (!(data is T))
                throw new InvalidCastException($"SceneDataPackの型が一致しません。{typeof(T)}を指定してください。");

            // シーンデータを渡す
            return (T)data;
        }

        public void SetSceneComplete()
        {
            SceneUtcs?.TrySetResult();
        }
    }

    public interface ISceneDataPack
    {
        /// <summary>
        /// シーンデータを取得する。
        /// </summary>
        public T GetData<T>() where T : notnull, new();

        /// <summary>
        /// シーンロード完了を通知する。
        /// </summary>
        public void SetSceneComplete();
    }
}
