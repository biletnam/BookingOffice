using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookingOffice
{
    public class Tickets
    {
        public Tickets(string _city, int _price)
        {
            city = _city;
            price = _price;
        }
        public readonly string city;
        public readonly int price;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Tickets[] _priceTable;
        private string textA, textB;
        private Worker worker;
        private Desk deskA, deskB;

        private int transactionNumber = 0;

        public MainWindow()
        {
            InitializeComponent();

            _priceTable = new Tickets[]
            {
                new Tickets("Berlin", 77),
                new Tickets("Kyiv", 28),
                new Tickets("Moskow", 37),
                new Tickets("Paris", 91),
                new Tickets("London", 50)
            };

            worker = new Worker();
        }

        public void WriteResponse(string response, int textBox)
        {
            switch (textBox)
            {
                case 0:
                    Dispatcher.Invoke((Action)(() => { textBoxA.Text += response + "\n>"; }));
                    break;
                case 1:
                    Dispatcher.Invoke((Action)(() => { textBoxB.Text += response + "\n>"; }));
                    break;
                default:
                    break;
            }

            MoveCursor(textBox);
        }

        private void WriteLetter(string letter, int textBox)
        {
            switch (textBox)
            {
                case 0:
                    textBoxA.Text += letter;
                    break;
                case 1:
                    textBoxB.Text += letter;
                    break;
                default:
                    break;
            }
        }

        private void textBoxA_KeyDown(object sender, KeyEventArgs e)
        {
            Tickets ticket = GetTicket(e, 0);
            if (ticket == null)
            {
                return;
            }

            deskA.Enqueue(new Transaction() { TransactionNumber = transactionNumber++, Cost = ticket.price });
        }

        private void textBoxB_KeyDown(object sender, KeyEventArgs e)
        {
            Tickets ticket = GetTicket(e, 1);
            if (ticket == null)
            {
                return;
            }

            deskB.Enqueue(new Transaction() { TransactionNumber = transactionNumber++, Cost = ticket.price });
        }

        private void MoveCursor(int textBox)
        {
            switch (textBox)
            {
                case 0:
                    Dispatcher.Invoke(() =>
                    {
                        textBoxA.Focus();
                        textBoxA.CaretIndex = textBoxA.Text.Length;
                    });
                    break;
                case 1:
                    Dispatcher.Invoke(() =>
                    {
                        textBoxB.Focus();
                        textBoxB.CaretIndex = textBoxB.Text.Length;
                    });
                    break;
                default:
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            StopAllThreads();
        }

        private Tickets GetTicket(KeyEventArgs e, int textBox)
        {
            WriteLetter(e.Key.ToString() + "\n>", textBox);
            e.Handled = true;

            switch (e.Key)
            {
                case Key.D0:
                    MoveCursor(textBox);
                    break;
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                    if ((textBox == 0 ? deskA : deskB) == null)
                    {
                        WriteResponse("Start service first! (0)", textBox);
                        MoveCursor(textBox);
                        return null;
                    }
                    break;
                default:
                    WriteResponse("Invalid key! (0..5)", textBox);
                    MoveCursor(textBox);
                    return null;
            }

            switch (e.Key)
            {
                case Key.D0:
                    if (textBox == 0)
                    {
                        if (deskA != null)
                            WriteResponse("Desk A is already started!", 0);
                        else
                        {
                            deskA = new Desk(this, worker, 0);
                            WriteResponse("Desk A was started!", 0);
                        }
                    }
                    else
                    {
                        if (deskB != null)
                            WriteResponse("Desk B is already started!", 1);
                        else
                        {
                            deskB = new Desk(this, worker, 1);
                            WriteResponse("Desk B was started!", 1);
                        }
                    }
                    return null;
                case Key.D1:
                    return _priceTable[0];
                case Key.D2:
                    return _priceTable[1];
                case Key.D3:
                    return _priceTable[2];
                case Key.D4:
                    return _priceTable[3];
                case Key.D5:
                    return _priceTable[4];
                default:
                    return null;
            }
        }

        private void StopAllThreads()
        {
            if (worker != null)
                worker.Stop();
            if (deskA != null)
                deskA.Stop();
            if (deskB != null)
                deskB.Stop();
        }
    }
}
