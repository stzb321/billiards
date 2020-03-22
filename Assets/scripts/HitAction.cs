using UnityEngine;
using System.Collections;

public class HitAction : MonoBehaviour
{
    public GameObject UIObject;
    public GameObject bar;

    private float minForce = 0.05f;

    // Use this for initialization
    void Start()
    {

    }

    void OnEnable()
    {
        UIObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDisable()
    {
        UIObject.SetActive(false);
    }
}
