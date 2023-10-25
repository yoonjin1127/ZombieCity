using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TPSCamera : MonoBehaviour
{
    // ī�޶� ��Ʈ�� ī�޶� �ΰ���, �Ÿ��� ����
    [SerializeField] Transform cameraRoot;
    [SerializeField] public float cameraSensitivity;
    [SerializeField] float lookDistance;
    [SerializeField] Transform aimTarget;
    [SerializeField] CinemachineVirtualCamera virtualCam;
    [SerializeField] Slider SenSlider;

    private Vector2 lookDelta;
    private float xRotation;
    private float yRotation;
    private bool IsZoom;

    private void Awake()
    {
        IsZoom = false;
    }

    private void OnEnable()
    {
        // ���콺�� ���� �߾� ��ǥ�� ������Ű��, Ŀ���� ������ �ʰ� ��
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        // ���콺 Ŀ�� ����ȭ
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        // ȸ�� �Լ� Ȱ��ȭ
        Rotate();
    }

    private void LateUpdate()
    {
        // ī�޶�� ������Ʈ�� ���󰡹Ƿ�, ���� �������� ȣ��Ǵ� ������Ʈ �Լ��� ���
        Look();
    }

    public void Rotate()
    {
        // ī�޶� �̵��� �� ĳ���͵� �Բ� ȸ��
        // ī�޶� ȸ���� �÷��̾� ȸ���� �Բ� �Ͼ�ٸ� ��û���� ������ �ȴ�
        // �׷��Ƿ� ī�޶� ī�޶� ��Ʈ�� ���� ȸ����Ű��, �׸� �÷��̾ �ٶ󺸰� �Ѵ�

        // lookPoint = ���� ī�޶��� ��ġ���� ���� �������� lookDistance��ŭ �̵��� ����
        // ���������� �÷��̾ lookPoint�� ���ϵ��� ȸ����Ŵ
        Vector3 lookPoint = Camera.main.transform.position + Camera.main.transform.forward * lookDistance;

        // �ٶ󺸰��� �ϴ� ��ġ�� ��� ��ġ��
        aimTarget.position = lookPoint;

        // aimTarget.position = lookPoint;
        // aimTarget.position = lookPoint;

        lookPoint.y = transform.position.y;
        transform.LookAt(lookPoint);
    }

    private void OnLook(InputValue value)
    {
        // �Է� ���� value���� Vector2�� ������, lookDelta�� ��´�
        lookDelta = value.Get<Vector2>();
    }

    private void Look()
    {
        // ���� * ���콺 �ΰ��� * ��ŸŸ�Ӹ�ŭ ȸ����Ŵ
        yRotation += lookDelta.x * cameraSensitivity * Time.deltaTime;
        xRotation -= lookDelta.y * cameraSensitivity * Time.deltaTime;

        // Clamp�� �ּ�, �ִ��� �����Ͽ� ȸ�������� ������ �Ѿ�� �ʰ� �Ѵ�
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // cameraRoot�� ���� ȸ���� ����
        // ī�޶��� �����¿� �̵��� ǥ���ϴ� �� ����
        cameraRoot.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
    
    private void ZoomIn()
    {
        // Ŭ������̸� ������ �ѹ��� �ٲ�
        // �ڷ�ƾ ����
        // or �ٿ� ������ ����Ǵ� ����
        virtualCam.m_Lens.FieldOfView = Mathf.Lerp(60, 10, Time.deltaTime*0.1f);
        cameraSensitivity = cameraSensitivity * 0.5f;
        IsZoom = true;
    }

    private void ZoomOut()
    {
        virtualCam.m_Lens.FieldOfView = Mathf.Lerp(10, 60, Time.deltaTime*0.1f);
        cameraSensitivity = cameraSensitivity * 2f;
        IsZoom = false;
    }

    private void OnZoomIn(InputValue value)
    {       
        if (IsZoom)
            ZoomOut();
        else
            ZoomIn();
    }

    private void OnZoomOut(InputValue value)
    {
        ZoomOut();
    }
}
