using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bird : MonoBehaviour
{

	private BirdModel model;
	public GameManager gm;
	// The model object.
	public Vector3 mouse_pos, world_pos;
	public Vector2 direction;
	public int counter = 0;
	private float cameraDiff;
	int screen_x, screen_y;
	float distanceFromMouse = 2;
	float speed_slider = 8f;
	// float speed = Screen.width / Screen.height * 8;
	float speed;
	public bool hasTrail;
	Vector2 slider_coords = new Vector2 (10, 10);
	Vector2 slider_size = new Vector2 (150, 30);
	Rect slider_rect, slider_box_rect;
	bool dead = false;

	//Variables For Replay
	public List<Vector3> positions;
	private Vector3 objPos;
	int index = 0; //index for positions list
	int ind = 0; // index for playback
	public bool playback = false;
	public bool first = true;
	private BirdModel model2;
	int i = 0; 

	public AudioSource birdAudio;
	public AudioClip birdClip;
	public Color trailColor;



	void OnGUI ()
	{
		if (!playback) {
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
		trailColor = getColor ();
		birdClip = Resources.Load<AudioClip> ("Sounds/Bird" + getsoundNum());
		initBirdModel (true);


		positions = new List<Vector3>(100);	//intiate position list for replay
	}

	void Update ()
	{
		if (!playback) {
			speed = Screen.width / Screen.height * speed_slider;
			updateCounter ();
			getMousePos ();
			if (counter % distanceFromMouse == 0) {
				//			direction = mouse_pos;
				move ();
				rotateTowardMouse ();
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
	}

	/****************** Replay Functions ***************/
	void recordPosition(){
		objPos = transform.position;
		positions.Add(objPos);
//		Debug.Log (index);
//		Debug.Log ("current posistion is: " + positions[index]);

		index++;
	}

	public void replay(int inx){

		model2.transform.position = positions [inx];
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
			GameObject modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);	// Create a quad object for holding the bird texture.
			model = modelObject.AddComponent<BirdModel> ();						// Add a bird_model script to control visuals of the bird.
			addTrail (modelObject, trailColor);
			addSound (modelObject, birdClip);
			model.init (this);
		} else {
			GameObject modelObject2 = GameObject.CreatePrimitive (PrimitiveType.Quad);	// Create a quad object for holding the bird texture.
			model2 = modelObject2.AddComponent<BirdModel> ();						// Add a bird_model script to control visuals of the bird.
			addTrail (modelObject2, trailColor);
			addSound (modelObject2, birdClip);
			model2.init (this);
			model2.mat.color = Color.black;
			//Set collider for dead birds to be a trigger
			this.GetComponent<CircleCollider2D>().isTrigger = true;
		}
	}

	void move ()
	{
		GameObject bird = this.gameObject;
		bird.transform.position = Vector2.MoveTowards (transform.position, mouse_pos, Time.deltaTime * speed);
	}

	void addTrail(GameObject modelObject, Color trailColor){
		TrailRenderer trail = modelObject.AddComponent<TrailRenderer> ();
		trail.time = 15;
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
		print ("print soundNum: " + soundNum.ToString());
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
		Vector3 cur_mouse_pos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
		Vector3 look_pos = Camera.main.ScreenToWorldPoint (cur_mouse_pos);
		look_pos = look_pos - transform.position;
		float angle = Mathf.Atan2 (look_pos.y, look_pos.x) * Mathf.Rad2Deg;
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
		//if the bird is outside of the boundaries (with a buffer for the size of the model)
		//move it back to the edge of the map
		if (Mathf.Abs (this.transform.position.x) > GameManager.x_coord - check) {
			if (this.transform.position.x > 0) {
				this.transform.position = new Vector3 (GameManager.x_coord - move, this.transform.position.y, 0);
			} else {
				this.transform.position = new Vector3 (GameManager.x_coord * -1 + move, this.transform.position.y, 0);
			}
		}
		if (Mathf.Abs (this.transform.position.y) > GameManager.y_coord*-1 - check) {
			if (this.transform.position.y > 0) {
				this.transform.position = 
					new Vector3 (this.transform.position.x, GameManager.y_coord*-1 - move, 0);
			} else {
				this.transform.position = new Vector3 (this.transform.position.x, GameManager.y_coord + move, 0);
			}
		}
	}

}

