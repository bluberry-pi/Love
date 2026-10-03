using UnityEngine;

public class CardChanger : MonoBehaviour
{
    public CardGame game;               // drag your GameManager here
    public CountdownTimer countdown;    // drag the object with your countdown here
    public DeckSwapper swapper;         // drag the object with DeckSwapper here

    float lastTime;
    char[] ops = { '+', '-', '*', '/' };

    void Start()
    {
        lastTime = countdown.time;
    }

    void Update()
    {
        // The countdown jumped back up, so it just hit 0 and restarted
        if (countdown.time > lastTime && game.top != 0)
        {
            swapper.ReplaceDeck();   // cards fall, DeckSwapper calls GiveNewCards(), cards rise
        }

        lastTime = countdown.time;
    }

    // DeckSwapper calls this while the cards are off screen
    public void GiveNewCards()
    {
        int count = game.cards.Length;

        // remember the old deck in case we can't find a solvable one
        char[] oldOp = new char[count];
        int[] oldNum = new int[count];
        for (int i = 0; i < count; i++)
        {
            oldOp[i] = game.cardOp[i];
            oldNum[i] = game.cardNum[i];
        }

        // try many random decks until one can reach 0 from the current top number
        for (int tries = 0; tries < 1000; tries++)
        {
            for (int i = 0; i < count; i++)
            {
                game.cardOp[i] = ops[Random.Range(0, 4)];
                game.cardNum[i] = Random.Range(1, 10);
                game.used[i] = false;   // every card is fresh again
            }

            if (CanBeSolved(game.top))
            {
                for (int i = 0; i < count; i++) game.ShowCard(i);
                return;
            }
        }

        // no solvable deck found, put the old values back
        for (int i = 0; i < count; i++)
        {
            game.cardOp[i] = oldOp[i];
            game.cardNum[i] = oldNum[i];
            game.used[i] = false;
        }
    }

    // Can we reach 0 from this number using the cards that are left?
    bool CanBeSolved(int number)
    {
        if (number == 0) return true;

        for (int i = 0; i < game.cards.Length; i++)
        {
            if (game.used[i]) continue;

            int result = ApplyCard(number, i);
            if (result == -9999) continue;   // bad division

            game.used[i] = true;
            bool solved = CanBeSolved(result);
            game.used[i] = false;

            if (solved) return true;
        }

        return false;
    }

    int ApplyCard(int number, int i)
    {
        char op = game.cardOp[i];
        int n = game.cardNum[i];

        if (op == '+') return number + n;
        if (op == '-') return number - n;
        if (op == '*') return number * n;

        if (number % n != 0) return -9999;
        return number / n;
    }
}