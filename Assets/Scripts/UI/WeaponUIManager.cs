using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponUIManager : MonoBehaviour
{
    public Image normalIcon;
    public Image explosiveIcon;
    public Image laserIcon;

    public Sprite normalWeaponActive;
    public Sprite explosiveWeaponActive;
    public Sprite laserWeaponActive;

    public Image weaponImage; // Image de l'arme � droite
    public Sprite weaponOverheatImage; // Image pour l'overheat
    public Sprite normalWeaponImage; // Image normale de l'arme

    private Weapon weapon;

    private Sprite normalWeaponInactive;
    private Sprite explosiveWeaponInactive;
    private Sprite laserWeaponInactive;

    private bool isBlinking = false;

    private void Start()
    {
        weapon = FindObjectOfType<Weapon>();

        // Stocker les sprites inactifs
        normalWeaponInactive = normalIcon.sprite;
        explosiveWeaponInactive = explosiveIcon.sprite;
        laserWeaponInactive = laserIcon.sprite;

        // D�finir l'image par d�faut de l'arme
        weaponImage.sprite = normalWeaponImage;
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
                break;
            case FireMode.Explosive:
                explosiveIcon.sprite = explosiveWeaponActive;
                SetIconOpacity(explosiveIcon, 1f);
                break;
            case FireMode.Laser:
                laserIcon.sprite = laserWeaponActive;
                SetIconOpacity(laserIcon, 1f);
                break;
        }

        // Si pas en surchauffe, d�finir l'image de l'arme normale
        if (!isBlinking)
        {
            weaponImage.sprite = normalWeaponImage;
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
            weaponImage.sprite = normalWeaponImage; // R�initialiser � l'image de l'arme normale
        }
    }

    private IEnumerator BlinkWeaponImageSmooth()
    {
        isBlinking = true;
        float duration = 0.5f;
        weaponImage.sprite = weaponOverheatImage; // D�finir l'image de surchauffe
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
}
