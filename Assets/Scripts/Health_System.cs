using System;
using UnityEngine;

public class Health_System : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private GameObject bloodVFX;
    [SerializeField] private GameObject blockVFX;

    private float currentHealth;
    private Animator animator;

    private bool isDead = false;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDied;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        OnDied += Died;
    }

    private void OnDisable()
    {
        OnDied -= Died;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        float actualDamage = animator.GetBool("isBlocking") ? 0 : damage;
        currentHealth -= actualDamage;

        animator?.SetTrigger("GetHit");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (actualDamage == 0)
        {
            SoundController.Instance.PlayBlockSound();
            Transform t = Instantiate(blockVFX.transform, transform.position, Quaternion.identity);
            t.position = GetComponent<Collider2D>().bounds.center;
            Destroy(t.gameObject, 1f);
        }
        else
        {
            SoundController.Instance.PlayTakeDamageSound();
            Transform t = Instantiate(bloodVFX.transform, transform.position, Quaternion.identity);
            t.position = GetComponent<Collider2D>().bounds.center;
            Destroy(t.gameObject, 1f);
        }
            
        if (currentHealth <= 0)
            OnDied?.Invoke();
    }
    private void Died()
    {
        isDead = true;
        currentHealth = 0;
        animator?.SetTrigger("OnDeath");
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        // Ölme sesini çal
        SoundController.Instance.PlayDeathSound();
    }

    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;
}