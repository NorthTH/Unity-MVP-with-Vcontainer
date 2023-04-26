using VContainer;
using MVP;

namespace Sample
{
    public sealed class Sample01SceneLifeTimeScope : MVPLifetimeScope<Sample01ScenePresenter, Sample01SceneModel, Sample01SceneView>
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }
    }
}
