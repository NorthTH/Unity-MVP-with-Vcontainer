using VContainer;
using MVP;

namespace Sample
{
    public sealed class Sample02SceneLifeTimeScope : MVPLifetimeScope<Sample02ScenePresenter, Sample02SceneModel, Sample02SceneView>
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }
    }
}
