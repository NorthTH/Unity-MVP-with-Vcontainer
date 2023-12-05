using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class PopUpBase : MonoBehaviour
{
    [SerializeField]
    protected WindowBase windowBase;
    [SerializeField]
    protected Button curtain;
    [SerializeField]
    protected Canvas canvas;
    [SerializeField]
    protected bool isRootPopUp = false;

    private PopUpContainer controller;

    public PopUpFrame PopUpFrame { get; set; }

    public RebuildData RebuildData
    {
        get
        {
            rebuildData ??= new RebuildData();
            return rebuildData;
        }
        set
        {
            rebuildData = value;
        }
    }
    protected RebuildData rebuildData;

    protected IObjectResolver container;

    public bool GetIsRootPopUp()
    {
        return isRootPopUp;
    }

    //======================
    // 仮想メソッド
    //======================
    protected virtual void OpenCallBack() { }
    protected virtual void CloseCallBack() { }
    protected virtual void CurtainCallBack() { }

    /// <summary>
    /// ポップアップの設定
    /// </summary>
    /// <param name="container"></param>
    public virtual void Initialize()
    {
        curtain.onClick.AddListener(() =>
        {
            CurtainCallBack();
        });
    }

    public void SetPopUpController(PopUpContainer controller)
    {
        this.controller = controller;
    }


    //======================
    // WIndow演出
    //======================


    /// <summary>
    /// WIndowを開くアニメーション(Rootの場合反映されない)
    /// </summary>
    public async UniTask WindowOpen()
    {
        await windowBase.Open(() =>
        {
            OpenCallBack();
            RebuildData.IsOpened = true;
        });
    }

    /// <summary>
    /// Windowを閉じるアニメーション(Rootの場合反映されない)
    /// </summary>
    public async UniTask WindowClose()
    {
        await windowBase.Close(async () =>
        {
            CloseCallBack();
            rebuildData.IsOpened = false;
        });
    }

    /// <summary>
    /// RootWIndowを閉じるアニメーション
    /// </summary>
    public async UniTask RootWindowClose()
    {
        await windowBase.Close(async () =>
        {
            CloseCallBack();
        });
    }

    public Canvas GetCanvas() => canvas;
    protected PopUpContainer GetController() => controller;
}

public class RebuildData
{
    public Type Type;
    public int Layer;
    public bool IsOpened;
    public bool IsColor;
    public bool IsOverlay;
}
