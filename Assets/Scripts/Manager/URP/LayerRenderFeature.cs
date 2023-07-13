using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

public class LayerRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class FilterSettings
    {
        // TODO: expose opaque, transparent, all ranges as drop down
        public RenderQueueType RenderQueueType;
        [Range(0, 49)]
        public int SortLayer;
        public LayerMask LayerMask;
        public string[] PassNames;

        public FilterSettings()
        {
            RenderQueueType = RenderQueueType.Transparent;
            LayerMask = 0;
        }
    }

    class LayerRenderPass : ScriptableRenderPass
    {
        RenderQueueType renderQueueType;
        FilteringSettings m_FilteringSettings;
        RenderObjects.CustomCameraSettings m_CameraSettings;
        string m_ProfilerTag;
        ProfilingSampler m_ProfilingSampler;

        public Material overrideMaterial { get; set; }
        public int overrideMaterialPassIndex { get; set; }

        List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
        List<ShaderTagId> m_ShaderTagIdList_2 = new List<ShaderTagId>();

        RenderStateBlock m_RenderStateBlock;

        public LayerRenderPass(string profilerTag, RenderPassEvent renderPassEvent, string[] shaderTags, RenderQueueType renderQueueType, int layerMask)
        {
            base.profilingSampler = new ProfilingSampler(nameof(RenderObjectsPass));

            m_ProfilerTag = profilerTag;
            m_ProfilingSampler = new ProfilingSampler(profilerTag);
            this.renderPassEvent = renderPassEvent;
            this.renderQueueType = renderQueueType;
            this.overrideMaterial = null;
            this.overrideMaterialPassIndex = 0;
            RenderQueueRange renderQueueRange = (renderQueueType == RenderQueueType.Transparent)
                ? RenderQueueRange.transparent
                : RenderQueueRange.opaque;
            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);

            m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
            m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));

            foreach (var passName in shaderTags)
                m_ShaderTagIdList_2.Add(new ShaderTagId(passName));

            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            SortingCriteria sortingCriteria = (renderQueueType == RenderQueueType.Transparent)
                ? SortingCriteria.CommonTransparent
                : renderingData.cameraData.defaultOpaqueSortFlags;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                //TODO : LoopしないとSRP Batcher 適用できない、LoopするとLightModeの各Tag同タイミング描画用できない
                DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings,
                        ref m_RenderStateBlock);

                for (int i = 0; i < m_ShaderTagIdList_2.Count; i++)
                {
                    DrawingSettings drawingSettings2 = CreateDrawingSettings(m_ShaderTagIdList_2[i], ref renderingData, sortingCriteria);
                    context.DrawRenderers(renderingData.cullResults, ref drawingSettings2, ref m_FilteringSettings,
                            ref m_RenderStateBlock);
                }
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    List<LayerRenderPass> m_ScriptablePassList;

    [SerializeField] string passTag = "LayerRenderFeature";
    [SerializeField] RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    [SerializeField] FilterSettings[] filterSettings;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePassList = new List<LayerRenderPass>();
        foreach (var filter in filterSettings)
        {
            var m_ScriptablePass = new LayerRenderPass(passTag, RenderPassEvent + filter.SortLayer, filter.PassNames,
                filter.RenderQueueType, filter.LayerMask);
            m_ScriptablePassList.Add(m_ScriptablePass);
        }
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        foreach (var m_ScriptablePass in m_ScriptablePassList)
        {
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}


