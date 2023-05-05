using VContainer;
using MVP;

namespace Sample
{
    public sealed class Sample04Sub01SceneLifeTimeScope : MVPLifetimeScope<Sample04Sub01ScenePresenter, Sample04Sub01SceneModel, Sample04Sub01SceneView>
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }
    }
}
