using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class WeaponUIManager : MonoBehaviour
{
    public Image normalIcon;
    public Image explosiveIcon;
    public Image laserIcon;

    public Sprite normalWeaponActive; // Sprite pour l'ic�ne active du mode Normal
    public Sprite explosiveWeaponActive; // Sprite pour l'ic�ne active du mode Explosive
    public Sprite laserWeaponActive; // Sprite pour l'ic�ne active du mode Laser

    public Image weaponImage; // Image de l'arme � droite
    public Sprite normalWeaponImage; // Image normale de l'arme
    public Sprite explosiveWeaponImage; // Image pour l'arme explosive
    public Sprite laserWeaponImage; // Image pour l'arme laser

    private Weapon weapon;

    private Sprite normalWeaponInactive; // Sprite pour l'ic�ne inactive du mode Normal
    private Sprite explosiveWeaponInactive; // Sprite pour l'ic�ne inactive du mode Explosive
    private Sprite laserWeaponInactive; // Sprite pour l'ic�ne inactive du mode Laser

    private bool isBlinking = false;
    public TMP_Text laserCooldownText;

    private void Start()
    {
        weapon = FindObjectOfType<Weapon>();

        // Stocker les sprites inactifs
        normalWeaponInactive = normalIcon.sprite;
        explosiveWeaponInactive = explosiveIcon.sprite;
        laserWeaponInactive = laserIcon.sprite;

        // D�finir l'image par d�faut de l'arme
        weaponImage.sprite = normalWeaponImage;
        Debug.Log("Normal Weapon Image: " + (normalWeaponImage != null));
        Debug.Log("Explosive Weapon Image: " + (explosiveWeaponImage != null));
        Debug.Log("Laser Weapon Image: " + (laserWeaponImage != null));
    }

    private void Update()
    {
        UpdateIcons();
        CheckOverheat();
    }

    private void UpdateIcons()
    {
        // R�initialiser les ic�nes aux images inactives et d�finir l'opacit� � moiti�
        normalIcon.sprite = normalWeaponInactive;
        explosiveIcon.sprite = explosiveWeaponInactive;
        laserIcon.sprite = laserWeaponInactive;

        SetIconOpacity(normalIcon, 0.5f);
        SetIconOpacity(explosiveIcon, 0.5f);
        SetIconOpacity(laserIcon, 0.5f);

        // Mettre en �vidence l'ic�ne du mode actuel et changer le sprite en image active
        switch (weapon.currentMode)
        {
            case FireMode.Normal:
                normalIcon.sprite = normalWeaponActive;
                SetIconOpacity(normalIcon, 1f);
                weaponImage.sprite = normalWeaponImage; // D�finir l'image de l'arme pour le mode Normal
                break;
            case FireMode.Explosive:
                explosiveIcon.sprite = explosiveWeaponActive;
                SetIconOpacity(explosiveIcon, 1f);
                weaponImage.sprite = explosiveWeaponImage; // D�finir l'image de l'arme pour le mode Explosive
                break;
            case FireMode.Laser:
                laserIcon.sprite = laserWeaponActive;
                SetIconOpacity(laserIcon, 1f);
                weaponImage.sprite = laserWeaponImage; // D�finir l'image de l'arme pour le mode Laser
                break;
        }

        // Si pas en surchauffe, d�finir l'image de l'arme normale (pour r�initialiser apr�s une surchauffe)
        if (!isBlinking)
        {
            switch (weapon.currentMode)
            {
                case FireMode.Normal:
                    weaponImage.sprite = normalWeaponImage;
                    break;
                case FireMode.Explosive:
                    weaponImage.sprite = explosiveWeaponImage;
                    break;
                case FireMode.Laser:
                    weaponImage.sprite = laserWeaponImage;
                    break;
            }
        }
    }

    private void SetIconOpacity(Image icon, float opacity)
    {
        Color color = icon.color;
        color.a = opacity;
        icon.color = color;
    }

    private void CheckOverheat()
    {
        if (weapon.overheated && !isBlinking)
        {
            StartCoroutine(BlinkWeaponImageSmooth());
        }
        else if (!weapon.overheated && isBlinking)
        {
            StopCoroutine(BlinkWeaponImageSmooth());
            isBlinking = false;
            SetIconOpacity(weaponImage, 1f); // R�initialiser l'opacit� � 1 quand pas en surchauffe
            // R�initialiser l'image de l'arme en fonction du mode actuel
            switch (weapon.currentMode)
            {
                case FireMode.Normal:
                    weaponImage.sprite = normalWeaponImage;
                    break;
                case FireMode.Explosive:
                    weaponImage.sprite = explosiveWeaponImage;
                    break;
                case FireMode.Laser:
                    weaponImage.sprite = laserWeaponImage;
                    break;
            }
        }
    }

    private IEnumerator BlinkWeaponImageSmooth()
    {
        isBlinking = true;
        float duration = 0.5f;
        while (weapon.overheated)
        {
            // Att�nuer
            yield return StartCoroutine(FadeWeaponImage(1f, 0.15f, duration));
            // R�appara�tre
            yield return StartCoroutine(FadeWeaponImage(0.15f, 1f, duration));
        }
        isBlinking = false;
    }

    private IEnumerator FadeWeaponImage(float startOpacity, float endOpacity, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newOpacity = Mathf.Lerp(startOpacity, endOpacity, elapsed / duration);
            SetIconOpacity(weaponImage, newOpacity);
            yield return null;
        }
        SetIconOpacity(weaponImage, endOpacity);
    }

    public void DisplayLaserCooldownText(float cooldownDuration)
    {
        StartCoroutine(ShowCooldownText(cooldownDuration));
    }

    private IEnumerator ShowCooldownText(float cooldownDuration)
    {
        laserCooldownText.gameObject.SetActive(true);
        laserCooldownText.text = Mathf.Ceil(cooldownDuration).ToString(); 

        float elapsedTime = 0f;
        while (elapsedTime < cooldownDuration)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            laserCooldownText.text = Mathf.Ceil(cooldownDuration - elapsedTime).ToString(); 
        }

        laserCooldownText.gameObject.SetActive(false);
    }

}
