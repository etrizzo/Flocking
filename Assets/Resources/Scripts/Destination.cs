using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Destination : MonoBehaviour {

	//private BackgroundModel bg_model;
	private GameObject modelObject;

	public void init() {
		float x = Random.Range (0f, 1f);
		float y = Random.Range (0f, 1f);
		print (x + ", " + y);
		if (x < .5f) {
			x = GameManager.x_coord * GameManager.BGSCALE * -1 ;
		} else {
			x = GameManager.x_coord * GameManager.BGSCALE;
		}

		if (y < .5f) {
			y = GameManager.y_coord * GameManager.BGSCALE * -1;
		} else {
			y = GameManager.y_coord * GameManager.BGSCALE;
		}





	
		this.modelObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
		DestinationModel dmodel = modelObject.AddComponent<DestinationModel> ();
		dmodel.init (this);

	
		this.gameObject.transform.position = new Vector3 (x, y, 0);
		this.modelObject.transform.position = gameObject.transform.position;
//		this.gameObject.AddComponent<BoxCollider2D> ();

	}



}
