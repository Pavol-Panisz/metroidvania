using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    //cached components
    public Animator anim;
    public Image img;

    private void Start() {
        img = GetComponent<Image>();
        img.enabled = true;

        anim = GetComponent<Animator>();

        FadeOut();
    }

    public void FadeIn() {
        anim.SetTrigger("trigFadeIn");
    }

    public void FadeOut() {
        anim.SetTrigger("trigFadeOut");
    }
}
