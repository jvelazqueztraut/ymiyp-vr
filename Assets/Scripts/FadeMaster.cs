using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeMaster : MonoBehaviour
{
    public Image canvas;
    public TMP_Text quote1, quote2;

    public float delay = 1.0f;
    private float delayTimer;

    public float quote1FadeTime = 1.0f, quote1DelayTime = 3.0f;
    private float quote1FadeTimer, quote1DelayTimer;
    public float  quote2FadeTime = 1.0f, quote2DelayTime = 3.0f;
    private float quote2FadeTimer, quote2DelayTimer;

    public float fadeTime = 1.0f;
    private float fadeTimer;
    private bool fadeIn = false;

    // Start is called before the first frame update
    void Start()
    {
        canvas.color = ReplaceAlpha(canvas.color, 1.0f);

        delayTimer = delay;

        fadeIn = true;
        fadeTimer = 0;

        quote1FadeTimer = quote1FadeTime;
        quote1DelayTimer = quote1DelayTime;
        quote2FadeTimer = quote2FadeTime;
        quote2DelayTimer = quote2DelayTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        if (quote1FadeTimer > 0)
        {
            quote1FadeTimer -= Time.deltaTime;
            float alpha = GetAlpha(quote1FadeTimer, quote1FadeTime, false);
            quote1.color = ReplaceAlpha(quote1.color, alpha);
        }
        else if(quote1DelayTimer > 0)
        {
            quote1DelayTimer -= Time.deltaTime;
        }
        else if(quote2FadeTimer > 0)
        {
            quote2FadeTimer -= Time.deltaTime;
            float alpha = GetAlpha(quote2FadeTimer, quote2FadeTime, false);
            quote2.color = ReplaceAlpha(quote2.color, alpha);
        }
        else if(quote2DelayTimer > 0)
        {
            quote2DelayTimer -= Time.deltaTime;

            if (quote2DelayTimer < 0) {
                fadeTimer = fadeTime;
            }
        }

        if (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
            float alpha = GetAlpha(fadeTimer, fadeTime, fadeIn);
            canvas.color = ReplaceAlpha(canvas.color, alpha);
            if (fadeIn) {
                quote1.color = ReplaceAlpha(quote1.color, alpha);
                quote2.color = ReplaceAlpha(quote2.color, alpha);
            }
        }
    }

    private float GetAlpha(float timer, float time, bool io)
    {
        float ratio = timer / time;
        return io ? ratio : 1.0f - ratio;
    }

    private Color ReplaceAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    public void FadeIn(GameObject obj)
    {
        fadeIn = true;
        fadeTimer = fadeTime;
    }

    public void FadeOut(GameObject obj)
    {
        fadeIn = false;
        fadeTimer = fadeTime;
    }

    public bool hasFinished()
    {
        return fadeTimer <= 0;
    }
}
