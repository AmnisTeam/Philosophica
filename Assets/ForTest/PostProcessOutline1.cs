using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessOutlineRenderer1), PostProcessEvent.BeforeStack, "Roystan/Post Process Outline")]
public sealed class PostProcessOutline1 : PostProcessEffectSettings
{
    public IntParameter scale = new IntParameter { value = 1 };
    public FloatParameter depthTreshold = new FloatParameter { value = 0.2f };
    public FloatParameter intensity = new FloatParameter { value = 0.2f };
}

public sealed class PostProcessOutlineRenderer1 : PostProcessEffectRenderer<PostProcessOutline1>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Roystan/Outline Post Process"));
        sheet.properties.SetFloat("_Scale", settings.scale);
        sheet.properties.SetFloat("_DepthThreshold", settings.depthTreshold);
        sheet.properties.SetFloat("_Intensity", settings.intensity);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}