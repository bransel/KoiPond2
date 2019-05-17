using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonElement : MonoBehaviour
{
    public RawImage image;
    public Text text;

    private TextureController targetCube;

    void Start()
    {
        AssignTargetCube(FindObjectOfType<TextureController>());
    }

    public void AssignTargetCube(TextureController targetCube)
    {
        this.targetCube = targetCube;
    }

    public void AssignTexture(Texture texture)
    {
        image.texture = texture;
    }

    public void AssignMessage(string message)
    {
        text.text = message;
    }

    public void ApplyTextureToCube()
    {
        targetCube.ApplyTexture(image.texture);
    }
}
