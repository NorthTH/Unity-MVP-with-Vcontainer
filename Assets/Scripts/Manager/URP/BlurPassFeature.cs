using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurPassFeature : ScriptableRendererFeature
{
    class BlurRenderPass : ScriptableRenderPass
    {
        private const string ProfilerTag = nameof(BlurRenderPass);

        int _ScaleDown;
        int _BlurSize;

        private int _blurredID1 = 0;
        private int _blurredID2 = 0;
        private int _blurSizeID = 0;

        private readonly Material material;

        // 描画対象をハンドリングする
        private RenderTargetHandle tmpRenderTargetHandle;
        private RenderTargetIdentifier cameraColorTarget;


        public BlurRenderPass(Shader shader, int scaleDown, int blurSize)
        {
            material = CoreUtils.CreateEngineMaterial(shader);
            renderPassEvent = RenderPassEvent.AfterRendering;
            tmpRenderTargetHandle.Init("_TempRT");

            _ScaleDown = scaleDown;
            _BlurSize = blurSize;

            _blurredID1 = Shader.PropertyToID("_Temp1");
            _blurredID2 = Shader.PropertyToID("_Temp2");
            _blurSizeID = Shader.PropertyToID("_BlurSize");
        }

        public void SetRenderTarget(RenderTargetIdentifier target)
        {
            cameraColorTarget = target;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isSceneViewCamera)
            {
                return;
            }

            // コマンドバッファの生成
            var cmd = CommandBufferPool.Get(ProfilerTag);

            var cameraData = renderingData.cameraData;
            // 現在描画しているカメラの解像度を　「_downSample」で除算  
            var w = cameraData.camera.scaledPixelWidth / _ScaleDown;
            var h = cameraData.camera.scaledPixelHeight / _ScaleDown;

            // RenderTextureDescriptorの取得
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            // 今回深度は不要なので0に
            // descriptor.depthBufferBits = 0;

            cmd.SetGlobalFloat(_blurSizeID, _BlurSize);

            cmd.GetTemporaryRT(_blurredID1, w, h, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(_blurredID2, w, h, 0, FilterMode.Bilinear);

            cmd.GetTemporaryRT(tmpRenderTargetHandle.id, descriptor);
            Blit(cmd, cameraColorTarget, tmpRenderTargetHandle.Identifier());

            Blit(cmd, tmpRenderTargetHandle.Identifier(), _blurredID1, material);
            Blit(cmd, _blurredID1, _blurredID2, material);

            Blit(cmd, _blurredID2, cameraColorTarget);
            cmd.ReleaseTemporaryRT(tmpRenderTargetHandle.id);
            cmd.ReleaseTemporaryRT(_blurredID1);
            cmd.ReleaseTemporaryRT(_blurredID2);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    [SerializeField]
    private Shader shader;

    [SerializeField, Range(1, 10)]
    private int _ScaleDown = 1;
    [SerializeField, Range(1, 100)]
    private int _BlurSize = 1;

    private BlurRenderPass blurRenderPass;

    // 初期化
    public override void Create()
    {
        blurRenderPass = new BlurRenderPass(shader, _ScaleDown, _BlurSize);
    }

    // 1つ、または複数のパスを追加する
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        blurRenderPass.SetRenderTarget(renderer.cameraColorTarget);
        renderer.EnqueuePass(blurRenderPass);
    }
}


