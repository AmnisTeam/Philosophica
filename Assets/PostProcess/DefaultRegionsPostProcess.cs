using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(DefaultRegionsPostProcessRenderer), PostProcessEvent.BeforeStack, "Custom/Region/DefaultRegionShader")]
public sealed class DefaultRegionsPostProcess : PostProcessEffectSettings
{
    public FloatParameter depthThreshold = new FloatParameter { value = 0.2f };
    public FloatParameter thickness = new FloatParameter { value = 1f };
    public FloatParameter depthMin = new FloatParameter { value = 0f };
    public FloatParameter depthMax = new FloatParameter { value = 1f };
}

public sealed class DefaultRegionsPostProcessRenderer : PostProcessEffectRenderer<DefaultRegionsPostProcess>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Custom/Region/DefaultRegionShader"));

        sheet.properties.SetFloat("_DepthThreshold", settings.depthThreshold);
        sheet.properties.SetFloat("_Thickness", settings.thickness);
        sheet.properties.SetFloat("_MinDepth", settings.depthMin);
        sheet.properties.SetFloat("_MaxDepth", settings.depthMax);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}