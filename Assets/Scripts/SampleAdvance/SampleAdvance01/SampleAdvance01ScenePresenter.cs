using MVP;
using VContainer;

namespace SampleAdvance
{
    public sealed class SampleAdvance01ScenePresenter : Presenter<SampleAdvance01SceneModel, SampleAdvance01SceneView>
    {
        public SampleAdvance01ScenePresenter(SampleAdvance01SceneModel model, SampleAdvance01SceneView view, IObjectResolver container) :
            base(model, view, container)
        {
        }

        protected override void Initialize() { }
    }
}
