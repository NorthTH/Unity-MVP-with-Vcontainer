using VContainer;
using MVP;

namespace Tetris
{
    public sealed class TetrisMainSceneLifeTimeScope : MVPLifetimeScope<TetrisMainScenePresenter, TetrisMainSceneModel, TetrisMainSceneView>
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }
    }
}
