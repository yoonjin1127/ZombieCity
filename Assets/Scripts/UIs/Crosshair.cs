using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    // ȭ�� ���� ũ�ν���� �̹���
    public Image aimPointReticle;

    private Camera screenCamera;

    // ������ǥ�� ��ġ�� ȭ��� ��ġ�� ��ȯ
    private Vector2 targetPoint;

    public void UpdatePosition(Vector3 worldPosition)
    {
        // ������ǥ�� ��ġ�� �Է����� �޾Ƽ� ȭ��� ��ǥ��� ��ȯ, targetPoint�� ����
        targetPoint = screenCamera.WorldToScreenPoint(worldPosition);
    }

    public void SetActiveCrosshair(bool active)
    {
        // ũ�ν���� Ȱ��ȭ
        aimPointReticle.enabled = active;
    }

}
