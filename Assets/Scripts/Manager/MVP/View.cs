namespace MVP
{
    public interface IView
    {
        public void SetUp(ISceneDataPack SceneDataPack);
        public void Initialize();
        public void PostInitialize();
        public void Refresh();
    }

    public abstract class View : SealedMonoBehaviour, IView
    {
        protected ISceneDataPack SceneDataPack;

        public virtual void SetUp(ISceneDataPack SceneDataPack)
        {
            this.SceneDataPack = SceneDataPack;
        }

        public abstract void Initialize();

        public virtual void PostInitialize() { }

        public virtual void Refresh() { }
    }
}
