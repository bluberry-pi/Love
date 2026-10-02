using UnityEngine;

public class CardChanger : MonoBehaviour
{
    public CardGame game;               // drag your GameManager here
    public CountdownTimer countdown;    // drag the object with your countdown here

    float lastTime;                     // the countdown number from the last frame
    char[] ops = { '+', '-', '*', '/' };

    void Start()
    {
        lastTime = countdown.time;
    }

    void Update()
    {
        // Your countdown goes 5, 4, 3 ... then jumps back up to 5.
        // If the number is now higher than last frame, it just hit 0 and restarted.
        if (countdown.time > lastTime)
        {
            ChangeRandomCard();
        }

        lastTime = countdown.time;
    }

    void ChangeRandomCard()
    {
        // do nothing if the player already won
        if (game.top == 0) return;

        // try up to 50 times to find a change that keeps the level solvable
        for (int tries = 0; tries < 50; tries++)
        {
            // 1. pick a random card
            int i = Random.Range(0, game.cards.Length);

            // 2. skip it if it was already used
            if (game.used[i]) continue;

            // 3. remember the old value so we can undo
            char oldOp = game.cardOp[i];
            int oldNum = game.cardNum[i];

            // 4. give it a new random value
            game.cardOp[i] = ops[Random.Range(0, 4)];
            game.cardNum[i] = Random.Range(1, 10);

            // 5. is it different, and can the level still be solved?
            bool different = (game.cardOp[i] != oldOp) || (game.cardNum[i] != oldNum);

            if (different && CanBeSolved(game.top))
            {
                game.ShowCard(i);   // update the text on the card
                return;             // done
            }

            // 6. not good, put the old value back and try again
            game.cardOp[i] = oldOp;
            game.cardNum[i] = oldNum;
        }
    }

    // Checks: using the cards that are left, can we reach 0 from this number?
    bool CanBeSolved(int number)
    {
        if (number == 0) return true;   // reached 0, solved

        // try each card that is still in the player's hand
        for (int i = 0; i < game.cards.Length; i++)
        {
            if (game.used[i]) continue;

            int result = ApplyCard(number, i);

            // -9999 means "can't use this card right now" (bad division)
            if (result == -9999) continue;

            // pretend we use this card, then check what is left
            game.used[i] = true;
            bool solved = CanBeSolved(result);
            game.used[i] = false;   // put it back

            if (solved) return true;
        }

        return false;   // no order of cards works
    }

    // Does the math of one card on a number
    int ApplyCard(int number, int i)
    {
        char op = game.cardOp[i];
        int n = game.cardNum[i];

        if (op == '+') return number + n;
        if (op == '-') return number - n;
        if (op == '*') return number * n;

        // division only works if it divides evenly
        if (number % n != 0) return -9999;
        return number / n;
    }
}