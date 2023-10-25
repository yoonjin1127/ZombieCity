using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TPSCamera : MonoBehaviour
{
    // 카메라 루트와 카메라 민감도, 거리를 설정
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
        // 마우스를 게임 중앙 좌표에 고정시키고, 커서를 보이지 않게 함
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        // 마우스 커서 정상화
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        // 회전 함수 활성화
        Rotate();
    }

    private void LateUpdate()
    {
        // 카메라는 오브젝트를 따라가므로, 가장 마지막에 호출되는 업데이트 함수를 사용
        Look();
    }

    public void Rotate()
    {
        // 카메라가 이동할 때 캐릭터도 함께 회전
        // 카메라 회전과 플레이어 회전이 함께 일어난다면 엄청나게 떨리게 된다
        // 그러므로 카메라를 카메라 루트에 맞춰 회전시키고, 그를 플레이어가 바라보게 한다

        // lookPoint = 현재 카메라의 위치에서 정면 방향으로 lookDistance만큼 이동한 지점
        // 마지막으로 플레이어를 lookPoint를 향하도록 회전시킴
        Vector3 lookPoint = Camera.main.transform.position + Camera.main.transform.forward * lookDistance;

        // 바라보고자 하는 위치를 쏘는 위치로
        aimTarget.position = lookPoint;

        // aimTarget.position = lookPoint;
        // aimTarget.position = lookPoint;

        lookPoint.y = transform.position.y;
        transform.LookAt(lookPoint);
    }

    private void OnLook(InputValue value)
    {
        // 입력 받은 value값을 Vector2로 추출해, lookDelta에 담는다
        lookDelta = value.Get<Vector2>();
    }

    private void Look()
    {
        // 방향 * 마우스 민감도 * 델타타임만큼 회전시킴
        yRotation += lookDelta.x * cameraSensitivity * Time.deltaTime;
        xRotation -= lookDelta.y * cameraSensitivity * Time.deltaTime;

        // Clamp로 최소, 최댓값을 설정하여 회전각도가 범위를 넘어가지 않게 한다
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // cameraRoot의 로컬 회전을 설정
        // 카메라의 상하좌우 이동을 표현하는 데 사용됨
        cameraRoot.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
    
    private void ZoomIn()
    {
        // 클릭방식이면 무조건 한번에 바뀜
        // 코루틴 권장
        // or 꾸욱 누르면 변경되는 형식
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
