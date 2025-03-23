using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform PlayerTransform;
    private float CameraSize;
    private float CameraHeight;
    // Start is called before the first frame update
    void Start()
    {
        CameraSize = Camera.main.orthographicSize;
        CameraHeight = CameraSize * 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
