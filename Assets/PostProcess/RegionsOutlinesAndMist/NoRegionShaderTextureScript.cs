using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class NoRegionShaderTextureScript : MonoBehaviour
{
    public RenderTexture noShaderTexture;
    public RenderTexture outlineTexture;
    public RegionsSystem regionsSystem;
    private int renderTextureDepth = 24;
    void Start()
    {
        noShaderTexture = new RenderTexture(Screen.width, Screen.height, renderTextureDepth);
        outlineTexture = new RenderTexture(Screen.width, Screen.height, renderTextureDepth);
    }

    void Update()
    {
        // Saving old parametors
        RenderTexture defaultTexture = GetComponent<Camera>().targetTexture;
        int oldMask = GetComponent<Camera>().cullingMask;

        // Setting parametors for renderTexture
        SyncRenderTexture(noShaderTexture, renderTextureDepth);
        GetComponent<Camera>().targetTexture = noShaderTexture;
        SetDrawOnlyColorToAllRegions(true);
        GetComponent<PostProcessVolume>().enabled = false;
        GetComponent<Camera>().cullingMask = 1 << LayerMask.NameToLayer("Regions");

        // Rendering
        GetComponent<Camera>().Render();

        SyncRenderTexture(outlineTexture, renderTextureDepth);
        GetComponent<Camera>().targetTexture = outlineTexture;
        SetDrawOutlineColorToAllRegions(true);

        // Rendering
        GetComponent<Camera>().Render();

        // Restoring old parametors;
        GetComponent<Camera>().targetTexture = defaultTexture;
        SetDrawOnlyColorToAllRegions(false);
        SetDrawOutlineColorToAllRegions(false);
        GetComponent<PostProcessVolume>().enabled = true;
        GetComponent<Camera>().cullingMask = oldMask;
    }

    public void SetDrawOnlyColorToAllRegions(bool state)
    {
        float value = state ? 1 : 0;
        for (int i = 0; i < regionsSystem.regionSerds.Count; i++)
            regionsSystem.regionSerds[i].region.GetComponent<Renderer>().materials[0].SetFloat("_DrawOnlyColor", value);
    }

    public void SetDrawOutlineColorToAllRegions(bool state)
    {
        float value = state ? 1 : 0;
        for (int i = 0; i < regionsSystem.regionSerds.Count; i++)
            regionsSystem.regionSerds[i].region.GetComponent<Renderer>().materials[0].SetFloat("_DrawOutlineColor", value);
    }

    public void SyncRenderTexture(RenderTexture renderTexture, int depth)
    {
        if (renderTexture.width != Screen.width || renderTexture.height != Screen.height)
        {
            renderTexture = new RenderTexture(Screen.width, Screen.height, depth);
        }
    }
}
