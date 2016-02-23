using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Background : MonoBehaviour {

	private BackgroundModel bg_model;
	// The Start function is good for initializing objects, but doesn't allow you to pass in parameters.
	// For any initialization that requires input, you'll probably want your own init function. 

	public void init(int x, int y) {
		var modelObject = GameObject.CreatePrimitive(PrimitiveType.Quad);	// Create a quad object for holding the gem texture.
		modelObject.name = "BG Model";
	//modelObject.layer = 0;
		this.bg_model = modelObject.AddComponent<BackgroundModel>();						// Add a gemModel script to control visuals of the gem.
		this.bg_model.init(this);					
		//this.boxcollider = modelObject.AddComponent < BoxCollider2D> ();
	}
}