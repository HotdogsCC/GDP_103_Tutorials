using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float moveSpeed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Cursor.lockState = CursorLockMode.None;

        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        Quaternion horizontalRotation = Quaternion.AngleAxis(horizontalInput * rotationSpeed * Time.deltaTime, Vector3.up);

        Quaternion verticalRotation = Quaternion.AngleAxis(-verticalInput * rotationSpeed * Time.deltaTime, Vector3.right);

        transform.rotation = horizontalRotation * transform.rotation * verticalRotation;

        float moveForwardBack = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime; // W and S keys
        float moveLeftRight = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; // A and D keys

        transform.Translate(Vector3.forward * moveForwardBack);
        transform.Translate(Vector3.right * moveLeftRight);


    }
}
