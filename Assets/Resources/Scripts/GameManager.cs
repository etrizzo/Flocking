using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

	public Bird bird;
	public GameObject bg_folder; 
	public Camera cam;
	List<Background> bgs;

	// The model object.


	public void  Start ()
	{
		
		this.bg_folder = new GameObject ();
		this.bg_folder.layer = 0;
		this.bgs = new List<Background> ();
		background ();
		makeBird ();
		//direction = new Vector2 (0, 1);
	}




	void makeBird(){
		GameObject birdObject = new GameObject();	
		birdObject.name = "Bird Object";
		//birdObject.layer = "
		bird = birdObject.AddComponent<Bird>();	
		bird.transform.position = new Vector3(0,0,0);	



		bird.Start();							

		bird.name = "Bird";
	}

	void background(){

		addBGtile (0, 0);
//		for (int i = -1; i < 2; i++) {
//			for (int j = -1; j <2; j++){
//				addBGtile (j * Screen.width, i * Screen.height); 
//			}
//		}

	}

	Background addBGtile(int x, int y) {
		//print ("Tile added at " + x + " " + y);
		GameObject bg_object = new GameObject();			// Create a new empty game object that will hold a gem.
		bg_object.name = "BG Object";
		Background bg = bg_object.AddComponent<Background>();			// Add the Gem.cs script to the object.
		// We can now refer to the object via this script.
		bg.transform.parent = bg_folder.transform;			// Set the gem's parent object to be the gem folder.

		var width = Camera.main.orthographicSize * 2.0 * Screen.width / Screen.height;
		//bg.transform.localScale = new Vector3 (Screen.width, Screen.height, 1);
		bg.transform.position = new Vector3(x,y,0);		// Position the gem at x,y.								

		//MAY need to add offset back?
		bg.init((int) x, (int) y);							// Initialize the gem script.

		bgs.Add(bg);										// Add the gem to the Gems list for future access.
		bg.name = "Background Tile "+bgs.Count;						// Give the gem object a name in the Hierarchy pane.

		return bg;							
	}



}

