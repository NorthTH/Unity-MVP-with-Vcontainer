using Cysharp.Threading.Tasks;

namespace MVP
{
    public interface IModel
    {
        public void Initialize();
    }

    public abstract class Model : IModel
    {
        public abstract void Initialize();
    }
}
