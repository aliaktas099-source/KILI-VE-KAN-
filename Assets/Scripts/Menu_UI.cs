using UnityEngine;
[System.Serializable]

public class Menu_UI : MonoBehaviour
{
    [Header("OBJ")]
    [SerializeField] private Transform Castle;
    [SerializeField] private Transform Shop;

    [SerializeField] private GameObject CastlePanel;
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private AudioSource clickSound;
    private PlayerEquipment playerEquipment;

    private Camera mainCamera;
    private bool isPanelActive = false;
    void Awake()
    {
        playerEquipment = FindAnyObjectByType<PlayerEquipment>();
        mainCamera = Camera.main;
        if (!PlayerPrefs.HasKey("Armor")) PlayerPrefs.SetString("Armor", "basic");
        if (!PlayerPrefs.HasKey("Weapon")) PlayerPrefs.SetString("Weapon", "basic");
        if (!PlayerPrefs.HasKey("Shield")) PlayerPrefs.SetString("Shield", "basic");
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && !isPanelActive) HandleMouseClick();
    }

    private void HandleMouseClick()
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 clickPos = new Vector2(worldPos.x, worldPos.y);

        RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);
       
        if (hit.collider != null)
        {
            if (hit.transform == Castle) ActivatePanel(CastlePanel);
            if (hit.transform == Shop) ActivatePanel(ShopPanel);
        }
    }

    private void ActivatePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
            isPanelActive = true;
            clickSound.PlayOneShot(clickSound.clip);
        }
    }

    public void DeactivatePanel(GameObject panel)
    {
        panel.SetActive(false);
        isPanelActive = false;
        clickSound.PlayOneShot(clickSound.clip);
    }
    public void EquipArmor(string level)
    {
        if (string.IsNullOrEmpty(level)) return;
        PlayerPrefs.SetString("Armor", level.ToLower());
       playerEquipment.LoadArmor(level.ToLower());
    }
    public void EquipWeapon(string level)
    {
        if (string.IsNullOrEmpty(level)) return;
        PlayerPrefs.SetString("Weapon", level.ToLower());
        playerEquipment.LoadWeapon(level.ToLower());
    }
    public void EquipShield(string level)
    {
        if (string.IsNullOrEmpty(level)) return;
        PlayerPrefs.SetString("Shield", level.ToLower());
        playerEquipment.LoadShield(level.ToLower());
    }
}
