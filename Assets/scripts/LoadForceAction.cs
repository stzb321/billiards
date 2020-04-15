using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class LoadForceAction : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject UIObject;
    public GameObject realStick;
    public Slider forceSlider;
    private float minForce = 2f;
    private float sliderFactor = 0.3f;
    private float maxDistance = 10;
    private bool hit = false;

    // Use this for initialization
    void Start()
    {
        
    }

    private void OnEnable()
    {
        hit = false;
        UIObject?.SetActive(true);
        realStick.SetActive(true);
    }

    private void OnDisable()
    {
        UIObject?.SetActive(false);
        realStick.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.touchCount != 1)
        //{
        //    return;
        //}

        //Touch touch = Input.GetTouch(0);
        
        //if (touch.phase == TouchPhase.Ended)
        //{
        //    if (forceSlider.value <= minForce)
        //        return;
        //    gameManager.OnLoadForce(forceSlider.value);
        //}else if(touch.phase == TouchPhase.Moved)
        //{
        //    UpdateForce(touch.deltaPosition.x * sliderFactor);
        //}
    }

    void UpdateForce(float delta)
    {
        forceSlider.value += delta;
    }

    public void OnDragEnd(Slider slider)
    {
        float val = slider.value;
        if(val > 0.1)
        {
            HitTheBall(slider);
        }
        slider.value = 0;
    }

    public void OnValueChange(Slider slider)
    {
        if (!hit)
        {
            realStick.transform.localPosition = (-realStick.transform.forward * maxDistance * slider.value) + gameManager.whiteBall.transform.position;
        }
    }

    void HitTheBall(Slider slider)
    {
        hit = true;
        float val = slider.value;
        realStick.transform.DOLocalMove(gameManager.whiteBall.transform.position, 0.1f).onComplete = ()=>
        {
            gameManager.OnHitClick(val);
        };
    }
}
