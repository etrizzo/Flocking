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

	//Various booleans for setting the state of the game
	public bool go;
	private bool done;
	public bool pause;


	void Start ()
	{
		go = false;
		done = false;
		pause = false;

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
		//newBird ();

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
		if (Input.GetKeyDown ("space")){
			if (!pause && go) {
				pause = true;
			} else {
				pause = false;
			}

		}
		if (!birdOnScreen && go && !pause && !done) {
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
		} else if(go && !pause && !done){
			replayBirds ();

		}
//		Debug.Log (dead_bird_list.Keys);


		//updates x and y coords of the screen in case the screen is resized
		//(seems like a lot of unneccessary calculation but there's no OnResize event that I can find)
		x_coord = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
		y_coord = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
	}

	// Start button that disappears once clicked (and triggers the start of the game)
	void OnGUI () {
		GUIStyle guiStyle = new GUIStyle();
		int xpos;
		int ypos;
		if ((!go && !done) || (pause && !done)) {
			guiStyle.fontSize = 80;
			guiStyle.normal.textColor = new Color(103, 58, 148);
			guiStyle.alignment = TextAnchor.MiddleCenter;
			xpos = ((Screen.width) - (300)) / 2;
			ypos = ((Screen.height) - (50)) / 2 - (Screen.height / 3);
			GUI.Label (new Rect (xpos, ypos, 300, 50), "FLOCKING", guiStyle);

		} else if (done) {
			go = false;
			guiStyle.normal.textColor = new Color(148, 58, 85);
			guiStyle.fontSize = 80;
			guiStyle.alignment = TextAnchor.MiddleCenter;
			xpos = ((Screen.width) - 300) / 2;
			ypos = ((Screen.height) - 50) / 2 - (Screen.height / 6);
			GUI.Label (new Rect (xpos, ypos, 300, 50), "GAME OVER", guiStyle);
			xpos = ((Screen.width)-(150))/2;
			ypos = ((Screen.height)-(60))/2+(Screen.height/6);
			if (done && GUI.Button (new Rect (xpos, ypos, 150, 60), "RESTART?")) {
				Start ();
			}
		}
		xpos = ((Screen.width)-(150))/2;
		ypos = ((Screen.height)-(60))/2+(Screen.height/4);
		if (!done && pause) {
			guiStyle.fontSize = 60;
			guiStyle.normal.textColor = new Color(58, 148, 130);
			GUI.Label (new Rect (xpos, ypos, 150, 60), "PAUSED", guiStyle);
		}

		if (!go && !done && !pause && GUI.Button (new Rect (xpos, ypos, 150, 60), "START")) {
			go = true;
			newBird ();
		}
	}


	// private void makeWeather ()
	// {
	//     Weather new_weather = gameObject.AddComponent<Weather> ();
	//     new_weather.init ("cloud");
	// }
}

