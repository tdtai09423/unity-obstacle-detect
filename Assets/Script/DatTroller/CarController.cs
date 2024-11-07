using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class MovementCommand {
    public float FrontLeftSpeed { get; private set; }
    public float FrontRightSpeed { get; private set; }
    public float RearLeftSpeed { get; private set; }
    public float RearRightSpeed { get; private set; }
    public float Duration { get; private set; }

    public MovementCommand(float frontLeftSpeed, float frontRightSpeed, float rearLeftSpeed, float rearRightSpeed, float duration) {
        FrontLeftSpeed = frontLeftSpeed;
        FrontRightSpeed = frontRightSpeed;
        RearLeftSpeed = rearLeftSpeed;
        RearRightSpeed = rearRightSpeed;
        Duration = duration;
    }
}

public class CarController: MonoBehaviour {
    private Rigidbody rb;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    public Transform frontRightWheelTransform;
    public Transform frontLeftWheelTransform;
    public Transform rearRightWheelTransform;
    public Transform rearLeftWheelTransform;

    public float detectionRange = 5f;
    public LayerMask obstacleLayer;

    private bool isProcessingEvent = false;


    private void FixedUpdate() {
        CheckForObstacle();

        UpdateWheels();
    }
    private void Update() {
        if (TrackBoundaryCheck.IsBoundary) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            SetSpeed(0, 0, 0, 0);

        }
    }

    private void Start() {
        rb = GetComponent<Rigidbody>();
        WebGLInput.captureAllKeyboardInput = false;
    }

    public void MoveCar(string command) {
        // Tách chuỗi thành các tham số tốc độ và thời gian
        string[] parameters = command.Split(',');
        float frontLeftSpeed = float.Parse(parameters[0]);
        float frontRightSpeed = float.Parse(parameters[1]);
        float rearLeftSpeed = float.Parse(parameters[2]);
        float rearRightSpeed = float.Parse(parameters[3]);
        float angle = float.Parse(parameters[4]);
        float duration = float.Parse(parameters[5]);

        frontLeftWheelCollider.steerAngle = angle;
        frontRightWheelCollider.steerAngle = angle;

        // Thiết lập tốc độ cho các bánh
        SetSpeed(frontLeftSpeed, frontRightSpeed, rearLeftSpeed, rearRightSpeed);

        // Chạy coroutine để dừng xe sau thời gian duration
        StartCoroutine(WaitAndStop(duration));
    }

    private void CheckForObstacle() {
        RaycastHit hit;
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        Debug.DrawRay(transform.position, forward * detectionRange, Color.red);
        // Bắn một Raycast về phía trước với khoảng cách detectionRange
        if (Physics.Raycast(transform.position, forward, out hit, detectionRange, obstacleLayer)) {
            if (!isProcessingEvent) {
                // Phát hiện vật cản trong phạm vi
                isProcessingEvent = true;
                OnObstacleDetected();
            }
        } else {
            isProcessingEvent = false;
        }
    }

    private void OnObstacleDetected() {
        Debug.Log("Obstacle detected within range!");


    }

    private IEnumerator WaitAndStop(float duration) {
        yield return new WaitForSeconds(duration);
        SetSpeed(0f, 0f, 0f, 0f); // Dừng xe sau khi hoàn tất thời gian
    }

    private void SetSpeed(float frontLeftSpeed, float frontRightSpeed, float rearLeftSpeed, float rearRightSpeed) {
        if (frontLeftSpeed != 0f && frontRightSpeed != 0f && rearLeftSpeed != 0f && rearRightSpeed != 0f) {
            frontLeftWheelCollider.motorTorque = frontLeftSpeed;
            frontRightWheelCollider.motorTorque = frontRightSpeed;
            rearLeftWheelCollider.motorTorque = rearLeftSpeed;
            rearRightWheelCollider.motorTorque = rearRightSpeed;
            ClearSpeed(0f);
        } else {
            ClearSpeed(1000f);
        }
        // Thiết lập tốc độ của từng bánh xe

    }
    private void ClearSpeed(float barkTorque) {
        frontLeftWheelCollider.brakeTorque = barkTorque;
        frontRightWheelCollider.brakeTorque = barkTorque;
        rearLeftWheelCollider.brakeTorque = barkTorque;
        rearRightWheelCollider.brakeTorque = barkTorque;

    }

    private void UpdateWheels() {
        RotateWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        RotateWheel(frontRightWheelCollider, frontRightWheelTransform);
        RotateWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        RotateWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void RotateWheel(WheelCollider wheelCollider, Transform transform) {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        transform.position = pos;
        transform.rotation = rot;
    }

}
