﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketDetect : MonoBehaviour
{
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {



        if(collision.gameObject == gameManager.whiteBall)
        {

        }
    }
}
