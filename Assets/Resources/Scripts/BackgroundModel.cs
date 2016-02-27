using UnityEngine;
using System.Collections;

public class BackgroundModel : MonoBehaviour
{
	private float clock;		// Keep track of time since creation for animation.
	private Background owner;			// Pointer to the parent object.
	private Material mat;		// Material for setting/changing texture and color.
	private float BGSCALE = 4f;

	public void init(Background owner) {
		this.owner = owner;

		transform.parent = owner.transform;					// Set the model's parent to the background.
		transform.localPosition = new Vector3(0,0,0f);		// Center the model on the parent.
		float quadHeight = Camera.main.orthographicSize * 2.0f;
		float quadWidth = quadHeight * Screen.width / Screen.height;
//		transform.localScale = new Vector3(quadWidth * BGSCALE, quadHeight * BGSCALE,1f); 
		transform.localScale = new Vector3(quadWidth, quadHeight,1f); 
		name = "Background Model";									// Name the object.

		mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
		mat.shader = Shader.Find ("Sprites/Default");						// Tell the renderer that our textures have transparency.
		mat.mainTexture = Resources.Load<Texture2D>("Textures/tileBlank");	// Set the texture.  Must be in Resources folder.
		mat.color = new Color(.55f,.6f,.65f);											// Set the color (easy way to tint things).

	}


}

