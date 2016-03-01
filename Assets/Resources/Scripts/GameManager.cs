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
	int bird_count = 0;
	public bool birdOnScreen = true;
	public Hashtable dead_bird_list;
	// List<Weather> weather_list;

	// Emily's Variables
	public Camera cam;		//main camera
	public Background bg;		//background tile (in case we need it)
	public static float x_coord, y_coord;	//x coordinate of right side, y coordinate of bottom side
	public static float BGSCALE = 2f;	//scaling factor for background (how many screens wide in migration mode)
	public float border_scale = 4f;		//thickness of the box collider borders
	float dist;				//distance from the camera to the game for coordinate calculations


	void Start ()
	{
		// Hardcode it TODO: don't do this
		zenMode = true;
		this.cam = Camera.main;

		dead_bird_list = new Hashtable ();

		dist = (transform.position - cam.transform.position).z;
		bird_folder = new GameObject ();
		bird_folder.name = "Birds";

		if (zenMode) {
			//zoom out the camera in zen mode
			cam.orthographicSize = 10f;
			//get coordinates of the edges according to:
			//http://answers.unity3d.com/questions/62189/detect-edge-of-screen.html
			x_coord = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
			y_coord = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
		}
		this.bg = addBGtile (0, 0);
		newBird ();

	}

	private void newBird() {
		GameObject birdObject = new GameObject ();
		birdObject.name = "bird object";
		Bird bird = birdObject.AddComponent<Bird> ();
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
		bird.init (this);
		bird.name = "Bird "+ bird_count++;
	}

	Background addBGtile(int x, int y) {
		//creates background tile
		GameObject bg_object = new GameObject();			
		bg_object.name = "BG Object";
		Background bg = bg_object.AddComponent<Background>();	
		bg.transform.position = new Vector3(x,y,0);		
		bg.init((int) x, (int) y);										
		bg.name = "Background";
		return bg;							
	}

	private void zenModeInit (Bird bird)
	{
		bird.hasTrail = true;
	}

	private void migrationModeInit (Bird bird) {
		// TODO
	}

	int i = 0;
	//method to replay each dead bird
	private void replayBirds(){
		foreach(Bird mouse in dead_bird_list.Values){
//			Debug.Log(mouse.name);
			if (mouse.first) {
				mouse.direction = new Vector2 (0, 1);
				mouse.initBirdModel (false);
				mouse.first = false;
			}
			if ( i < mouse.positions.Count) {
				mouse.replay (i);
				i++;
			}
		} 
	}

	void Update(){
		if (!birdOnScreen) {
			birdOnScreen = true;
			i = 0; //reset replay
//			Debug.Log (dead_bird_list.Count);
//			Debug.Log(dead_bird_list.Keys.ToString());
//			foreach (Bird mouse in dead_bird_list.Values) {
//				Destroy (mouse);
//			}
			newBird ();
//			foreach(string key in dead_bird_list.Keys)
//			{
//				Debug.Log(key +"     "+ dead_bird_list[key]);
//			}
		} else {
			replayBirds ();

		}
//		Debug.Log (dead_bird_list.Keys);


		//updates x and y coords of the screen in case the screen is resized
		//(seems like a lot of unneccessary calculation but there's no OnResize event that I can find)
		x_coord = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
		y_coord = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
	}


	// private void makeWeather ()
	// {
	//     Weather new_weather = gameObject.AddComponent<Weather> ();
	//     new_weather.init ("cloud");
	// }
}

