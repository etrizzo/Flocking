using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Background : MonoBehaviour {

    private BackgroundModel bg_model;
    private GameObject modelObject;

    public void init(int x, int y) {
        this.modelObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        modelObject.name = "BG Model";
        this.bg_model = modelObject.AddComponent<BackgroundModel>();
        this.bg_model.init(this);

    }

}

