using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	GameObject bird_folder;
	/* 
	 * 0 - Zen Mode
	 * 1 - Migration Mode
	 */
	bool zenMode;
	Bird bird;
	int bird_count = 0;
	bool birdOnScreen = true;
	List<Bird> dead_bird_list;
	// List<Weather> weather_list;

	// Emily's Variables
	public Camera cam;
	public Background bg;
	public static float screen_height = Camera.main.orthographicSize * 2.0f;
	public static float screen_width = screen_height * Screen.width / Screen.height;
	public static float x_coord, y_coord;
	public static float BGSCALE = 2f;
	public float border_scale = 4f;

	void Start ()
	{
		// Hardcode it TODO: don't do this
		zenMode = true;


		bird_folder = new GameObject ();
		bird_folder.name = "Birds";

		this.cam = Camera.main;
		if (zenMode) {
			cam.orthographicSize = 10f;
			float dist = (transform.position - Camera.main.transform.position).z;
			x_coord = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
			y_coord = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
		}
		this.bg = addBGtile (0, 0);
		newBird ();
	}

	private void newBird() {
		bird = gameObject.AddComponent<Bird> ();
		bird.transform.parent = bird_folder.transform;
		if (zenMode) {
			zenModeInit (bird);
		} else {
			migrationModeInit (bird);
		
		}
		Rigidbody2D rb = bird.gameObject.AddComponent<Rigidbody2D> ();
		rb.gravityScale = 0;
		//rb.isKinematic = true;
		CircleCollider2D col = bird.gameObject.AddComponent<CircleCollider2D> ();
		col.name = "Bird Collider";
		bird.init ();
		bird.gameObject.name = "Bird "+ bird_count++;
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
		addBorder (0, y_coord);
		addBorder (0, y_coord - bs);
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

	private void zenModeInit (Bird bird)
	{
		bird.hasTrail = true;
	}

	private void migrationModeInit (Bird bird) {
		// TODO
	}

	void Update(){
		if(!birdOnScreen){
			newBird ();
		}
	}


	// private void makeWeather ()
	// {
	//     Weather new_weather = gameObject.AddComponent<Weather> ();
	//     new_weather.init ("cloud");
	// }
}

