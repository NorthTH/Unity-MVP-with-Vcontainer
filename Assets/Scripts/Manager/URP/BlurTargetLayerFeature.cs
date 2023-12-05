using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurTargetLayerFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Shader shader;
        public float sigma = 5.0f;              // 使用していない
        public float blurAmount = 2.0f;         // 使用していない
        public float blurDensityRatio = 0f;
        public Color color = new Color(0, 0, 0, 0);
        [Range(0, 150)]
        public uint kernelStep = 9;
    }

    class BlurTargetLayerPass : ScriptableRenderPass
    {
        private const string ProfilerTag = nameof(BlurTargetLayerPass);

        RenderTargetHandle tempColorTarget;
        Settings settings;

        ScriptableRenderer renderer;
        RenderTargetIdentifier cameraTarget;
        Material material;
        private BlurAdjuster blurAdjuster = new BlurAdjuster();

        public BlurTargetLayerPass(Settings Settings, RenderPassEvent RenderPassEvent)
        {
            settings = Settings;
            renderPassEvent = RenderPassEvent;
            material = new Material(settings.shader);
        }

        public void Setup(ScriptableRenderer renderer)
        {
            tempColorTarget.Init("_TempRT");
            this.renderer = renderer;

            if (material != default)
            {
                material.SetFloat("_Sigma", BlurAdjuster.Sigma);
                material.SetFloat("_BlurAmount", BlurAdjuster.DefaultBlurAmount);
                material.SetFloat("_KernelStep", BlurAdjuster.KernelStep);
                material.SetColor("_Color", settings.color);
                material.SetFloat("_BlurDensityRatio", settings.blurDensityRatio);
            }
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(tempColorTarget.id, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // blurAdjuster.AdjustBlur();

            // Setup()で行っている
            // material.SetFloat("_Sigma", BlurAdjuster.Sigma);
            // material.SetFloat("_BlurAmount", BlurAdjuster.DefaultBlurAmount);  //TODO: fade考慮
            // material.SetFloat("_KernelStep", BlurAdjuster.KernelStep);
            // material.SetFloat("_BlurDensityRatio", settings.blurDensityRatio);

            this.cameraTarget = renderer.cameraColorTarget;

            CommandBuffer cmd = CommandBufferPool.Get(ProfilerTag);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            var temp = tempColorTarget.Identifier();
            Blit(cmd, cameraTarget, temp, material, 0);
            Blit(cmd, temp, cameraTarget, material, 1);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempColorTarget.id);
        }
    }

    BlurTargetLayerPass grabPass;
    [SerializeField] public Settings settings;
    [SerializeField] RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    [SerializeField, Range(0, 49)] int OrderInSortLayer = 0;

    public override void Create()
    {
        grabPass = new BlurTargetLayerPass(settings, renderPassEvent + OrderInSortLayer);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        grabPass.Setup(renderer);
        renderer.EnqueuePass(grabPass);
    }
}