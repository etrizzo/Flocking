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
	public static float x_coord;
	public static float BGSCALE = 2f;
	public static bool zen = true;
	public float border_scale = 4f;

	// The model object.


	public void  Start ()
	{
		
		this.cam = Camera.main;
		if (zen) {
			cam.orthographicSize = 10f;
			float dist = (transform.position - Camera.main.transform.position).z;
			x_coord = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
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
		float bs = border_scale - 2;
		//float halfsies = BGSCALE / 2;
		addBorder (x_coord +bs, 0);
		addBorder (x_coord * -1 - bs, 0);
		addBorder (0, screen_height/2 + bs);
		addBorder (0, screen_height/2 * -1 - bs);
		return bg;							
	}

	void addBorder(float x, float y){
		GameObject border = GameObject.CreatePrimitive (PrimitiveType.Quad);
		border.transform.position = new Vector3 (x, y, 0);
		if (x != 0){
				border.transform.localScale = new Vector2(border_scale, screen_height * BGSCALE);
		} else if (y!= 0){
				border.transform.localScale = new Vector2(screen_width*BGSCALE, border_scale);
			}
		border.name = "Border";
	
		MeshCollider mcol = border.GetComponent<MeshCollider>();
		if (mcol != null) {
				DestroyImmediate (mcol);
			}

		//delete the renderer so that the wall isn't visible
		MeshRenderer mrend = border.GetComponent<MeshRenderer> ();
		if (mrend != null) {
			DestroyImmediate (mrend);
		}
		BoxCollider2D col = border.AddComponent<BoxCollider2D> ();
	}
		



}

