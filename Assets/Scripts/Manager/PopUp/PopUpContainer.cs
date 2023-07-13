using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PopUpContainer : MonoBehaviour
{
    [SerializeField]
    PopUpFrame popUpFramePrefab;
    [SerializeField]
    List<PopUpBase> popUpObjList = new List<PopUpBase>();

    List<RebuildData> rebuildPopUpDataList;
    List<PopUpBase> popUpList;

    /// <summary>
    /// 初期化時に必ず呼ぶ
    /// </summary>
    /// <param name="rebuildPopUpDataList">データ復元用</param>
    public void Initialize(List<RebuildData> rebuildPopUpDataList = null)
    {
        this.rebuildPopUpDataList = rebuildPopUpDataList;
        popUpList = new List<PopUpBase>();
    }

    /// <summary>
    /// ポップアップの作成
    /// </summary>
    /// <param name="Layer">CanvasのSortedOrder</param>
    public T CreatePopup<T>(int Layer = 0) where T : PopUpBase
    {
        var popUp = popUpObjList.FirstOrDefault(x => x is T);
        if (popUp == default)
            throw new ArgumentException($"指定された情報が存在しませんでした。[ T : {typeof(T)} ]");

        var popUpFrame = Instantiate<PopUpFrame>(popUpFramePrefab, this.transform);
        var popUpBase = popUpFrame.SetPopUpBase<T>(popUp as T, Layer);

        popUpBase.SetPopUpController(this);
        popUpBase.Initialize();
        popUpList.Add(popUpBase);

        if (rebuildPopUpDataList != null && !popUpBase.GetIsRootPopUp())
        {
            popUpBase.Initialize();
            popUpBase.RebuildData.Type = popUpBase.GetType();
            popUpBase.RebuildData.Layer = Layer;
            rebuildPopUpDataList.Add(popUpBase.RebuildData);
        }

        return popUpBase as T;
    }

    public void RebuildPopUpList()
    {
        if (rebuildPopUpDataList != null)
        {
            foreach (var rebuildPopUpData in rebuildPopUpDataList)
            {
                var rebuildPopUp = popUpObjList.FirstOrDefault(x => x.GetType() == rebuildPopUpData.Type);
                if (rebuildPopUp.GetIsRootPopUp())
                    continue;

                var popUpFrame = Instantiate<PopUpFrame>(popUpFramePrefab, this.transform);
                var popUpBase = popUpFrame.SetPopUpBase<PopUpBase>(rebuildPopUp, rebuildPopUpData.Layer);
                popUpBase.SetPopUpController(this);
                popUpBase.Initialize();
                popUpList.Add(popUpBase);
                popUpBase.Initialize();

                if (rebuildPopUpData.IsOpened)
                {
                    popUpBase.WindowOpen();
                }
                else
                {
                    popUpBase.WindowClose();
                }
            }
        }
    }

    /// <summary>
    /// ポップアップの削除
    /// </summary>
    public async UniTask DeletePopup()
    {
        if (popUpList.Count == 0)
        {
            Debug.LogWarning("削除するポップアップがありませんでした。");
            return;
        }
        var popUpBase = popUpList.Last();
        await popUpBase.WindowClose();
        popUpList.Remove(popUpBase);
        rebuildPopUpDataList?.Remove(popUpBase.RebuildData);
        DestroyImmediate(popUpBase.PopUpFrame.gameObject);
    }

    public void ApplyCurrentRootPopupBlur(bool isActive)
    {
        if (isActive)
        {
            foreach (var popUp in popUpList)
            {
                popUp.PopUpFrame.gameObject.SetActive(true);
                popUp.WindowOpen();
            }
        }
        else
        {
            foreach (var popUp in popUpList)
            {
                popUp.WindowClose();
                popUp.PopUpFrame.gameObject.SetActive(false);
            }
        }
    }
}
