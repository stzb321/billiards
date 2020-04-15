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
        
        if(collision.gameObject.tag == GameConst.BallsTag.WhiteBall)
        {
            Debug.Log(string.Format("OnCollisionEnter {0}", collision.gameObject.name));
            Handheld.Vibrate();   // 现在是固定震动时长和强度。可以调整成根据撞击强度来调整时长和强度。
        }
    }
}
