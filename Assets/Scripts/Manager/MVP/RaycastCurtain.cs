using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class RaycastCurtain : MonoBehaviour, ICanvasRaycastFilter
{
    [SerializeField] Button button;
    bool isCurtainTapped;
    RectTransform target;

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (target == null) return true;
        var targetLocalPos = transform.InverseTransformPoint(target.position); ;
        var targetHalfSizeDelta = target.sizeDelta / 2;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, sp, Camera.main, out var touchLocalPos);

        //ターゲットのタップ範囲なら当たり判定をfalseにしてraycastを通す
        bool isTargetInside = targetLocalPos.x - targetHalfSizeDelta.x <= touchLocalPos.x &&
                              touchLocalPos.x <= targetLocalPos.x + targetHalfSizeDelta.x &&
                              targetLocalPos.y - targetHalfSizeDelta.y <= touchLocalPos.y &&
                              touchLocalPos.y <= targetLocalPos.y + targetHalfSizeDelta.y;
        return !isTargetInside;
    }

    public void SetTapTarget(RectTransform target) => this.target = target;

    public void Init()
    {
        if (button == null) button = GetComponent<Button>();
        button.onClick.AddListener(() => isCurtainTapped = true);
    }

    public async UniTask WaitTapCurtain(CancellationTokenSource cts)
    {
        await UniTask.WaitUntil(() => isCurtainTapped, cancellationToken: cts.Token);
        isCurtainTapped = false;
    }
}