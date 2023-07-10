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

            /// PresenterのみSingletonであるため、外部コンテナはModelとViewの参照を取れなく、Presenterを介してのみ参照できる。
            builder.Register<TModel>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
            builder.RegisterComponent(view).AsImplementedInterfaces();
            builder.Register<TPresenter>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.RegisterEntryPoint<EntryPoint>(Lifetime.Scoped);
        }

        /// <summary>
        /// シーンがロードされた際、Presenterの初期化を行う
        /// </summary>
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
