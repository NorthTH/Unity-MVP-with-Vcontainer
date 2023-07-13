using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class WindowBase : MonoBehaviour
{
    public abstract void Open();
    public abstract UniTask Open(Action onFinish = null);
    public abstract void Close();
    public abstract UniTask Close(Action onFinish = null);

}
