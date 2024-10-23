using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header ("Player Movement")]
    //플레이어의 움직임 속도를 설정하는 변수
    public float moveSpeed = 5.0f;          //이동속도
    public float jumpForce = 5.0f;          //점프 힘
    public float rotationSpeed = 10f;       //회전 속도

    //카메라 설정 변수
    [Header("Camera Settings")]
    public Camera firstPersonCamera;        //1인칭 카메라
    public Camera thirdPersonCamera;        //3인칭 카메라

    public float radius = 5.0f;            //3인칭 카메라와 플레이어 간의 거리
    public float minRadius = 1.0f;         // 카메라 최소 거리 
    public float maxRadius = 10.0f;        //카메라 최대 거리

    public float yMinLimit = 30;                //
    public float yMaxLimit = 90;                //

    private float theta = 0.0f;
    private float phi = 0.0f;
    private float targetVerticalRoataion = 0;
    private float verticalRotationSpeed = 240f;

    public float mouseSenseitivity = 2f;    //마우스 감도



    //내부 변수들
    private bool isFirstPerson = true;      //1인칭 모드 인지 여부
    private bool isGrounded;               //플레이어가 땅에 있는지 여부
    private Rigidbody rb;                  // 플레이어의 리즈드바디 


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        SetupCameras();
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        HandleJump();
        HandleCameraToggle();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void SetActiveCamera()
    {
        firstPersonCamera.gameObject.SetActive(isFirstPerson);              //1인칭 카메라 활성화
        thirdPersonCamera.gameObject.SetActive(!isFirstPerson);              //3인칭 카메라 활성화
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSenseitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSenseitivity;

        //수평회전
        theta += mouseX;
        theta = Mathf.Repeat(theta, 360f);

        //수직 회전 처리
        targetVerticalRoataion -= mouseY;
        targetVerticalRoataion = Mathf.Clamp(targetVerticalRoataion, yMinLimit, yMaxLimit);
        phi = Mathf.MoveTowards(phi, targetVerticalRoataion, verticalRotationSpeed * Time.deltaTime);

        //플레이어 회전 (캐릭터가 수평으로만 회전)
        transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);

        if (isFirstPerson)
        {
            transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
            firstPersonCamera.transform.localRotation = Quaternion.Euler(phi, 0.0f, 0.0f);

        }
        else
        {
            //3인칭 카메라 구면 좌표계에서 위치 및 회전 계산
            float x = radius * Mathf.Sin(Mathf.Deg2Rad * phi) * Mathf.Cos(Mathf.Deg2Rad * theta);
            float y = radius * Mathf.Cos(Mathf.Deg2Rad * phi);
            float z = radius * Mathf.Sin(Mathf.Deg2Rad * phi) * Mathf.Sin(Mathf.Deg2Rad * theta);

            thirdPersonCamera.transform.position = transform.position + new Vector3(x, y, z);
            thirdPersonCamera.transform.LookAt(transform);

            radius = Mathf.Clamp(radius - Input.GetAxis("Mouse ScrollWheel") * 5, minRadius, maxRadius);
        }

    }

    //1인칭과 3인칭 카메라를 전화하는 함수
    void HandleCameraToggle()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isFirstPerson = !isFirstPerson;
            SetActiveCamera();
        }
    }

    //카메라 초기 위치 및 회전을 설정하는 함수
    void SetupCameras()
    {
        firstPersonCamera.transform.localPosition = new Vector3(0f, 0.6f, 0f);
        firstPersonCamera.transform.localRotation = Quaternion.identity;
    }


    //플레이어 점프를 처리하는 함수
    void HandleJump()
    {
        //점프 버튼을 누르고 땅에 있을 떄
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);         //위쪽으로 힘을 가해 점프
            isGrounded = false;                                             //공중에 있는 상태로 전환
        }

    }

    void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVerical = Input.GetAxis("Vertical");
       
        Vector3 movement;
        if (!isFirstPerson)
        {
            Vector3 cameraForward = thirdPersonCamera.transform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            Vector3 cameraRight = thirdPersonCamera.transform.right;
            cameraRight.y = 0f;
            cameraRight.Normalize();

            //이동 벡터 계산
            movement = cameraForward * moveVerical + cameraRight * moveHorizontal;
            
        }
        else 
        {
            //캐릭터 기준으로 이동(1인칭)
            movement = transform.right * moveHorizontal + transform.forward * moveVerical;

        }

        //이동 방향으로 캐릭터 회전
        if (movement.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

        }

        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);



    }

    //플레이어가 땅에 닿아 있는지 감지
    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;               //충돌 중이면 플레이어는 땅에 있다.
    }
}
