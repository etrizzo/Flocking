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
	public Bird live;
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

	public int bird_num = 8;
	public int bird_life = 10;


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
		bird_num--;
		GameObject birdObject = new GameObject ();
		birdObject.name = "bird object";
		Bird bird = birdObject.AddComponent<Bird> ();
		bird.transform.parent = bird_folder.transform;
		bird.transform.localPosition = new Vector3 (0, 0, 0);
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
		live = bird;
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
		bird.hasRadius = true; //DELETE
	}

	private void migrationModeInit (Bird bird) {
		bird.hasRadius = true;
	}

	int i = 0;
	//method to replay each dead bird
	private void replayBirds(){
		bool clearAll = false;
		//print ("updating dead birds");
		foreach(Bird mouse in dead_bird_list.Values){
//			Debug.Log(mouse.name);

			if (mouse.first) {
				mouse.direction = new Vector2 (0, 1);
				mouse.initBirdModel (false);
//				print ("Trail: " + mouse.model2.birdTrail);
//				mouse.model2.birdTrail.Clear ();
				mouse.first = false;
			}
			if ( i < mouse.movements.Count) {
				mouse.replay (i);

			}

			// positions.Count is slightly different length for each bird OMG
			// basically some birds don't reach the end of their list to clear the trail lolol
			// we probably need to change how this is done.
//			if (i >= mouse.positions.Count - bird_count || clearAll){		//kind of makes it a little bit better?!? but this is not a good thing
			if (i >= mouse.movements.Count || clearAll){		//lol wacky trailz
				mouse.model2.birdTrail.Clear ();
				clearAll = true;
			}

		}
//		if (clearAll) {
//			foreach (Bird mouse in dead_bird_list.Values) {
//				mouse.model2.birdTrail.Clear ();
//			}
//		}
		i++;	
	}

	void Update(){
		if (Input.GetKeyDown ("space")){
			if (!pause && go) {
				pause = true;
				Time.timeScale = 0;
			} else {
				pause = false;
				Time.timeScale = 1;
			}

		}
		if (!birdOnScreen && go && !pause && !done && bird_num > 0) {
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
		} else if (go && !pause && !done && bird_num >= 0) {
			replayBirds ();

		} else if (bird_num <= 0) {
			done = true;
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
			ypos = ((Screen.height) - (10)) / 2 - (Screen.height / 3);
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
			/*if (done && GUI.Button (new Rect (xpos, ypos, 150, 60), "RESTART?")) {
				Start ();
			}*/
		}
		xpos = ((Screen.width)-(150))/2;
		ypos = ((Screen.height)-(60))/2+(Screen.height/4);
		if (!done && pause) {
			guiStyle.fontSize = 60;
			guiStyle.normal.textColor = new Color(58, 148, 130);
			GUI.Label (new Rect (xpos, ypos, 150, 60), "PAUSED", guiStyle);
		}

		if (!go && !done && !pause) {
			Vector2 num_slider_coords = new Vector2 (((Screen.width)-(150))/2, (Screen.height)/2);
			Vector2 num_slider_size = new Vector2 (150, 30);
			Rect num_slider_rect, num_slider_box_rect;


			num_slider_rect = new Rect (num_slider_coords.x, num_slider_coords.y, num_slider_size.x, num_slider_size.y);
			num_slider_box_rect = new Rect (num_slider_coords.x, num_slider_coords.y + num_slider_size.y, num_slider_size.x, num_slider_size.y);

			Vector2 life_slider_coords = new Vector2 (((Screen.width)-(150))/2, (Screen.height)/2-(Screen.height/8));
			Vector2 life_slider_size = new Vector2 (150, 30);
			Rect life_slider_rect, life_slider_box_rect;

			life_slider_rect = new Rect (life_slider_coords.x, life_slider_coords.y, life_slider_size.x, life_slider_size.y);
			life_slider_box_rect = new Rect (life_slider_coords.x, life_slider_coords.y + life_slider_size.y, life_slider_size.x, life_slider_size.y);

			GUI.Box (num_slider_rect, "Number of Birds: " + bird_num.ToString ());
			bird_num = (int)GUI.HorizontalSlider (num_slider_box_rect, (float)bird_num, 0.0F, 30.0F);

			GUI.Box (life_slider_rect, "Bird Lifetime: " + bird_life.ToString ());
			bird_life = (int)GUI.HorizontalSlider (life_slider_box_rect, (float)bird_life, 0.0F, 60.0F);
		}

		xpos = ((Screen.width)-(150))/2;
		ypos = ((Screen.height)/2+(Screen.height/8));
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

