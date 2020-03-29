using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using MonsterLove.StateMachine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public StateMachine<GameConst.GameState> fsm;
    private ARSessionOrigin sessionOrigin;
    private List<ARRaycastHit> rRaycastHits;
    private ARRaycastManager raycastManager;
    private AREnvironmentProbeManager environmentProbeManager;
    public GameObject model;
    public GameObject debugHir;
    public GameObject debugInspector;

    private GameObject table;
    [HideInInspector]
    public GameObject whiteBall;
    [HideInInspector]
    public Vector3 whiteBallPos;
    private Vector3 originForward;
    [HideInInspector]
    public Vector3 whiteBallForward;
    private GameObject refGO;
    public GameObject stick;

    private float currentScale = 1;
    private float scaleFactor = 100;
    private float maxForce = 10;

    // Use this for initialization
    void Start()
    {
        fsm = StateMachine<GameConst.GameState>.Initialize(this);
        sessionOrigin = GetComponent<ARSessionOrigin>();
        raycastManager = GetComponent<ARRaycastManager>();
        environmentProbeManager = GetComponent<AREnvironmentProbeManager>();
        rRaycastHits = new List<ARRaycastHit>();
        refGO = GameObject.Find("ref");

        InitModel();

        StartCoroutine(GameLoop());
    }

    void InitModel()
    {
        table = Instantiate(model);
        table.transform.position = new Vector3(999, 999, 999);
        whiteBall = GameObject.FindGameObjectWithTag("WhiteBall");
        whiteBallPos = whiteBall.transform.position;
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

        while(!IsGameOver())
        {
            yield return StartCoroutine(Aim());

            yield return StartCoroutine(LoadForce());

            yield return StartCoroutine(RollingBall());
        }
    }

    bool IsGameOver()
    {
        return false;
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
            RaycastHit hitInfo;
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo))
            {
                if(hitInfo.collider.gameObject.name == "whiteball")
                {
                    fsm.ChangeState(GameConst.GameState.LoadForce);
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
            //whiteBallForward = Camera.main.transform.forward;
            whiteBallForward.y = Camera.main.transform.forward.y;
            stick.transform.forward = whiteBallForward;
            stick.transform.Rotate(new Vector3(6, 0, 0));
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
        //Pose pose = new Pose(table.transform.position, Quaternion.Euler(table.transform.rotation.x, table.transform.rotation.y, table.transform.rotation.z));
        //environmentProbeManager.AddEnvironmentProbe(pose, Vector3.one, Vector3.one);
        table.GetComponent<TableAction>().enabled = true;
    }


    void Aim_Enter()
    {
        Debug.Log("Aim_Enter");
    }

    void Aim_Exit()
    {
        Debug.Log("Aim_Exit");
        whiteBallForward =  whiteBall.transform.position - refGO.transform.position;
        whiteBallForward.y = 0;
        originForward = whiteBallForward;
    }

    void LoadForce_Enter()
    {
        Debug.Log("LoadForce_Enter");
        GetComponent<ReflectLine>().enabled = true;
        GetComponent<LoadForceAction>().enabled = true;
        //stick.transform.forward = whiteBallForward;
        //stick.transform.Rotate(new Vector3(6, 0, 0));
        stick.transform.position = whiteBall.transform.position;
    }

    void LoadForce_Exit()
    {
        Debug.Log("LoadForce_Exit");
        GetComponent<ReflectLine>().enabled = false;
        GetComponent<LoadForceAction>().enabled = false;
    }

    void Rolling_Enter()
    {
        Debug.Log("Rolling_Enter");
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

    public void OnRotateBallStick(Slider slider)
    {
        whiteBallForward = Quaternion.Euler(0, Mathf.Lerp(0, 360, slider.value), 0) * originForward;
        stick.transform.forward = whiteBallForward;
        stick.transform.Rotate(new Vector3(6, 0, 0));
    }

    public void OnHitClick(float value)
    {
        int maxForce = 300;
        whiteBall.GetComponent<Rigidbody>().AddForce(whiteBallForward * maxForce * value);
        fsm.ChangeState(GameConst.GameState.Rolling);
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

    private void OnDrawGizmos()
    {
        //if(fsm!=null && fsm.State == GameConst.GameState.LoadForce)
        //{
        //    Gizmos.DrawRay(whiteBall.transform.position, whiteBallForward);
        //}
    }
}
