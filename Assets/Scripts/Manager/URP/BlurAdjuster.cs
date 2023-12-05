using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BlurAdjuster
{

    public static float Sigma = 5.137f;   //2.446 ~ 5.137
    public static float DefaultBlurAmount = 3.919f;   //2.659 ~ 3.919
    public static uint KernelStep = 18;    //13 ~ 18

    private const float SigmaMin = 2.446f;
    private const float SigmaMax = 5.137f;
    private const float BlurAmountMin = 2.659f;
    private const float BlurAmountMax = 3.919f;
    private const uint KernelStepMin = 13;
    private const uint KernelStepMax = 18;
    private const float FpsBad = 29;
    private const float FpsOK = 45;

    private int frameCount = 0;
    private const int adjustFrameInterval = 30;
    private float deltaTime = 0;
    private float fps = 0;
    private AdjustState adjustState = AdjustState.None;
    private enum AdjustState
    {
        Weaken,
        Strengthen,
        None
    }

    public BlurAdjuster()
    {
    }

    public bool AdjustBlur()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
        frameCount++;
        frameCount %= adjustFrameInterval;
        if(frameCount != 0)
        {
            return false;
        }
        if(fps < FpsBad)
        {
            if(adjustState == AdjustState.Strengthen)
            {
                return false;
            }
            return Weaken();
        }
        else if(fps > FpsOK)
        {
            if(adjustState == AdjustState.Weaken)
            {
                return false;
            }
            return Strengthen();
        }
        adjustState = AdjustState.None;
        return false;
    }

    private bool Weaken()
    {
        if(KernelStep <= KernelStepMin)
        {
            return false;
        }
        KernelStep--;
        Adjust().Forget();
        adjustState = AdjustState.Weaken;
        return true;
    }

    private bool Strengthen()
    {
        if(KernelStep >= KernelStepMax)
        {
            return false;
        }
        KernelStep++;
        Adjust().Forget();
        adjustState = AdjustState.Strengthen;
        return true;
    }

    private async UniTaskVoid Adjust()
    {
        int count = 0;
        float targetSigma = Mathf.Lerp(SigmaMin, SigmaMax, (KernelStep - KernelStepMin) / (float)(KernelStepMax - KernelStepMin));
        float targetDefaultBlurAmount = Mathf.Lerp(BlurAmountMin, BlurAmountMax, (KernelStep - KernelStepMin) / (float)(KernelStepMax - KernelStepMin));
        while(count < adjustFrameInterval)
        {
            Sigma = Mathf.Lerp(Sigma, targetSigma, count / (float)adjustFrameInterval);
            DefaultBlurAmount = Mathf.Lerp(DefaultBlurAmount, targetDefaultBlurAmount, count / (float)adjustFrameInterval);
            await UniTask.DelayFrame(1);
            count++;
        }
    }
}
