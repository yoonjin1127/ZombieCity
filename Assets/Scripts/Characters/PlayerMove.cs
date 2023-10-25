using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class PlayerMove : MonoBehaviour
{
    // 싱글톤 추가
    public static PlayerMove Instance { get; private set; }

    [SerializeField] private float jumpSpeed;
    //[SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    private CharacterController controller;
    private PlayerInput playerInput;
    public AudioClip itemPickupClip;
    public int lifeRemains = 3;
    private AudioSource playerAudioPlayer;
    private PlayerHealth playerHealth;
    private PlayerMove playerMove;
    private PlayerShooter playerShooter;
    private Gun gun;
    private TPSCamera tpsCamera;
    private Vector3 moveDir;
    private float ySpeed = 0;
    private Animator anim;
    //private bool isWalking;
    private float moveSpeed;

    public AudioSource gameOverAudio;   // 게임오버 소리


    private void Awake()
    {
        Instance = this;
        // ĳ���� ��Ʈ�ѷ� ������Ʈ�� controller ������ �Ҵ�
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        playerMove = GetComponent<PlayerMove>();    
        playerShooter = GetComponent<PlayerShooter>();
        playerHealth = GetComponent<PlayerHealth>();
        playerAudioPlayer = GetComponent<AudioSource>();
        // 플레이어가 사망하면 HandleDeath 함수를 실행
        playerHealth.OnDeath += HandleDeath;
        tpsCamera = GetComponent<TPSCamera>();

        UIManager.Instance.UpdateLifeText(lifeRemains);
        Cursor.visible = false;
    }

    private void HandleDeath()
    {
        playerMove.enabled = false;
        playerShooter.enabled = false;
        tpsCamera.enabled = false;
        playerInput.enabled = false;

        if (lifeRemains > 0) 
        {
            lifeRemains--;
            UIManager.Instance.UpdateLifeText(lifeRemains);
            Invoke(("Respawn"), 3f);
        }
        else
        {
            GameManager.Instance.EndGame();
            gameOverAudio.Play();
        }
        Cursor.visible = true;
    }

    public void Respawn()
    {
        gameObject.SetActive(false);
        // 일정 거리 떨어진 장소에서 리스폰
        transform.position = Utility.GetRandomPointOnNavMesh(transform.position, 10f, UnityEngine.AI.NavMesh.AllAreas);

        gameObject.SetActive(true);
        playerInput.enabled = true;
        playerShooter.enabled = true;
        playerMove.enabled = true;
        tpsCamera.enabled = true;

        playerShooter.gun.ammoRemain = 120;

        Cursor.visible = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        // 아이템과 충돌한 경우 해당 아이템을 사용하는 처리
        // 사망하지 않은 경우에만 아이템 사용가능
        if (!playerHealth.dead)
        {
            // 충돌한 상대방으로부터 Item 컴포넌트를 가져오기 시도
            var item = other.GetComponent<IItem>();

            // 충돌한 상대방으로부터 Item 컴포넌트를 가져오는데 성공했다면
            if (item != null)
            {
                // Use 메서드를 실행하여 아이템 사용
                item.Use(gameObject);
                // 아이템 습득소리 재생
                playerAudioPlayer.PlayOneShot(itemPickupClip);
            }
        }
    }

    private void Update()
    {
        Move();
        Jump();
    }

    private void Move()
    {
        if (moveDir.magnitude == 0)     // �� ������
        {
            // õõ�� ���߰� �ϱ�
            // moveSpeed�� 0.5f�� �ش��ϴ� �ӵ��� ����
            moveSpeed = Mathf.Lerp(moveSpeed, 0, 0.5f);
        }
        /*
         * else if (isWalking)
        {
            // moveSpeed�� walkSpeed�� 0.5f�� �ӵ��� ��ȭ
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, 0.5f);
        }
        */
        else
        {
            // moveSpeed�� runSpeed�� 0.5f�� �ӵ��� ��ȭ
            moveSpeed = Mathf.Lerp(moveSpeed, runSpeed, 0.5f);
        }

        // ���ϴ� ����,�ӷ����� �̵�
        // forward�� z�� ����ϰ�, right�� x�� ����� �̵��Ѵ�
        // �̿����� back�̳� left �������� �̵��Ϸ��� forward�� right�� �ݴ���� ���͸� ����ϸ� �ȴ�
        controller.Move(transform.forward * moveDir.z * moveSpeed * Time.deltaTime);
        controller.Move(transform.right * moveDir.x * moveSpeed * Time.deltaTime);

        anim.SetFloat("XSpeed", moveDir.x, 0.5f, Time.deltaTime);
        anim.SetFloat("YSpeed", moveDir.z, 0.5f, Time.deltaTime);
        anim.SetFloat("Speed", moveSpeed);

        //anim.SetBool("YSpeed", moveDir.sqrMagnitude > 0);

    }

    private void OnMove(InputValue value)
    {
        // ���ӳ������� WASD �̵��� x��� z���� �����
        Vector2 input = value.Get<Vector2>();

        // moveDir�� x�� = �Է¹��� x��, y�� = 0, z�� = �Է¹��� y���̴�
        // �Է°��� ������� �� �̵� ���� moveDir�� �����Ѵ�
        moveDir = new Vector3(input.x, 0, input.y);

    }


    private void Jump()
    {
        // y�������� ����ؼ� �ӷ��� ����
        ySpeed += Physics.gravity.y * Time.deltaTime;

        // ���� CharacterController ������Ʈ���� IsGrounded ������ ������������, ���е��� ���� ������ �� ������� ����
        // GroundCheck�� �����߰� ySpeed�� �����϶�, ySpeed�� -1�̴�
        if (GroundCheck() && ySpeed < 0)
            ySpeed = -1;

        // �� �������� y���ǵ常ŭ ���ư��� �Ѵ�
        // ��ŸŸ������ ��ǻ�ͺ� �ӵ��� ���Ͻ�Ŵ
        controller.Move(Vector3.up * ySpeed * Time.deltaTime);
    }

    private void OnJump(InputValue value)
    {
        if (GroundCheck())
        // �����ӷ¸�ŭ y�������� ���� ����
        {
            ySpeed = jumpSpeed;
            anim.SetTrigger("Jump");
        }
    }

    private bool GroundCheck()
    {
        RaycastHit hit;
        // SphereCast�� ����Ͽ� ��ü ������ ���� �浹 üũ
        // ������, ��� ������ �ѷ���, ��� ��������, ��� ������ ��������, �󸶸�ŭ�� ���̷� ����� ����
        return Physics.SphereCast(transform.position + Vector3.up * 1, 0.5f, Vector3.down, out hit, 0.6f);
    }

    /*
    private void OnWalk(InputValue value)
    {
        isWalking = value.isPressed;

        //anim.SetBool("YSpeed", isWalking);
    }
    */

}
