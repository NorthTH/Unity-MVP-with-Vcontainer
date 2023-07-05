using UnityEngine;
using VContainer;

namespace MVP
{
    public abstract class Presenter<TModel, TView> : IPresenter
     where TModel : IModel where TView : IView
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

        public void Initialize()
        {
            Debug.Log($"Initialize: {this}");

            try
            {
                var sceceDataPack = Container.Resolve<ISceneDataPack>();
                Model.SetUp(sceceDataPack);
                View.SetUp(sceceDataPack);
            }
            catch
            {
                var sceceDataPack = HistoryManager.GetFirstTimeSceneDataPack();
                Model.SetUp(sceceDataPack);
                View.SetUp(sceceDataPack);
                Debug.LogWarning("This Warning alway happen when first's scene loaded");
            }
            Model.Initialize();
            View.Initialize();

            Model.PostInitialize();
            View.PostInitialize();
        }

        public virtual void ReceiveData(object data)
        {
            Model.ReceiveData(data);
            View.Refresh();
        }
        public virtual void Refresh()
        {
            View.Refresh();
        }
    }

    public interface IPresenter
    {
        void ReceiveData(object data);
        void Refresh();
    }
}
