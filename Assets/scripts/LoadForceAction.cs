using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadForceAction : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject UIObject;
    public Slider slider;
    private float minForce = 2f;
    private float sliderFactor = 0.3f;

    // Use this for in i tialization
    void Start()
    {

    }

    private void OnEnable()
    {
        UIObject?.SetActive(true);
    }

    private void OnDisable()
    {
        UIObject?.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount != 1)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);
        
        if (touch.phase == TouchPhase.Ended)
        {
            if (slider.value <= minForce)
                return;
            gameManager.OnLoadForce(slider.value);
        }else if(touch.phase == TouchPhase.Moved)
        {
            UpdateForce(touch.deltaPosition.x * sliderFactor);
        }
    }

    void UpdateForce(float delta)
    {
        slider.value += delta;
    }
}
