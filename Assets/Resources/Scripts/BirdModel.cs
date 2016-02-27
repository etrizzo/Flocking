using UnityEngine;
using System.Collections;

public class BirdModel : MonoBehaviour
{
	private float clock;
	// Keep track of time since creation for animation.
	private Bird owner;
	// Pointer to the parent object.
	private Material mat;
	// Material for setting/changing texture and color.

	public void init (Bird owner)
	{
		this.owner = owner;

		transform.parent = owner.transform;// Set the model's parent to the bird.
		transform.localPosition = new Vector3 (0, 0, -.01f);// Center the model on the parent.
		name = "Bird Model";// Name the object.

		mat = new Material (Shader.Find ("Sprites/Default"));
		mat.mainTexture = Resources.Load<Texture2D> ("Textures/marble");// Set the texture.  Must be in Resources folder.
		mat.color = new Color (1, 1, 1);// Set the color (easy way to tint things).
		// mat.shader = Shader.Find ("Sprites/Default");// Tell the renderer that our textures have transparency.
		GetComponent<Renderer> ().material = mat;// Get the material component of this quad object.
		DestroyImmediate (GetComponent<MeshCollider> ());
		SphereCollider sc = gameObject.AddComponent<SphereCollider> ();
		Rigidbody rb = gameObject.AddComponent<Rigidbody> ();
		sc.isTrigger = true;
		rb.isKinematic = false;
		rb.useGravity = false;
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
	}
}

