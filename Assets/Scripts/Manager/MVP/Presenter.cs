using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP
{
    public abstract class Presenter<TModel, TView> : IStartable where TModel : IModel where TView : IView
    {
        protected IObjectResolver Container;
        protected TModel Model { get; }
        protected TView View { get; }

        protected Presenter(TModel model, TView view, IObjectResolver container)
        {
            Model = model;
            View = view;
            Container = container;
        }

        public void Start()
        {
            Debug.Log($"Initialize: {this}");
            Initialize();

            try
            {
                var sceceDataPack = Container.Resolve<ISceneDataPack>();
                Model.SetUp(sceceDataPack);
                View.SetUp(sceceDataPack);
            }
            catch
            {
                var sceceDataPack = new SceneDataPack(null);
                Model.SetUp(sceceDataPack);
                View.SetUp(sceceDataPack);
                Debug.LogWarning("This Warning alway happen when first's scene loaded");
            }

            View.Initialize();
            Model.Initialize();
        }

        protected abstract void Initialize();
    }
}
