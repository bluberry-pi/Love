using UnityEngine;
using TMPro;

public class CardGame : MonoBehaviour
{
    public TMP_Text topText;       // the big number on top
    public GameObject[] cards;     // your 5 cards
    public TMP_Text[] cardTexts;   // the text on each card (same order)

    // public so CardChanger can use them
    [HideInInspector] public int top;
    [HideInInspector] public char[] cardOp;
    [HideInInspector] public int[] cardNum;
    [HideInInspector] public bool[] used;

    char[] ops = { '+', '-', '*', '/' };

    void Start()
    {
        int n = cards.Length;
        cardOp = new char[n];
        cardNum = new int[n];
        used = new bool[n];

        do
        {
            // random cards
            for (int i = 0; i < n; i++)
            {
                cardOp[i] = ops[Random.Range(0, 4)];
                cardNum[i] = Random.Range(1, 10);
            }

            // start at 0 and undo 3 cards backwards, so the top number is solvable
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
        }
        while (top <= 0 || top > 99 || SolvedByOne());

        // show everything
        topText.text = top.ToString();
        for (int i = 0; i < n; i++)
        {
            ShowCard(i);
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;   // tutorial is open, ignore clicks

        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mouse);
        if (hit == null) return;

        for (int i = 0; i < cards.Length; i++)
            if (hit.transform == cards[i].transform) UseCard(i);
    }

    // updates the text on one card
    public void ShowCard(int i)
    {
        char symbol = cardOp[i];
        if (symbol == '/') symbol = '÷';
        cardTexts[i].text = symbol + "" + cardNum[i];
    }

    void UseCard(int i)
    {
        if (cardOp[i] == '+') top += cardNum[i];
        if (cardOp[i] == '-') top -= cardNum[i];
        if (cardOp[i] == '*') top *= cardNum[i];
        if (cardOp[i] == '/')
        {
            if (top % cardNum[i] != 0) { Debug.Log("Can't divide that evenly"); return; }
            top /= cardNum[i];
        }

        used[i] = true;   // so CardChanger knows this card is gone
        cards[i].GetComponent<CardPick>().Use();
        topText.text = top.ToString();

        if (top == 0) Debug.Log("You win!");
    }

    // true if a single "-" card would already take the top number to 0
    bool SolvedByOne()
    {
        for (int i = 0; i < cards.Length; i++)
            if (cardOp[i] == '-' && cardNum[i] == top) return true;
        return false;
    }
}