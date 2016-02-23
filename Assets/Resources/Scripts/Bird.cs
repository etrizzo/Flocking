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
	Vector2 slider_coords = new Vector2 (10, 10);
	Vector2 slider_size = new Vector2 (150, 30);
	Rect slider_rect, slider_box_rect;

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
		speed = Screen.width / Screen.height * speed_slider;
		updateCounter ();
		getMousePos ();
		if (counter % distanceFromMouse == 0) {
			//			direction = mouse_pos;
			move ();
			rotateTowardMouse ();
		}
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

	void addTrail(GameObject modelObject){
		TrailRenderer trail = modelObject.AddComponent<TrailRenderer> ();
		trail.time = 30f;
		trail.startWidth = .05f;
		trail.endWidth = .05f;
		trail.material.color = new Color(Random.value, Random.value, Random.value);
	}

	private Color getColor(){
		float r = Random.value;
		float g = Random.value;
		float b = Random.value;
		print ("r is: " + r);
		print ("g is: " + g);
		print ("b is: " + b);
		return new Color (r, g, b, .5f);
	}

	void initBirdModel ()
	{
		GameObject modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);	// Create a quad object for holding the bird texture.
		model = modelObject.AddComponent<BirdModel> ();	// Add a bird_model script to control visuals of the bird.
		addTrail(modelObject);
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
}

