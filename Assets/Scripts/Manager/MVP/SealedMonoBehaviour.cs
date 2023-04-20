using UnityEngine;

public abstract class SealedMonoBehaviour : SealBase
{
    public sealed override void Awake() { }
    public sealed override void Start() { }
}

public abstract class SealBase : MonoBehaviour
{
    public abstract void Awake();
    public abstract void Start();
}
