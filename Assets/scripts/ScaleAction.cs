using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAction : MonoBehaviour
{
    public GameObject UIObject;
    private GameManager gameManager;
    private float preDistance;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    void OnEnable()
    {
        UIObject?.SetActive(true);
    }

    void OnDisable()
    {
        UIObject?.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount != 2)
        {
            return;
        }
        
        Touch finger1 = Input.touches[0];
        Touch finger2 = Input.touches[1];

        if (finger1.phase == TouchPhase.Moved || finger2.phase == TouchPhase.Moved)
        {
            float distance = (finger1.position - finger2.position).magnitude;
            float delta = preDistance == 0 ? 0 : distance - preDistance;

            Debug.Log(string.Format("two finger moved!!  {0}", delta));
            gameManager.OnScale(delta);

            preDistance = distance;
        }
    }
}
