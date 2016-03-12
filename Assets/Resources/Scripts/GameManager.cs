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
	public struct GuiState
	{
		public int mode;
	}

	public GuiState state;

	public float score;
	public bool zenMode;
	public AudioSource gameAudio;
	public AudioClip gameClip;
	public AudioSource migrationAudio;
	public AudioClip migrationClip;
	int checkCall;

	int bird_count = 0;
	public bool birdOnScreen = true;
	public Hashtable dead_bird_list;
	public Bird live;
	public float timeUntilKill;
	public bool inRadius;
	public float birdSpeed;

	// Emily's Variables
	public Camera cam;
	//main camera
	public Background bg;
	//background tile (in case we need it)
	public static float x_coord, y_coord;
	//x coordinate of right side, y coordinate of bottom side
	public static float BGSCALE = 2f;
	//scaling factor for background (how many screens wide in migration mode)
	public float border_scale = 4f;
	//thickness of the box collider borders
	float dist;
	//distance from the camera to the game for coordinate calculations

	//Various booleans for setting the state of the game
	public bool go;
	private bool done;
	public bool pause;
	public float loadScreenCounter = 5f;

	public int bird_num = 8;
	public int bird_life = 10;
	float birdspeed = 4f;

	Destination dest;

	int weather_count = 50;
	public List<WeatherModel> weather_list = new List<WeatherModel>();

	int seed_count = 10;

	public GameObject seedFolder;

	void Start ()
	{
		seedFolder =  new GameObject();
		seedFolder.name = "Seeds";
		go = false;
		done = false;
		pause = false;

		// Hardcode it TODO: don't do this
		zenMode = false;

		this.cam = Camera.main;

		dead_bird_list = new Hashtable ();

		dist = (transform.position - cam.transform.position).z;
		bird_folder = new GameObject ();
		bird_folder.name = "Birds";
		//get coordinates of the edges according to:
		//http://answers.unity3d.com/questions/62189/detect-edge-of-screen.html
		x_coord = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, dist)).x;		//x coord of the right of the screen
		y_coord = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, dist)).y;		//y coord of the bottom of the screen

		//this.bg = addBGtile (0, 0);
		//newBird ();

		gameClip = Resources.Load<AudioClip> ("Sounds/StartMenu");
		initSound (gameClip);

	}

	private void initMode ()
	{
		if (zenMode) {
			BGSCALE = 1f;
			//zoom out the camera in zen mode
			cam.orthographicSize = 10f;
			//get coordinates of the edges according to:
			//http://answers.unity3d.com/questions/62189/detect-edge-of-screen.html
			x_coord = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, dist)).x;
			y_coord = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, dist)).y;
		} else { //in migration mode

//			BGSCALE = 2f;
			birdSpeed = 6f;

			BGSCALE = 2f;
			birdSpeed = 3f;

			inRadius = true;
			makeDestination ();
			makeWeather ();
			makeSeeds ();
		}
		this.bg = addBGtile (0, 0);
		//newBird ();
	}

	private void newBird ()
	{
		bg.bgMat.color = new Color (1, 1, 1);

		bird_num--;
		if (bird_num >= 0) {
			birdSpeed++;
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

//		rb.gravityScale = 0;
			//rb.isKinematic = true;
			DestroyImmediate (bird.gameObject.GetComponent<MeshCollider> ());
			PolygonCollider2D col = bird.gameObject.AddComponent<PolygonCollider2D> ();
			Rigidbody2D rb = bird.gameObject.AddComponent<Rigidbody2D> ();
			col.isTrigger = true;
			rb.isKinematic = true;
//		rb.gravityScale = 0;
			//rb.useGravity = false;
			col.name = "Bird Collider";
			bird.init (this, birdspeed);
			bird.name = "Bird " + bird_count++;
			live = bird;
			birdspeed += .25f;
		}
	}

	Background addBGtile (int x, int y)
	{
		//creates background tile
		GameObject bg_object = new GameObject ();			
		bg_object.name = "BG Object";
		Background bg = bg_object.AddComponent<Background> ();	
		bg.transform.position = new Vector3 (x, y, 0);		
		bg.init ((int)x, (int)y, this);										
		bg.name = "Background";
		return bg;							
	}

	private void zenModeInit (Bird bird)
	{
		bird.hasTrail = true;
		bird.hasRadius = false;
	}

	private void migrationModeInit (Bird bird)
	{
//		Debug.Log ("inside migrationmode init :)))");
		bird.hasRadius = true;
		bird.hasTrail = false;
		migrationAudio = this.gameObject.AddComponent<AudioSource> ();
		migrationAudio.loop = false;
		migrationAudio.playOnAwake = false;
		checkCall = 0;
	}

	private string getsoundNum(){
		int soundNum = (int) ((Random.value * 1000) % 11 ) + 1;
		return soundNum.ToString();
	}

	private void birdCall(){
		migrationClip = Resources.Load<AudioClip> ("Sounds/MigrationSounds/flock"+getsoundNum());
		migrationAudio.clip = migrationClip;
		migrationAudio.Play ();
	}

	private void playBirdCall(){
		float playCall = Random.value;
		if (playCall < .03f) {
			birdCall ();
			print ("CHEEP! "+ playCall);
		}
	}

	int i = 0;
	//method to replay each dead bird
	private void replayBirds ()
	{
			bool clearAll = false;
			//print ("updating dead birds");
			foreach (Bird mouse in dead_bird_list.Values) {
//			Debug.Log(mouse.name);

				if (mouse.first) {
					mouse.direction = new Vector2 (0, 1);
					mouse.initBirdModel (false);
//				print ("Trail: " + mouse.model2.birdTrail);
//				mouse.model2.birdTrail.Clear ();
					mouse.first = false;
				}
				if (i < mouse.movements.Count) {
//					print (mouse.name + " " + mouse.movements.Count);
					mouse.replay (i);

				}

				// positions.Count is slightly different length for each bird OMG
				// basically some birds don't reach the end of their list to clear the trail lolol
				// we probably need to change how this is done.
//			if (i >= mouse.positions.Count - bird_count || clearAll){		//kind of makes it a little bit better?!? but this is not a good thing
				if (zenMode) {
					if (i >= mouse.movements.Count || clearAll) {		//lol wacky trailz
						mouse.model2.birdTrail.Clear ();
						clearAll = true;
					}
				}

			}
//		if (clearAll) {
//			foreach (Bird mouse in dead_bird_list.Values) {
//				mouse.model2.birdTrail.Clear ();
//			}
//		}
			i++;
	}


	public void checkBirdInRadius ()
	{
		inRadius = false;
		timeUntilKill = 2f;
		foreach (Bird mouse in dead_bird_list.Values) {
			if (mouse.model2.radius.containsBird) {
				inRadius = true;
			}
		}
		/*if (!inRadius) {
			cam.transform.localPosition = new Vector3 (0, 0, 10f);
			Destroy (live.gameObject);
			birdOnScreen = false;
			clearWeather ();
		} */
	}

	private void initSound(AudioClip gameClip){
		gameAudio = this.gameObject.AddComponent<AudioSource> ();
		gameAudio.loop = true;
		gameAudio.volume = .6f;
		getSound (gameClip);
	}

	private void getSound(AudioClip gameClip){
		gameAudio.clip = gameClip;
		gameAudio.Play ();
	}

	private float lowestDelta = 1;
	private float highestDelta = 0;
	void Update(){
		/*if (Time.deltaTime < lowestDelta) {
			lowestDelta = Time.deltaTime;
		}
		if (Time.deltaTime > highestDelta) {
			highestDelta = Time.deltaTime;
		}*/

		//print ("Lowest deltaTime: " + lowestDelta);
		//print ("Highest deltaTime: " + highestDelta);
		if (Input.GetKeyDown ("space") && state.mode != 6){
			if (!pause && go) {
				pause = true;
				state.mode = 4;
			} else {
				pause = false;
				state.mode = 5;
			}

		}
		if(bird_num < 0){
			
			done = true;
			state.mode = 7;
		}
			
		else if (!birdOnScreen && go && !pause && !done && bird_num >= 0) {
			
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
			loadScreenCounter = 5f;
			state.mode = 6;
		} else if (go && !pause && !done && bird_num >= 0) {
			
			replayBirds ();

		}
//		Debug.Log (dead_bird_list.Keys);

		if (zenMode) {
			//updates x and y coords of the screen in case the screen is resized
			//(seems like a lot of unneccessary calculation but there's no OnResize event that I can find)
			x_coord = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, dist)).x;
			y_coord = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, dist)).y;
		} else if (!inRadius) {
			timeUntilKill -= Time.deltaTime;
			/*foreach (Bird b in dead_bird_list.Values) {
				if (b) {
					b.model.radius.mat.color = Color.red;
				}
			}*/
			if (timeUntilKill < 0 && live) {
				cam.transform.localPosition = new Vector3 (0, 0, 10f);
				inRadius = true;
				Destroy (live.gameObject);
				birdOnScreen = false;
				clearWeather ();
			}
		}
		if (!zenMode && state.mode == 5) {
			if (checkCall % 17 == 0) {
				playBirdCall ();
			}
			checkCall++;
		}
	}

	/************************ Start Gui Stuff ****************************/

	// Start button that disappears once clicked (and triggers the start of the game)
	void OnGUI ()
	{
		
		switch (state.mode) {
		case 0:
			getMode ();
			break;
		case 1:
			zenOptions ();
			break;
		case 2:
			migrationOptions ();
			break;
		case 3:
			startGame ();
			break;
		case 4:
			pauseGame ();
			break;
		case 5:
			unpauseGame ();
			break;
		case 6:
			loadScreen ();
			break;
		case 7:
			endScreen();
			break;
		}
	}

	private void getMode(){

		GUIStyle guiStyle = new GUIStyle();
		int xpos;
		int ypos;
		if ((!go && !done) || (pause && !done)) {
			guiStyle.fontSize = 200;
			guiStyle.normal.textColor = new Color (.40f, .23f, .58f, .9f);
			guiStyle.alignment = TextAnchor.MiddleCenter;
			xpos = ((Screen.width) - (300)) / 2;
			ypos = ((Screen.height) - (10)) / 2 - ((Screen.height / 3)-(Screen.height/30));
			GUI.Label (new Rect (xpos, ypos, 300, 50), "FLOCKING", guiStyle);
		}
		if (!go && !done) {
			xpos = ((Screen.width) + (25)) / 2;
			ypos = ((Screen.height) / 2 + (Screen.height / 8));
			if (GUI.Button (new Rect (xpos, ypos, 150, 60), "Zen Mode")) {
				state.mode = 1;
				/*go = true;
				zenMode = true;
				chooseMode ();
				newBird ();*/
			}
			xpos = ((Screen.width) - (325)) / 2;
			ypos = ((Screen.height) / 2 + (Screen.height / 8));
			if (GUI.Button (new Rect (xpos, ypos, 150, 60), "Migration Mode")) {
				state.mode = 2;
				/*go = true;
				zenMode = false;
				chooseMode ();
				newBird ();*/
			}
		}
	}

	private void zenOptions ()
	{
		zenMode = true;
		showNumSlider ();
		showLifeSlider ();
		int xpos = ((Screen.width) - (150)) / 2;
		int ypos = ((Screen.height) / 2 + (Screen.height / 8));
		if (GUI.Button (new Rect (xpos, ypos, 150, 60), "FLY!")) {
			state.mode = 3;
		}
	}

	private void migrationOptions ()
	{
		zenMode = false;
		//showNumSlider ();
		int xpos1 = ((Screen.width) - (150)) / 4;
		int ypos1 = ((Screen.height) / 2 + (Screen.height / 8));
		int xpos2 = ((Screen.width) - (150)) / 2;
		int ypos2 = ((Screen.height) / 2 + (Screen.height / 8));
		int xpos3 = (((Screen.width) - (150)) / 4)*3;
		int ypos3 = ((Screen.height) / 2 + (Screen.height / 8));
		if (GUI.Button (new Rect (xpos1, ypos1, 150, 60), "A COVEY")) {
			bird_num = 3;
			state.mode = 3;
		}
		else if (GUI.Button (new Rect (xpos2, ypos2, 150, 60), "A FLIGHT")) {
			bird_num = 6;
			state.mode = 3;
		}
		else if (GUI.Button (new Rect (xpos3, ypos3, 150, 60), "A FLOCK")) {
			bird_num = 9;
			state.mode = 3;
		}
	}

	private void showNumSlider ()
	{
		Vector2 num_slider_coords = new Vector2 (((Screen.width) - (150)) / 2, (Screen.height) / 2);
		Vector2 num_slider_size = new Vector2 (150, 30);
		Rect num_slider_rect, num_slider_box_rect;

		num_slider_rect = new Rect (num_slider_coords.x, num_slider_coords.y, num_slider_size.x, num_slider_size.y);
		num_slider_box_rect = new Rect (num_slider_coords.x, num_slider_coords.y + num_slider_size.y, num_slider_size.x, num_slider_size.y);

		GUI.Box (num_slider_rect, "Number of Birds: " + bird_num.ToString ());
		bird_num = (int)GUI.HorizontalSlider (num_slider_box_rect, (float)bird_num, 1.0F, 30.0F);
	}

	private void showLifeSlider ()
	{
		Vector2 life_slider_coords = new Vector2 (((Screen.width) - (150)) / 2, (Screen.height) / 2 - (Screen.height / 8));
		Vector2 life_slider_size = new Vector2 (150, 30);
		Rect life_slider_rect, life_slider_box_rect;

		life_slider_rect = new Rect (life_slider_coords.x, life_slider_coords.y, life_slider_size.x, life_slider_size.y);
		life_slider_box_rect = new Rect (life_slider_coords.x, life_slider_coords.y + life_slider_size.y, life_slider_size.x, life_slider_size.y);

		GUI.Box (life_slider_rect, "Bird Lifetime (sec): " + bird_life.ToString ());
		bird_life = (int)GUI.HorizontalSlider (life_slider_box_rect, (float)bird_life, 1.0F, 60.0F);
	}

	private void startGame(){
		if (!zenMode) {
			gameClip = Resources.Load<AudioClip> ("Sounds/BackgroundMigration");
			getSound (gameClip);
		} else {
			DestroyImmediate (gameAudio);
		}
		go = true;
		initMode ();
		newBird ();
		loadScreenCounter = 5f;
		state.mode = 6;
	}

	private void pauseGame ()
	{
		if (!zenMode) {
			GUI.Box (new Rect (Screen.width - 100, -1, 100, 30), "Score: " + (int)score);
		}
		Time.timeScale = 0;
		GUIStyle guiStyle = new GUIStyle ();
		int xpos = ((Screen.width) - (800)) / 2;
		int ypos = ((Screen.height) + (250)) / 2 - (Screen.height / 3);
		if (!done && pause) {
			guiStyle.fontSize = 200;
			guiStyle.normal.textColor = new Color (255, 255, 255, .5f);
			GUI.Label (new Rect (xpos, ypos, 300, 10), "PAUSED", guiStyle);
		} else {
			state.mode = 5;
		}
	}

	private void unpauseGame ()
	{
		if (!zenMode) {
			GUI.Box (new Rect (Screen.width - 100, -1, 100, 30), "Score: " + (int)score);
		}
		Time.timeScale = 1;
		if (pause) {
			state.mode = 4;
		}
	}

	private void makeDestination ()
	{
		GameObject destinationObject = new GameObject ();
		destinationObject.name = "Destination";
		dest = destinationObject.AddComponent<Destination> ();
		dest.init ();

	}
		
	private void loadScreen(){
		Time.timeScale = 0;
		if (!zenMode) {
			GUI.Box (new Rect (Screen.width - 100, -1, 100, 30), "Score: " + (int)score);
		}
		if (Time.unscaledDeltaTime > .1) {
			print ("OMG DELTA TIME IS HUGE : " + Time.unscaledDeltaTime);
		} else {
			loadScreenCounter -= Time.unscaledDeltaTime * .5f;
		}
		int countdown = (int)loadScreenCounter+1;
		GUIStyle guiStyle = new GUIStyle ();
		int xpos1 = ((Screen.width) - (400)) / 2;
		int xpos2 = ((Screen.width) - (100)) / 2;
		int ypos = ((Screen.height) - (200)) / 2;
		guiStyle.fontSize = 60;
		guiStyle.normal.textColor = new Color (0f, 0f, 0f, .3f);
//		if (zenMode) {
//
//		}
		if (loadScreenCounter > 3) {
			GUI.Label (new Rect (xpos1, ypos, 150, 60), "Bird "+bird_count+" Ready?", guiStyle);
		}
		else if (loadScreenCounter > 0) {
			
			GUI.Label (new Rect (xpos2, ypos, 150, 60), countdown.ToString ()+"...", guiStyle);
		} 
		else if(loadScreenCounter > -1){
			GUI.Label (new Rect (xpos2, ypos, 150, 60), "Go!", guiStyle);
		}
		else {
			state.mode = 5;
		}
	}

	private void endScreen(){
		go = false;
		GUIStyle guiStyle = new GUIStyle ();
		guiStyle.fontSize = 200;
		guiStyle.normal.textColor = new Color (.40f, .23f, .58f, .5f);
		guiStyle.alignment = TextAnchor.MiddleCenter;
		int xpos = ((Screen.width) - (300)) / 2;
		int ypos = ((Screen.height) - (10)) / 2 - ((Screen.height / 3)-(Screen.height/30));
		GUI.Label (new Rect (xpos, ypos, 300, 50), "GAME OVER", guiStyle);

		xpos = ((Screen.width) - (150)) / 2;
		ypos = ((Screen.height) - (60)) / 2 + (Screen.height / 6);
		if (GUI.Button (new Rect (xpos, ypos, 150, 60), "Menu")) {
			print ("restarting");
			Application.LoadLevel (Application.loadedLevel);
			state.mode = 0;
		}
	}


	/************************ End Gui Stuff ****************************/


	private void makeWeather ()
	{
		Weather new_weather = gameObject.AddComponent<Weather> ();
		do {
			WeatherModel new_model = new_weather.init ("cloud", this);
			weather_list.Add(new_model);
		} while (--weather_count != 0);
	}

	public void makeSeeds(){
		int i = 0;
		while (i < seed_count) {
			float x = Random.Range (x_coord * -1, x_coord);
			float y = Random.Range (y_coord, y_coord * -1);
			Collider2D col = Physics2D.OverlapArea(new Vector2(x -2f, y - 2f), new Vector2(x + 2f, y + 2f)); 
			if (!col && Mathf.Abs(x) > 5f && Mathf.Abs(y) > 5f) {
				createSeed (x, y);
				i++;
			}
		}
	}

	public void clearWeather() {
		foreach (WeatherModel w_model in weather_list) {
			w_model.move_location();
		}
	}

	private void createSeed(float x, float y){
		GameObject seedObject = new GameObject ();
		Seed seed = seedObject.AddComponent<Seed> ();
		seedObject.name = "Seed";
		seedObject.transform.parent = seedFolder.transform;
		print ("Seeeeds" + x + " " + y);
		seed.init (x, y, this);
	}

	public void clearAllTrails(){
		if (zenMode) {
			foreach (Bird birb in dead_bird_list.Values) {
				print (birb.model2);
				if (birb.model2.birdTrail){
					birb.model2.birdTrail.Clear ();
				}

			}
		}
	}
}

