using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Background : MonoBehaviour {

    private BackgroundModel bg_model;
	public Material bgMat;
    private GameObject modelObject;
	public GameManager gm;

	public void init(int x, int y, GameManager gm) {
		this.gm = gm;
        this.modelObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        modelObject.name = "BG Model";
        this.bg_model = modelObject.AddComponent<BackgroundModel>();
        this.bg_model.init(this);
		bgMat = bg_model.GetComponent<Renderer> ().material;

    }

}

