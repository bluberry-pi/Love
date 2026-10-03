using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CardGame : MonoBehaviour
{
    public TMP_Text topText;       // the big number on top
    public GameObject[] cards;     // your 5 cards
    public TMP_Text[] cardTexts;   // the text on each card (same order)
    //public GameObject winText;

    [Header("Win animation")]
    public float worldDistance = 10f;      // how far normal objects slide (units)
    public float uiDistance = 1500f;       // how far UI objects slide (pixels)
    public GameObject[] moveUpObjects;     // these slide UP when you win
    public GameObject[] moveDownObjects;   // these slide DOWN when you win
    public float moveDistance = 10f;       // how far they slide
    public float moveTime = 1f;            // how long the slide takes
    public float waitAfter = 1f;           // pause before the next scene

    // public so CardChanger can use them
    [HideInInspector] public int top;
    [HideInInspector] public char[] cardOp;
    [HideInInspector] public int[] cardNum;
    [HideInInspector] public bool[] used;

    char[] ops = { '+', '-', '*', '/' };
    bool won = false;
    // UI objects have a RectTransform, so give them the bigger distance
    float DistanceFor(GameObject obj)
    {
        if (obj.GetComponent<RectTransform>() != null) return uiDistance;
        return worldDistance;
    }

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
        if (won) return;                    // already won, ignore clicks

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

        if (top == 0)
        {
            Debug.Log("You win!");
            StartCoroutine(WaitAndExecute(1f));
            StartCoroutine(WinSequence());
        }
    }

    // true if a single "-" card would already take the top number to 0
    bool SolvedByOne()
    {
        for (int i = 0; i < cards.Length; i++)
            if (cardOp[i] == '-' && cardNum[i] == top) return true;
        return false;
    }

    // slide the objects away, wait a little, then load the next scene
    IEnumerator WaitAndExecute(float seconds)
    {
        Debug.Log("Action started...");

        // This pauses the execution of THIS function for 3 seconds
        yield return new WaitForSeconds(seconds);

        Debug.Log("3 seconds have passed! Action finishes.");
    }
    IEnumerator WinSequence()
    {
        won = true;

        // remember where every object starts
        Vector3[] upStart = new Vector3[moveUpObjects.Length];
        for (int i = 0; i < moveUpObjects.Length; i++)
            upStart[i] = moveUpObjects[i].transform.position;

        Vector3[] downStart = new Vector3[moveDownObjects.Length];
        for (int i = 0; i < moveDownObjects.Length; i++)
            downStart[i] = moveDownObjects[i].transform.position;

        // slide for moveTime seconds
        float t = 0f;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / moveTime);   // 0 to 1, smooth

            for (int i = 0; i < moveUpObjects.Length; i++)
            {
                float d = DistanceFor(moveUpObjects[i]);
                moveUpObjects[i].transform.position =
                    Vector3.Lerp(upStart[i], upStart[i] + Vector3.up * d, p);
            }

            for (int i = 0; i < moveDownObjects.Length; i++)
            {
                float d = DistanceFor(moveDownObjects[i]);
                moveDownObjects[i].transform.position =
                    Vector3.Lerp(downStart[i], downStart[i] + Vector3.down * d, p);
            }

            yield return null;
        }

        // short pause, then the next scene
        yield return new WaitForSeconds(waitAfter);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}