using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BookingOffice
{
    class Coins
    {
        public int Value { get; set; }
        public int Left { get; set; }
    }

    class MailBox
    {
        public int Sum { get; set; }
        public bool Allowed { get; set; }
    }

    class Worker
    {
        private Coins[] coins;
        public List<MailBox> MailBoxes;
        private Thread workerThread;

        public Worker()
        {
            coins = new Coins[]
            {
                new Coins() {Value = 50, Left = 5 },
                new Coins() {Value = 25, Left = 10 },
                new Coins() {Value = 10, Left = 15 },
                new Coins() {Value = 5, Left = 20 },
                new Coins() {Value = 2, Left = 25 },
                new Coins() {Value = 1, Left = 50 }
            };

            MailBoxes = new List<MailBox>();
            workerThread = new Thread(Server);
            workerThread.Start();
        }

        public MailBox MakeNewMailBox()
        {
            MailBoxes.Add(new MailBox());
            return MailBoxes[MailBoxes.Count - 1];
        }

        private void Server()
        {
            while (true)
            {
                int mailBoxes = MailBoxes.Count;
                for (int i = 0; i < mailBoxes; i++)
                {
                    MailBox currentBox = MailBoxes[i];
                    if (currentBox != null && currentBox.Sum > 0)
                    {
                        currentBox.Allowed = DoCalculations(currentBox.Sum);
                        currentBox.Sum = 0;
                    }
                }
            }
            
        }

        private bool DoCalculations(int sum)
        {
            int change = 100 - sum;
            int[] collection = new int[coins.Length];

            // Fill collection with maximum count of coins
            for (int k = 0; k < coins.Length; k++)
                collection[k] = coins[k].Left;

            int i = GetCountOfChecks();
            while (i > 0)
            {
                if (CheckPossibilityOfMaxValues(collection, change))
                {
                    for (int coin = 0; coin < coins.Length; coin++)
                        coins[coin].Left -= collection[coin];

                    return true;
                }

                for (int m = coins.Length - 1; m > -1; m--)
                {
                    if (collection[m] != 0)
                    {
                        collection[m]--;
                        break;
                    }

                    collection[m] = coins[m].Left;
                }

                i--;
            }

            return false;
        }

        private int GetCountOfChecks()
        {
            if (coins.Length < 1)
                return 0;

            int count = coins[0].Left + 1;
            for (int i = 1; i < coins.Length; i++)
                count *= coins[i].Left + 1;

            return count;
        }

        private bool CheckPossibilityOfMaxValues(int[] coinsCollection, int change)
        {
            int currentChange = 0;
            for (int i = 0; i < coinsCollection.Length; i++)
                currentChange += coins[i].Value * coinsCollection[i];

            return currentChange == change ? true : false;
        }
    }
}
