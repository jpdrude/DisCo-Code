using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class InitializeBlock : MonoBehaviour
{
    public Vector3 position;
    public Quaternion rotation;
    public int id;

    public Connection conA;
    public Connection conB;
    int frame = 0;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    private void Update()
    {
        if (frame == 1)
        {
            transform.position = position;
            transform.rotation = rotation;

            GetComponent<Part>().FreezePart(id);

            var testAlign = gameObject.AddComponent<TestPlaneAlignment>();
            testAlign.conA = conA;
            testAlign.conB = conB;

            //Debug.Log("#43 Block Spawned: " + GetComponent<Part>().ID + ", Position = " + transform.position.ToString("F2") + ", Rotation = " + transform.rotation.eulerAngles.ToString("F2"));
            Destroy(this);
        }
        ++frame;
    }
}
