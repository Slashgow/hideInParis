using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class BlackReplacementFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        [Header("Replacement Color")]
        public Color replacementColor = Color.red;

        [Header("Black Detection")]
        [Range(0f, 1f)]
        [Tooltip("Maximum brightness to be considered 'black' (0 = pure black only, higher = darker grays included)")]
        public float brightnessThreshold = 0.1f;

        [Range(0f, 1f)]
        [Tooltip("Soften the edge of the detection. 0 = hard cut, higher = smooth blend")]
        public float edgeSoftness = 0.05f;

        [Range(0f, 1f)]
        [Tooltip("Maximum saturation allowed. Keeps highly-saturated dark colors from being replaced")]
        public float maxSaturation = 0.2f;

        [Header("Rendering")]
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }
    [SerializeField] private Shader _shader;

    public Settings settings = new Settings();

    private BlackReplacementPass _pass;
    private Material _material;

    public override void Create()
    {
        if (_shader == null)
        {
            Debug.LogError("[BlackReplacementFeature] Shader not assigned.");
            return;
        }
        _material = CoreUtils.CreateEngineMaterial(_shader); // use CoreUtils instead of new Material()
        _pass = new BlackReplacementPass(_material, settings);
        _pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_material == null || _pass == null) return;
        _pass.UpdateSettings(settings);
        renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing)
    {
        if (_material != null)
            CoreUtils.Destroy(_material);
    }
}

public class BlackReplacementPass : ScriptableRenderPass
{
    private Material _material;
    private BlackReplacementFeature.Settings _settings;

    private static readonly int ReplacementColorID = Shader.PropertyToID("_ReplacementColor");
    private static readonly int BrightnessThreshID = Shader.PropertyToID("_BrightnessThreshold");
    private static readonly int EdgeSoftnessID = Shader.PropertyToID("_EdgeSoftness");
    private static readonly int MaxSaturationID = Shader.PropertyToID("_MaxSaturation");

    // Passdata holds references for the RenderGraph lambda
    private class PassData
    {
        public Material material;
        public TextureHandle source;
    }

    public BlackReplacementPass(Material material, BlackReplacementFeature.Settings settings)
    {
        _material = material;
        _settings = settings;
        // Required in RenderGraph mode: declare we read+write the camera color
        requiresIntermediateTexture = true;
    }

    public void UpdateSettings(BlackReplacementFeature.Settings settings)
    {
        _settings = settings;
        _material.SetColor(ReplacementColorID, settings.replacementColor);
        _material.SetFloat(BrightnessThreshID, settings.brightnessThreshold);
        _material.SetFloat(EdgeSoftnessID, settings.edgeSoftness);
        _material.SetFloat(MaxSaturationID, settings.maxSaturation);
    }

    // ----------------------------------------------------------------
    // RenderGraph path (Unity 6 / URP 17+)
    // ----------------------------------------------------------------
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (_material == null) return;

        var resourceData = frameData.Get<UniversalResourceData>();
        if (!resourceData.activeColorTexture.IsValid()) return;

        var sourceDesc = renderGraph.GetTextureDesc(resourceData.activeColorTexture);
        sourceDesc.name = "_BlackReplacementTemp";
        sourceDesc.clearBuffer = false;
        sourceDesc.depthBufferBits = 0;

        TextureHandle tempTex = renderGraph.CreateTexture(sourceDesc);

        // --- Pass 1: camera ? temp via replacement shader ---
        using (var builder = renderGraph.AddRasterRenderPass<PassData>("BlackReplacement_Blit", out var passData))
        {
            passData.material = _material;
            passData.source = resourceData.activeColorTexture;

            builder.UseTexture(passData.source, AccessFlags.Read);
            builder.SetRenderAttachment(tempTex, 0, AccessFlags.Write);

            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                Blitter.BlitTexture(ctx.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
            });
        }

        // --- Pass 2: temp ? camera, plain copy using Blitter's own copy material ---
        using (var builder = renderGraph.AddRasterRenderPass<PassData>("BlackReplacement_CopyBack", out var passData))
        {
            passData.material = null; // unused in this pass
            passData.source = tempTex;

            builder.UseTexture(passData.source, AccessFlags.Read);
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);

            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                // BlitTexture overload that takes no material — uses the internal copy shader
                Blitter.BlitTexture(ctx.cmd, data.source, new Vector4(1, 1, 0, 0), 0, false);
            });
        }
    }

    // ----------------------------------------------------------------
    // Compatibility / non-RenderGraph path (URP 14–16, or if
    // RenderGraph is disabled in Project Settings)
    // ----------------------------------------------------------------
    private RTHandle _tempRT;

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        var desc = renderingData.cameraData.cameraTargetDescriptor;
        desc.depthBufferBits = 0;
        RenderingUtils.ReAllocateHandleIfNeeded(ref _tempRT, desc, name: "_BlackReplacementTemp");
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (_material == null) return;

        var cmd = CommandBufferPool.Get("BlackReplacement");
        var cameraTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

        Blitter.BlitCameraTexture(cmd, cameraTarget, _tempRT, _material, 0);
        Blitter.BlitCameraTexture(cmd, _tempRT, cameraTarget);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        // RTHandle lifetime managed by ReAllocateIfNeeded
    }
}