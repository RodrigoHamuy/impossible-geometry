using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DisolveScript : MonoBehaviour {

	public Material material;

	void OnRenderImage (RenderTexture input, RenderTexture output) {

		Graphics.Blit (input, output, material);

	}

}