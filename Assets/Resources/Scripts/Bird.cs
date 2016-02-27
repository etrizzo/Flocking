using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour
{

	private BirdModel model;

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
//	float maxspeed;
	private bool mouse = true;
	Vector2 slider_coords = new Vector2 (10, 10);
	Vector2 slider_size = new Vector2 (150, 30);
	Rect slider_rect, slider_box_rect;
	public Vector3 vel = Vector3.zero;


	//For Arrows
	//float arrow_speed = 2f; // now dependent on speed slider. May want to change
	float rotation_angle = 100f; //how to update subject to change

	void OnGUI ()
	{
		GUI.Box (slider_rect, "Speed: " + speed_slider.ToString ());
		speed_slider = GUI.HorizontalSlider (slider_box_rect, speed_slider, 0.0F, 20.0F);
	}

	public void  Start ()
	{
		initSlider ();
		getMousePos ();
		cameraDiff = Camera.main.transform.position.y - this.transform.position.y;
		direction = new Vector2 (0, 1);
		initBirdModel ();

	}

	void Update ()
	{
		if (mouse) {
			speed = Screen.width / Screen.height * speed_slider;
//			maxspeed = speed;		///may try to fix edge jitter with this later
			updateCounter ();
			getMousePos ();
			if (counter % distanceFromMouse == 0) {
				//			direction = mouse_pos;
				move ();
				rotateTowardMouse ();
			}
		} else {
			speed = speed_slider;
			arrowMove ();
		}
		if (!GameManager.zen) {
			moveCam ();
		}
		checkBoundaries ();
	}

	void arrowMove(){
		float rot_speed = Input.GetAxis ("Vertical");        //-1, 1, or 0 depending on up/down keys
		float rotation = Input.GetAxis ("Horizontal") * rotation_angle * -1;
		rot_speed = ((rot_speed / 2) + 1);    //changes rotation speed multiplier to  1.5 or .5
		transform.Translate (0, speed*Time.deltaTime, 0);
		transform.Rotate (0, 0, (rotation * rot_speed) * Time.deltaTime);

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

	void getMousePos ()
	{
		mouse_pos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10));
	}

	void initSlider ()
	{
		slider_rect = new Rect (slider_coords.x, slider_coords.y, slider_size.x, slider_size.y);
		slider_box_rect = new Rect (slider_coords.x, slider_coords.y + slider_size.y, slider_size.x, slider_size.y);
	}

	void initBirdModel ()
	{
		GameObject modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);	// Create a quad object for holding the bird texture.
		model = modelObject.AddComponent<BirdModel> ();						// Add a bird_model script to control visuals of the bird.
		model.init (this);

	}

	void move ()
	{
		GameObject bird = this.gameObject;
		bird.transform.position = Vector2.MoveTowards (transform.position, mouse_pos, Time.deltaTime * speed);
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

	//makes the edge jitter like a tiny bit better
	//work on this more later
	void checkBoundaries(){
		float smooth = 3f;
		//print (speed + " (" + GameManager.x_coord + " " + GameManager.screen_height + ") (" + this.transform.position.x + " " + this.transform.position.y + ")");
		GameObject birb = this.gameObject;
		if (Mathf.Abs(this.transform.position.x) > GameManager.x_coord - .1f) {
			if (this.transform.position.x > 0){
				this.transform.position = new Vector3(GameManager.x_coord , this.transform.position.y,0);
			} else {
				this.transform.position = new Vector3(GameManager.x_coord*-1, this.transform.position.y,0);
			}
		} if (Mathf.Abs(this.transform.position.y) > GameManager.screen_height/2 - .1f) {
			if (this.transform.position.y > 0){
				this.transform.position  = 
					new Vector3(this.transform.position.x, GameManager.screen_height / 2 ,0);
			} else {
				this.transform.position = new Vector3(this.transform.position.x, GameManager.screen_height / -2,0);
			}
		}

	}



	void OnCollisionEnter(){
		print ("Collision with bird");
	}
}

