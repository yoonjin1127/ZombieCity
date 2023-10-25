using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{


    [SerializeField] Rig aimRig;
    [SerializeField] private float reloadTime;
    [SerializeField] WeaponHolder weaponHolder;
    [SerializeField] public LayerMask excludeTarget;

    public Gun gun; // 사용할 총
    private PlayerInput playerInput;    // 입력
    private Camera playerCamera;        // 플레이어 카메라
    private Animator anim;
    private bool isReloading;

    // public GameObject[] grenades;
    public int hasGrenades;     // 수류탄 개수
    public GameObject grenadeObj;   // 수류탄
    // public Camera followCam;


    // 마지막 발사 시간
    private float lastFireInputTime;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (excludeTarget != (excludeTarget | (1 << gameObject.layer)))
        {
            excludeTarget |= 1 << gameObject.layer;
        }
    }

    private void Start()
    {
        playerCamera = Camera.main; // 메인카메라 가져옴
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        // gun 오브젝트 활성화
        gun.gameObject.SetActive(true);
        // gun의 Setup함수 실행
        gun.SetUp(this);
    }

    private void OnDisable()
    {
        gun.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateUI();
    }

    /*
    private void updateAimTarget()
    {
        RaycastHit hit;

        var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        if (Physics.Raycast(ray, out hit, gun.maxDistance, ~excludeTarget))
        {
            aimPoint = hit.point; 

            if (Physics.Linecast(gun.fireTransform.position, hit.point, out hit, ~excludeTarget))
            {
                aimPoint = hit.point;
            }
        }
    }
    */

    private void UpdateUI()
    {
        if (gun == null || UIManager.Instance == null) return;

        // 탄약 UI 갱신
        UIManager.Instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);
    }

    private void OnReload(InputValue value)
    {
        if (isReloading)
            return;
        StartCoroutine(ReloadRoutine());
        weaponHolder.Reload();
    }

    IEnumerator ReloadRoutine()
    {
        anim.SetTrigger("Reload");
        isReloading = true;
        aimRig.weight = 0f;
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
        aimRig.weight = 1f;
    }

    public void Fire()
    {
        lastFireInputTime = Time.time;
        weaponHolder.Fire();
        anim.SetTrigger("Fire");
    }

    private void OnFire(InputValue value)
    {
        if (isReloading)
            return;

        Fire();
    }

    private void Grenade()
    {
        if (hasGrenades == 0)
            return;

        if(!isReloading)
        {
            // 메인 카메라로부터 스크린의 점
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //  Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            Debug.DrawLine(ray.origin, Camera.main.transform.forward * 50000000, Color.green, 10f);

            RaycastHit rayHit;  // 충돌에 대한 정보를 rayHit에 저장함
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point = transform.position;
                nextVec.y = 10f; // 가하는 힘

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Force);
                rigidGrenade.AddTorque(Vector3.forward * 1, ForceMode.Impulse);   // AddTorque는 축을 기준으로 힘을 가한다

                hasGrenades--;
            }
            anim.SetTrigger("Grenade");
        }
    }

    private void OnGrenade(InputValue value)
    {
        if (isReloading || hasGrenades <= 0)
            return;

        else
        Grenade();
    }    

}
