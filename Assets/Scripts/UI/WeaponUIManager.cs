using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class WeaponUIManager : MonoBehaviour
{
    public Image normalIcon;
    public Image explosiveIcon;
    //public Image laserIcon;

    public Sprite normalWeaponActive; // Sprite pour l'icône active du mode Normal
    public Sprite explosiveWeaponActive; // Sprite pour l'icône active du mode Explosive
    //public Sprite laserWeaponActive; // Sprite pour l'icône active du mode Laser

    public Image normalWeaponImage; // Image normale de l'arme
    public Image explosiveWeaponImage; // Image pour l'arme explosive
    //public Image laserWeaponImage; // Image pour l'arme laser
    public Image explosiveChargeGainImage;
    public Transform startTransform;
    public Transform targetTransform;
    private Vector3 initialPosition;

    private Weapon weapon;

    private Sprite normalWeaponInactive; // Sprite pour l'icône inactive du mode Normal
    private Sprite explosiveWeaponInactive; // Sprite pour l'icône inactive du mode Explosive
    //private Sprite laserWeaponInactive; // Sprite pour l'icône inactive du mode Laser

    private bool isBlinking = false;
    //public TMP_Text laserCooldownText;

    private void Start()
    {
        weapon = FindObjectOfType<Weapon>();

        // Stocker les sprites inactifs
        normalWeaponInactive = normalIcon.sprite;
        explosiveWeaponInactive = explosiveIcon.sprite;
        //laserWeaponInactive = laserIcon.sprite;

        // Masquer toutes les images de l'arme au départ
        normalWeaponImage.gameObject.SetActive(false);
        explosiveWeaponImage.gameObject.SetActive(false);
        //laserWeaponImage.gameObject.SetActive(false);

        // Afficher l'image de l'arme par défaut
        normalWeaponImage.gameObject.SetActive(true);

        initialPosition = explosiveChargeGainImage.rectTransform.localPosition;
    }

    private void Update()
    {
        UpdateIcons();
        CheckOverheat();
    }

    private void UpdateIcons()
    {
        // Réinitialiser les icônes aux images inactives et définir l'opacité à moitié
        if (normalIcon != null)
        {
            normalIcon.sprite = normalWeaponInactive;
            SetIconOpacity(normalIcon, 0.5f);
        }
        if (explosiveIcon != null)
        {
            explosiveIcon.sprite = explosiveWeaponInactive;
            SetIconOpacity(explosiveIcon, 0.5f);
        }
        /*if (laserIcon != null)
        {
            laserIcon.sprite = laserWeaponInactive;
            SetIconOpacity(laserIcon, 0.5f);
        }*/

        // Masquer toutes les images d'arme au début
        if (normalWeaponImage != null)
            normalWeaponImage.gameObject.SetActive(false);
        if (explosiveWeaponImage != null)
            explosiveWeaponImage.gameObject.SetActive(false);
        /*if (laserWeaponImage != null)
            laserWeaponImage.gameObject.SetActive(false);*/

        // Mettre en évidence l'icône du mode actuel et changer le sprite en image active
        switch (weapon.currentMode)
        {
            case FireMode.Normal:
                if (normalIcon != null)
                {
                    normalIcon.sprite = normalWeaponActive;
                    SetIconOpacity(normalIcon, 1f);
                }
                if (normalWeaponImage != null)
                    normalWeaponImage.gameObject.SetActive(true); // Afficher l'image de l'arme pour le mode Normal
                break;
            case FireMode.Explosive:
                if (explosiveIcon != null)
                {
                    explosiveIcon.sprite = explosiveWeaponActive;
                    SetIconOpacity(explosiveIcon, 1f);
                }
                if (explosiveWeaponImage != null)
                    explosiveWeaponImage.gameObject.SetActive(true); // Afficher l'image de l'arme pour le mode Explosive
                break;
            /*case FireMode.Laser:
                if (laserIcon != null)
                {
                    laserIcon.sprite = laserWeaponActive;
                    SetIconOpacity(laserIcon, 1f);
                }
                if (laserWeaponImage != null)
                    laserWeaponImage.gameObject.SetActive(true); // Afficher l'image de l'arme pour le mode Laser
                break;*/
        }

        // Si pas en surchauffe, définir l'image de l'arme normale (pour réinitialiser après une surchauffe)
        if (!isBlinking)
        {
            switch (weapon.currentMode)
            {
                case FireMode.Normal:
                    if (normalWeaponImage != null)
                        normalWeaponImage.gameObject.SetActive(true);
                    break;
                case FireMode.Explosive:
                    if (explosiveWeaponImage != null)
                        explosiveWeaponImage.gameObject.SetActive(true);
                    break;
               /* case FireMode.Laser:
                    if (laserWeaponImage != null)
                        laserWeaponImage.gameObject.SetActive(true);
                    break;*/
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
            ResetWeaponImagesOpacity();

            // Réinitialiser l'image de l'arme en fonction du mode actuel
            switch (weapon.currentMode)
            {
                case FireMode.Normal:
                    normalWeaponImage.gameObject.SetActive(true);
                    break;
                case FireMode.Explosive:
                    explosiveWeaponImage.gameObject.SetActive(true);
                    break;
                /*case FireMode.Laser:
                    laserWeaponImage.gameObject.SetActive(true);
                    break;*/
            }
        }
    }

    private void ResetWeaponImagesOpacity()
    {
        SetIconOpacity(normalWeaponImage, 1f);
        SetIconOpacity(explosiveWeaponImage, 1f);
        //SetIconOpacity(laserWeaponImage, 1f);
    }

    private IEnumerator BlinkWeaponImageSmooth()
    {
        isBlinking = true;
        float duration = 0.5f;
        while (weapon.overheated)
        {
            // Atténuer
            yield return StartCoroutine(FadeWeaponImage(1f, 0.15f, duration));
            // Réapparaître
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
            SetIconOpacity(normalWeaponImage, newOpacity);
            yield return null;
        }
        SetIconOpacity(normalWeaponImage, endOpacity);
    }

    public void ShowExplosiveChargeGain()
    {
        StartCoroutine(DisplayExplosiveChargeGain());
    }

    private IEnumerator DisplayExplosiveChargeGain()
    {
        explosiveChargeGainImage.gameObject.SetActive(true); // Activer l'image
        explosiveChargeGainImage.rectTransform.localPosition = startTransform.localPosition; // Assurez-vous qu'il commence à partir de la position de départ
        yield return SlideImageToTarget(explosiveChargeGainImage, startTransform, targetTransform, 0.5f); // Glisser l'image vers la cible (ajustez la durée selon vos besoins)
        yield return FadeOutImage(explosiveChargeGainImage, 1f); // Faire disparaître l'image progressivement
        SetIconOpacity(explosiveChargeGainImage, 1f); // Réinitialiser l'opacité
    }

    private IEnumerator FadeOutImage(Image image, float duration)
    {
        float startOpacity = image.color.a;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newOpacity = Mathf.Lerp(startOpacity, 0f, elapsed / duration);
            SetIconOpacity(image, newOpacity);
            yield return null;
        }
        SetIconOpacity(image, 0f);
        image.gameObject.SetActive(false); // Désactiver l'image une fois le fade out terminé
        image.rectTransform.localPosition = startTransform.localPosition; // Remettre à la position initiale
    }

    private IEnumerator SlideImageToTarget(Image image, Transform start, Transform target, float duration)
    {
        Vector3 startPosition = start.localPosition;
        Vector3 endPosition = target.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            image.rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        image.rectTransform.localPosition = endPosition;
    }

    /*public void DisplayLaserCooldownText(float cooldownDuration)
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
    }*/
}
