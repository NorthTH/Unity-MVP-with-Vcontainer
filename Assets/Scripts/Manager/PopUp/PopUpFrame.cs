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
    Canvas canvas;

    // レンダリング定数定義
    private const int PopUpRender = 2;
    private const int DefaultRender = 0;

    public T SetPopUpBase<T>(T popUpBase, int layer, bool isColorCurtain, bool isOverlay) where T : PopUpBase
    {
        T popUp = default;

        if (popUpBase.GetIsRootPopUp())
        {
            var canvas = popUpBase.GetCanvas();
            canvas.transform.SetParent(this.transform);
            this.canvas = canvas;
            popUp = popUpBase;
            popUpCamera.depth = canvas.sortingOrder;
            DestroyImmediate(canvasBase.gameObject);
            canvasBase = this.canvas;
        }
        else
        {
            canvas = canvasBase;
            canvas.sortingOrder = layer;
            canvas.renderMode = (isOverlay) ? RenderMode.ScreenSpaceOverlay : RenderMode.ScreenSpaceCamera;
            popUpCamera.depth = layer;
            popUp = Instantiate<T>(popUpBase, canvas.transform);
            popUp.transform.SetParent(this.canvasBase.transform);
        }

        mainCameraData = Camera.main.GetUniversalAdditionalCameraData();

        // ポップアップ用のカメラをメインカメラのStackに追加
        mainCameraData.cameraStack.Add(popUpCamera);

        // ポップアップ用のカメラをCanvasに設定
        canvas.worldCamera = popUpCamera;

        // ポップアップベースとフレームを紐づけ
        popUp.PopUpFrame = this;

        return popUp;
    }

    void OnDestroy()
    {
        // ポップアップ用のカメラを削除
        mainCameraData.cameraStack.Remove(popUpCamera);

        Sort();
    }

    void Sort()
    {
        // カメラStackをソート
        mainCameraData.cameraStack.Sort((x, y) => x.depth.CompareTo(y.depth));

        // 既存のカメラの全てのレンダリング設定をデフォルトに戻す
        foreach (var camera in mainCameraData.cameraStack)
        {
            var cameraStackData = camera.GetUniversalAdditionalCameraData();
            cameraStackData.SetRenderer(DefaultRender);
        }

        // カメラStackが空ではない場合は最後のポップアップカメラを設定
        mainCameraData.cameraStack.LastOrDefault(x => x.isActiveAndEnabled)?.GetUniversalAdditionalCameraData().SetRenderer(PopUpRender);
    }
}
