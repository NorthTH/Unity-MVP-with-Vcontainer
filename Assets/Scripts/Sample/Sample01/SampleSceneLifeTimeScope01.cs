using VContainer;
using MVP;

namespace Sample
{
    public sealed class SampleSceneLifeTimeScope01 : MVPLifetimeScope<SampleScenePresenter01, SampleSceneModel01, SampleSceneView01>
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }
    }
}
