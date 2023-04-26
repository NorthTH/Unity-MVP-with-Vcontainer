namespace MVP
{
    public interface IView
    {
        public void Initialize();
        public void RefrechView();
    }

    public abstract class View : SealedMonoBehaviour, IView
    {
        public abstract void Initialize();
        public virtual void RefrechView() { }
    }
}
