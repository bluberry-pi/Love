using System.Collections;
using UnityEngine;

public class DeckSwapper : MonoBehaviour
{
    public Transform[] cards;           // drag your 5 cards here
    public CardChanger changer;         // drag the CardChanger object here

    public float dropTime = 0.3f;       // time to fall away
    public float riseTime = 0.3f;       // time to come back up
    public float dropDistance = 8f;     // how far down they fall (world units)
    public float spread = 1.5f;         // how far apart they split sideways
    public float spin = 25f;            // extra tilt while falling

    public bool IsBusy;                 // ignore card clicks while this is true

    // Call this when the countdown hits 0
    public void ReplaceDeck()
    {
        if (!IsBusy) StartCoroutine(SwapRoutine());
    }

    IEnumerator SwapRoutine()
    {
        IsBusy = true;

        int count = cards.Length;
        Vector3[] homePos = new Vector3[count];
        Quaternion[] homeRot = new Quaternion[count];
        Vector3[] awayPos = new Vector3[count];
        Quaternion[] awayRot = new Quaternion[count];

        float middle = (count - 1) / 2f;

        for (int i = 0; i < count; i++)
        {
            homePos[i] = cards[i].position;
            homeRot[i] = cards[i].rotation;

            float side = i - middle; // negative = left cards, positive = right cards
            awayPos[i] = homePos[i] + new Vector3(side * spread, -dropDistance, 0f);
            awayRot[i] = homeRot[i] * Quaternion.Euler(0f, 0f, -side * spin);
        }

        // 1) Cards fall down and split apart
        yield return Move(homePos, homeRot, awayPos, awayRot, dropTime);

        // 2) Cards are off screen: give them new values
        changer.GiveNewCards();

        // 3) New cards rise back into the hand
        yield return Move(awayPos, awayRot, homePos, homeRot, riseTime);

        IsBusy = false;
    }

    IEnumerator Move(Vector3[] fromPos, Quaternion[] fromRot, Vector3[] toPos, Quaternion[] toRot, float time)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / time;
            float e = Mathf.SmoothStep(0f, 1f, t);

            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].position = Vector3.Lerp(fromPos[i], toPos[i], e);
                cards[i].rotation = Quaternion.Lerp(fromRot[i], toRot[i], e);
            }
            yield return null;
        }
    }
}