using SARP.Entitys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tst : MonoBehaviour
{
    [SerializeField]
    Mover mover;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Break");
            Debug.Break();
        }
    }
}
