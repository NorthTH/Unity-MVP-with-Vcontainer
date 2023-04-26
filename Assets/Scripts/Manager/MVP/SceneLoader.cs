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
        public void SetUp(Action PreAction, Action PostAction, Action<bool> SetShowCurtain);
    }

    public partial class SceneLoader : ISceneLoader
    {
        public static SceneLoader Instance;

        readonly LifetimeScope rootLifetimeScope;
        Dictionary<string, LifetimeScope> lifetimeScopeDic;

        Action PreAction;
        Action PostAction;
        Action<bool> SetShowCurtain;

        public LifetimeScope LastLifetimeScope
        {
            get
            {
                var lastLifetimeScope = (lifetimeScopeDic.Count == 0) ? rootLifetimeScope : lifetimeScopeDic.Last().Value;
                return lastLifetimeScope;
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

        public void SetUp(Action PreAction, Action PostAction, Action<bool> SetShowCurtain)
        {
            this.PreAction = PreAction;
            this.PostAction = PostAction;
            this.SetShowCurtain = SetShowCurtain;
        }

        public UniTask LoadScene(string sceneName)
        {
            return LoadScene(sceneName, LoadSceneMode.Single, new SceneDataPack());
        }

        public UniTask LoadScene(string sceneName, LoadSceneMode mode, SceneDataPack sceceDataPack = null)
        {
            return InternalLoadScene(sceneName, mode, sceceDataPack ?? new SceneDataPack());
        }

        UniTask InternalLoadScene(string sceneName, LoadSceneMode mode, SceneDataPack sceceDataPack)
        {
            var utcs = new UniTaskCompletionSource();

            if (mode == LoadSceneMode.Single)
                SetShowCurtain.Invoke(true);

            PreAction?.Invoke();
            InternalLoadScene(sceneName, mode, sceceDataPack, () =>
            {
                utcs.TrySetResult();
                if (mode == LoadSceneMode.Single)
                    SetShowCurtain.Invoke(false);
                PostAction?.Invoke();
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
                    }, Lifetime.Scoped);
                }))
                {
                    await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                }
            }
            else if (mode == LoadSceneMode.Additive)
            {
                var parent = LastLifetimeScope;
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
                    }, Lifetime.Scoped);
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

    public class SceneDataPack
    {
        public object SendData;
    }
}
