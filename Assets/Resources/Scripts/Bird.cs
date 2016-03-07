using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bird : MonoBehaviour
{
	public BirdModel model;
	public GameManager gm;
	// The model object.
	public Vector3 mouse_pos, world_pos;
	public Vector2 direction;
	public int counter = 0;
	private float cameraDiff;
	int screen_x, screen_y;
	float distanceFromMouse = 2;
	float speed_slider = 4f;
	// float speed = Screen.width / Screen.height * 8;
	float speed;

	public bool hasTrail = false;
	public bool hasRadius = false;

	Vector2 slider_coords = new Vector2 (10, 10);
	Vector2 slider_size = new Vector2 (150, 30);
	Rect slider_rect, slider_box_rect;
	bool dead = false;
	public bool alive;

	//Variables For Replay
	public List<Vector3> positions;
	public List<ArrayList> movements;
	private Vector3 objPos;
	private Quaternion objRot;
	int index = 0; //index for positions list
	int ind = 0; // index for playback
	public bool playback = false;
	public bool first = true;
	public BirdModel model2;
	int i = 0; 

	public AudioSource birdAudio;
	public AudioClip birdClip;
	public Color trailColor;

	public BirdRadius radius;

	public bool pause;
	public bool AtDestination = false;

	public bool mouse = true;
	float rotation_angle = 100f; //how to update subject to change



	void OnGUI ()
	{
		if (!playback && gm.zenMode) {
			GUI.Box (slider_rect, "Speed: " + speed_slider.ToString ());
			speed_slider = GUI.HorizontalSlider (slider_box_rect, speed_slider, 0.0F, 20.0F);
		}
	}

	public void init(GameManager gm)
	{
		this.gm = gm;
		initSlider ();
		getMousePos ();
		cameraDiff = Camera.main.transform.position.y - this.transform.position.y;
		direction = new Vector2 (0, 1);
		birdClip = Resources.Load<AudioClip> ("Sounds/Bird" + getsoundNum());
		trailColor = getColor ();
		alive = true;
		initBirdModel (alive);
		positions = new List<Vector3>(100);	//intiate position list for replay
		movements = new List<ArrayList> (); // instantiate movements 2D list
		pause = false;

	}

	public void Update ()
	{
		if (Input.GetKeyDown ("space")){
			if (!pause) {
				pause = true;
				Time.timeScale = 0;
			} else {
				pause = false;
				Time.timeScale = 1;
			}

		}
		if (!playback && !pause) {
			gm.score += Time.deltaTime;
			speed = Screen.width / Screen.height * speed_slider;
			updateCounter ();
			//getMousePos ();
			if (mouse) {
				if (counter % distanceFromMouse == 0) {
					//	direction = mouse_pos;
					move ();
					rotateTowardMouse ();
				}
			} else {
				speed = speed_slider;
				arrowMove ();
			}
			checkBoundaries ();
			recordPosition ();
		} //else {
//			speed = Screen.width / Screen.height * speed_slider;
//			if (first) {
//				direction = new Vector2 (0, 1);
//				initBirdModel (false);
//				first = false;
//			}
//			if ( i < index) {
//				replay (i);
//				i++;
//			}
//		}

		if (!gm.zenMode) {
			moveCam ();

		}
		getMousePos ();
	}

	/****************** Replay Functions ***************/
	void recordPosition(){
		objPos = transform.position;
		objRot = transform.rotation;
//		positions.Add(objPos);
		movements.Add (new ArrayList { objPos, objRot});
//		Debug.Log (index);
//		Debug.Log ("current posistion is: " + positions[index]);

		index++;
	}

	public void replay(int inx){

		model2.transform.position = (Vector3) movements[inx][0];
		model2.transform.rotation = (Quaternion) movements[inx][1];
//		Debug.Log ("inside replay for... something:   "); 
	}
	/****************** End Replay Functions ***************/

	void getMousePos ()
	{
		mouse_pos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10));
	}

	void initSlider ()
	{
		slider_rect = new Rect (slider_coords.x, slider_coords.y, slider_size.x, slider_size.y);
		slider_box_rect = new Rect (slider_coords.x, slider_coords.y + slider_size.y, slider_size.x, slider_size.y);
	}

	public void initBirdModel (bool alive)
	{
		
		if (alive) {
			Camera.main.transform.position = new Vector3 (0, 0, -10);
			GameObject modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);	// Create a quad object for holding the bird texture.
			model = modelObject.AddComponent<BirdModel> ();						// Add a bird_model script to control visuals of the bird.
			if (hasTrail) {
				addTrail (modelObject, trailColor);
				addSound (modelObject, birdClip);
			}
			model.init (this);
		} else {
			//print (positions.Count);
			GameObject modelObject2 = GameObject.CreatePrimitive (PrimitiveType.Quad);	// Create a quad object for holding the bird texture.
			//model2.birdTrail.Clear();
			model2 = modelObject2.AddComponent<BirdModel> ();						// Add a bird_model script to control visuals of the bird.
			//print(modelObject2.name);
			if (hasTrail) {
				addTrail (modelObject2, trailColor);
				addSound (modelObject2, birdClip);
			}
			model2.init (this);
			model2.mat.color = Color.black;

			//Set collider for dead birds to be a trigger


			//this.GetComponent<CircleCollider2D>().isTrigger = true;

			//model2.radiusCollider.isTrigger = true;
		}
		//Set collider for dead birds to be a trigger
		this.GetComponent<CircleCollider2D>().isTrigger = true;
	}

	void move ()
	{
		//print ("Live Bird");
		GameObject bird = this.gameObject;
		bird.transform.position = Vector2.MoveTowards (transform.position, mouse_pos, Time.deltaTime * speed);
	}

	void addTrail(GameObject modelObject, Color trailColor){
		TrailRenderer trail = modelObject.AddComponent<TrailRenderer> ();
		trail.material.shader = Shader.Find ("Sprites/Default");						// Tell the renderer that our textures have transparency.
		trail.receiveShadows = false;
		trail.time = 10;
		trail.startWidth = 0.05f;
		trail.endWidth = 0.5f;
		trail.material.color = trailColor;
	}

	void addSound(GameObject modelObject, AudioClip birdClip){
		AudioSource birdSound = modelObject.AddComponent<AudioSource> ();
		birdSound.loop = true;
//		AudioClip birdClip = Resources.Load<AudioClip> ("Sounds/Bird" + getsoundNum());
		birdSound.clip = birdClip;
		birdSound.playOnAwake = false;
	}
	private string getsoundNum(){
 		int soundNum = (int) ((Random.value * 1000) % 30 ) + 1;
		//print ("print soundNum: " + soundNum.ToString());
 		return soundNum.ToString();
 	}
	

	private Color getColor(float opacity = 0.5f){
		//print("Lololol colors");
		float r = Random.value;
		float g = Random.value;
		float b = Random.value;
//		print ("r is: " + r);
//		print ("g is: " + g);
//		print ("b is: " + b);
		return new Color (r, g, b, opacity);
	}

	void rotateTowardMouse ()
	{
		// The following is modified from:
		// http://answers.unity3d.com/questions/653798/character-always-facing-mouse-cursor-position.html
//		Vector3 cur_mouse_pos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
//		Vector3 look_pos = Camera.main.ScreenToWorldPoint (cur_mouse_pos);

		//something about the timing of when the mouse position is recorded was messing up playback in migration mode
		//changing from the other mouse position works
		Vector3 look_pos = mouse_pos - transform.position;
		float angle = Mathf.Atan2 (look_pos.y, look_pos.x) * Mathf.Rad2Deg;
		//float angle = Mathf.Atan2 (mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis (angle - 90, Vector3.forward);
	}


	void updateCounter ()
	{
		counter++;
	}

	//fixes the edge jitter
	//work on this more later - 
	//	eliminates need for collider borders but feels a little rigid (maybe an animation?)
	void checkBoundaries(){
		
		float check = .4f;
		float move = .4f;
		float x = GameManager.x_coord * GameManager.BGSCALE;
		float y = GameManager.y_coord * GameManager.BGSCALE;
		//if the bird is outside of the boundaries (with a buffer for the size of the model)
		//move it back to the edge of the map
		if (Mathf.Abs (this.transform.position.x) > x - check) {
			if (this.transform.position.x > 0) {
				this.transform.position = new Vector3 (x - move, this.transform.position.y, 0);
			} else {
				this.transform.position = new Vector3 (x * -1 + move, this.transform.position.y, 0);
			}
		}
		if (Mathf.Abs (this.transform.position.y) > y*-1 - check) {
			if (this.transform.position.y > 0) {
				this.transform.position = 
					new Vector3 (this.transform.position.x, y*-1 - move, 0);
			} else {
				this.transform.position = new Vector3 (this.transform.position.x, y + move, 0);
			}
		}

	}

	void moveCam(){
		Camera cam = Camera.main;
		float smooth = 3.0f;
		Vector3 cameraPos = new Vector3 (this.transform.position.x, this.transform.position.y, -10);
		cam.transform.position = cameraPos;
		cam.transform.position = Vector3.Lerp (
			cam.transform.position, cameraPos, Time.deltaTime * smooth
		);
	}

	void arrowMove(){
		float rot_speed = Input.GetAxis ("Vertical");        //-1, 1, or 0 depending on up/down keys
		float rotation = Input.GetAxis ("Horizontal") * rotation_angle * -1;
		rot_speed = ((rot_speed / 2) + 1);    //changes rotation speed multiplier to  1.5 or .5
		transform.Translate (0, speed*Time.deltaTime, 0);
		transform.Rotate (0, 0, (rotation * rot_speed) * Time.deltaTime);
	}

}

