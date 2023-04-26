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
        public void Initialize();
    }

    public partial class SceneLoader : ISceneLoader
    {
        readonly LifetimeScope rootLifetimeScope;
        Dictionary<string, LifetimeScope> lifetimeScopeDic;

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
        }

        public void Initialize()
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

        public UniTask LoadScene(string sceneName, SceceDataPack sceceDataPack = null)
        {
            return LoadScene(sceneName, LoadSceneMode.Single, sceceDataPack);
        }

        public UniTask LoadScene(string sceneName, LoadSceneMode mode, SceceDataPack sceceDataPack)
        {
            return InternalLoadScene(sceneName, mode, sceceDataPack);
        }

        UniTask InternalLoadScene(string sceneName, LoadSceneMode mode, SceceDataPack sceceDataPack)
        {
            var utcs = new UniTaskCompletionSource();

            InternalLoadScene(sceneName, mode, () =>
            {
                utcs.TrySetResult();
            }).Forget();

            return utcs.Task;
        }

        async UniTask InternalLoadScene(string sceneName, LoadSceneMode mode, Action OnComplete)
        {
            if (mode == LoadSceneMode.Single)
            {
                lifetimeScopeDic.Clear();
                using (LifetimeScope.EnqueueParent(rootLifetimeScope))
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

    public class SceceDataPack { }
}
