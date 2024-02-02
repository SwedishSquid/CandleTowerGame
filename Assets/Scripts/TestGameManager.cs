using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameManager : MonoBehaviour
{
    private CutSceneHandler cutSceneHandler;

    public void Awake()
    {
        cutSceneHandler = gameObject.GetComponent<CutSceneHandler>();
    }

    public void Start()
    {
        StartCoroutine(StartShowing());
    }

    private IEnumerator StartShowing()
    {
        Debug.Log("coroutine started");
        while (cutSceneHandler.TryShowNext())
        {
            yield return new WaitForSeconds(3);
        }
        Debug.Log("coroutime ended");
    }
}
