using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Manager.Rendering
{
    [Flags]
    public enum BlurLayer
    {
        None = 0,
        UI = 1 << 0,
        Overlay = 1 << 1,
        Blur = UI | Overlay,
        BlackColor = 1 << 4,
        WhiteColor = 1 << 5,
        ColorCurtain = BlackColor | WhiteColor,
    }

    public interface IRenderFeatureManager { }

    public class RenderFeatureManager : MonoBehaviour, IRenderFeatureManager
    {
        public static RenderFeatureManager Instance;

        [SerializeField]
        BlurTargetLayerFeature uiBlurFeature;
        [SerializeField]
        BlurTargetLayerFeature overlayBlurFeature;
        [SerializeField]
        Color BlackColor = Color.black;
        [SerializeField]
        Color WhiteColor = Color.white;

        BlurLayer currentBlurType = BlurLayer.None;
        public BlurLayer BlurType => currentBlurType;
        const float MinBlurSize = 0.001f;
        const float ColorAlpha = 0.3f;

        const float Sigma = 5.137f;   //2.446 ~ 5.137
        const float DefaultBlurAmount = 3.919f;   //2.659 ~ 3.919
        const uint KernelStep = 18;    //13 ~ 18

        bool isInitialized = false;
        float backgroundBlurAmount = MinBlurSize;

        float currentBlurDensityRatio = 0;

        public RenderFeatureManager()
        {
            Instance = this;
        }

        public void SetEnableBlur(BlurLayer value, float blurAmount = -1, float duration = 0.1f)
        {
            if (currentBlurType.HasFlag(value))
                return;

            var tempBlurType = currentBlurType;
            currentBlurType |= value;

            // valueがカーテンフラグがある場合は、フラグを絞ってカーテンの色を設定する
            if ((value & BlurLayer.ColorCurtain) != BlurLayer.None)
            {
                // カーテンフラグがNoneから増えた場合は、カーテンをフェードインさせる
                var colorFlags = currentBlurType & BlurLayer.ColorCurtain;
                bool isAnimation = (tempBlurType & BlurLayer.ColorCurtain) == BlurLayer.None;
                CheckCurtainFlags(colorFlags, isAnimation, duration);
            }

            // valueがブラーフラグがある場合は、フラグを絞ってブラーを設定する
            if ((value & BlurLayer.Blur) != BlurLayer.None)
            {
                // ブラーフラグがNoneから増えた場合は、ブラーをフェードインさせる
                var blurFlags = currentBlurType & BlurLayer.Blur;
                bool isAnimation = (tempBlurType & BlurLayer.Blur) == BlurLayer.None;
                CheckBlurFlags(blurFlags, isAnimation, duration);
            }
        }

        public void SetDisableBlur(BlurLayer value, float duration = 0.1f)
        {
            if (!currentBlurType.HasFlag(value))
                return;

            currentBlurType &= ~value;

            // valueがカーテンフラグがある場合は、フラグを絞ってカーテンの色を設定する
            if ((value & BlurLayer.ColorCurtain) != BlurLayer.None)
            {
                // カーテンフラグがなくなった場合は、カーテンをフェードアウトさせる
                var colorFlags = currentBlurType & BlurLayer.ColorCurtain;
                bool isAnimation = colorFlags == BlurLayer.None;
                CheckCurtainFlags(colorFlags, isAnimation, duration);
            }

            // valueがブラーフラグがある場合は、フラグを絞ってブラーを設定する
            if ((value & BlurLayer.Blur) != BlurLayer.None)
            {
                // ブラーフラグがなくなった場合は、ブラーをフェードアウトさせる
                var blurFlags = currentBlurType & BlurLayer.Blur;
                bool isAnimation = blurFlags == BlurLayer.None;
                CheckBlurFlags(blurFlags, isAnimation, duration);
            }
        }

        void PlayColorCurtainFadeInAnimation(float duration)
        {
            uiBlurFeature.settings.color.a = 0;
            overlayBlurFeature.settings.color.a = 0;
            DOTween.ToAlpha(() => uiBlurFeature.settings.color, color => uiBlurFeature.settings.color = color, ColorAlpha, duration).SetEase(Ease.InQuad).SetUpdate(true);
            DOTween.ToAlpha(() => overlayBlurFeature.settings.color, color => overlayBlurFeature.settings.color = color, ColorAlpha, duration).SetEase(Ease.InQuad).SetUpdate(true);
        }

        void PlayColorCurtainFadeOutAnimation(float duration)
        {
            uiBlurFeature.settings.color.a = ColorAlpha;
            overlayBlurFeature.settings.color.a = ColorAlpha;
            DOTween.ToAlpha(() => uiBlurFeature.settings.color, color => uiBlurFeature.settings.color = color, 0, duration).SetEase(Ease.OutQuad).SetUpdate(true);
            DOTween.ToAlpha(() => overlayBlurFeature.settings.color, color => overlayBlurFeature.settings.color = color, 0, duration).SetEase(Ease.InQuad).SetUpdate(true);
        }

        void Initialize()
        {
            uiBlurFeature.settings.sigma = Sigma;
            overlayBlurFeature.settings.sigma = Sigma;

            uiBlurFeature.settings.kernelStep = KernelStep;
            overlayBlurFeature.settings.kernelStep = KernelStep;

            isInitialized = true;
        }


        void CheckCurtainFlags(BlurLayer value, bool isAnimation, float duration)
        {
            if (value == BlurLayer.None)
            {
                if (isAnimation)
                {
                    PlayColorCurtainFadeOutAnimation(duration);
                }
            }
            else if (value == BlurLayer.BlackColor)
            {
                uiBlurFeature.settings.color = BlackColor;
                overlayBlurFeature.settings.color = BlackColor;
                if (isAnimation)
                {
                    PlayColorCurtainFadeInAnimation(duration);
                }
            }
            else if (value == BlurLayer.WhiteColor)
            {
                uiBlurFeature.settings.color = WhiteColor;
                overlayBlurFeature.settings.color = WhiteColor;
                if (isAnimation)
                {
                    PlayColorCurtainFadeInAnimation(duration);
                }
            }
        }

        async UniTask PlayBlurFadeInAnimation(float duration)
        {
            // 前までの処理が確認出来たら消してしまって大丈夫です
            // var sequence = DOTween.Sequence();
            // sequence.Join(DOVirtual.Float(MinBlurSize, DefaultBlurAmount, duration, value =>
            // {
            //     grabBlurFeature.settings.blurAmount    = value;
            //     uiBlurFeature.settings.blurAmount      = value;
            //     overlayBlurFeature.settings.blurAmount = value;
            // }).SetEase(Ease.InQuad).SetUpdate(true));
            // sequence.Join(DOVirtual.Float(MinBlurSize, backgroundBlurAmount, duration, value =>
            // {
            //     backgroundBlurFeature.settings.blurAmount = value;
            // }).SetEase(Ease.InQuad).SetUpdate(true));
            // await sequence.AsyncWaitForCompletion();

            await DOVirtual.Float(currentBlurDensityRatio = 0f, 1f, duration, ratio =>
            {
                currentBlurDensityRatio = ratio;
                var defaultAmount = (DefaultBlurAmount - MinBlurSize) * ratio + MinBlurSize;
                uiBlurFeature.settings.blurAmount = defaultAmount;
                overlayBlurFeature.settings.blurAmount = defaultAmount;

                SetBlurDensityRatio(ratio);
            }).SetEase(Ease.InQuad).SetUpdate(true);
        }

        async UniTask PlayBlurFadeOutAnimation(float duration)
        {
            await DOVirtual.Float(currentBlurDensityRatio = 1f, 0f, duration, ratio =>
            {
                currentBlurDensityRatio = ratio;
                SetBlurAmount((DefaultBlurAmount - MinBlurSize) * ratio + MinBlurSize);
                SetBlurDensityRatio(ratio);
            }).SetEase(Ease.OutQuad).SetUpdate(true);
        }

        void SetBlurAmount(float value)
        {
            uiBlurFeature.settings.blurAmount = value;
            overlayBlurFeature.settings.blurAmount = value;
        }

        void SetBlurDensityRatio(float value)
        {
            uiBlurFeature.settings.blurDensityRatio = value;
            overlayBlurFeature.settings.blurDensityRatio = value;
        }

        async void CheckBlurFlags(BlurLayer value, bool isAnimation, float duration)
        {
            if (!isInitialized)
            {
                Initialize();
            }

            if (value == BlurLayer.None)
            {
                if (isAnimation)
                {
                    await PlayBlurFadeOutAnimation(duration);
                }

                overlayBlurFeature.SetActive(false);
                uiBlurFeature.SetActive(false);
            }
            else if (value.HasFlag(BlurLayer.Overlay))
            {
                overlayBlurFeature.SetActive(true);
                uiBlurFeature.SetActive(false);

                if (isAnimation)
                {
                    await PlayBlurFadeInAnimation(duration);
                }
            }
            else if (value.HasFlag(BlurLayer.UI))
            {
                overlayBlurFeature.SetActive(false);
                uiBlurFeature.SetActive(true);

                if (isAnimation)
                {
                    await PlayBlurFadeInAnimation(duration);
                }
            }
            else
            {
                overlayBlurFeature.SetActive(false);
                uiBlurFeature.SetActive(false);
            }
        }

#if UNITY_EDITOR
        void ForceDisable()
        {
            SetBlurAmount(DefaultBlurAmount);
            currentBlurType = BlurLayer.None;
            overlayBlurFeature.SetActive(false);
            uiBlurFeature.SetActive(false);
        }

        [InitializeOnLoad]
        public static class PlayStateNotifier
        {
            static PlayStateNotifier()
            {
                EditorApplication.playModeStateChanged += ModeChanged;
            }

            static void ModeChanged(PlayModeStateChange playModeState)
            {
                if (playModeState == PlayModeStateChange.ExitingPlayMode)
                {
                    Debug.Log("Exiting Play mode.");
                    Instance.ForceDisable();
                }
            }
        }
#endif
    }
}