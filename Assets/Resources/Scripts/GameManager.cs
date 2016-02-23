using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

	public Bird bird;
	public Camera cam;
	public Background bg;

	// The model object.


	public void  Start ()
	{
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

		this.bg = addBGtile (0, 0);

	}

	Background addBGtile(int x, int y) {
		GameObject bg_object = new GameObject();			
		bg_object.name = "BG Object";
		Background bg = bg_object.AddComponent<Background>();	
		// We can now refer to the object via this script.
		//bg.transform.parent = bg_folder.transform;			
		bg.transform.position = new Vector3(x,y,0);		
		bg.init((int) x, (int) y);										
		bg.name = "Background";

		return bg;							
	}



}

