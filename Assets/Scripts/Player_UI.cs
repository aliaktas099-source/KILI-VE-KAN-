using UnityEngine;
using UnityEngine.UI;
[DefaultExecutionOrder(-1)]
public class Player_UI : MonoBehaviour
{
    public GameObject GameOverCanvas;
    public Image healthBar;
    [Header("Health Bar Animation")]
    [SerializeField] private float smoothSpeed = 5f;
    
    private Health_System healthSystem;
   
    private float x=1f;

    void Awake()
    {
        healthSystem = GetComponent<Health_System>();
        x = healthBar.transform.localScale.x;
    }

    private void OnEnable()
    {
        healthSystem.OnDied += OnGameOver;
        healthSystem.OnHealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        healthSystem.OnDied -= OnGameOver;
        healthSystem.OnHealthChanged -= OnHealthChanged;
    }

    private void Update()
    {
       float smoothX = Mathf.Lerp(healthBar.transform.localScale.x, x, smoothSpeed * Time.deltaTime);
        healthBar.transform.localScale = new Vector3(smoothX, 1f, 1f);
    }

    void OnGameOver()
    {
        GameOverCanvas.SetActive(true);
    }

    void OnHealthChanged(float cur, float max)
    {
        x = cur / max;
    }
}
