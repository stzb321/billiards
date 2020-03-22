using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

public class GameManager : MonoBehaviour
{

    private ARSessionOrigin sessionOrigin;
    private List<ARRaycastHit> rRaycastHits;
    private ARRaycastManager raycastManager;
    public GameObject model;
    public GameObject debugHir;
    public GameObject debugInspector;

    private GameObject table;
    private GameObject whiteBall;

    private float currentScale = 1;
    private float scaleFactor = 100;
    private float maxForce = 10;

    public GameConst.GameState state = GameConst.GameState.None;

    // Use this for initialization
    void Start()
    {
        sessionOrigin = GetComponent<ARSessionOrigin>();
        raycastManager = GetComponent<ARRaycastManager>();
        rRaycastHits = new List<ARRaycastHit>();

        InitModel();

        StartCoroutine(GameLoop());
    }

    void InitModel()
    {
        table = Instantiate(model);
        whiteBall = GameObject.FindGameObjectWithTag("WhiteBall");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GameLoop()
    {
        // Find a place
        state = GameConst.GameState.FindPlace;
        yield return StartCoroutine(FindPlace());

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(LoadForce());

    }

    IEnumerator FindPlace()
    {
        while(state == GameConst.GameState.FindPlace)
        {
            Vector2 center = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
            if (raycastManager.Raycast(center, rRaycastHits, TrackableType.Planes))
            {
                ARRaycastHit hit = rRaycastHits[0];
                table.transform.position = hit.pose.position;
            }

            yield return null;
        }
    }

    IEnumerator HitBall()
    {
        while(state == GameConst.GameState.Aim)
        {
            if(Input.touchCount == 1)
            {
                Debug.Log("add force to whiteball 0");
                Touch touch = Input.GetTouch(0);
                if(touch.phase == TouchPhase.Ended)
                {
                    Debug.Log("add force to whiteball 1");
                    whiteBall.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 1));
                    state = GameConst.GameState.LoadForce;
                    Debug.Log("add force to whiteball 2");
                }
            }

            yield return null;
        }

        yield return null;
    }

    IEnumerator LoadForce()
    {
        while( state == GameConst.GameState.LoadForce)
        {
            yield return null;
        }

        yield return null;
    }


    public void OnScale(float delta)
    {
        float scaleDelta = delta / scaleFactor;
        currentScale += scaleDelta;
        currentScale = Mathf.Clamp(currentScale, 0.5f, 1.5f);
        table.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
    }

    // percent: 0 - 100
    public void OnLoadForce(float sliderValue)
    {
        float percent = sliderValue / 100f;
        whiteBall.GetComponent<Rigidbody>().AddForce(new Vector3(percent * maxForce, 0, percent * maxForce));
        state = GameConst.GameState.Rolling;
        GetComponent<LoadForceAction>().enabled = false;
    }

    public void OnPlaceClick()
    {
        state = GameConst.GameState.LoadForce;
        GetComponent<ScaleAction>().enabled = false;
        GetComponent<LoadForceAction>().enabled = true;
    }

    public void OnShowInspectClick()
    {
        debugHir.SetActive(!debugHir.activeSelf);
        debugInspector.SetActive(!debugInspector.activeSelf);
    }

    public void TestHit()
    {
        OnLoadForce(50);
    }
}
