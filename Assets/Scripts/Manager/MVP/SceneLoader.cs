using System;
using System.Collections.Generic;
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
        public void SetUp(Action<string> PreAction, Action<string> PostAction, Action<bool> SetShowCurtain);
    }

    public partial class SceneLoader : ISceneLoader
    {
        public static SceneLoader Instance;

        readonly LifetimeScope rootLifetimeScope;
        Dictionary<string, LifetimeScope> lifetimeScopeDic;

        Action<string> PreAction;
        Action<string> PostAction;
        Action<bool> SetShowCurtain;

        public IObjectResolver LastContainer
        {
            get
            {
                var lastLifetimeScope = (lifetimeScopeDic.Count == 0) ? rootLifetimeScope : lifetimeScopeDic.Last().Value;
                return lastLifetimeScope.Container;
            }
        }

        public SceneLoader(LifetimeScope rootLifetimeScope)
        {
            this.rootLifetimeScope = rootLifetimeScope;
            Instance = this;

            Initialize();
        }

        void Initialize()
        {
            Debug.Log("Initialize SceneLoader");
            lifetimeScopeDic = new Dictionary<string, LifetimeScope>();

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
                if (lifetimeScopeDic.ContainsKey(scene.name))
                    lifetimeScopeDic.Remove(scene.name);
            };
        }

        public void SetUp(Action<string> PreAction, Action<string> PostAction, Action<bool> SetShowCurtain)
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
            return InternalLoadScene(sceneName, mode, new SceneDataPack(data));
        }

        UniTask InternalLoadScene(string sceneName, LoadSceneMode mode, SceneDataPack sceceDataPack)
        {
            var utcs = new UniTaskCompletionSource();
            UniTaskCompletionSource sceneUtcs = new UniTaskCompletionSource();
            sceceDataPack.SetUniTaskCompletionSource(sceneUtcs);

            if (mode == LoadSceneMode.Single)
                SetShowCurtain.Invoke(true);

            PreAction?.Invoke(sceneName);
            HistoryManager.AddScene(sceneName, mode, sceceDataPack);

            InternalLoadScene(sceneName, mode, sceceDataPack, async () =>
            {
                PostAction?.Invoke(sceneName);
                await (sceneUtcs?.Task ?? UniTask.CompletedTask);
                if (mode == LoadSceneMode.Single)
                    SetShowCurtain.Invoke(false);
                utcs.TrySetResult();
            }).Forget();

            return utcs.Task;
        }

        UniTask ReturnLoadScene(string sceneName, LoadSceneMode mode, SceneDataPack sceceDataPack)
        {
            var utcs = new UniTaskCompletionSource();
            UniTaskCompletionSource sceneUtcs = new UniTaskCompletionSource();
            sceceDataPack.SetUniTaskCompletionSource(sceneUtcs);

            PreAction?.Invoke(sceneName);
            HistoryManager.AddScene(sceneName, mode, sceceDataPack);

            InternalLoadScene(sceneName, mode, sceceDataPack, async () =>
            {
                PostAction?.Invoke(sceneName);
                await (sceneUtcs?.Task ?? UniTask.CompletedTask);
                utcs.TrySetResult();
            }).Forget();

            return utcs.Task;
        }

        async UniTask InternalLoadScene(string sceneName, LoadSceneMode mode, SceneDataPack sceneDataPack, Action OnComplete)
        {
            if (mode == LoadSceneMode.Single)
            {
                lifetimeScopeDic.Clear();
                using (LifetimeScope.EnqueueParent(rootLifetimeScope))
                using (LifetimeScope.Enqueue(builder =>
                {
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
                var parent = lifetimeScopeDic.Last().Value;
                using (LifetimeScope.EnqueueParent(parent))
                using (LifetimeScope.Enqueue(builder =>
                {
                    builder.Register<ParentContainer>(container =>
                    {
                        var parentContainer = new ParentContainer(parent.Container);
                        return parentContainer;
                    }, Lifetime.Scoped);
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
    }

    public class ParentContainer
    {
        IObjectResolver container;
        public IObjectResolver Container => container;

        public ParentContainer(IObjectResolver container)
        {
            this.container = container;
        }

        public IModel GetIModel()
        {
            return container.Resolve<IModel>();
        }

        public IView GetIView()
        {
            return container.Resolve<IView>();
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

        public void SetUniTaskCompletionSource(UniTaskCompletionSource sceneUtcs)
        {
            this.SceneUtcs = sceneUtcs;
        }

        public T GetData<T>() where T : notnull, new()
        {
            if (data == null)
                data = new T();
            return (T)data;
        }

        public void SetSceneComplete()
        {
            SceneUtcs?.TrySetResult();
        }
    }

    public interface ISceneDataPack
    {
        public T GetData<T>() where T : notnull, new();
        public void SetSceneComplete();
    }
}
