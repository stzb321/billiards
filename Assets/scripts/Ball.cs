using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
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
        Debug.Log(string.Format("OnCollisionEnter {0}", collision.gameObject.name));
        if(collision.gameObject.name == "whiteball")
        {
            Handheld.Vibrate();
        }
    }
}
