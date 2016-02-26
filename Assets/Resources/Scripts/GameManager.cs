using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

	public Bird bird;
	public Camera cam;
	public Background bg;
	public static float screen_height = Camera.main.orthographicSize * 2.0f;
	public static float screen_width = screen_height * Screen.width / Screen.height;
	public static float BGSCALE = 2f;

	// The model object.


	public void  Start ()
	{
		background ();
		makeBird ();

		//direction = new Vector2 (0, 1);
	}




	void makeBird(){
		GameObject birdObject = new GameObject();	
		birdObject.name = "Bird Object";
		//birdObject.layer = "
		bird = birdObject.AddComponent<Bird>();	
		bird.transform.position = new Vector3(0,0,0);	

		Rigidbody2D rb = birdObject.AddComponent<Rigidbody2D> ();
		rb.gravityScale = 0;
		//rb.isKinematic = true;
		CircleCollider2D col = birdObject.AddComponent<CircleCollider2D> ();
		col.name = "Bird Collider";

		//bird.init();							

		bird.name = "Bird";
	}

	void background(){

		this.bg = addBGtile (0, 0);
		float halfsies = BGSCALE / 2;
		addBorder (screen_width * halfsies, 0);
		addBorder (screen_width * halfsies * -1, 0);
		addBorder (0, screen_height * halfsies);
		addBorder (0, screen_height * halfsies * -1);



	}

	void addBorder(float x, float y){
		GameObject border = GameObject.CreatePrimitive (PrimitiveType.Quad);
		//WallModel model = border.AddComponent<WallModel> ();						// Add a bird_model script to control visuals of the bird.
		//model.init (this);
		border.transform.position = new Vector3 (x, y, 0);
		if (x != 0){
			border.transform.localScale = new Vector2(1f, screen_height * BGSCALE);
		} else if (y!= 0){
			border.transform.localScale = new Vector2(screen_width*BGSCALE, 1f);
		}
		border.name = "Border" + x + " " + y;
		//BoxCollider2D c = border.AddComponent<BoxCollider2D> ();

		MeshCollider mcol = border.GetComponent<MeshCollider>();
		if (mcol != null) {
			DestroyImmediate (mcol);
		}
		MeshRenderer mrend = border.GetComponent<MeshRenderer> ();
//		if (mrend != null){
//			DestroyImmediate (mrend);
//		}
////		Rigidbody2D rb = border.AddComponent<Rigidbody2D> ();
////		rb.isKinematic = true;
		BoxCollider2D col = border.AddComponent<BoxCollider2D> ();
//		col.name = "Border Collider";
	
	}

	Background addBGtile(int x, int y) {
		GameObject bg_object = new GameObject();			
		bg_object.name = "BG Object";
		Background bg = bg_object.AddComponent<Background>();	
		// We can now refer to the object via this script.
		//bg.transform.parent = bg_folder.transform;			
		bg.transform.position = new Vector3(x,y,0);		
		bg.init((int) x, (int) y);										
		bg.name = "Background";

		return bg;							
	}



}

