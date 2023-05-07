using VContainer;
using MVP;

namespace SampleAdvance
{
    public sealed class SampleAdvance01SceneLifeTimeScope : MVPLifetimeScope<SampleAdvance01ScenePresenter, SampleAdvance01SceneModel, SampleAdvance01SceneView>
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }
    }
}
