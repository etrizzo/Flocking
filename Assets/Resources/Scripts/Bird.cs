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
    Rect slider_rect = new Rect(10, 10, 100, 30);


    public void  Start ()
    {
        direction = new Vector2 (0, 1);
        initBirdModel ();
    }

void OnGUI ()
	{
		GUI.Box (slider_rect, "Speed: " + arrow_speed.ToString ());
		arrow_speed = GUI.HorizontalSlider (new Rect(slider_rect.x, slider_rect.y, slider_rect.width, slider_rect.height + 10), arrow_speed, 0.0F, 20.0F);
	}

    void Update ()
    {
		arrowMove ();
    }

	void arrowMove(){
		float rot_speed = Input.GetAxis ("Vertical");        //-1, 1, or 0 depending on up/down keys
		float rotation = Input.GetAxis ("Horizontal") * rotation_angle * -1;
		rot_speed = ((rot_speed / 2) + 1);    //changes rotation speed multiplier to  1.5 or .5
		transform.Translate (0, arrow_speed*Time.deltaTime, 0);
		transform.Rotate (0, 0, (rotation * rot_speed) * Time.deltaTime);

	}

    void initBirdModel ()
    {
        GameObject modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);	// Create a quad object for holding the bird texture.
        model = modelObject.AddComponent<BirdModel> ();						// Add a bird_model script to control visuals of the bird.
        model.init (this);
    }

}

