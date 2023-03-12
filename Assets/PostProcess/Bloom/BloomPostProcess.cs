using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(BloomPostProcessRender), PostProcessEvent.BeforeStack, "Custom/BloomPostProcess")]
public class BloomPostProcess : PostProcessEffectSettings
{

}

public sealed class BloomPostProcessRender : PostProcessEffectRenderer<BloomPostProcess>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Custom/BloomPostProcess"));

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}