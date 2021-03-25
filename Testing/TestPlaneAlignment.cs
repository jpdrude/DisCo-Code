using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TestPlaneAlignment : MonoBehaviour
{
    public Connection conA;
    public Connection conB;

    private void Start()
    {
        if (!NetworkBlockBehaviour.CheckConncetionAlignment(conA, conB))
        {
            Debug.Log("#44 Stopped at start. Position: " + gameObject.transform.position.ToString("F3")+ ", Rotation: " + gameObject.transform.rotation.eulerAngles.ToString("F3"));   
        }

        Destroy(this);
    }

}
