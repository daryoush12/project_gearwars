using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace rtsprj{

public class RTSCamera : MonoBehaviour {
    public LayerMask groundLayer;
    [System.Serializable]
    public class PositionSettings {
        public bool invertPan = true;
        public float panSmooth = 7F;
        public float distanceFromGround = 40;
        public bool allowZoom = true;
        public float zoomSmooth = 5;
        public float zoomStep = 5;
        public float maxZoom = 25;
        public float minZoom = 80;
        [HideInInspector]
        public float newDistance = 40;
    }
    [System.Serializable]
    public class OrbitSettings {
        public float xRotation = 50;
        public float yRotation = 0;
        public bool AllowYOrbit = true;
        public float yOrbitSMooth = 0.5F;
    }
    [System.Serializable]
    public class InputSettings {
        public string PAN = "MousePan";
        public string ORBIT_Y = "MouseTurn";
        public string ZOOM = "Mouse ScrollWheel";
    }
    [System.Serializable]
    public class Mouse_ScreenSettings {
        public float speed = 10.0f;
        public int GUISize = 25;
        public Vector3 movement = Vector3.zero;
        public float maxScreenWidth = Screen.width - 20;
        public float maxScreenHeight = Screen.height - 20;
        public float numberOfPixlesToMove = 1;
    }
    //Initiate our support classes:
    public PositionSettings position = new PositionSettings();
    public OrbitSettings orbit = new OrbitSettings();
    public InputSettings input = new InputSettings();
    public Mouse_ScreenSettings MouseS = new Mouse_ScreenSettings();
    //Vector 3's: Positions:
    Vector3 destination = Vector3.zero;
    Vector3 camVel = Vector3.zero;
    Vector3 previousMousePos = Vector3.zero;
    Vector3 currentMousePos = Vector3.zero;
    float panInput, orbitInput, zoomInput;
    int panDirection = 0;
    //Methods:
    void Start() {
        panInput = 0;
        orbitInput = 0;
        zoomInput = 0;
    }
    void GetInput() {
        //Responsible for setting input variables:
        panInput = Input.GetAxis(input.PAN);
        orbitInput = Input.GetAxis(input.ORBIT_Y);
        zoomInput = Input.GetAxis(input.ZOOM);
        //Updating current mouse position:
        previousMousePos = currentMousePos;
        currentMousePos = Input.mousePosition;
    }
    void Update() {
        //Handles Zooming, rotating panning.
        GetInput();
        if (position.allowZoom)
            Zoom();
        if (orbit.AllowYOrbit)
            Rotate();
        PanWorld();
    }
    void FixedUpdate()
    {
        //Handles camera distance
        HandleCameraDistance();
    }
    void LateUpdate() {
        var recdown = new Rect(0, 0, MouseS.maxScreenWidth, MouseS.GUISize);
        var recup = new Rect(0, MouseS.maxScreenHeight - MouseS.GUISize, MouseS.maxScreenWidth, MouseS.GUISize);
        var recleft = new Rect(0, 0, MouseS.GUISize, MouseS.maxScreenHeight);
        var recright = new Rect(Screen.width - MouseS.GUISize, 0, MouseS.GUISize, Screen.height);
        //TODO:
        //fire raycasts from cursors position to detect units or structures.
        //if scrolling: scroll view in x axis
        if (recdown.Contains(Input.mousePosition))
            transform.Translate(0, 0, -MouseS.speed, Space.World);
        if (recup.Contains(Input.mousePosition))
            transform.Translate(0, 0, MouseS.speed, Space.World);
        if (recleft.Contains(Input.mousePosition))
            transform.Translate(-MouseS.speed, 0, 0, Space.World);
        if (recright.Contains(Input.mousePosition))
            transform.Translate(MouseS.speed, 0, 0, Space.World);
    }
    void PanWorld() {
        Vector3 targetPos = transform.position;
        if (position.invertPan)
            panDirection = -1;
        else
            panDirection = 1;
        if (panInput > 0) {
            //Because forward axis changes we need to adapt to the situation:
            targetPos += transform.right * (currentMousePos.x - previousMousePos.x) 
             * position.panSmooth * panDirection * Time.deltaTime;
            targetPos += Vector3.Cross(transform.right, Vector3.up) * (currentMousePos.y - previousMousePos.y) 
             * position.panSmooth * panDirection * Time.deltaTime;
        }
        transform.position = targetPos;
    }
    void HandleCameraDistance()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, groundLayer)) {
            destination = Vector3.Normalize(transform.position - hit.point) * position.distanceFromGround;
            destination += hit.point;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref camVel, 0.3f);
        }
    }
    void Zoom()
    {
        position.newDistance += position.zoomStep * -zoomInput;
        //
        position.distanceFromGround = Mathf.Lerp(position.distanceFromGround, 
        position.newDistance, position.zoomSmooth * Time.deltaTime);
        //
        if(position.distanceFromGround < position.maxZoom)
        {
            position.distanceFromGround = position.maxZoom;
            position.newDistance = position.maxZoom;
        }
        if (position.distanceFromGround > position.minZoom)
        {
            position.distanceFromGround = position.minZoom;
            position.newDistance = position.minZoom;
        }
    }
    void Rotate() {
        if (orbitInput > 0) {
            orbit.yRotation += (currentMousePos.x - previousMousePos.x) * orbit.yOrbitSMooth * Time.deltaTime;
        }
        transform.rotation = Quaternion.Euler(orbit.xRotation, orbit.yRotation, 0);
    }
}
}