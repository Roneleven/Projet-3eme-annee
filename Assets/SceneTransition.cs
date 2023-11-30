using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public Image blackFade; // L'objet Image pour le panneau UI noir
    public Animator anim; // L'Animator pour l'objet Image


    public void ReloadScene()
    {
        StartCoroutine(Fading());
    }

    IEnumerator Fading()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        anim.Play("FadeOut");
        yield return new WaitUntil(() => blackFade.color.a == 0);
    }
}

