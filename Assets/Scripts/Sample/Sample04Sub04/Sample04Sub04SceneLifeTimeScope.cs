using VContainer;
using MVP;

namespace Sample
{
    public sealed class Sample04Sub04SceneLifeTimeScope : MVPLifetimeScope<Sample04Sub04ScenePresenter, Sample04Sub04SceneModel, Sample04Sub04SceneView>
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }
    }
}
