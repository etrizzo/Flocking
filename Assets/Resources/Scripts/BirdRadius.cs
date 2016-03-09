using UnityEngine;
using System.Collections;


public class BirdRadius : MonoBehaviour
{
	private float clock;		// Keep track of time since creation for animation.
	private BirdModel owner;			// Pointer to the parent object.

	public Material mat;		// Material for setting/changing texture and color.

	public Material radiusMat;
	public CircleCollider2D radiusCollider;

	public bool pause;

	public bool containsBird;

	public void init(BirdModel owner) {
		this.owner = owner;

		transform.parent = owner.transform;					// Set the model's parent to the bird.
		transform.localPosition = new Vector3 (0, 0, 0);
		transform.localScale = new Vector3 (7f, 7f, 1f);
		name = "Bird Radius";

		containsBird = false;

		DestroyImmediate(this.gameObject.GetComponent<MeshCollider> ());
		radiusCollider = this.gameObject.AddComponent<CircleCollider2D> ();
		radiusCollider.radius = .5f;
		radiusCollider.isTrigger = true;

		mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
		mat.shader = Shader.Find ("Sprites/Default");						// Tell the renderer that our textures have transparency.
		mat.mainTexture = Resources.Load<Texture2D>("Textures/radius");	// Set the texture.  Must be in Resources folder.
		mat.color = new Color(1,1,1, .5f);											// Set the color (easy way to tint things).
	}

	void Start () {
		clock = 0f;
	}

	void Update () {

		// Incrememnt the clock based on how much time has elapsed since the previous update.
		// Using deltaTime is critical for animation and movement, since the time between each call
		// to Update is unpredictable.
		clock = clock + Time.deltaTime;

		/*if (!owner.owner.gm.inRadius) {
			mat.color = Color.red;
		}
		else{
			mat.color = new Color(1,1,1, .5f);
		}*/
	}

	void OnTriggerEnter2D(Collider2D other){
		Bird otherBird = other.gameObject.GetComponent<Bird> ();
		if (otherBird != null && otherBird.alive) {
			containsBird = true;
			owner.owner.gm.checkBirdInRadius ();
		}
	}

	void OnTriggerExit2D(Collider2D other){
		Bird otherBird = other.gameObject.GetComponent<Bird> ();
		if (otherBird != null && otherBird.alive) {
			containsBird = false;
			owner.owner.gm.checkBirdInRadius ();
		}
	}
}
