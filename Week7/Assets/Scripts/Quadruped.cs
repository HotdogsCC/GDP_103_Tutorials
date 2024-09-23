using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadruped : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float speed = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * movementSpeed;
        Vector3 position = transform.position;
        position -= transform.forward * speed;
        transform.position = position;  
    }
}
