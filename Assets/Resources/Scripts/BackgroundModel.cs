
using UnityEngine;
using System.Collections;

public class BackgroundModel : MonoBehaviour
{
    private float clock;		// Keep track of time since creation for animation.
    private Background owner;			// Pointer to the parent object.
    private Material mat;		// Material for setting/changing texture and color.
    private float BGSCALE = 4f;
	private float quadHeight;
	private float quadWidth;

    public void init(Background owner) {
        this.owner = owner;

        transform.parent = owner.transform;					// Set the model's parent to the background.
        transform.localPosition = new Vector3(0,0,1f);		// Center the model on the parent.
        //quadHeight = Camera.main.orthographicSize * 2.0f;
        //quadWidth = quadHeight * Screen.width / Screen.height;
        //		transform.localScale = new Vector3(quadWidth * BGSCALE, quadHeight * BGSCALE,1f);
		transform.localScale = new Vector3(GameManager.x_coord * GameManager.BGSCALE*2, GameManager.y_coord*GameManager.BGSCALE*2,1f);
        name = "Background Model";									// Name the object.

        mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
        mat.shader = Shader.Find ("Sprites/Default");						// Tell the renderer that our textures have transparency.
		if (!owner.gm.zenMode) {
			mat.mainTexture = Resources.Load<Texture2D> ("Textures/background");	// Set the texture.  Must be in Resources folder.
			mat.color = new Color(.55f,.6f,.65f);											// Set the color (easy way to tint things).

		} else {
			mat.mainTexture = Resources.Load<Texture2D> ("Textures/zenbackground");	// Set the texture.  Must be in Resources folder.
//			mat.color = new Color(.95f,.95f,.95f);											// Set the color (easy way to tint things).

		}

    }

	public void Update(){
		//resizes the background if the screen size is changed
		//(also seems like a lot of unused calculations, idk if we care that much
		if (owner.gm.zenMode) {
			float newheight = Camera.main.orthographicSize * 2.0f;
			float newwidth = quadHeight * Screen.width / Screen.height;
			if (quadHeight != newheight || quadWidth != newwidth) {
				quadHeight = newheight;
				quadWidth = newwidth;
				transform.localScale = new Vector3 (newwidth, newheight, 1f);
			}
		}
	}


}
