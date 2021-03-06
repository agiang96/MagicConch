﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceConch : MonoBehaviour {

    //adjust this to change speed
    float speed = 3f;
    //adjust this to change how high it goes
    float height = 0.4f;



    void Update()
    {
        //get the objects current position and put it in a variable so we can access it later with less code
        Vector2 pos = transform.position;
//        Debug.Log(pos);
        //calculate what the new Y position will be
        float newY = Mathf.Sin(Time.time * speed);
        //set the object's Y to the new calculated Y
        transform.position = new Vector2(pos.x, newY*height);
    }
}
