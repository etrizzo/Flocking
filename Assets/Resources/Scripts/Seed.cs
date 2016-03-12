using UnityEngine;
using System.Collections;

public class Seed : MonoBehaviour
{
	// This should be the name of the texture the type uses in its model, for clarity
	public GameManager gm;
	SeedModel model;

	public void init (float x, float y, GameManager gm)
	{
		this.transform.position = new Vector3 (x, y, 0);
		this.gm = gm;
		this.name = "Seed";
		GameObject modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);	// Create a quad object for holding the bird texture.
		model = modelObject.AddComponent<SeedModel> ();						// Add a bird_model script to control visuals of the bird.
		model.init (this);
	}
		


}
