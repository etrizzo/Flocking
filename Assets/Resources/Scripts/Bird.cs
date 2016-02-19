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
	float speed = Screen.width / Screen.height * 10;
	float distanceFromMouse = 2;

	public void  Start ()
	{
		getMousePos ();
		cameraDiff = Camera.main.transform.position.y - this.transform.position.y;
		direction = new Vector2 (0, 1);
		var modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);	// Create a quad object for holding the bird texture.
		model = modelObject.AddComponent<BirdModel> ();						// Add a bird_model script to control visuals of the bird.
		model.init (this);
	}

	void Update ()
	{
		updateCounter ();
		getMousePos ();
		if (counter % distanceFromMouse == 0) {
//			direction = mouse_pos;
			move ();
		}
		rotateTowardMouse ();
	}

	void getMousePos ()
	{
		mouse_pos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0));
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
		Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
		Vector3 lookPos = Camera.main.ScreenToWorldPoint (mousePos);
		lookPos = lookPos - transform.position;
		float angle = Mathf.Atan2 (lookPos.y, lookPos.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis (angle - 90, Vector3.forward);
	}


	void updateCounter ()
	{
		counter++;
	}
}

