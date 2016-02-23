using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Background : MonoBehaviour {

	private BackgroundModel bg_model; 

	public void init(int x, int y) {
		var modelObject = GameObject.CreatePrimitive(PrimitiveType.Quad);	
		modelObject.name = "BG Model";
		this.bg_model = modelObject.AddComponent<BackgroundModel>();						
		this.bg_model.init(this);					

	}
}