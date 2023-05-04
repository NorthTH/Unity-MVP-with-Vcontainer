using VContainer;
using MVP;

namespace Sample
{
    public sealed class Sample03SceneLifeTimeScope : MVPLifetimeScope<Sample03ScenePresenter, Sample03SceneModel, Sample03SceneView>
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }
    }
}
