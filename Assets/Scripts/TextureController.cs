using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TextureController : MonoBehaviour
{
    public SkinnedMeshRenderer meshRenderer;
	public UnityEvent OnTextureChange;

    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    public void ApplyTexture(Texture texture)
    {
        if (!meshRenderer)
            meshRenderer = GetComponent<SkinnedMeshRenderer>();
        
        meshRenderer.material.SetTexture("_MainTex", texture);

		OnTextureChange.Invoke();
    }

	public void Flash()
	{
		StartCoroutine(FlashCoro());
	}

	IEnumerator FlashCoro()
	{
		Color original = meshRenderer.material.GetColor("_EmissionColor");
		Color color = Color.cyan / 2f;
		
		for (float t = 0; t < 1; t += Time.deltaTime / 0.5f)
		{
			meshRenderer.material.SetColor("_EmissionColor", Color.Lerp(original, color, Mathf.Sin(t * Mathf.PI)));

			yield return null;
		}

		meshRenderer.material.SetColor("_EmissionColor", original);
	}
}
