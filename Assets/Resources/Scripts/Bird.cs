using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bird : MonoBehaviour
{

	private BirdModel model;
	// The model object.
	public Vector2 direction;

	//For Arrows
	float arrow_speed = 2f; // default speed is 8f. Subject to change
	float rotation_angle = 100f; //how to update subject to change

	//For recording
	List<Vector3> positions;
	private Vector3 objPos;
	int index = 0; //index for positions list
	int ind = 0; // index for playback
	public bool playback = false;
	bool first = true;
	private BirdModel model2;
	int i = 0; 

	public void  Start ()
	{
		direction = new Vector2 (0, 1);
		positions = new List<Vector3>(100);
		initBirdModel(1);
	}

	void Update ()
	{
		if (!playback) {	
			float translation = Input.GetAxis ("Vertical") * arrow_speed;
			if (translation < 0) {translation = 0;}			//effectively disabling the down button... birds cant fly backwards :/

			float rotation = Input.GetAxis ("Horizontal") * rotation_angle *-1;		//multiply by -1 to negate default rotation

			float new_speed = translation *Time.deltaTime;
			rotation *= Time.deltaTime;
			transform.Translate (0, new_speed, 0);
			transform.Rotate (0, 0, rotation);
			recordPosition ();
		} else {			
			if (first) {
				direction = new Vector2 (0, 1);
				initBirdModel (2);
				first = false;
			}
			if ( i < index) {
				replay (i);
				i++;
			}
		}
	}


	void initBirdModel (int type)
	{
		if (type == 2) { //creates new bird to display replay
			GameObject modelObject2 = GameObject.CreatePrimitive (PrimitiveType.Quad);	// Create a quad object for holding the bird texture.
			model2 = modelObject2.AddComponent<BirdModel> ();						// Add a bird_model script to control visuals of the bird.
			model2.init (this);
			model2.mat.color = Color.black;
		} else {
			GameObject modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);	// Create a quad object for holding the bird texture.
			model = modelObject.AddComponent<BirdModel> ();						// Add a bird_model script to control visuals of the bird.
			model.init (this);
		}

	}

	void recordPosition(){
		objPos = transform.position;
		positions.Add(objPos);
		Debug.Log (index);
		Debug.Log ("current posistion is: " + positions[index]);

		index++;
	}

	void replay(int inx){
		
		model2.transform.position = positions [inx];
		Debug.Log ("inside replay...:   "+ model2.transform.position); 
	}

	void OnGUI()
	{
		
		GUILayout.BeginArea (new Rect (Screen.width / 2 - Screen.width / 8, 10, Screen.width / 4, Screen.height / 4));
		GUILayout.Box ("Press Spacebar to end");
		GUILayout.EndArea ();
	}

}

