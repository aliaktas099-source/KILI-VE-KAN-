using UnityEngine;

[System.Serializable]
public enum Direction
{
    Front,
    Back,
    Left,
    Right
}

[System.Serializable]
public struct PlayerDirection
{
    public Transform Front;
    public Transform Back;
    public Transform Left;
    public Transform Right;

    public PlayerDirection(Transform front, Transform back, Transform left, Transform right)
    {
        Front = front;
        Back = back;
        Left = left;
        Right = right;
    }
}

public class Character_Controller : MonoBehaviour
{

    [SerializeField] private Direction currentDirection;
    [SerializeField] private PlayerDirection playerDirection;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform attackPoint;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private UnityEngine.Events.UnityAction<Direction> onDirectionChanged;
    private bool isAttacking = false;
    private bool isBlocking = false;

    public const string _horizontal = "Horizontal";
    public const string _vertical = "Vertical";

    [Header("Mobile Controls")]
    [SerializeField] private FixedJoystick joystick;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        onDirectionChanged += OnDirectionChanged;
        onDirectionChanged?.Invoke(Direction.Front);
        GetComponent<Health_System>().OnDied += () => rb.bodyType = RigidbodyType2D.Static;
    }

    private void OnDisable()
    {
        onDirectionChanged -= OnDirectionChanged;
        GetComponent<Health_System>().OnDied -= () => rb.bodyType = RigidbodyType2D.Static;
    }

    private void Update()
    {
        if (rb.bodyType == RigidbodyType2D.Static) return;
#if !UNITY_ANDROID
        movement = new Vector2(
            Input.GetAxisRaw(_horizontal),
            Input.GetAxisRaw(_vertical)
        ).normalized;

#else
        movement = new Vector2(
           joystick.Direction.normalized.x,
           joystick.Direction.normalized.y
       ).normalized;
#endif
        if (movement != Vector2.zero)
        {
            animator.SetBool("isMoving", true);
            SoundController.Instance.PlayWalkSound();

            UpdatePlayerDirection(movement);
        }
        else animator.SetBool("isMoving", false);

#if !UNITY_ANDROID
        if (Input.GetMouseButtonDown(0)) OnBeginAttack();
        if (Input.GetMouseButtonDown(1)) OnBlock(true);
        if (Input.GetMouseButtonUp(1)) OnBlock(false);
#endif
    }

    private void FixedUpdate()
    {
        if (rb.bodyType == RigidbodyType2D.Static) return;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdatePlayerDirection(Vector2 movement)
    {
        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            if (movement.x > 0)
            {
                if (currentDirection != Direction.Right)
                {
                    currentDirection = Direction.Right;
                    onDirectionChanged?.Invoke(currentDirection);
                }
            }
            else
            {
                if (currentDirection != Direction.Left)
                {
                    currentDirection = Direction.Left;
                    onDirectionChanged?.Invoke(currentDirection);
                }
            }
        }
        else
        {
            if (movement.y < 0)
            {
                if (currentDirection != Direction.Front)
                {
                    currentDirection = Direction.Front;
                    onDirectionChanged?.Invoke(currentDirection);
                }
            }
            else
            {
                if (currentDirection != Direction.Back)
                {
                    currentDirection = Direction.Back;
                    onDirectionChanged?.Invoke(currentDirection);
                }
            }
        }
    }

    private void OnDirectionChanged(Direction newDirection)
    {
        if (newDirection == Direction.Left)
        {
            playerDirection.Left.gameObject.SetActive(true);
            playerDirection.Right.gameObject.SetActive(false);
            playerDirection.Front.gameObject.SetActive(false);
            playerDirection.Back.gameObject.SetActive(false);
            attackPoint.localPosition = new Vector3(-0.75f, 0.75f, 0f);
        }
        if (newDirection == Direction.Right)
        {
            playerDirection.Left.gameObject.SetActive(false);
            playerDirection.Right.gameObject.SetActive(true);
            playerDirection.Front.gameObject.SetActive(false);
            playerDirection.Back.gameObject.SetActive(false);
            attackPoint.localPosition = new Vector3(0.75f, 0.75f, 0f);
        }
        if (newDirection == Direction.Front)
        {
            playerDirection.Left.gameObject.SetActive(false);
            playerDirection.Right.gameObject.SetActive(false);
            playerDirection.Front.gameObject.SetActive(true);
            playerDirection.Back.gameObject.SetActive(false);
            attackPoint.localPosition = new Vector3(0, -0.5f, 0f);
        }
        if (newDirection == Direction.Back)
        {
            playerDirection.Left.gameObject.SetActive(false);
            playerDirection.Right.gameObject.SetActive(false);
            playerDirection.Front.gameObject.SetActive(false);
            playerDirection.Back.gameObject.SetActive(true);
            attackPoint.localPosition = new Vector3(0, 0.5f, 0f);
        }
    }
    private void OnBeginAttack()
    {
        if (!isBlocking && !isAttacking)
        {
            animator.SetTrigger("isAttacking");
            isAttacking = true;

            // Saldýrý sesini çal
            SoundController.Instance.PlayAttackSound();
        }
    }
    private void OnAttack()
    {
        if (isAttacking)
        {
            RaycastHit2D[] enemys = Physics2D.CircleCastAll(attackPoint.position, 0.75f, Vector2.zero);
            if (enemys.Length > 0)
            {
                foreach (RaycastHit2D enemy in enemys)
                {
                    if (enemy.collider.CompareTag("Enemy") &&
                        enemy.collider.TryGetComponent(out Health_System healthSystem))
                    {
                        healthSystem?.TakeDamage(10f);

                        // Vuruþ sesini çal
                        SoundController.Instance.PlayHitSound();

                        Debug.Log("Enemy Hit: " + enemy.collider.name);
                    }
                }
            }
            isAttacking = false;
        }
    }

    private void OnBlock(bool block)
    {
        isBlocking = block;
        if (block) isAttacking = !block;
        animator?.SetBool("isBlocking", isBlocking);
    }
#if UNITY_ANDROID
    public void MobileBlockButton(bool block) => OnBlock(block);
    public void MobileAttackButton() => OnBeginAttack();
#endif
}
