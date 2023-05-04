namespace MVP
{
    public interface IView
    {
        public void SetUp(ISceneDataPack SceneDataPack);
        public void Initialize();
        public void Refresh();
        public void ReceiveData(object data);
    }

    public abstract class View : SealedMonoBehaviour, IView
    {
        protected ISceneDataPack SceneDataPack;

        public virtual void SetUp(ISceneDataPack SceneDataPack)
        {
            this.SceneDataPack = SceneDataPack;
        }

        public abstract void Initialize();

        public virtual void Refresh() { }

        public virtual void ReceiveData(object data) { }
    }
}
