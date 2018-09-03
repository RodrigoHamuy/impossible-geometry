using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DisolveScript : MonoBehaviour {

  public Material material;

  public float max = 0.15f;

  bool start;

  public void SetDisolveIntensity (float value) {
    material.SetFloat ("_D", value * max);
  }

  public void SetDisolveIntensityInverted (float value) {
    SetDisolveIntensity(1.0f - value);
  }

  void OnRenderImage (RenderTexture input, RenderTexture output) {

    Graphics.Blit (input, output, material);

  }

}