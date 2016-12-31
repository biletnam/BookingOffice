using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BookingOffice
{
    class Transaction
    {
        public int TransactionNumber { get; set; }
        public int Cost { get; set; }
    }
    class Desk
    {
        private Queue<Transaction> transactionsQueue;
        private Worker worker;
        private MailBox mail;
        private Thread connectorThread;
        private MainWindow mainWindow;
        private int textBox;

        public Desk(MainWindow mainWnd, Worker _worker, int box)
        {
            mainWindow = mainWnd;
            textBox = box;
            worker = _worker;
            mail = worker.MakeNewMailBox();
            transactionsQueue = new Queue<Transaction>();

            connectorThread = new Thread(Connector);
            connectorThread.Start();
        }

        public void Enqueue(Transaction transaction)
        {
            transactionsQueue.Enqueue(transaction);
        }

        private void Connector()
        {
            while (true)
            {
                if (transactionsQueue.Count < 1)
                {
                    Thread.Sleep(1);
                    continue;
                }

                Transaction transaction = transactionsQueue.Dequeue();
                mail.Sum = transaction.Cost;

                while (mail.Sum != 0)
                {
                    Thread.Sleep(100);
                }

                string response = mail.Allowed 
                    ? "Transaction №" + transaction.TransactionNumber + " complete" 
                    : "Transaction №" + transaction.TransactionNumber + " fail";
                mainWindow.WriteResponse(response, textBox);
            }
        }
    }
}
