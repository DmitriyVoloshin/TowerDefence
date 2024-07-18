using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfMap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.position.x) > 50.0f ||
            Mathf.Abs(transform.position.y) > 50.0f)
        {
            Destroy(gameObject);
        }
    }
}
