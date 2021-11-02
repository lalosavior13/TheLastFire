using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))] 
public class ScrollingUVs : MonoBehaviour 
{
    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2( 1.0f, 0.0f );
    public string textureName = "_MainTex";
    private Renderer renderer;
    private Vector2 uvOffset = Vector2.zero;
 
    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    private void LateUpdate() 
    {
        uvOffset += ( uvAnimationRate * Time.deltaTime );
        
        if(renderer.enabled )
        renderer.materials[ materialIndex ].SetTextureOffset( textureName, uvOffset );
    }
}