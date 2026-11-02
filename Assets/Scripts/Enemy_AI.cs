using UnityEngine;

/// <summary>
/// Düşmanın yapay zekasını yöneten sınıf.
/// Oyuncuya doğru hareket eder, saldırı yapar ve ok fırlatır.
/// </summary>
public class Enemy_AI : MonoBehaviour
{
    [SerializeField] private GameObject[] directions; // 0: Front, 1: Back, 2: Left, 3: Right
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float attackRange = 1.0f;
    [SerializeField] private float damage = 5f;
    [Header("Arrow Settings")]
    [SerializeField] private bool isArcherEnemy = false;
    [SerializeField] private Transform arrow;
    [SerializeField] private float accuracyDeviation = 0.15f;
    public static Vector3 SPAWN_OFFSET = new Vector3(0, 0.5f, 0);
    
    private Transform player;
    private Animator animator;
    private Vector2 m_direction;
    private float distanceToPlayer;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (isArcherEnemy && arrow != null)
            animator.SetBool("isArcher", true);
        if (player == null)
        {
            Debug.LogError("Player not found in the scene. Make sure the player has the 'Player' tag.");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        GetComponent<Health_System>().OnDied += OnDied;
    }

    private void OnDisable()
    {
        GetComponent<Health_System>().OnDied -= OnDied;
    }

    private void LateUpdate()
    {
        if (rb.bodyType == RigidbodyType2D.Static) return;
        SelectDirectionEnabled(m_direction.x, m_direction.y);
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
        m_direction = (Vector2)(player.position - transform.position).normalized;
        if (distanceToPlayer >= attackRange)
            Move();
        else
            Attack();
    }

    private void Move()
    {
        transform.position += (Vector3)m_direction * speed * Time.deltaTime;

            animator.SetBool("isMoving", true);
            SoundController.Instance.PlayWalkSound();
    }

    private void Attack()
    {
        animator.SetBool("isMoving", false);
        animator.SetTrigger("isAttacking");
        
    }

    private void OnAttack()
    {
        if (isArcherEnemy) return;
        SoundController.Instance.PlayAttackSound();
        if (distanceToPlayer <= attackRange)
        {
            player.GetComponent<Health_System>().TakeDamage(damage);

            // Vuruş sesini çal
           
            SoundController.Instance.PlayHitSound();
        }
    }

    private void OnBowAttack()
    {
        if (!isArcherEnemy) return;
        
        // Oyuncuya yönelik temel yönü hesapla
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        
        // Ufak sapma ekle (hedefi %15 arasında ıskala)
        float deviationX = Random.Range(-accuracyDeviation, accuracyDeviation);
        float deviationY = Random.Range(-accuracyDeviation, accuracyDeviation);
        
        Vector3 deviatedTargetPosition = player.position + new Vector3(deviationX, deviationY, 0);

        // Ok atış sesini çal
        SoundController.Instance.PlayArrowShootSound();

        // Oku başlangıç konumundan oluştur
        Transform arrowInstance = Instantiate(arrow, transform.position + SPAWN_OFFSET, Quaternion.identity);
        
        // Arrow sınıfını bul ve başlat
        Arrow arrowScript = arrowInstance.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.Initialize(arrowInstance.position, deviatedTargetPosition);
        }
        else
        {
            Debug.LogWarning("Arrow prefab'ında Arrow script'i bulunamadı!");
        }
    }

    private void SelectDirectionEnabled(float x, float y)
    {
        GameObject dir = null;
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            if (x > 0)
                dir = directions[3];
            else
                dir = directions[2];
        }
        else
        {
            if (y > 0)
                dir = directions[1];
            else
                dir = directions[0];
        }
        
        foreach (GameObject direction in directions)
        {
            if (dir.Equals(direction))
                direction.SetActive(true);
            else 
                direction.SetActive(false);
        }
    }

    private void OnDied()
    {
        // Ölme sesini çal
        SoundController.Instance.PlayDeathSound();
        enabled = false;
    }
}
