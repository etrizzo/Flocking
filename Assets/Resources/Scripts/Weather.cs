using UnityEngine;
using System.Collections;

public class Weather : MonoBehaviour
{
	// This should be the name of the texture the type uses in its model, for clarity
	public string type;
	WeatherModel model;
	public GameManager gm;
	private static int weather_border = 1;
	float weather_distance = 6f;

	Vector2 x_range = new Vector2 (GameManager.x_coord * GameManager.BGSCALE - weather_border, 0);
	Vector2 y_range = new Vector2 (GameManager.y_coord * GameManager.BGSCALE + weather_border, 0);

	public void init (GameManager gm)
	{
		this.gm = gm;
		init ("tileBlank", gm);
	}

	public WeatherModel init (string weather_type, GameManager gm)
	{
		this.gm = gm;
		this.type = weather_type;
		move_location ();
		return initWeatherModel ();
	}

	public WeatherModel initWeatherModel ()
	{
		GameObject modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);	// Create a quad object for holding the bird texture.
		model = modelObject.AddComponent<WeatherModel> ();						// Add a bird_model script to control visuals of the bird.
		model.init (this);

		return model;
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
}
