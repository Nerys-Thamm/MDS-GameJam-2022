using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateCamera : MonoBehaviour
{
    public InputAction rotateAction;
    // Start is called before the first frame update
    void Start()
    {
        
        rotateAction.Enable();
    }
    void Rotate(float input)
    {
        transform.Rotate(Vector3.up, (float)input * Time.deltaTime * 100);
        
    }
    // Update is called once per frame
    void Update()
    {
        //Rotate based on input
        Rotate(rotateAction.ReadValue<float>());
    }
}
