using UnityEngine;
using VContainer.Unity;

namespace MVP
{
    public abstract class Presenter<TModel, TView> : IStartable where TModel : IModel where TView : View
    {
        protected TModel Model { get; }
        protected TView View { get; }

        protected Presenter(TModel model, TView view)
        {
            Model = model;
            View = view;
        }

        public void Start()
        {
            Debug.Log($"Start: {this}");
            Initialize();

            Model.Initialize();
            View.Initialize();
        }

        protected abstract void Initialize();
    }
}
