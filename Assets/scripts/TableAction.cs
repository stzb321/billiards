using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TableAction : MonoBehaviour
{
    private GameObject whiteBallPos;
    private GameObject ballsPos;
    // Start is called before the first frame update
    void Start()
    {
        whiteBallPos = transform.Find("table/white ball pos").gameObject;
        ballsPos = transform.Find("table/balls pos").gameObject;

        whiteBallPos.SetActive(false);
        ballsPos.SetActive(false);
    }

    private void OnEnable()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(false);
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
        GetComponent<Animator>().SetTrigger("tableScale");
    }

    private void OnDisable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTableScaleEnd()
    {
        Debug.Log("OnTableScaleEnd");
        whiteBallPos.SetActive(true);
        ballsPos.SetActive(true);
        Collider[] colliders = GetComponentsInChildren<Collider>(false);
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
    }
}
