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

	public bool web = true;
	public GuiState state;
	GUIStyle guiStyle;
	GUIStyle guiStyle2;
	GUIStyle scoreStyle;
	GUIStyle buttonStyle;
	GUIStyle textStyle;
	GUIContent homebutton;
	GUIContent coveybutton;
	GUIContent flightbutton;
	GUIContent flockbutton;
	GUIContent migrationbutton;
	GUIContent zenbutton;
	GUIContent helpbutton;
	GUIContent quitbutton;
	GUIStyleState buttonHover;
	GUIStyleState homeHover;
	GUIStyleState homeHoverAlt;
	GUIStyleState zenHover;
	GUIStyleState migrationHover;
	GUIStyleState helpHover;
	GUIStyleState quitHover;
	GUIStyleState quitHoverAlt;

	public float score; //in game score
    float highscore; //player's overall highscore
	float fastestTime; //fastest completed game
	float clock;

	public bool zenMode;
	public AudioSource gameAudio;
	public AudioClip gameClip;
	public AudioSource migrationAudio;
	public AudioClip migrationClip;
	int checkCall;
	public AudioSource countdownAudio;
	public AudioClip countdownClip;

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
	public bool three;
	public bool two;
	public bool one;
	public bool fly;
	public bool covey;
	public bool flight;
	public bool flock;

	public int bird_num;
	float birdspeed = 5f;

	Destination dest;

	int weather_count = 50;
	public List<Weather> weather_list = new List<Weather>();

	int seed_count = 10;

	public GameObject seedFolder;
	public GameObject cloudFolder;

	void Start ()
	{
		initStyles ();

		seedFolder =  new GameObject();
		seedFolder.name = "Seeds";

		cloudFolder =  new GameObject();
		cloudFolder.name = "Clouds";
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
		initCountdownSound ();

        //loads a previous high score if it exists
        highscore = PlayerPrefs.GetInt("High Score");
		fastestTime = PlayerPrefs.GetFloat ("Fastest Time");

    }

	private void initStyles(){
		Cursor.SetCursor((Texture2D)Resources.Load("Textures/cursor"), new Vector2(4,4), CursorMode.Auto);

		guiStyle = new GUIStyle ();
		//guiStyle.font = (Font)Resources.Load("Fonts/Mathlete-Skinny");
		guiStyle.alignment = TextAnchor.MiddleCenter;
		guiStyle.font = (Font)Resources.Load("Fonts/Mathlete-Skinny");
//		if (web) {
//			guiStyle.fontSize = 50;
//		}

		//HOME MENU
		guiStyle2 = new GUIStyle ();
		guiStyle2.fontSize = 200;
		if (web) {
			guiStyle2.fontSize = 150;
		}
		guiStyle2.alignment = TextAnchor.MiddleCenter;
		guiStyle2.font = (Font) Resources.Load("Fonts/Metrica");
		guiStyle2.normal.textColor = new Color (.40f, .23f, .58f, .9f);

		//SCORE
		scoreStyle = new GUIStyle ();
		scoreStyle.font = (Font)Resources.Load("Fonts/Mathlete-Skinny");
		scoreStyle.fontSize = 30;
		scoreStyle.normal.textColor = new Color (0f, 0f, 0f, .5f);

		//HOME BUTTON
		buttonStyle = new GUIStyle ();
		buttonStyle.font = (Font) Resources.Load("Fonts/Mathlete-Skinny");
		homebutton = new GUIContent ();
		homebutton.image = Resources.Load<Texture2D> ("Textures/nest");
		coveybutton = new GUIContent ();
		coveybutton.image = Resources.Load<Texture2D> ("Textures/covey");
		flightbutton = new GUIContent ();
		flightbutton.image = Resources.Load<Texture2D> ("Textures/flight");
		flockbutton = new GUIContent ();
		flockbutton.image = Resources.Load<Texture2D> ("Textures/flock");
		migrationbutton = new GUIContent ();
		migrationbutton.image = Resources.Load<Texture2D> ("Textures/migration");
		zenbutton = new GUIContent ();
		zenbutton.image = Resources.Load<Texture2D> ("Textures/zen");
		helpbutton = new GUIContent ();
		helpbutton.image = Resources.Load<Texture2D> ("Textures/help");
		quitbutton = new GUIContent ();
		quitbutton.image = Resources.Load<Texture2D> ("Textures/quit");
		buttonStyle.normal.textColor = new Color (0, 0, 0, .3f);
		//homebutton.text = "Home";
		buttonHover = new GUIStyleState ();
		buttonHover.background = Resources.Load<Texture2D> ("Textures/glow");
		homeHover = new GUIStyleState ();
		homeHover.background = Resources.Load<Texture2D> ("Textures/homeglow");
		homeHoverAlt = new GUIStyleState ();
		homeHoverAlt.background = Resources.Load<Texture2D> ("Textures/homeglow2");
		zenHover = new GUIStyleState ();
		zenHover.background = Resources.Load<Texture2D> ("Textures/zenglow");
		migrationHover = new GUIStyleState ();
		migrationHover.background = Resources.Load<Texture2D> ("Textures/migrationglow");
		helpHover = new GUIStyleState ();
		helpHover.background = Resources.Load<Texture2D> ("Textures/helpglow");
		quitHover = new GUIStyleState ();
		quitHover.background = Resources.Load<Texture2D> ("Textures/quitglow");
		quitHoverAlt = new GUIStyleState ();
		quitHoverAlt.background = Resources.Load<Texture2D> ("Textures/quitglow2");
		buttonStyle.hover = buttonHover;

		// Help text
		textStyle = new GUIStyle(buttonStyle);
		textStyle.fontSize = 60;
		if (web) {
			textStyle.fontSize = 40;
		}
		textStyle.font = (Font) Resources.Load("Fonts/Mathlete-Skinny");
		textStyle.normal.textColor = new Color (.9f, .8f, .9f, 1f);
		textStyle.richText = true;
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
			birdSpeed = 5f;

			BGSCALE = 2f;
			birdSpeed = 5f;

			clock = 0f;
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
		three = true;
		two = true;
		one = true;
		fly = true;

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

	private void initCountdownSound(){
		countdownAudio = this.gameObject.AddComponent<AudioSource> ();
		countdownAudio.loop = false;
		countdownAudio.playOnAwake = false;
	}

	private void countdownSound321(){
		countdownClip = Resources.Load<AudioClip> ("Sounds/CountdownSounds/StartCountdown(321_sound)");
		countdownAudio.clip = countdownClip;
		countdownAudio.Play();
	}

	private void countdownSoundGo(){
		countdownClip = Resources.Load<AudioClip> ("Sounds/CountdownSounds/StartCountdown(go_sound)");
		countdownAudio.clip = countdownClip;
		countdownAudio.Play();
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
				/*if (zenMode) {
					if (i >= mouse.movements.Count || clearAll) {		//lol wacky trailz
						mouse.model2.birdTrail.Clear ();
						clearAll = true;
					}
				}*/

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
		if ((Input.GetKeyDown ("space") || Input.GetKeyDown ("escape")) && (state.mode == 5 || state.mode == 4)){
			if (!pause && go) {
				pause = true;
				state.mode = 4;
			} else {
				pause = false;
				state.mode = 5;
			}

		}
		if (state.mode == 5 && !zenMode) {
			clock += Time.deltaTime;
		}

		if(bird_num < 0){
			
			done = true;
			state.mode = 7; //Go to End Game Screen
            if(score > highscore && !zenMode) //updates highscore if player beat their previous high score in migration mode
            {
                Debug.Log("You've Beat Your Previous High Score!! Old High Score is " + highscore);
                highscore = score;
                PlayerPrefs.SetInt("High Score", (int) highscore);

                Debug.Log("New High Score is " + highscore);
            }
			if (fastestTime == 0f && !zenMode) {
				fastestTime = clock;
				PlayerPrefs.SetFloat ("Fastest Time", fastestTime);
			}
			else if (clock < fastestTime && !zenMode) {
				fastestTime = clock;
				PlayerPrefs.SetFloat ("Fastest Time", fastestTime);
			}

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
				makeSeeds ();
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
		case 8:
			helpScreen ();
			break;
		}
	}

	private void getMode(){
		int xpos;
		int ypos;
		if ((!go && !done) || (pause && !done)) {
			xpos = ((Screen.width) - (300)) / 2;
			ypos = ((Screen.height) - (80)) / 2 - ((Screen.height / 3)-(Screen.height/10));
			GUI.Label (new Rect (xpos, ypos, 300, 50), "FLOCKING", guiStyle2);
		}
		if (!go && !done) {
			xpos = (((Screen.width) - (150)) / 2) +150;
			ypos = ((Screen.height) / 2 - (0));
			buttonStyle.hover = zenHover;
			if (GUI.Button (new Rect (xpos, ypos, 150, 225), zenbutton, buttonStyle)) {
                zenMode = true;
                state.mode = 1;

			}
			xpos = (((Screen.width) - (150)) / 2)-150;
			ypos = ((Screen.height) / 2 - (0));
			buttonStyle.hover = migrationHover;
			if (GUI.Button (new Rect (xpos, ypos, 150, 225), migrationbutton, buttonStyle)) {
				state.mode = 2;
			}
			xpos = ((Screen.width) - (60)) / 2 + 50;
			ypos = ((Screen.height) / 2 + (200));
			buttonStyle.hover = helpHover;
			if (GUI.Button (new Rect (xpos, ypos, 60, 90), helpbutton, buttonStyle)) {
				state.mode = 8;
			}

			xpos = ((Screen.width) - (60)) / 2 -50;
			ypos = ((Screen.height) / 2 + (200));
			buttonStyle.hover = quitHover;
			if (GUI.Button (new Rect (xpos, ypos, 60, 90), quitbutton, buttonStyle)) {
				Application.Quit();
			}
		}
	}
    
    
    
    //Options screen for zen mode. Get to Choose lifetime of bird and number of birds.
	private void zenOptions ()
	{
        bird_num = 30;
        state.mode = 3;

        /*
		guiStyle.font = (Font) Resources.Load("Fonts/Mathlete-Skinny");
		showNumSlider ();
		showLifeSlider ();
		bird_num = 30;
		int xpos = ((Screen.width) - (150)) / 2;
		int ypos = ((Screen.height) / 2 + (Screen.height / 8));
		if (GUI.Button (new Rect (xpos, ypos, 150, 60), "FLY!")) {
			state.mode = 3;
		}
			
		xpos = ((Screen.width) - (90)) / 2;
		ypos = ((Screen.height) / 2 + (Screen.height / 4));
		buttonStyle.hover = homeHover;
		if (GUI.Button (new Rect (xpos, ypos, 90, 135), homebutton, buttonStyle)) {
			Debug.Log ("menu");
			Application.LoadLevel (Application.loadedLevel);
			state.mode = 0;
		}
        */
	}
    
	private void migrationOptions ()
	{
		zenMode = false;
		covey = false;
		flight = false;
		flock = false;
		//showNumSlider ();
		guiStyle.fontSize = 100;
		guiStyle.normal.textColor = new Color (.80f, .63f, .98f, .3f);
		guiStyle.alignment = TextAnchor.MiddleCenter;
		guiStyle.font = (Font) Resources.Load("Fonts/Mathlete-Skinny");
		int xpos = ((Screen.width) - (300)) / 2;
		int ypos = ((Screen.height) - (50)) / 2 - ((Screen.height / 3)-(Screen.height/30));
		GUI.Label (new Rect (xpos, ypos, 300, 50), "HOW MANY BIRDS?", guiStyle);

		int xpos1 = ((Screen.width) - (60)) / 3;
		int ypos1 = ((Screen.height) / 2 -50);// + (Screen.height / 8));
		int xpos2 = ((Screen.width) - (90)) / 2;
		int ypos2 = ((Screen.height) / 2 -50);// + (Screen.height / 8));
		int xpos3 = (((Screen.width) - (120)) / 3)*2;
		int ypos3 = ((Screen.height) / 2 -50);// + (Screen.height / 8));
		buttonStyle.hover = buttonHover;
		//if (GUI.Button (new Rect (xpos1, ypos1, 150, 60), "A COVEY (3)")) {
		if (GUI.Button (new Rect (xpos1, ypos1, 90, 90), coveybutton, buttonStyle)) {
			bird_num = 3;
			covey = true;
			state.mode = 3;
		}
		else if (GUI.Button (new Rect (xpos2, ypos2, 90, 90), flightbutton, buttonStyle)) {
			bird_num = 6;
			flight = true;
			state.mode = 3;
		}
		else if (GUI.Button (new Rect (xpos3, ypos3, 90, 90), flockbutton, buttonStyle)) {
			bird_num = 9;
			flock = true;
			state.mode = 3;
		}

		xpos = ((Screen.width) - (90)) / 2;
		ypos = ((Screen.height) / 2 + (Screen.height / 6));
		buttonStyle.hover = homeHover;
		if (GUI.Button (new Rect (xpos, ypos, 90, 135), homebutton, buttonStyle)) {
			Debug.Log ("menu");
			//Application.LoadLevel (Application.loadedLevel);
			state.mode = 0;
		}
	}

	//private void showNumSlider ()
	//{
	//	Vector2 num_slider_coords = new Vector2 (((Screen.width) - (150)) / 2, (Screen.height) / 2);
	//	Vector2 num_slider_size = new Vector2 (150, 30);
	//	Rect num_slider_rect, num_slider_box_rect;

	//	num_slider_rect = new Rect (num_slider_coords.x, num_slider_coords.y, num_slider_size.x, num_slider_size.y);
	//	num_slider_box_rect = new Rect (num_slider_coords.x, num_slider_coords.y + num_slider_size.y, num_slider_size.x, num_slider_size.y);

	//	GUI.Box (num_slider_rect, "Number of Birds: " + bird_num.ToString ());
	//	bird_num = (int)GUI.HorizontalSlider (num_slider_box_rect, (float)bird_num, 1.0F, 30.0F);
	//}

	//private void showLifeSlider ()
	//{
	//	Vector2 life_slider_coords = new Vector2 (((Screen.width) - (150)) / 2, (Screen.height) / 2 - (Screen.height / 8));
	//	Vector2 life_slider_size = new Vector2 (150, 30);
	//	Rect life_slider_rect, life_slider_box_rect;

	//	life_slider_rect = new Rect (life_slider_coords.x, life_slider_coords.y, life_slider_size.x, life_slider_size.y);
	//	life_slider_box_rect = new Rect (life_slider_coords.x, life_slider_coords.y + life_slider_size.y, life_slider_size.x, life_slider_size.y);

	//	GUI.Box (life_slider_rect, "Bird Lifetime (sec): " + bird_life.ToString ());
	//	bird_life = (int)GUI.HorizontalSlider (life_slider_box_rect, (float)bird_life, 1.0F, 60.0F);
	//}

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
			displayScore ();
		}
		Time.timeScale = 0;
		int xpos = ((Screen.width) - (450)) / 2;
		int ypos = ((Screen.height) + (200)) / 2 - (Screen.height / 3);
		if (!done && pause) {
			guiStyle.fontSize = 500;
			if (web) {
				guiStyle.fontSize = 450;
			}
			guiStyle.normal.textColor = new Color (255, 255, 255, .5f);
			GUI.Label (new Rect (xpos, ypos, 500, 10), "PAUSED", guiStyle);
		} else {
			state.mode = 5;
		}
		xpos = ((Screen.width) - (90)) / 2 + 70;
		ypos = ((Screen.height) / 2 + (Screen.height / 6));
		buttonStyle.hover = homeHoverAlt;
		if (GUI.Button (new Rect (xpos, ypos, 90, 135), homebutton, buttonStyle)) {
			Debug.Log ("menu");
			Application.LoadLevel (Application.loadedLevel);
			state.mode = 0;
		}
		xpos = ((Screen.width) - (90)) / 2 - 70;
		ypos = ((Screen.height) / 2 + (Screen.height / 6));
		buttonStyle.hover = quitHoverAlt;
		if (GUI.Button (new Rect (xpos, ypos, 90, 135), quitbutton, buttonStyle)) {
			Application.Quit();
		}
	}

	private void displayScore() {
		GUI.Label (new Rect (Screen.width - 100, -1, 100, 30), "Score: " + (int)score, scoreStyle);
	}

	private void unpauseGame ()
	{
		if (!zenMode) {
			displayScore ();
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
			displayScore ();
		}
		if (Time.unscaledDeltaTime > .1) {
			print ("OMG DELTA TIME IS HUGE : " + Time.unscaledDeltaTime);
		} else {
			loadScreenCounter -= Time.unscaledDeltaTime * .5f;
		}
		int countdown = (int)loadScreenCounter+1;
		int xpos1 = ((Screen.width) - (200)) / 2;
		int xpos2 = ((Screen.width) - (100)) / 2;
		int ypos = ((Screen.height) - (200)) / 2;
		guiStyle.fontSize = 60;
		guiStyle.normal.textColor = new Color (0f, 0f, 0f, .3f);
//		if (zenMode) {
//
//		}
		if (loadScreenCounter > 3) {
			GUI.Label (new Rect (xpos1, ypos, 200, 60), "Bird " + bird_count + " Ready?", guiStyle);
		} else if (loadScreenCounter > 2) {
			GUI.Label (new Rect (xpos2, ypos, 100, 60), countdown.ToString () + "...", guiStyle);
			if (three) {
				countdownSound321 ();
				three = false;
			} 
		}
		else if (loadScreenCounter > 1) {
			GUI.Label (new Rect (xpos2, ypos, 100, 60), countdown.ToString () + "...", guiStyle);
			if (two) {
				countdownSound321 ();
				two = false;
			} 
		}
		else if (loadScreenCounter > 0) {
			GUI.Label (new Rect (xpos2, ypos, 100, 60), countdown.ToString () + "...", guiStyle);
			if (one) {
				countdownSound321 ();
				one = false;
			} 
		}
		else if(loadScreenCounter > -1){
			GUI.Label (new Rect (xpos2, ypos, 100, 60), "Fly!", guiStyle);
			if (fly) {
				countdownSoundGo ();
				fly = false;
			}
		}
		else {
			state.mode = 5;
		}
	}

	private void endScreen(){
		go = false;
		guiStyle.font = (Font) Resources.Load("Fonts/Mathlete-Skinny");
		guiStyle.fontSize = 350;
		if (web) {
			guiStyle.fontSize = 300;
		}
		guiStyle.normal.textColor = new Color (.40f, .23f, .58f, .5f);
		guiStyle.alignment = TextAnchor.MiddleCenter;
		int xpos = ((Screen.width) - (300)) / 2;
		int ypos = ((Screen.height) - (100)) / 2 - ((Screen.height / 3)-(Screen.height/30));
		GUI.Label (new Rect (xpos, ypos, 300, 50), "GAME OVER", guiStyle);


		//SCORE
		if (!zenMode) {
			guiStyle.fontSize = 60;
			guiStyle.font = (Font)Resources.Load ("Fonts/Mathlete-Skinny");
			guiStyle.normal.textColor = new Color (1f, 1f, 1f, .4f);

			//Display Scores
			xpos = ((Screen.width) - (300)) / 2 - 200;
			ypos = ((Screen.height) + (250)) / 2 - ((Screen.height / 4));// - (Screen.height / 30));
			GUI.Label (new Rect (xpos, ypos, 300, 50), "Final Score: " + (int)score, guiStyle);
			xpos = ((Screen.width) - (300)) / 2 - 200;
			ypos = ((Screen.height) + (400)) / 2 - ((Screen.height / 4));// - (Screen.height / 30));
			GUI.Label (new Rect (xpos, ypos, 300, 50), "High Score: " + (int)highscore, guiStyle);

			//Display times
			xpos = ((Screen.width) - (300)) / 2 + 200;
			ypos = ((Screen.height) + (250)) / 2 - ((Screen.height / 4));// - (Screen.height / 30));
			GUI.Label (new Rect (xpos, ypos, 300, 50), "Final Time: " + clock.ToString("N2"), guiStyle);
			xpos = ((Screen.width) - (300)) / 2 + 200;
			ypos = ((Screen.height) + (400)) / 2 - ((Screen.height / 4));// - (Screen.height / 30));
			GUI.Label (new Rect (xpos, ypos, 300, 50), "Fastest Time: " + fastestTime.ToString("N2"), guiStyle);
		}

		//MENU
		if (web) {
			xpos = ((Screen.width) - (90)) / 2;
			ypos = ((Screen.height) / 2 + (Screen.height / 4));
		} else {
			xpos = ((Screen.width) - (90)) / 2;
			ypos = ((Screen.height) / 2 + (Screen.height / 8));
		}
		buttonStyle.hover = homeHoverAlt;
		if (GUI.Button (new Rect (xpos + 70, ypos, 90, 135), homebutton, buttonStyle)) {
			Debug.Log ("menu");
			Application.LoadLevel (Application.loadedLevel);
			state.mode = 0;
		}
		buttonStyle.hover = quitHoverAlt;
		if (GUI.Button (new Rect (xpos - 70, ypos, 90, 135), quitbutton, buttonStyle)) {
			Application.Quit();
		}

		/*//RESTART
		xpos = ((Screen.width) - (325)) / 2;
			ypos = ((Screen.height) / 2 + (Screen.height / 8));
		if (GUI.Button (new Rect (xpos, ypos, 150, 60), "Restart")) {
			Debug.Log ("restart");
			Application.LoadLevel (Application.loadedLevel);
			state.mode = 2;
		}*/
	}

	private void helpScreen(){
		

		string us = "<size=35>   Made by Alejandro Belgrave, Andres Cuervo, Linnea Kirby, Emily Rizzo, and Margaret McCarthy.</size>";

		GUILayout.BeginArea(new Rect(10, Screen.height /8, Screen.width, 35));
		GUILayout.Label("\n\n\n\n"+us, textStyle);
        GUILayout.EndArea();
		int xpos;// = (900);
		int ypos;// = ((Screen.height) - (50));
		//GUI.Label (new Rect (xpos, ypos, 900, 50), us, guiStyle);


		if (web) {
			GUILayout.BeginArea(new Rect(0, 10, Screen.width, Screen.height * (3.5f/4)));
			GUILayout.Label ("\t                    <size=100>Flocking</size>", textStyle);
			GUILayout.EndArea();
		} else {
			GUILayout.BeginArea(new Rect(10, 10, Screen.width, Screen.height * (3.5f/4)));
			GUILayout.Label ("\t             <size=120>Flocking</size>", textStyle);
			GUILayout.EndArea();
		}

		if (web) {
			GUILayout.BeginArea (new Rect (10, 10, Screen.width, Screen.height));
			GUILayout.Label ("\n\n   <size=80>Migration Mode</size>", textStyle);
			GUILayout.Label (
				"  <size=25>  Pick the size of your flock and then build it bird by bird, \n" +
				"     keeping all your birds close. Help your flock migrate towards \n" +
				"     the sunset, which has randomly spawned in a corner. \n" +
				"     You gain points the longer you explore the map, \n" +
				"     but watch out - the clouds are waiting to roast \n" +
				"     your tiny bird body!</size>", textStyle);
			GUILayout.Label ("<size=25>\n</size>");
			GUILayout.Label (us, textStyle);
			GUILayout.EndArea ();
		} else {
			GUILayout.BeginArea (new Rect (10, 10, Screen.width, Screen.height));
			GUILayout.Label ("\n\n   <size=80>Migration Mode</size>", textStyle);
			GUILayout.Label (
				"  <size=30>  Pick the size of your flock and then build it bird by bird, \n" +
				"     keeping all your birds close. Help your flock migrate towards \n" +
				"     the sunset, which has randomly spawned in a corner. \n" +
				"     You gain points the longer you explore the map, \n" +
				"     but watch out - the clouds are waiting to roast \n" +
				"     your tiny bird body!</size>", textStyle);
			GUILayout.Label ("<size=25>\n</size>");
			GUILayout.Label (us, textStyle);
			GUILayout.EndArea ();

		}
		if (web) {
			GUILayout.BeginArea (new Rect (Screen.width / 2 -10, 10, Screen.width / 2, Screen.width / 2));
			GUILayout.Label (" <size=25></size>", textStyle);
			GUILayout.Label ("\n                   <size=80>  Zen Mode</size>", textStyle);
			GUILayout.Label (
				"<size=25>                You get to make pretty swirls, swoops, and swooshes \n" +
				"             with the birds. No weather or tiny birdy skeletons here,\n" +
				"                                    just nice sounds and colors.</size>", textStyle);
			GUILayout.EndArea ();

		} else {
			GUILayout.BeginArea (new Rect (Screen.width / 2 + 10, 10, Screen.width / 2, Screen.width / 2));
			GUILayout.Label (" <size=30></size>", textStyle);
			GUILayout.Label ("\n                   <size=80>  Zen Mode</size>", textStyle);
			GUILayout.Label (
				"<size=30>                You get to make pretty swirls, swoops, and swooshes \n" +
				"             with the birds. No weather or tiny birdy skeletons here,\n" +
				"                                      just nice sounds and colors.</size>", textStyle);
			GUILayout.EndArea ();

		}
		if (web) {
			GUILayout.BeginArea (new Rect (Screen.width / 2 - Screen.width / 15, (Screen.height / 3) * 2 - 80, Screen.width / 3, Screen.height / 3));
			GUILayout.Label ("<size=60>  Controls</size>", textStyle);
			GUILayout.Label ("<size=25>      spacebar/esc pauses</size>", textStyle);
			GUILayout.Label ("<size=25>  new birds follow your cursor</size>", textStyle);
			GUILayout.EndArea ();
		} else {
			GUILayout.BeginArea (new Rect (Screen.width / 2 - Screen.width / 12, (Screen.height / 3) * 2 - 100, Screen.width / 3, Screen.height / 3));
			GUILayout.Label ("<size=80>  Controls</size>", textStyle);
			GUILayout.Label ("<size=30>      spacebar/esc pauses</size>", textStyle);
			GUILayout.Label ("<size=30>  new birds follow your cursor</size>", textStyle);
			GUILayout.EndArea ();
		}

		xpos = ((Screen.width) - (300)) / 2;
		ypos = ((Screen.height) - (100)) / 2 - ((Screen.height / 3)-(Screen.height/30));
		xpos = ((Screen.width) - (90) - Screen.width/15);
		ypos = ((Screen.height)-135) - Screen.height/30;
		buttonStyle.hover = homeHover;
		if (web) {
			if (GUI.Button (new Rect (xpos, ypos-40, 90, 135), homebutton, buttonStyle)) {
				Debug.Log ("menu");
				//Application.LoadLevel (Application.loadedLevel);
				state.mode = 0;
			}
		} else {
			if (GUI.Button (new Rect (xpos, ypos, 90, 135), homebutton, buttonStyle)) {
				Debug.Log ("menu");
				//Application.LoadLevel (Application.loadedLevel);
				state.mode = 0;
			}
		}
	}


	/************************ End GUI Stuff ****************************/


	private void makeWeather ()
	{
//		print ("lol");
		
		do {
			GameObject weatherObject = new GameObject();
			Weather new_weather = weatherObject.AddComponent<Weather> ();
			new_weather.init ("cloud", this);
			weather_list.Add(new_weather);
			new_weather.transform.parent = cloudFolder.transform;
		} while (--weather_count != 0);
	}

	public void makeSeeds(){
		int seeds = 0;
//		print ("ha");
		int tries = 0;
		float scale = BGSCALE * 3 / 4;
		while (seeds < seed_count && tries < 500) {
			float x = Random.Range (x_coord * scale * -1, x_coord*scale);
			float y = Random.Range (y_coord * scale, y_coord * -1*scale);
			Collider2D col = Physics2D.OverlapArea(new Vector2(x -2f, y - 2f), new Vector2(x + 2f, y + 2f)); 
			if (!col && Mathf.Abs (x) > 4f && Mathf.Abs (y) > 4f) {
				createSeed (x, y);
				seeds++;
			} else {
				tries++;
//				print ("thinkin about seeds <3");
			}
			if (tries >= 500) {
				print ("too many seeeds :o");
			}

		}
	}

	public void clearWeather() {
		foreach (Weather w in weather_list) {
			w.move_location();
		}
	}

	private void createSeed(float x, float y){
		GameObject seedObject = new GameObject ();
		Seed seed = seedObject.AddComponent<Seed> ();
		seedObject.name = "Seed";
		seedObject.transform.parent = seedFolder.transform;
//		print ("Seeeeds" + x + " " + y);
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

