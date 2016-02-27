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
        MeshCollider mcol = modelObject.GetComponent<MeshCollider>();
        if (mcol != null) {
            DestroyImmediate (mcol);
        }
        Vector2[] points = {new Vector2(.5f,.5f), new Vector2(.5f,-.5f), new Vector2(-.5f,-.5f), new Vector2(-.5f,.5f), new Vector2(.5f,.5f)};
        EdgeCollider2D col = modelObject.AddComponent<EdgeCollider2D> ();
        col.points = points;
        this.bg_model.init(this);

    }

}
