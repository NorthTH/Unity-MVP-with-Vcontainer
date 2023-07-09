using MVP;
using VContainer;

namespace Tetris
{
    public sealed class TetrisMainScenePresenter : Presenter<TetrisMainSceneModel, TetrisMainSceneView>
    {
        public TetrisMainScenePresenter(TetrisMainSceneModel model, TetrisMainSceneView view, IObjectResolver container) :
            base(model, view, container)
        {
        }

        protected override void Bind()
        {
            Model.ShowField = View.ShowField;
            Model.ShowGameOver = View.ShowGameOver;

            View.UserInputKey = Model.UserInput;
        }
    }
}
