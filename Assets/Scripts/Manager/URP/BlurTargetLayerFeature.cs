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
        [HideInInspector]
        public float blurSize = 3.0f;
    }

    class BlurTargetLayerPass : ScriptableRenderPass
    {
        private const string ProfilerTag = nameof(BlurTargetLayerPass);

        RenderTargetHandle tempColorTarget1;
        RenderTargetHandle tempColorTarget2;
        Settings settings;

        ScriptableRenderer renderer;
        RenderTargetIdentifier cameraTarget;
        Material material;

        public BlurTargetLayerPass(Settings Settings, RenderPassEvent RenderPassEvent)
        {
            settings = Settings;
            renderPassEvent = RenderPassEvent;
            material = (settings.shader != null) ? new Material(settings.shader) : default;
        }

        public void Setup(ScriptableRenderer renderer)
        {
            tempColorTarget1.Init("_TempRT1");
            tempColorTarget2.Init("_TempRT2");
            this.renderer = renderer;

            if (material != default)
            {
                material.SetFloat("_Factor", settings.blurSize);
                material.SetFloat("_Factor_Y", settings.blurSize);
                material.SetFloat("_Range", 0);
                material.SetFloat("_Range_Y", 0);
                material.SetFloat("_Darkness", 0);
            }
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(tempColorTarget1.id, cameraTextureDescriptor);
            cmd.GetTemporaryRT(tempColorTarget2.id, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            this.cameraTarget = renderer.cameraColorTarget;

            CommandBuffer cmd = CommandBufferPool.Get(ProfilerTag);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            var temp1 = tempColorTarget1.Identifier();
            var temp2 = tempColorTarget2.Identifier();
            Blit(cmd, cameraTarget, temp1, material, 0);
            Blit(cmd, temp1, temp2, material, 1);
            Blit(cmd, temp2, cameraTarget);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempColorTarget1.id);
            cmd.ReleaseTemporaryRT(tempColorTarget2.id);
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