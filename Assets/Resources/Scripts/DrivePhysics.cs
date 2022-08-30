using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivePhysics : MonoBehaviour
{
    
    public WheelCollider wheelL;
    public WheelCollider wheelR;

    [Header("Kinetics")]
    [Range(0f, 3f)]
    public float maxMotorTorqueL = 1f;
    [Range(0f, 3f)]
    public float maxMotorTorqueR = 1f;

    public float currentSpeed;
    public float maxSpeed = 3f;
    public Vector3 centerOfMass;

    float _maxMotorTorqueL;
    float _maxMotorTorqueR;

    [Header("Sensors")]
    public float radius = 1f;
    [Range(0,720)]
    public float frequency = 360f;
    public Vector3 lidarPosition = new Vector3(0f, 0.15f, 0f);
    float angle = 0f;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
    }

    // Update is called once per frame
    private void Update(){
        _maxMotorTorqueL = maxMotorTorqueL;
        _maxMotorTorqueR = maxMotorTorqueR;
        Sensors();
    }

    private void FixedUpdate()
    {   
        Move();
    }

    private void Sensors() {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += lidarPosition;

        // Lidar Center Raycast
        Debug.Log(angle);
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(angle, transform.up) * transform.forward, out hit, radius))
        {
            Debug.DrawLine(sensorStartPos, hit.point, Color.red);
        }
        else
        {
            Debug.DrawLine(sensorStartPos, sensorStartPos + (Quaternion.AngleAxis(angle, transform.up) * transform.forward).normalized * radius);
        }

       angle += frequency * Time.deltaTime;
       //angle = (angle >= 360) ? angle - 360 : angle;
    }

    private void Move() {
        currentSpeed = 2 * Mathf.PI * wheelL.radius * wheelL.rpm / 60;

        if (currentSpeed < maxSpeed) {
            wheelL.motorTorque = _maxMotorTorqueL;
            wheelR.motorTorque = _maxMotorTorqueR;
        } else {
            wheelL.motorTorque = 0;
            wheelR.motorTorque = 0;
        }
    }
}
