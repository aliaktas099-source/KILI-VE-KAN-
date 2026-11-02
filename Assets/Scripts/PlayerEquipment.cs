using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Karakterin belirli bir yöndeki (Ön, Arka, Sol, Sað) ekipman parçalarýný tutan yapý.
/// Baþ, gövde, silah vb. Transform referanslarýný depolamak için kullanýlýr.
/// </summary>
[System.Serializable]
public struct EquipmentDir
{
    public Transform 
        Head,      // Karakterin baþý
        Body,      // Karakterin gövdesi
        LeftArm,   // Sol kol
        RightArm,  // Sað kol
        LeftLeg,   // Sol bacak
        RightLeg,  // Sað bacak
        LeftWeapon,   // Sol elde tutacaðý silah (kalkan vb.)
        RightWeapon;  // Sað elde tutacaðý silah

    // Yapýnýn tüm alanlarýný baþlatan constructor
    public EquipmentDir(Transform head, Transform body, Transform leftArm, Transform rightArm, Transform leftLeg, Transform rightLeg, Transform leftWeapon, Transform rightWeapon)
    {
        Head = head;
        Body = body;
        LeftArm = leftArm;
        RightArm = rightArm;
        LeftLeg = leftLeg;
        RightLeg = rightLeg;
        LeftWeapon = leftWeapon;
        RightWeapon = rightWeapon;
    }
}

/// <summary>
/// Karakterin tüm yönlerdeki ekipmanlarýný tutan yapý.
/// (Ön, Arka, Sol ve Sað görünüþleri depolamak için kullanýlýr)
/// </summary>
[System.Serializable]
public struct Equipments
{
    public EquipmentDir Front;  // Karakterin ön taraftan görünüþü
    public EquipmentDir Back;   // Karakterin arka taraftan görünüþü
    public EquipmentDir Left;   // Karakterin sol taraftan görünüþü
    public EquipmentDir Right;  // Karakterin sað taraftan görünüþü

    // Dört yönün tamamýný baþlatan constructor
    public Equipments(EquipmentDir front, EquipmentDir back, EquipmentDir left, EquipmentDir right)
    {
        Front = front;
        Back = back;
        Left = left;
        Right = right;
    }
}

/// <summary>
/// Oyuncunun zýrh, silah ve kalkan ekipmanlarýný yöneten sýnýf.
/// PlayerPrefs'ten kaydedilen seviyelere göre doðru sprite'larý yükler.
/// </summary>
public class PlayerEquipment : MonoBehaviour
{
    /// <summary>
    /// Karakterin tüm vücut parçalarý ve yönleri için sprite renderer'larý tutan yapý
    /// </summary>
    [SerializeField] private Equipments equipments;

    /// <summary>
    /// Zýrh dokularý listesi: [0] = Basic, [1] = Normal, [2] = Legend
    /// Resources klasöründen yüklenmek için düzenlenmiþ Texture2D'ler
    /// </summary>
    [SerializeField] private List<Texture2D> armorTextures;

    /// <summary>
    /// Silah dokularý listesi: [0] = Basic, [1] = Normal, [2] = Legend
    /// </summary>
    [SerializeField] private List<Texture2D> weaponTextures;

    /// <summary>
    /// Kalkan dokularý listesi: [0] = Basic, [1] = Normal, [2] = Legend
    /// </summary>
    [SerializeField] private List<Texture2D> shieldTextures;

    /// <summary>
    /// Ön taraftan görünen kalkan sprite renderer'larý (sol ve sað)
    /// </summary>
    [SerializeField] private SpriteRenderer shieldFrontLeft, shieldFrontRight;

    /// <summary>
    /// Oyun baþladýðýnda çalýþan metod.
    /// PlayerPrefs'ten kaydedilen zýrh, silah ve kalkan seviyelerini yükler.
    /// </summary>
    private void Start()
    {
        // PlayerPrefs'ten kaydedilen deðerleri oku ve ilgili yükleme metotlarýný çaðýr
        LoadArmor(PlayerPrefs.GetString("Armor"));
        LoadWeapon(PlayerPrefs.GetString("Weapon"));
        LoadShield(PlayerPrefs.GetString("Shield"));
    }

