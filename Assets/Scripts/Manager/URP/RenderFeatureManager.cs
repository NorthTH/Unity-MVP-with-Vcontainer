// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Rendering;
// using Cysharp.Threading.Tasks;
// using DG.Tweening;

// namespace Manager.Rendering
// {
//     [Flags]
//     public enum BlurLayer
//     {
//         None = 0,
//         Grab = 1,
//         UI = 1 << 1,
//         Overlay = 1 << 2,
//         All = UI | Grab | Overlay,
//     }

//     public interface IRenderFeatureManager
//     {
//         public void SetEnableBlur(BlurLayer value);
//         public void SetDisableBlur(BlurLayer value);
//     }

//     public class RenderFeatureManager : MonoBehaviour, IRenderFeatureManager
//     {
//         // #if UNITY_EDITOR
//         //         static RenderFeatureManager Instance;
//         // #endif
//         [SerializeField]
//         GrabBlurTargetLayerFeature grabBlurFeature;
//         [SerializeField]
//         BlurTargetLayerFeature uiBlurFeature;
//         [SerializeField]
//         BlurTargetLayerFeature overlayBlurFeature;
//         [SerializeField, Range(0, 5)]
//         float blurSize = 2.0f;
//         [SerializeField, Range(0, 1)]
//         float duration = 0.1f;

//         BlurLayer currentBlurLayer = BlurLayer.None;

//         // #if UNITY_EDITOR
//         //         public RenderFeatureManager()
//         //         {
//         //             Instance = this;
//         //         }
//         // #endif

//         public void SetEnableBlur(BlurLayer value)
//         {
//             if (currentBlurLayer.HasFlag(value))
//                 return;

//             currentBlurLayer = currentBlurLayer | value;

//             CheckFlags();
//         }

//         public void SetDisableBlur(BlurLayer value)
//         {
//             if (!currentBlurLayer.HasFlag(value))
//                 return;

//             currentBlurLayer = currentBlurLayer & ~value;

//             CheckFlags();
//         }


//         async void CheckFlags()
//         {
//             if (currentBlurLayer == BlurLayer.None)
//             {
//                 await DOVirtual.Float(blurSize, 0f, duration, value =>
//                 {
//                     // grabBlurFeature.settings.blurSize = value;
//                     uiBlurFeature.settings.blurSize = value;
//                     overlayBlurFeature.settings.blurSize = value;
//                 }).SetEase(Ease.Linear);
//             }

//             if (currentBlurLayer.HasFlag(BlurLayer.Overlay))
//             {
//                 grabBlurFeature.settings.blurSize = 0;
//                 overlayBlurFeature.SetActive(true);
//                 uiBlurFeature.SetActive(false);
//                 // grabBlurFeature.SetActive(false);
//             }

//             else if (currentBlurLayer.HasFlag(BlurLayer.UI))
//             {
//                 grabBlurFeature.settings.blurSize = 0;
//                 overlayBlurFeature.SetActive(false);
//                 uiBlurFeature.SetActive(true);
//                 // grabBlurFeature.SetActive(false);
//             }
//             else if (currentBlurLayer.HasFlag(BlurLayer.Grab))
//             {
//                 grabBlurFeature.settings.blurSize = blurSize;
//                 overlayBlurFeature.SetActive(false);
//                 uiBlurFeature.SetActive(false);
//                 // grabBlurFeature.SetActive(true);
//             }
//             else
//             {
//                 grabBlurFeature.settings.blurSize = 0;
//                 overlayBlurFeature.SetActive(false);
//                 uiBlurFeature.SetActive(false);
//                 // grabBlurFeature.SetActive(false);
//             }

//             if (currentBlurLayer != BlurLayer.None)
//             {
//                 await DOVirtual.Float(0f, blurSize, duration, value =>
//                 {
//                     uiBlurFeature.settings.blurSize = value;
//                     overlayBlurFeature.settings.blurSize = value;
//                 }).SetEase(Ease.Linear);
//             }
//         }
//     }
// }
