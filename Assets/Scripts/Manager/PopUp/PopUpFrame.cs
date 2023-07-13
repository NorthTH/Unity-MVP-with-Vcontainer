using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PopUpFrame : MonoBehaviour
{
    [SerializeField]
    Camera popUpCamera;
    [SerializeField]
    Canvas canvasBase;

    UniversalAdditionalCameraData mainCameraData;
    Canvas Canvas;

    public T SetPopUpBase<T>(T popUpBase, int layer) where T : PopUpBase
    {
        T PopUpBase = default;

        if (popUpBase.GetIsRootPopUp())
        {
            var canvas = popUpBase.GetCanvas();
            canvas.transform.SetParent(this.transform);
            Canvas = canvas;
            PopUpBase = popUpBase;
            DestroyImmediate(canvasBase.gameObject);
            canvasBase = Canvas;
        }
        else
        {
            Canvas = canvasBase;
            Canvas.sortingOrder = layer;
            PopUpBase = Instantiate<T>(popUpBase, Canvas.transform);
            PopUpBase.transform.SetParent(this.canvasBase.transform);
        }

        mainCameraData = Camera.main.GetUniversalAdditionalCameraData();

        // 既存のカメラを全てのレンダリング設定をデフォルトに戻す
        foreach (var camera in mainCameraData.cameraStack)
        {
            var cameraStackData = camera.GetUniversalAdditionalCameraData();
            cameraStackData.SetRenderer(0);
        }

        // ポップアップ用のカメラを設定
        mainCameraData.cameraStack.Add(popUpCamera);

        Canvas.worldCamera = popUpCamera;

        PopUpBase.PopUpFrame = this;

        return PopUpBase;
    }

    void OnDestroy()
    {
        // ポップアップ用のカメラを削除
        mainCameraData.cameraStack.Remove(popUpCamera);

        // カメラStackが空ではない場合は最後のポップアップカメラを設定
        mainCameraData.cameraStack.LastOrDefault()?.GetUniversalAdditionalCameraData().SetRenderer(2);
    }
}
