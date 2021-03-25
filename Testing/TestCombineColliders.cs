using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCombineColliders : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        List<MeshCollider> colliders = new List<MeshCollider>();
        foreach (GameObject go in GlobalReferences.FrozenParts.Values)
        {
            colliders.AddRange(go.GetComponents<MeshCollider>());
            foreach (MeshCollider c in go.GetComponents<MeshCollider>())
            {
                c.enabled = false;
            }
        }

        foreach (MeshCollider mc in colliders)
        {
            MeshCollider col = gameObject.AddComponent<MeshCollider>();
            System.Reflection.FieldInfo[] fields = mc.GetType().GetFields();

            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(col, field.GetValue(mc));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
