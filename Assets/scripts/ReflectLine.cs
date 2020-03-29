using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectLine : MonoBehaviour
{
    public GameObject linePerfab;
    public GameManager gameManager;
    private GameObject lineDir;
    private GameObject lineReflect;
    private GameObject lineBound;
    private Vector3 preForward = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        lineDir = Instantiate(linePerfab);
        lineReflect = Instantiate(linePerfab);
        lineBound = Instantiate(linePerfab);
        ClearAllLine();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.fsm.State == GameConst.GameState.LoadForce && preForward != gameManager.whiteBallForward)
        {
            DrawBallLine(gameManager.whiteBallForward);
            preForward = gameManager.whiteBallForward;
        }
    }

    private void OnEnable()
    {
        ClearAllLine();
    }

    private void OnDisable()
    {
        ClearAllLine();
    }

    void DrawBallLine(Vector3 dir)
    {
        GameObject whiteBall = gameManager.whiteBall;
        Vector3 from = whiteBall.transform.position;
        RaycastHit hitInfo;
        if (whiteBall.GetComponent<Rigidbody>().SweepTest(dir, out hitInfo, 1000))
        {
            DrawLine(lineDir, from, hitInfo.point);
            DrawLine(lineReflect, hitInfo.point, Vector3.Reflect(dir, hitInfo.normal).normalized);   //reflect
        }
    }

    void DrawLine(GameObject line, Vector3 from, Vector3 to)
    {
        Vector3 lineDir = to - from;
        line.transform.position = from + lineDir / 2f;

        line.transform.forward = lineDir;
        line.transform.localScale = new Vector3(line.transform.localScale.x, 1, lineDir.magnitude);
    }

    void ClearDrawLine(GameObject line)
    {
        line.transform.position = new Vector3(1000, 1000, 1000);
    }

    void ClearAllLine()
    {
        ClearDrawLine(lineDir);
        ClearDrawLine(lineReflect);
        ClearDrawLine(lineBound);
    }
}
