using UnityEngine;
using TMPro;
using System.Collections;

public class CardValueFlicker : MonoBehaviour
{
    public CardGame game;            // leave empty, it finds it on the GameManager

    [Header("Timing (seconds)")]
    public float startVisibleTime = 2f;  // values readable at the start of the level
    public float hiddenTime = 3f;        // how long ALL values stay gone
    public float visibleTime = 2f;       // how long they stay readable after reappearing
    public float fadeTime = 0.3f;        // fade in/out duration

    [Header("Behaviour")]
    public bool reappear = true;         // false = values vanish after 2 sec and never come back

    void Start()
    {
        if (game == null) game = GetComponent<CardGame>();
        StartCoroutine(Cycle());
    }

    IEnumerator Cycle()
    {
        SetAllAlpha(1f);

        // 1. visible for the first 2 seconds
        yield return new WaitForSeconds(startVisibleTime);

        while (true)
        {
            // 2. ALL values fade out together
            yield return FadeAll(0f);

            if (!reappear) yield break;

            yield return new WaitForSeconds(hiddenTime);

            // 3. ALL values fade back in together
            yield return FadeAll(1f);
            yield return new WaitForSeconds(visibleTime);
        }
    }

    IEnumerator FadeAll(float target)
    {
        float start = game.cardTexts[0].alpha;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            SetAllAlpha(Mathf.Lerp(start, target, t / fadeTime));
            yield return null;
        }

        SetAllAlpha(target);
    }

    void SetAllAlpha(float a)
    {
        for (int i = 0; i < game.cardTexts.Length; i++)
        {
            game.cardTexts[i].alpha = a;
        }
    }
}