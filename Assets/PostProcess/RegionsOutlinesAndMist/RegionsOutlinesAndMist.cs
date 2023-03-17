using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;

[Serializable]
[PostProcess(typeof(RegionsOutlinesAndMistRenderer), PostProcessEvent.BeforeStack, "Custom/RegionsOutlinesAndMistShader")]
public class RegionsOutlinesAndMist : PostProcessEffectSettings
{
    public IntParameter iterations = new IntParameter { value = 1 };
}

public sealed class RegionsOutlinesAndMistRenderer : PostProcessEffectRenderer<RegionsOutlinesAndMist>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Custom/RegionsOutlinesAndMistShader"));

        Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        RenderTexture noShaderTexture = camera.GetComponent<NoRegionShaderTextureScript>().noShaderTexture;


        int width = noShaderTexture.width;
        int height = noShaderTexture.height;

        RenderTexture currentSource = RenderTexture.GetTemporary(width, height, 0); ;
        Graphics.Blit(noShaderTexture, currentSource);
        
        for (int i = 0; i < settings.iterations; i++)
        {
            width /= 2;
            height /= 2;
            RenderTexture currentDestination = RenderTexture.GetTemporary(width, height, 0);
            Graphics.Blit(currentSource, currentDestination);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDestination;
        }

        sheet.properties.SetTexture("_NoShaderTexture", currentSource);
        RenderTexture.ReleaseTemporary(currentSource);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
