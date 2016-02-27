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
	public static bool zen = true;

	// The model object.


	public void  Start ()
	{
		this.cam = Camera.main;
		if (zen) {
			cam.orthographicSize = 10f;
		}
		this.bg = addBGtile (0, 0);
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





	Background addBGtile(int x, int y) {
		GameObject bg_object = new GameObject();			
		bg_object.name = "BG Object";
		Background bg = bg_object.AddComponent<Background>();	
		bg.transform.position = new Vector3(x,y,0);		
		bg.init((int) x, (int) y);										
		bg.name = "Background";

		return bg;							
	}



}

