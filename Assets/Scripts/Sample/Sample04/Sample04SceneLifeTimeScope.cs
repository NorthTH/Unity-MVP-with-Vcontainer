using VContainer;
using MVP;

namespace Sample
{
    public sealed class Sample04SceneLifeTimeScope : MVPLifetimeScope<Sample04ScenePresenter, Sample04SceneModel, Sample04SceneView>
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }
    }
}
