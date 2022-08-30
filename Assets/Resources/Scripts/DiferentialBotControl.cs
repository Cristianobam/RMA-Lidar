using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiferentialBotControl : MonoBehaviour
{

    [Header("Robot Definition")]
    public float wheelRadius = 0.017f; //Raio da roda em metros
    public float bodyRadius = 0.05f; //Raio do corpo do robo em metros
    public float maxLeftWheelSpeed = 0.0f;
    public float maxRightWheelSpeed = 0.0f;
    public float leftWheelSpeed;
    public float rightWheelSpeed;
    public GameObject robotChassis;

    float cos_theta = 0, sin_theta = 0;
    float theta = 0;
    float xAtual, xAnterior, yAtual, yAnterior, anguloAnterior = 0;

    [Header("Lidar")]
    public bool renderLidar = true;
    public float fov = 90f;
    public int numberOfRays = 21;
    public float rayRange = 0.35f;
    public GameObject Lidar;
    public Vector3 lidarOffset = new Vector3(0,0,0);

    private void Update() {
        Sensors();
        Main();
    }

    void OnDrawGizmos(){
        if (renderLidar){
        for (int i=0; i<numberOfRays; ++i){
            var rotation = this.transform.rotation;
            var rotationMod = Quaternion.AngleAxis(i/((float) numberOfRays-1) * fov - fov/2, robotChassis.transform.up);
            var direction = rotation * rotationMod * this.transform.worldToLocalMatrix.MultiplyVector(-transform.right);
            Gizmos.DrawRay(Lidar.transform.position + lidarOffset, direction * rayRange);
        }
        }
    }

    private void Sensors() {
        leftWheelSpeed = maxLeftWheelSpeed;
        rightWheelSpeed = maxRightWheelSpeed;
        for (int i=0; i<numberOfRays; ++i){
            var rotation = this.transform.rotation;
            var currentAngle = i/((float) numberOfRays-1) * fov - fov/2;
            var rotationMod = Quaternion.AngleAxis(currentAngle, robotChassis.transform.up);
            var direction = rotation * rotationMod * this.transform.worldToLocalMatrix.MultiplyVector(-transform.right);
            
            RaycastHit hitInfo;
            if (Physics.Raycast(Lidar.transform.position + lidarOffset, direction.normalized, out hitInfo, rayRange)){
                if(hitInfo.collider.CompareTag("Obstacle")){
                    if (renderLidar) {Debug.DrawLine(Lidar.transform.position + lidarOffset, hitInfo.point, Color.red);}
                    if (currentAngle < 0){
                        leftWheelSpeed -=  (1f / numberOfRays) * maxLeftWheelSpeed;
                    } else if (currentAngle > 0) {
                        rightWheelSpeed -=  (1f / numberOfRays) * maxRightWheelSpeed;
                    } else if (currentAngle == 0) {
                        leftWheelSpeed = maxLeftWheelSpeed;
                        rightWheelSpeed = -maxRightWheelSpeed;
                    }
                } 
                // else {
                //     leftWheelSpeed += (1f / numberOfRays) * maxLeftWheelSpeed;
                //     rightWheelSpeed += (1f / numberOfRays) * maxRightWheelSpeed;
                // }
            }
        }
    }

    void Main()
    {
        float v = -1 * FowardSpeed();
        float omega = GetOmega();
        Vector3 posAnterior = transform.position;
        xAtual = posAnterior.x + (v * cos_theta * Time.deltaTime);
        yAtual = posAnterior.z + (v * sin_theta * Time.deltaTime);
        cos_theta = Mathf.Cos(theta);
        sin_theta = Mathf.Sin(theta);

        xAnterior = xAtual;
        yAnterior = yAtual;
        anguloAnterior = theta;

        //movimentacao
        transform.position = new Vector3(xAtual, 0, yAtual);
        transform.Rotate(0, (-(omega * Time.deltaTime) * Mathf.Rad2Deg), 0);

        theta = anguloAnterior + (omega * Time.deltaTime);
    }

    float FowardSpeed()
    {
        float speed = 0;
        speed = ((leftWheelSpeed * wheelRadius) / 2) + ((rightWheelSpeed * wheelRadius) / 2);
        return speed;
    }

    float GetOmega()
    {
        float t = ((leftWheelSpeed * wheelRadius) - (rightWheelSpeed * wheelRadius)) / (2 * bodyRadius);
        return t;
    }
 
}
