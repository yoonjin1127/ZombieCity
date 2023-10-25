using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    // 화면 상의 크로스헤어 이미지
    public Image aimPointReticle;

    private Camera screenCamera;

    // 월드좌표계 위치를 화면상 위치로 변환
    private Vector2 targetPoint;

    public void UpdatePosition(Vector3 worldPosition)
    {
        // 월드좌표계 위치를 입력으로 받아서 화면상 좌표계로 변환, targetPoint에 대입
        targetPoint = screenCamera.WorldToScreenPoint(worldPosition);
    }

    public void SetActiveCrosshair(bool active)
    {
        // 크로스헤어 활성화
        aimPointReticle.enabled = active;
    }

}
