using VContainer;
using MVP;

namespace Sample
{
    public sealed class Sample04Sub02SceneLifeTimeScope : MVPLifetimeScope<Sample04Sub02ScenePresenter, Sample04Sub02SceneModel, Sample04Sub02SceneView>
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }
    }
}
