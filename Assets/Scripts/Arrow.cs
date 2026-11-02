using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float travelDuration = 2f;
    [SerializeField] private AnimationCurve arcCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);
    [SerializeField] private float maxArcHeight = 2f;
    [SerializeField] private GameObject vfx;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float elapsedTime;
    private bool hasHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;
        SoundController.Instance.PlayHitSound();
        hasHit = true;
        if (collision.CompareTag("Player"))
        {
            Health_System playerHealth = collision.GetComponent<Health_System>();
            
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else
        {
            GameObject vfx = Instantiate(this.vfx, transform.position, Quaternion.identity);
            Destroy(vfx, 1f);
            Destroy(gameObject, 2);
        }
    }

    public void Initialize(Vector3 start, Vector3 target)
    {
        startPosition = start;
        targetPosition = target;
        elapsedTime = 0f;
        hasHit = false;
        SoundController.Instance.PlayArrowFlySound();
    }

    private void Update()
    {
        if (hasHit) return;

        elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedTime / travelDuration);

        // Doðrusal hareket (X ve Z eksenleri)
        Vector3 horizontalPosition = Vector3.Lerp(startPosition, targetPosition, progress);

        // Parabolik hareket (Y ekseni) - yükselip düþer
        float arcHeight = arcCurve.Evaluate(progress) * maxArcHeight;
        Vector3 newPosition = new Vector3(horizontalPosition.x, horizontalPosition.y + arcHeight, horizontalPosition.z);

        // Ok ucunun rotasyonunu harekete göre ayarla
        Vector3 direction = (newPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }

        transform.position = newPosition;

        // Hedefine ulaþtýnca yok et
        if (progress >= 1f)
        {
            hasHit = true;
            SoundController.Instance.PlayHitSound();
            
            GameObject vfx = Instantiate(this.vfx, transform.position, Quaternion.identity);
            Destroy(vfx, 1f);
            Destroy(gameObject, 2);
        }
    }
}