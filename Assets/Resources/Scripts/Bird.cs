using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour
{

	private BirdModel model;
	// The model object.
	public Vector2 direction;

	//For Arrows
	float arrow_speed = 2f; // default speed is 8f. Subject to change
	float rotation_angle = 100f; //how to update subject to change


	public void  Start ()
	{
		direction = new Vector2 (0, 1);
		initBirdModel ();
	}

	void Update ()
	{

		float translation = Input.GetAxis ("Vertical") * arrow_speed;
		if (translation < 0) {translation = 0;}			//effectively disabling the down button... birds cant fly backwards :/

		float rotation = Input.GetAxis ("Horizontal") * rotation_angle *-1;		//multiply by -1 to negate default rotation

		float new_speed = translation *Time.deltaTime;
		rotation *= Time.deltaTime;
		transform.Translate (0, new_speed, 0);
		transform.Rotate (0, 0, rotation);
	}


	void initBirdModel ()
	{
		GameObject modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);	// Create a quad object for holding the bird texture.
		model = modelObject.AddComponent<BirdModel> ();						// Add a bird_model script to control visuals of the bird.
		model.init (this);
	}

}

