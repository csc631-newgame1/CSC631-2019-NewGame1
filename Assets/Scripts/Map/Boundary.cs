using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public class Boundary : MonoBehaviour
{
    public void generateBoundary()
    {
        Material mat = GetComponent<Renderer>().material;
        Material surfacemat = transform.parent.Find("Surface").GetComponent<Renderer>().material;

        mat.SetTexture("_FluidTex", surfacemat.GetTexture("_FluidTex"));
        mat.SetTexture("_FluidGradient", surfacemat.GetTexture("_FluidGradient"));
        mat.SetFloat("_Oscillation", surfacemat.GetFloat("_Oscillation"));
        mat.SetFloat("_Detail", surfacemat.GetFloat("_Detail"));
        mat.SetFloat("_Bias", surfacemat.GetFloat("_Bias"));
        mat.SetFloat("_Flow", surfacemat.GetFloat("_Flow"));
        mat.SetVector("_FluidTex_ST", surfacemat.GetVector("_FluidTex_ST"));
        mat.SetVector("_FluidGradient_ST", surfacemat.GetVector("_FluidGradient_ST"));

        MapConfiguration config = transform.parent.GetComponent<MapConfiguration>();
        transform.position = new Vector3(0, -config.wall_height, 0);
    }
}