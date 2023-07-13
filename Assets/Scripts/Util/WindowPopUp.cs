using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class WindowPopUp : WindowBase
{
    CanvasGroup CanvasGroup => canvasGroup == null ? canvasGroup = GetComponent<CanvasGroup>() : canvasGroup;
    CanvasGroup canvasGroup;

    const float OpenDuration = 0.2f;
    const float CloseDuration = 0.2f;

    public override void Open()
    {
        Open();
    }

    public override async UniTask Open(Action OnFinish = null)
    {
        CanvasGroup.interactable = false;
        CanvasGroup.alpha = 0;
        transform.localScale = Vector3.zero;

        gameObject.SetActive(true);

        Sequence openTask = DOTween.Sequence();
        await openTask.Append(CanvasGroup.DOFade(1f, OpenDuration))
                        .Join(transform.DOScale(Vector3.one, OpenDuration)).SetUpdate(Time.timeScale != 1);

        CanvasGroup.interactable = true;
        OnFinish?.Invoke();
    }

    public override void Close()
    {
        Close();
    }

    public override async UniTask Close(Action OnFinish = null)
    {
        CanvasGroup.interactable = false;

        Sequence openTask = DOTween.Sequence();
        await openTask.Append(CanvasGroup.DOFade(0f, CloseDuration))
                        .Join(transform.DOScale(Vector3.zero, CloseDuration)).SetUpdate(Time.timeScale != 1);

        gameObject.SetActive(false);
        OnFinish?.Invoke();
    }
}
