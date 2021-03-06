﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureController : MonoBehaviour
{
    public SkinnedMeshRenderer meshRenderer;
    public bool preloadedMode = false;
    public Texture[] preloaded;

    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();

        if(preloadedMode)
        {
            meshRenderer.material.SetTexture("_MainTex",preloaded[Random.Range(0, preloaded.Length - 1)]);
        }
    }

    public void ApplyTexture(Texture texture)
    {
        if (!meshRenderer)
            meshRenderer = GetComponent<SkinnedMeshRenderer>();
        
        meshRenderer.material.SetTexture("_MainTex", texture);
    }

	public void Flash()
	{
		StartCoroutine(FlashCoro());
	}

	IEnumerator FlashCoro()
	{
		if (!meshRenderer)
            meshRenderer = GetComponent<SkinnedMeshRenderer>();

		Color original = meshRenderer.materials[0].GetColor("_EmissionColor");
		Color color = Color.yellow;

        for (float t = 0; t < 1; t += Time.deltaTime * 0.3f)
		{
			meshRenderer.materials[0].SetColor("_EmissionColor", Color.Lerp(original, color, Mathf.Sin(t * Mathf.PI)));

			yield return null;
		}

		meshRenderer.materials[0].SetColor("_EmissionColor", original);
	}
}
