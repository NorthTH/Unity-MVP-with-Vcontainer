using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP
{
    public abstract class MVPLifetimeScope<TPresenter, TModel, TView> : LifetimeScope, IMVPLifetimeScope
       where TPresenter : Presenter<TModel, TView>
       where TModel : Model
       where TView : View
    {
        [SerializeField]
        protected TView view;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.Register<TModel>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
            builder.RegisterComponent(view).AsImplementedInterfaces();
            builder.Register<TPresenter>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.RegisterEntryPoint<EntryPoint>(Lifetime.Scoped);
        }

        private class EntryPoint : IStartable
        {
            TPresenter presenter { get; }

            private EntryPoint(TPresenter presenter)
            {
                this.presenter = presenter;
            }

            public void Start()
            {
                presenter.Initialize();
            }
        }

#if UNITY_EDITOR
        public void SetView(View view)
        {
            this.view = view as TView;
        }
#endif
    }
    public interface IMVPLifetimeScope
    {
#if UNITY_EDITOR
        public void SetView(View view);
#endif
    }
}
