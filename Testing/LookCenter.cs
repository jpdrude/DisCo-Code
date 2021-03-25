using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCenter : MonoBehaviour
{
    GameObject target;
    void Start()
    {
        float x = PartsHolder.Holder.MinX + (PartsHolder.Holder.MaxX - PartsHolder.Holder.MinX) / 2;
        float z = PartsHolder.Holder.MinZ + (PartsHolder.Holder.MaxZ - PartsHolder.Holder.MinZ) / 2;
        float y = transform.position.y;

        target = new GameObject();
        target.transform.position = new Vector3(x, y, z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.transform);
    }
}
