using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    Bird bird;
    //	List<Bird> bird_list;
    // List<Weather> weather_list;

    void Start ()
    {
        bird = gameObject.AddComponent<Bird> ();
        bird.init ();
        // makeWeather ();
    }

    // private void makeWeather ()
    // {
    //     Weather new_weather = gameObject.AddComponent<Weather> ();
    //     new_weather.init ("cloud");
    // }
}

