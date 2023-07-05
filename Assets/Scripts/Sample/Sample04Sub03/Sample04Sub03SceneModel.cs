using MVP;
using VContainer;

namespace Sample
{
    public sealed class Sample04Sub03SceneModel : Model, ISample04Sub03SceneModel
    {
        IObjectResolver container;
        ISample04Sub03SceneDataPack ISample04Sub03SceneDataPack;

        public Sample04Sub03SceneModel(IObjectResolver container)
        {
            this.container = container;
        }

        public override void Initialize()
        {
            ISample04Sub03SceneDataPack = SceneDataPack.GetData<ISample04Sub03SceneDataPack>();
        }
    }

    public interface ISample04Sub03SceneModel { }

    public class ISample04Sub03SceneDataPack { }
}
