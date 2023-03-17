using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class NoRegionShaderTextureScript : MonoBehaviour
{
    public RenderTexture noShaderTexture;
    public RegionsSystem regionsSystem;
    private int renderTextureDepth = 24;
    void Start()
    {
        noShaderTexture = new RenderTexture(Screen.width, Screen.height, renderTextureDepth);
    }

    void Update()
    {
        // Saving old parametors
        RenderTexture defaultTexture = GetComponent<Camera>().targetTexture;
        int oldMask = GetComponent<Camera>().cullingMask;

        // Setting parametors for renderTexture
        SyncRenderTexture(renderTextureDepth);
        GetComponent<Camera>().targetTexture = noShaderTexture;
        SetDrawOnlyColorToAllRegions(true);
        GetComponent<PostProcessVolume>().enabled = false;
        GetComponent<Camera>().cullingMask = 1 << LayerMask.NameToLayer("Regions");

        // Rendering
        GetComponent<Camera>().Render();

        // Restoring old parametors;
        GetComponent<Camera>().targetTexture = defaultTexture;
        SetDrawOnlyColorToAllRegions(false);
        GetComponent<PostProcessVolume>().enabled = true;
        GetComponent<Camera>().cullingMask = oldMask;
    }

    public void SetDrawOnlyColorToAllRegions(bool state)
    {
        float value = state ? 1 : 0;
        for (int i = 0; i < regionsSystem.regionSerds.Count; i++)
            regionsSystem.regionSerds[i].region.GetComponent<Renderer>().materials[0].SetFloat("_DrawOnlyColor", value);
    }

    public void SyncRenderTexture(int depth)
    {
        if (noShaderTexture.width != Screen.width || noShaderTexture.height != Screen.height)
        {
            noShaderTexture = new RenderTexture(Screen.width, Screen.height, depth);
        }
    }
}
