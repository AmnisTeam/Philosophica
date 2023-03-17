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

        Shader boxSamplingShader = Shader.Find("Custom/BoxSamplingShader");
        Material boxSamplingMaterial = new Material(boxSamplingShader);
        RenderTexture[] textures = new RenderTexture[16];

        int width = noShaderTexture.width;
        int height = noShaderTexture.height;

        textures[0] = RenderTexture.GetTemporary(width, height, 0);
        RenderTexture currentSource = textures[0];
        Graphics.Blit(noShaderTexture, currentSource, boxSamplingMaterial);

        int lastIteration = 0;
        
        for (int i = 1; i < settings.iterations; i++)
        {
            width /= 2;
            height /= 2;

            if (width < 2 || height < 2)
                break;

            lastIteration = i;

            textures[i] = RenderTexture.GetTemporary(width, height, 0);
            Graphics.Blit(currentSource, textures[i], boxSamplingMaterial);
            currentSource = textures[i];
        }

        for (int i = lastIteration - 1; i >= 0; i--)
        {
            RenderTexture currentDestination = textures[i];
            textures[i] = null;
            Graphics.Blit(currentSource, currentDestination, boxSamplingMaterial);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDestination;
        }


        sheet.properties.SetTexture("_RegionsColorsTexture", noShaderTexture);
        sheet.properties.SetTexture("_NoShaderTexture", currentSource);
        RenderTexture.ReleaseTemporary(currentSource);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
