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
    public GameObject linePerfab;
    private GameObject lineInst;
    public GameObject debugHir;
    public GameObject debugInspector;

    private GameObject table;
    private GameObject whiteBall;
    private Vector3 whiteBallForward;

    private float currentScale = 1;
    private float scaleFactor = 100;
    private float maxForce = 10;

    // Use this for initialization
    void Start()
    {
        fsm = StateMachine<GameConst.GameState>.Initialize(this);
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
        lineInst = Instantiate(linePerfab);
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

        yield return StartCoroutine(Aim());

        yield return StartCoroutine(LoadForce());

        yield return StartCoroutine(RollingBall());

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

    IEnumerator Aim()
    {
        while (fsm.State == GameConst.GameState.Aim)
        {
            yield return null;
        }
        yield return null;
    }

    IEnumerator LoadForce()
    {
        while(fsm.State == GameConst.GameState.LoadForce)
        {
            DrawBallLine(whiteBallForward);
            yield return null;
        }

        yield return null;
    }

    IEnumerator RollingBall()
    {
        while (fsm.State == GameConst.GameState.Rolling)
        {
            if (CheckAllBallIsStop())
            {
                fsm.ChangeState(GameConst.GameState.Aim);
            }
            yield return null;
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

    void ClearDrawLine()
    {
        lineInst.transform.position = new Vector3(1000, 1000, 1000);
    }

    void DrawBallLine(Vector3 dir)
    {
        Vector3 from = whiteBall.transform.position;
        RaycastHit hitInfo;
        if (Physics.Raycast(from, dir, out hitInfo, 10))
        {
            DrawLine(from, hitInfo.point);
            //DrawLine(hitInfo.point, Vector3.Reflect(dir, hitInfo.normal));   //reflect
        }
    }

    void DrawLine(Vector3 from, Vector3 to)
    {
        Vector3 lineDir = to - from;
        lineInst.transform.position = from + lineDir / 2f;

        lineInst.transform.forward = lineDir;
        lineInst.transform.localScale = new Vector3(lineInst.transform.localScale.x, 1, lineDir.magnitude);
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
        Debug.Log("FindPlace_Enter");
        GetComponent<ScaleAction>().enabled = true;
        GetComponent<LoadForceAction>().enabled = false;
    }

void FindPlace_Exit()
    {
        Debug.Log("FindPlace_Exit");
        GetComponent<ScaleAction>().enabled = false;
    }


    void Aim_Enter()
    {
        Debug.Log("Aim_Enter");
    }

    void Aim_Exit()
    {
        Debug.Log("Aim_Exit");
        whiteBallForward = whiteBall.transform.position - Camera.main.transform.position;
        whiteBallForward.y = 0;
    }

    void LoadForce_Enter()
    {
        Debug.Log("LoadForce_Enter");
        GetComponent<LoadForceAction>().enabled = true;
    }

    void LoadForce_Exit()
    {
        Debug.Log("LoadForce_Exit");
        GetComponent<LoadForceAction>().enabled = false;
    }

    void Rolling_Enter()
    {
        Debug.Log("Rolling_Enter");
        ClearDrawLine();
    }

    void Rolling_Exit()
    {
        Debug.Log("Rolling_Exit");
        
    }

    //////////////////////////////////////state functions////////////////////////////
    public void TestHit()
    {
        whiteBall.GetComponent<Rigidbody>().AddForce(new Vector3(1, 0, 1) * 10);
    }

    public void NextState()
    {
        switch(fsm.State)
        {
            case GameConst.GameState.FindPlace: fsm.ChangeState(GameConst.GameState.Aim); break;
            case GameConst.GameState.Aim: fsm.ChangeState(GameConst.GameState.LoadForce); break;
            case GameConst.GameState.LoadForce: TestHit(); fsm.ChangeState(GameConst.GameState.Rolling); break;
            case GameConst.GameState.Rolling:  break;
            case GameConst.GameState.GameOver:  break;
        }
    }
}
