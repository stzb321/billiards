using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketDetect : MonoBehaviour
{
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("ArSession").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);


        if(collision.gameObject == gameManager.whiteBall)
        {
            collision.gameObject.transform.position = gameManager.whiteBallPos;
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
