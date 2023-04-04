using JetBrains.Annotations;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[System.Serializable]
public class Region : MonoBehaviour
{
    float aspect = 0;
    public Player hostPlayer = null;
    private float selectionOffset = 0;
    public GameplayManager gameplayManager;

    public void Init(RegionsSystem regionsSystem, GameplayManager gameplayManager)
    {
        gameObject.layer = LayerMask.NameToLayer("Regions");
        SetColor(regionsSystem.nextRegionColor);
        SetOutlineColor(new Color(0, 0, 0, 0));
        SetInnerGlowColor(new Color(0, 0, 0, 0));
        this.gameplayManager = gameplayManager;
    }

    public void SetColor(UnityEngine.Color color)
    {
        if (gameObject.GetComponent<Renderer>().sharedMaterial != null)
        {
            Material material = gameObject.GetComponent<Renderer>().materials[0];
            material.SetColor("_RegionColor", color);
        }
        else
            Debug.LogError("Region instance has no material component with RegionShader shader", this);
    }

    public UnityEngine.Color GetColor(UnityEngine.Color color)
    {
        if (GetComponent<Renderer>().materials[0] != null)
        {
            Material material = GetComponent<Renderer>().materials[0];
            return material.GetColor("_RegionColor");
        }
        else
            Debug.LogError("Region instance has no material component with RegionShader shader", this);
        return new UnityEngine.Color(255, 0, 255);
    }

    public void UpdateMesh(List<Vector3> points)
    {
        if (GetComponent<MeshFilter>().sharedMesh == null)
            GetComponent<MeshFilter>().sharedMesh = new Mesh();

        aspect = Triangulator.Triangulate(points, GetComponent<MeshFilter>().sharedMesh);
        GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
    }

    public void SetOutlineColorToRegionColor()
    {
        Material material = gameObject.GetComponent<Renderer>().materials[0];
        Color regionColor = material.GetColor("_RegionColor");

        material.SetColor("_OutlineColor", regionColor);
    }

    public void SetOutlineColor(Color color)
    {
        gameObject.GetComponent<Renderer>().materials[0].SetColor("_OutlineColor", color);
    }

    public void SetInnerGlowColor(Color color)
    {
        gameObject.GetComponent<Renderer>().materials[0].SetColor("_InnerGlowColor", color);
    }

    public void SetSelectionColor(Color color)
    {
        gameObject.GetComponent<Renderer>().materials[0].SetColor("_SelectionColor", color);
    }

    public void SetSelectionOffset(float offset)
    {
        gameObject.GetComponent<Renderer>().materials[0].SetFloat("_SelectionOffset", offset);
    }


    public void GraduallyChangeColor(Color color, float time)
    {
        Color regionColor = gameObject.GetComponent<Renderer>().materials[0].GetColor("_RegionColor");
        LeanTween.value(0, 1, time).setOnUpdate((float val) =>
        {
            Color currentColor = Color.Lerp(regionColor, color, val);
            SetColor(currentColor);
        }).setEaseOutSine();
    }

    public void GraduallyChangeOutlineColor(Color color, float time)
    {
        Color outlineColor = gameObject.GetComponent<Renderer>().materials[0].GetColor("_OutlineColor");
        LeanTween.value(0, 1, time).setOnUpdate((float val) =>
        {
            Color currentColor = Color.Lerp(outlineColor, color, val);
            SetOutlineColor(currentColor);
        }).setEaseOutSine();
    }

    public void GraduallyChangeInnerGlowColor(Color color, float time)
    {
        Color innerGlowColor = gameObject.GetComponent<Renderer>().materials[0].GetColor("_InnerGlowColor");
        LeanTween.value(0, 1, time).setOnUpdate((float val) =>
        {
            Color currentColor = Color.Lerp(innerGlowColor, color, val);
            SetInnerGlowColor(currentColor);
        }).setEaseOutSine();
    }

    public void GraduallyChangeSelectionColor(Color color, float time)
    {
        Color selectionColor = gameObject.GetComponent<Renderer>().materials[0].GetColor("_SelectionColor");
        LeanTween.value(0, 1, time).setOnUpdate((float val) =>
        {
            Color currentColor = Color.Lerp(selectionColor, color, val);
            SetSelectionColor(currentColor);
        }).setEaseOutSine();
    }

    public void UpdateSelectionMove()
    {
        selectionOffset += gameplayManager.selectionOffsetSpeed * Time.deltaTime;
        SetSelectionOffset(selectionOffset);
    }

    public void UpdateShaderAspectAndWidth()
    {
        Vector3 size = GetComponent<Renderer>().bounds.size;
        GetComponent<Renderer>().materials[0].SetFloat("_Aspect", size.y / size.x);
        GetComponent<Renderer>().materials[0].SetFloat("_Width", size.x);
    }

    public void Awake()
    {
        hostPlayer = null;
        SetOutlineColorToRegionColor();
    }


    //float time = 4;
    //float timer = 0;

    public void Start()
    {
        
    }


    public void Update()
    {
        UpdateShaderAspectAndWidth();
        UpdateSelectionMove();

        //timer += Time.deltaTime;
        //if (timer >= time)
        //{
        //    GraduallyChangeOutlineColor(new Color(1, 1, 1), 2);
        //    GraduallyChangeInnerGlowColor(new Color(1, 1, 1), 2);
        //    timer = float.NaN;
        //}
    }
}
