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
	private Material bgMat;

	public AudioSource weatherAudio;
	public AudioClip weatherClip;

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

	float speed;

	float weather_distance = 6f;

	//	void OnGUI ()
	//	{
	//		GUI.Box (slider_rect, "Scale: " + scale.ToString ());
	//		scale = GUI.HorizontalSlider (slider_box_rect, scale, 1F, 20.0F);
	//	}

	void Start ()
	{
		this.clock = Random.Range(0,2);
	}

	void Awake ()
	{
		x_range.y = -1 * x_range.x;
		y_range.y = -1 * y_range.x;
	}

	public void move_location ()
	{
		transform.position = new Vector3 (Random.Range (x_range.x, x_range.y), Random.Range (y_range.x, y_range.y), 0);
//		print ("Position: " + transform.position);
		Vector2 pos = new Vector2 (transform.position.x, transform.position.y);
		Vector2 world_center = new Vector2 (0, 0);
		Vector2 dest_center = DestinationModel.dest_center;
		print (dest_center);
		float x = transform.position.x;
		float y = transform.position.y;
		Collider2D col = Physics2D.OverlapArea(new Vector2(x -4f, y - 4f), new Vector2(x + 4f, y + 4f));
//		print (col.gameObject);
//		if (col.gameObject.GetComponent<DestinationModel> ()) {
//			print (col);
//		}
		while (PointInsideSphere(pos, world_center, weather_distance) | PointInsideSphere(pos, dest_center, weather_distance) ) {
			move_location ();
			pos = new Vector2 (transform.position.x, transform.position.y);
//			print ("pos was in the dest or world sphere!");
		}
	}

	public bool PointInsideSphere (Vector2 point, Vector2 center, float radius)
	{
		return Vector3.Distance (point, center) < radius;
	}

	public void init (Weather owner)
	{
		this.speed = Random.Range(.4f, .7f) * 5f;
		this.owner = owner;
		transform.parent = owner.transform;
		transform.localPosition = new Vector3 (0, 0, 0);
		slider_rect = new Rect (slider_coords.x, slider_coords.y, slider_size.x, slider_size.y);
		slider_box_rect = new Rect (slider_coords.x, slider_coords.y + slider_rect.height, slider_size.x, slider_size.y);

//		transform.parent = owner.transform;// Set the model's parent to the bird.
//		transform.localPosition = new Vector3 (0, 0, 0);// Center the model on the parent.
		name = "Weather Model " + ++weather_counter + "— " + owner.type;// Name the object.
//		move_location ();
		transform.localScale = new Vector3 (scale, scale, 1);

		mat = new Material (Shader.Find ("Sprites/Default"));
		mat.mainTexture = Resources.Load<Texture2D> ("Textures/" + owner.type);// Set the texture.  Must be in Resources folder.
		mat.color = Color.gray;
//		mat.color = Color.blue;// Set the color (easy way to tint things).
		// mat.shader = Shader.Find ("Sprites/Default");// Tell the renderer that our textures have transparency.
		GetComponent<Renderer> ().material = mat;// Get the material component of this quad object.

		DestroyImmediate (GetComponent<MeshCollider> ());
		PolygonCollider2D col = gameObject.AddComponent<PolygonCollider2D>();

		//set the points array for the polygon collider
		Vector2[] points = new Vector2[]{new Vector2(.43f, -.3f), new Vector2(-.4f, -.3f), 
			new Vector2(-.45f, 0f), new Vector2(-.15f, .35f), new Vector2(.3f, .15f), new Vector2(.45f, 0f),new Vector2(.43f, -.3f)};
		col.points = points;
		col.isTrigger = true;

		initSound ();

	}

	private void initSound(){
		weatherAudio = this.gameObject.AddComponent<AudioSource> ();
		weatherAudio.loop = true;
		weatherAudio.playOnAwake = false;
	}

	private string getsoundNum(){
		int soundNum = (int) ((Random.value * 1000) % 2 ) + 1;
		return soundNum.ToString();
	}

	private void weatherSound(){
		weatherClip = Resources.Load<AudioClip> ("Sounds/WeatherSounds/Thunder"+getsoundNum());
		weatherAudio.clip = weatherClip;
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		Bird otherBird = other.gameObject.GetComponent<Bird> ();
		if (otherBird != null && otherBird.alive) {
			weatherSound ();
			containsBird = true;
			weatherAudio.Play ();
		}
	}

	int flash = 0;
	void OnTriggerStay2D (Collider2D other)
	{
		Bird bird = other.gameObject.GetComponent<Bird> ();
		if (bird) {
			bird.gm.score -= .2f;
			if (bird.gm.score < 0) {
				bird.gm.score = 0;
			}
//		}
//		if (containsBird) {
			string random_texture = (Random.RandomRange(-1, 1) < 0 ? "cloud" : "cloud-lightning");
			mat.mainTexture = Resources.Load<Texture2D> ("Textures/" + random_texture);// Set the texture.  Must be in Resources folder.
			float color_value = (float)  (Random.value * 0.5);
			mat.color = new Color (color_value, color_value, color_value, 1);

			if (flash % 4 == 0) {
				bgMat = owner.gm.bg.bgMat;
				float bgcolor_value = (float)(Random.value * 0.5);
				bgMat.color = new Color (bgcolor_value, bgcolor_value, bgcolor_value);
			} 
			if (flash % 8 == 0) {
				bird.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/dead-bird");
			}
			else {
				bird.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/bird");
			}
			flash++;
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		weatherAudio.Stop ();
		Bird otherBird = other.gameObject.GetComponent<Bird> ();
		if (otherBird != null && otherBird.alive) {
			containsBird = false;
		}
		if (otherBird) {
			mat.mainTexture = Resources.Load<Texture2D> ("Textures/cloud");// Set the texture.  Must be in Resources folder.
			mat.color = Color.gray;
			bgMat.color = new Color(1f, 1f, 1f);
			otherBird.model.mat.mainTexture = Resources.Load<Texture2D>("Textures/bird");
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
//		clock = clock + Time.deltaTime;
		if (owner.gm.state.mode != 4) {
			clock += Time.unscaledDeltaTime/speed;
			transform.localPosition = new Vector3 (Mathf.Sin (10 * clock / 6), Mathf.Sin (5 * clock / 6), 0);	
		}
//		transform.localScale = new Vector3 (scale, scale, scale);
	}
}
