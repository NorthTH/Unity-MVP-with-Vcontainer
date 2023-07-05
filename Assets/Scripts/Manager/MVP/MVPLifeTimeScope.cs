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

        public class EntryPoint : IStartable
        {
            TPresenter Presenter;

            private EntryPoint(TPresenter Presenter)
            {
                this.Presenter = Presenter;
            }

            public void Start()
            {
                Presenter.Initialize();
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
