using UnityEngine;
using System.Collections;

public class WeatherModel : MonoBehaviour
{
	// Keep track of time since creation for animation.
	private float clock;
	// Pointer to the parent object.
	private Weather owner;
	// Material for setting/changing texture and color.
	private Material mat;

	// Random weather position min/max
	float move_min = -3;
	float move_max = 3;
	Vector2 slider_coords = new Vector2 (200, 10);
	Vector2 slider_size = new Vector2 (150, 30);
	float scale = 1f;
	Rect slider_rect, slider_box_rect;


	bool debug = true;
	int timeIn = 0;

	void OnGUI ()
	{
		GUI.Box (slider_rect, "Scale: " + scale.ToString ());
		scale = GUI.HorizontalSlider (slider_box_rect, scale, 1F, 20.0F);
	}

	public void init (Weather owner)
	{
		this.owner = owner;
		slider_rect = new Rect (slider_coords.x, slider_coords.y, slider_size.x, slider_size.y);
		slider_box_rect = new Rect (slider_coords.x, slider_coords.y + slider_rect.height, slider_size.x, slider_size.y);

//		transform.parent = owner.transform;// Set the model's parent to the bird.
//		transform.localPosition = new Vector3 (0, 0, 0);// Center the model on the parent.
		name = "Weather Model — " + owner.type;// Name the object.
		transform.localPosition = new Vector3 (Random.Range (move_min, move_max), Random.Range (move_min, move_max), 0);
		transform.localScale = new Vector3 (3, 3, 3);

		mat = new Material (Shader.Find ("Sprites/Default"));
		mat.mainTexture = Resources.Load<Texture2D> ("Textures/" + owner.type);// Set the texture.  Must be in Resources folder.
//		mat.color = Color.blue;// Set the color (easy way to tint things).
		// mat.shader = Shader.Find ("Sprites/Default");// Tell the renderer that our textures have transparency.
		GetComponent<Renderer> ().material = mat;// Get the material component of this quad object.

		SphereCollider sc = gameObject.AddComponent<SphereCollider> ();
		sc.isTrigger = false;
	}

	void OnTriggerStay (Collider other)
	{
		// Glow blue randomly
		mat.color = new Color (0, 0, Random.Range (-255, 255));
		if (debug) {
			print ("booped " + other.name + ", time: " + timeIn++);
		}
//		Destroy (other.gameObject);
	}

	void OnTriggerExit (Collider other)
	{
		if (debug) {
			print ("Oh, buh bye!");
			print ("------");
		}
		mat.color = Color.gray;
		timeIn = 0;
	}

	void Start ()
	{
		clock = 0f;
	}

	void Update ()
	{
		// Incrememnt the clock based on how much time has elapsed since the previous update.
		// Using deltaTime is critical for animation and movement, since the time between each call
		// to Update is unpredictable.
		clock = clock + Time.deltaTime;

		transform.localScale = new Vector3 (scale, scale, scale);
	}
}