    /// <summary>
    /// Belirtilen seviyede zýrhý yükler ve tüm karakter parçalarýna sprite atar.
    /// </summary>
    /// <param name="ArmorLevel">Zýrh seviyesi: "basic", "normal" veya "legend"</param>
    internal void LoadArmor(string ArmorLevel)
    {
        // Yüklenecek tüm sprite'larý depolamak için liste oluþtur
        List<Sprite> sprites = new List<Sprite>();
        string ArmorSpriteName = "";

        // Seçilen seviyeye göre dokutexture seç
        if (ArmorLevel == "basic") ArmorSpriteName = armorTextures[0].name;
        if (ArmorLevel == "normal") ArmorSpriteName = armorTextures[1].name;
        if (ArmorLevel == "legend") ArmorSpriteName = armorTextures[2].name;

        // Seçilen dokudaki TÜM sprite'larý yükle ve listeye ekle
        sprites.AddRange(Resources.LoadAll<Sprite>(ArmorSpriteName));

        // Yüklenen her bir sprite'ý adýna göre sýfýfla ve doðru konuma ata
        foreach (Sprite sprite in sprites)
        {
            switch (sprite.name)
            {
                // ===== UYARI: ÖN TARAFTAN GÖRÜNEN PARÇALAR =====
                case "FrontHead":
                    equipments.Front.Head.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "FrontBody":
                    equipments.Front.Body.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "FrontArmL":
                    equipments.Front.LeftArm.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "FrontArmR":
                    equipments.Front.RightArm.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "FrontLegL":
                    equipments.Front.LeftLeg.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "FrontLegR":
                    equipments.Front.RightLeg.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;

                // ===== ARKA TARAFTAN GÖRÜNEN PARÇALAR =====
                case "BackHead":
                    equipments.Back.Head.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "BackBody":
                    equipments.Back.Body.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "BackArmL":
                    equipments.Back.LeftArm.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "BackArmR":
                    equipments.Back.RightArm.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "BackLegL":
                    equipments.Back.LeftLeg.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "BackLegR":
                    equipments.Back.RightLeg.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;

                // ===== SOL VE SAÐ TARAFTAN GÖRÜNEN PARÇALAR =====
                // Not: Sol ve sað spriteleri aynýdýr, bu yüzden ikisine de ayný sprite atanýr
                case "LeftHead":
                    equipments.Left.Head.GetComponent<SpriteRenderer>().sprite =
                    equipments.Right.Head.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "LeftBody":
                    equipments.Left.Body.GetComponent<SpriteRenderer>().sprite =
                    equipments.Right.Body.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "LeftArmL":
                    equipments.Left.LeftArm.GetComponent<SpriteRenderer>().sprite =
                    equipments.Right.LeftArm.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "LeftArmR":
                    equipments.Left.RightArm.GetComponent<SpriteRenderer>().sprite =
                    equipments.Right.RightArm.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "LeftLegL":
                    equipments.Left.LeftLeg.GetComponent<SpriteRenderer>().sprite =
                    equipments.Right.LeftLeg.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                case "LeftLegR":
                    equipments.Left.RightLeg.GetComponent<SpriteRenderer>().sprite =
                    equipments.Right.RightLeg.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
            }
        }
    }

    /// <summary>
    /// Belirtilen seviyede silahý yükler ve karakterin tüm yönlerine atar.
    /// </summary>
    /// <param name="WeaponLevel">Silah seviyesi: "basic", "normal" veya "legend"</param>
    internal void LoadWeapon(string WeaponLevel)
    {
        // Yüklenecek sprite'ý tutmak için deðiþken oluþtur
        Sprite sprite = null;
        string WeaponSpriteName = "";

        // Seçilen seviyeye göre silah dokusunu seç
        if (WeaponLevel == "basic") WeaponSpriteName = weaponTextures[0].name;
        if (WeaponLevel == "normal") WeaponSpriteName = weaponTextures[1].name;
        if (WeaponLevel == "legend") WeaponSpriteName = weaponTextures[2].name;

        // Seçilen dokudaki sprite'ý yükle
        sprite = Resources.Load<Sprite>(WeaponSpriteName);

        // Yüklenen silah sprite'ýný karakterin tüm yönlerindeki sað eline ata
        equipments.Front.RightWeapon.GetComponent<SpriteRenderer>().sprite =
        equipments.Back.RightWeapon.GetComponent<SpriteRenderer>().sprite =
        equipments.Left.RightWeapon.GetComponent<SpriteRenderer>().sprite =
        equipments.Right.RightWeapon.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    /// <summary>
    /// Belirtilen seviyede kalkaný yükler ve karakterin tüm yönlerine atar.
    /// Ayrýca ön taraftan görünen kalkanlarý da günceller.
    /// </summary>
    /// <param name="ShieldLevel">Kalkan seviyesi: "basic", "normal" veya "legend"</param>
    internal void LoadShield(string ShieldLevel)
    {
        // Yüklenecek sprite'ý tutmak için deðiþken oluþtur
        Sprite sprite = null;
        string ShieldSpriteName = "";

        // Seçilen seviyeye göre kalkan dokusunu seç
        if (ShieldLevel == "basic") ShieldSpriteName = shieldTextures[0].name;
        if (ShieldLevel == "normal") ShieldSpriteName = shieldTextures[1].name;
        if (ShieldLevel == "legend") ShieldSpriteName = shieldTextures[2].name;

        // Seçilen dokudaki sprite'ý yükle
        sprite = Resources.Load<Sprite>(ShieldSpriteName);

        // Yüklenen kalkan sprite'ýný karakterin tüm yönlerindeki sol eline ata
        equipments.Front.LeftWeapon.GetComponent<SpriteRenderer>().sprite =
        equipments.Back.LeftWeapon.GetComponent<SpriteRenderer>().sprite =
        equipments.Left.LeftWeapon.GetComponent<SpriteRenderer>().sprite =
        equipments.Right.LeftWeapon.GetComponent<SpriteRenderer>().sprite = sprite;

        // Ön taraftan görünen kalkanlarý da güncelle
        shieldFrontLeft.sprite = shieldFrontRight.sprite = sprite;
    }
}
