using UnityEngine;
using System.Collections;

public class SeedModel : MonoBehaviour
{

	Material mat;
	Seed owner;
	float clock;
	float speed;

	public void init (Seed owner)
	{
		this.speed = Random.Range(.3f, .6f) * 3f;
		this.owner = owner;
//		this.transform.position = this.owner.transform.position;
		transform.parent = owner.transform;
		transform.localPosition = new Vector3 (0, 0, 0);
		name = "Seed Model";		// Name the object.

		mat = new Material (Shader.Find ("Sprites/Default"));
		mat.mainTexture = Resources.Load<Texture2D> ("Textures/seed");// Set the texture.  Must be in Resources folder.
		//mat.color = Color.gray;
		//		mat.color = Color.blue;// Set the color (easy way to tint things).
		// mat.shader = Shader.Find ("Sprites/Default");// Tell the renderer that our textures have transparency.
		GetComponent<Renderer> ().material = mat;// Get the material component of this quad object.

		DestroyImmediate (GetComponent<MeshCollider> ());
		PolygonCollider2D col = gameObject.AddComponent<PolygonCollider2D>();

		//set the points array for the polygon collider
		Vector2[] points = new Vector2[]{new Vector2(-.35f, -.5f), new Vector2(-.2f, .5f), new Vector2(.5f, .45f), new Vector2(.5f, .2f), new Vector2(-.25f,-.5f)};
//		Vector2[] points = new Vector2[]{new Vector2(-.5f, -.5f), new Vector2(-.4f, -.3f), 
//			new Vector2(-.45f, 0f), new Vector2(-.15f, .35f), new Vector2(.3f, .15f), new Vector2(.45f, 0f),new Vector2(.43f, -.3f)};
		col.points = points;
		col.isTrigger = true;
	}

	void Start(){
		this.clock = Random.value;
	}


	void Update(){
		if (owner.gm.state.mode != 4) {
			clock += Time.unscaledDeltaTime/speed;
			transform.localPosition = new Vector3 (Mathf.Sin (10 * clock / 6), Mathf.Sin (5 * clock / 6), 0);	
		}
	}




	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.GetComponent<Bird> ()) {
			this.owner.gm.score += 30;
			Destroy (this.gameObject);
		}
	}
}

