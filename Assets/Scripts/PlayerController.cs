using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header ("Player Movement")]
    //�÷��̾��� ������ �ӵ��� �����ϴ� ����
    public float moveSpeed = 5.0f;          //�̵��ӵ�
    public float jumpForce = 5.0f;          //���� ��
    public float rotationSpeed = 10f;       //ȸ�� �ӵ�

    //ī�޶� ���� ����
    [Header("Camera Settings")]
    public Camera firstPersonCamera;        //1��Ī ī�޶�
    public Camera thirdPersonCamera;        //3��Ī ī�޶�

    public float radius = 5.0f;            //3��Ī ī�޶�� �÷��̾� ���� �Ÿ�
    public float minRadius = 1.0f;         // ī�޶� �ּ� �Ÿ� 
    public float maxRadius = 10.0f;        //ī�޶� �ִ� �Ÿ�

    public float yMinLimit = 30;                //
    public float yMaxLimit = 90;                //

    private float theta = 0.0f;
    private float phi = 0.0f;
    private float targetVerticalRoataion = 0;
    private float verticalRotationSpeed = 240f;

    public float mouseSenseitivity = 2f;    //���콺 ����



    //���� ������
    private bool isFirstPerson = true;      //1��Ī ��� ���� ����
    private bool isGrounded;               //�÷��̾ ���� �ִ��� ����
    private Rigidbody rb;                  // �÷��̾��� �����ٵ� 


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
        firstPersonCamera.gameObject.SetActive(isFirstPerson);              //1��Ī ī�޶� Ȱ��ȭ
        thirdPersonCamera.gameObject.SetActive(!isFirstPerson);              //3��Ī ī�޶� Ȱ��ȭ
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSenseitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSenseitivity;

        //����ȸ��
        theta += mouseX;
        theta = Mathf.Repeat(theta, 360f);

        //���� ȸ�� ó��
        targetVerticalRoataion -= mouseY;
        targetVerticalRoataion = Mathf.Clamp(targetVerticalRoataion, yMinLimit, yMaxLimit);
        phi = Mathf.MoveTowards(phi, targetVerticalRoataion, verticalRotationSpeed * Time.deltaTime);

        //�÷��̾� ȸ�� (ĳ���Ͱ� �������θ� ȸ��)
        transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);

        if (isFirstPerson)
        {
            transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
            firstPersonCamera.transform.localRotation = Quaternion.Euler(phi, 0.0f, 0.0f);

        }
        else
        {
            //3��Ī ī�޶� ���� ��ǥ�迡�� ��ġ �� ȸ�� ���
            float x = radius * Mathf.Sin(Mathf.Deg2Rad * phi) * Mathf.Cos(Mathf.Deg2Rad * theta);
            float y = radius * Mathf.Cos(Mathf.Deg2Rad * phi);
            float z = radius * Mathf.Sin(Mathf.Deg2Rad * phi) * Mathf.Sin(Mathf.Deg2Rad * theta);

            thirdPersonCamera.transform.position = transform.position + new Vector3(x, y, z);
            thirdPersonCamera.transform.LookAt(transform);

            radius = Mathf.Clamp(radius - Input.GetAxis("Mouse ScrollWheel") * 5, minRadius, maxRadius);
        }

    }

    //1��Ī�� 3��Ī ī�޶� ��ȭ�ϴ� �Լ�
    void HandleCameraToggle()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isFirstPerson = !isFirstPerson;
            SetActiveCamera();
        }
    }

    //ī�޶� �ʱ� ��ġ �� ȸ���� �����ϴ� �Լ�
    void SetupCameras()
    {
        firstPersonCamera.transform.localPosition = new Vector3(0f, 0.6f, 0f);
        firstPersonCamera.transform.localRotation = Quaternion.identity;
    }


    //�÷��̾� ������ ó���ϴ� �Լ�
    void HandleJump()
    {
        //���� ��ư�� ������ ���� ���� ��
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);         //�������� ���� ���� ����
            isGrounded = false;                                             //���߿� �ִ� ���·� ��ȯ
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

            //�̵� ���� ���
            movement = cameraForward * moveVerical + cameraRight * moveHorizontal;
            
        }
        else 
        {
            //ĳ���� �������� �̵�(1��Ī)
            movement = transform.right * moveHorizontal + transform.forward * moveVerical;

        }

        //�̵� �������� ĳ���� ȸ��
        if (movement.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

        }

        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);



    }

    //�÷��̾ ���� ��� �ִ��� ����
    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;               //�浹 ���̸� �÷��̾�� ���� �ִ�.
    }
}
