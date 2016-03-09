using UnityEngine;
using System.Collections;

public class WeatherModel : MonoBehaviour
{

	int weather_counter = 0;
	// Keep track of time since creation for animation.
	private float clock;
	// Pointer to the parent object.
	private Weather owner;
	// Material for setting/changing texture and color.
	private Material mat;

	// Random weather position min/max
	private static int weather_border = 1;
	Vector2 x_range = new Vector2 (GameManager.x_coord * GameManager.BGSCALE - weather_border, 0);
	Vector2 y_range = new Vector2 (GameManager.y_coord * GameManager.BGSCALE + weather_border, 0);
	Vector2 slider_coords = new Vector2 (200, 10);
	Vector2 slider_size = new Vector2 (150, 30);
	float scale = 3f;
	Rect slider_rect, slider_box_rect;


	bool debug = true;
	int timeIn = 0;
	bool containsBird;

//	void OnGUI ()
//	{
//		GUI.Box (slider_rect, "Scale: " + scale.ToString ());
//		scale = GUI.HorizontalSlider (slider_box_rect, scale, 1F, 20.0F);
//	}

	void Start ()
	{
		clock = 0f;
	}

	void Awake() {
		x_range.y = -1 * x_range.x;
		y_range.y = -1 * y_range.x;
	}

	public void move_location() {
		transform.localPosition = new Vector3 (Random.Range (x_range.x, x_range.y), Random.Range (y_range.x, y_range.y), 0);
	}

	public void init (Weather owner)
	{
		this.owner = owner;
		slider_rect = new Rect (slider_coords.x, slider_coords.y, slider_size.x, slider_size.y);
		slider_box_rect = new Rect (slider_coords.x, slider_coords.y + slider_rect.height, slider_size.x, slider_size.y);

//		transform.parent = owner.transform;// Set the model's parent to the bird.
//		transform.localPosition = new Vector3 (0, 0, 0);// Center the model on the parent.
		name = "Weather Model " + ++weather_counter + "— " + owner.type;// Name the object.
		transform.localPosition = new Vector3 (Random.Range (x_range.x, x_range.y), Random.Range (y_range.x, y_range.y), 0);
		transform.localScale = new Vector3 (scale, scale, 1);

		mat = new Material (Shader.Find ("Sprites/Default"));
		mat.mainTexture = Resources.Load<Texture2D> ("Textures/" + owner.type);// Set the texture.  Must be in Resources folder.
//		mat.color = Color.blue;// Set the color (easy way to tint things).
		// mat.shader = Shader.Find ("Sprites/Default");// Tell the renderer that our textures have transparency.
		GetComponent<Renderer> ().material = mat;// Get the material component of this quad object.

		DestroyImmediate (GetComponent<MeshCollider> ());
		CircleCollider2D cc = gameObject.AddComponent<CircleCollider2D> ();
		cc.isTrigger = true;
	}

	void OnTriggerEnter2D(Collider2D other){
		Bird otherBird = other.gameObject.GetComponent<Bird> ();
		if (otherBird != null && otherBird.alive) {
			containsBird = true;
		}
	}

	void OnTriggerStay2D (Collider2D other)
	{
		Bird bird = other.gameObject.GetComponent<Bird> ();
		if (bird) {
			bird.gm.score -= .2f;
			if (bird.gm.score < 0) {
				bird.gm.score = 0;
			}
		}
		if (containsBird) {
		// Glow blue randomly
		mat.color = new Color (0, 0, Random.Range (-255, 255));
//		if (debug) {
//			print ("booped " + other.name + ", time: " + timeIn++);
//		}
		}
	}

	void OnTriggerExit2D(Collider2D other){
		Bird otherBird = other.gameObject.GetComponent<Bird> ();
		if (otherBird != null && otherBird.alive) {
			containsBird = false;
		}
		if (otherBird){
			mat.color = Color.gray;
		}
	}

//	void OnTriggerExit2D (Collider2D other)
//	{
//		if (debug) {
//			print ("Oh, buh bye!");
//			print ("------");
//		}
//		mat.color = Color.gray;
//		timeIn = 0;
//	}


	void Update ()
	{
		// Incrememnt the clock based on how much time has elapsed since the previous update.
		// Using deltaTime is critical for animation and movement, since the time between each call
		// to Update is unpredictable.
		clock = clock + Time.deltaTime;

//		transform.localScale = new Vector3 (scale, scale, scale);
	}
}
