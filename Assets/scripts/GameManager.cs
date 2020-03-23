using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using MonsterLove.StateMachine;

public class GameManager : MonoBehaviour
{
    private StateMachine<GameConst.GameState> fsm;
    private ARSessionOrigin sessionOrigin;
    private List<ARRaycastHit> rRaycastHits;
    private ARRaycastManager raycastManager;
    public GameObject model;
    public GameObject line;
    public GameObject debugHir;
    public GameObject debugInspector;

    private GameObject table;
    private GameObject whiteBall;

    private float currentScale = 1;
    private float scaleFactor = 100;
    private float maxForce = 10;

    public GameObject tt;

    // Use this for initialization
    void Start()
    {
        fsm = StateMachine<GameConst.GameState>.Initialize(this);
        sessionOrigin = GetComponent<ARSessionOrigin>();
        raycastManager = GetComponent<ARRaycastManager>();
        rRaycastHits = new List<ARRaycastHit>();

        InitModel();

        StartCoroutine(GameLoop());

        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(0.5f);

        tt.GetComponent<Rigidbody>().AddForce(new Vector3(0.5f, 0, 1) * 400);
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
        fsm.ChangeState(GameConst.GameState.FindPlace);
        
        yield return StartCoroutine(FindPlace());

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(FindPlace());

        yield return StartCoroutine(LoadForce());

    }

    IEnumerator FindPlace()
    {
        while(fsm.State == GameConst.GameState.FindPlace)
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
        while(fsm.State == GameConst.GameState.Aim)
        {
            if(Input.touchCount == 1)
            {
                Debug.Log("add force to whiteball 0");
                Touch touch = Input.GetTouch(0);
                if(touch.phase == TouchPhase.Ended)
                {
                    Debug.Log("add force to whiteball 1");
                    whiteBall.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 1));
                    fsm.ChangeState(GameConst.GameState.LoadForce);
                    Debug.Log("add force to whiteball 2");
                }
            }

            yield return null;
        }

        yield return null;
    }

    IEnumerator LoadForce()
    {
        while(fsm.State == GameConst.GameState.LoadForce)
        {
            if(CheckAllBallIsStop())
            {
                fsm.ChangeState(GameConst.GameState.Aim);
            }
        }

        yield return null;
    }

    bool CheckAllBallIsStop()
    {
        if(whiteBall.GetComponent<Rigidbody>().velocity.magnitude <= 0.01f)
        {
            return true;
        }
        return false;
    }

    void DrawBallLine(Vector3 to)
    {
        Vector3 from = whiteBall.transform.position;
        Vector3 dir = to - from;
        RaycastHit hitInfo;
        if (Physics.Raycast(from, to, out hitInfo, 10))
        {
            DrawLine(from, hitInfo.point);
            DrawLine(hitInfo.point, Vector3.Reflect(dir, hitInfo.normal));   //reflect
            
        }
    }

    void DrawLine(Vector3 from, Vector3 to)
    {
        Vector3 lineDir = to - from;
        line.transform.position = from + lineDir / 2f;

        line.transform.forward = lineDir;
        line.transform.localScale = new Vector3(line.transform.localScale.x, 1, lineDir.magnitude);
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
        fsm.ChangeState(GameConst.GameState.Rolling);
    }

    public void OnPlaceClick()
    {
        fsm.ChangeState(GameConst.GameState.Aim);
    }

    public void OnShowInspectClick()
    {
        debugHir.SetActive(!debugHir.activeSelf);
        debugInspector.SetActive(!debugInspector.activeSelf);
    }


    //////////////////////////////////////////////////////////////////
    /// <summary>
    /// state functions
    /// </summary>
    void FindPlace_Enter()
    {
        GetComponent<ScaleAction>().enabled = true;
        GetComponent<LoadForceAction>().enabled = false;
    }

void FindPlace_Exit()
    {
        GetComponent<ScaleAction>().enabled = false;
        GetComponent<LoadForceAction>().enabled = true;
    }


    void Aim_Enter()
    {

    }

    void Aim_Exit()
    {

    }

    void LoadForce_Enter()
    {
        GetComponent<LoadForceAction>().enabled = true;
    }

    void LoadForce_Exit()
    {
        GetComponent<LoadForceAction>().enabled = false;
    }


    //////////////////////////////////////state functions////////////////////////////
    public void TestHit()
    {
        whiteBall.GetComponent<Rigidbody>().AddForce(new Vector3(1, 0, 1) * 10);
    }
}
