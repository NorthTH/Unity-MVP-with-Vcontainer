using VContainer;
using MVP;

namespace Sample
{
    public sealed class Sample04Sub03SceneLifeTimeScope : MVPLifetimeScope<Sample04Sub03ScenePresenter, Sample04Sub03SceneModel, Sample04Sub03SceneView>
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }
    }
}
