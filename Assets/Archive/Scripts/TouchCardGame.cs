using UnityEngine;
using TMPro;
using System.Collections;

public class TouchCardGame : MonoBehaviour
{
    public TMP_Text topText;       // the big number on top
    public GameObject[] cards;     // your cards
    public TMP_Text[] cardTexts;   // the text on each card (same order)
    public float flySpeed = 15f;

    int top;
    char[] ops = { '+', '-', '*', '/' };
    char[] cardOp;
    int[] cardNum;
    bool[] used;
    Collider2D[] cardColliders;
    Collider2D player;
    bool won = false;

    void Start()
    {
        // find the player by its "Player" tag
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();

        int n = cards.Length;
        cardOp = new char[n];
        cardNum = new int[n];
        used = new bool[n];
        cardColliders = new Collider2D[n];

        for (int i = 0; i < n; i++)
        {
            cardColliders[i] = cards[i].GetComponent<Collider2D>();
        }

        MakeLevel();

        // show everything
        topText.text = top.ToString();
        for (int i = 0; i < n; i++)
        {
            char symbol = cardOp[i];
            if (symbol == '/') symbol = '÷';
            cardTexts[i].text = symbol + "" + cardNum[i];
        }
    }

    // Makes random cards and a top number that needs at least 3 cards
    void MakeLevel()
    {
        int n = cards.Length;
        int tries = 0;

        while (tries < 1000)
        {
            tries++;

            // 1. random cards
            for (int i = 0; i < n; i++)
            {
                cardOp[i] = ops[Random.Range(0, 4)];
                cardNum[i] = Random.Range(1, 10);
            }

            // 2. start at 0 and undo 3 cards backwards
            top = 0;
            int start = Random.Range(0, n);
            for (int k = 0; k < 3; k++)
            {
                int i = (start + k) % n;
                if (cardOp[i] == '+') top -= cardNum[i];
                if (cardOp[i] == '-') top += cardNum[i];
                if (cardOp[i] == '/') top *= cardNum[i];
                if (cardOp[i] == '*' && top % cardNum[i] == 0) top /= cardNum[i];
            }

            // 3. top number must be between 1 and 99
            if (top <= 0 || top > 99) continue;

            // 4. too easy if 1 or 2 cards can already make 0, try again
            if (CanSolve(top, 2)) continue;

            // 5. safety: it must still be solvable with all the cards
            if (!CanSolve(top, n)) continue;

            break;   // this level is good
        }
    }

    // Can we reach 0 from this number using at most "stepsLeft" cards?
    bool CanSolve(int number, int stepsLeft)
    {
        if (number == 0) return true;        // reached 0
        if (stepsLeft == 0) return false;    // out of cards to use

        for (int i = 0; i < cards.Length; i++)
        {
            if (used[i]) continue;

            int result = ApplyCard(number, i);
            if (result == int.MinValue) continue;   // bad division, skip

            // pretend we use this card, then check what is left
            used[i] = true;
            bool solved = CanSolve(result, stepsLeft - 1);
            used[i] = false;

            if (solved) return true;
        }

        return false;
    }

    // Does the math of one card on a number
    int ApplyCard(int number, int i)
    {
        char op = cardOp[i];
        int n = cardNum[i];

        if (op == '+') return number + n;
        if (op == '-') return number - n;
        if (op == '*') return number * n;

        // division only works if it divides evenly
        if (number % n != 0) return int.MinValue;
        return number / n;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;   // tutorial is open
        if (won) return;

        // check every card that is still there
        for (int i = 0; i < cards.Length; i++)
        {
            if (used[i]) continue;

            if (cardColliders[i].IsTouching(player))
            {
                UseCard(i);
                if (won) break;
            }
        }
    }

    void UseCard(int i)
    {
        int result = ApplyCard(top, i);

        // can't divide evenly, so the card stays and nothing happens
        if (result == int.MinValue) return;

        top = result;
        used[i] = true;
        cardColliders[i].enabled = false;   // can't touch it again

        StartCoroutine(Fly(cards[i].transform));
        topText.text = top.ToString();

        if (top == 0)
        {
            won = true;
            Debug.Log("You win!");
        }
    }

    // the usual fly away animation
    IEnumerator Fly(Transform card)
    {
        while (card.localScale.x > 0.05f)
        {
            card.position += Vector3.up * flySpeed * Time.deltaTime;
            card.Rotate(0, 0, 360f * Time.deltaTime);
            card.localScale = Vector3.Lerp(card.localScale, Vector3.zero, Time.deltaTime * 4f);
            yield return null;
        }

        card.gameObject.SetActive(false);
    }
}