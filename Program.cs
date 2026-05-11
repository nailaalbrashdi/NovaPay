using System.Transactions;

namespace NovaPay
{
    internal class Program
    {

        interface IDepositable
        {
            public void Deposit(double amount);
        }

        interface IWithdrawable
        {
            public void Withdraw(double amount);
        }

        interface IStatementPrintable
        {
            public void PrintStatement();
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }

        abstract class BankAccount : IDepositable, IWithdrawable, IStatementPrintable
        {
            //Static Fields
            private static int nextAccountNumber = 1001;
            private static int totalTransactionsProcessed;

            //Protected Fields
            protected double balance;

            protected List<Transaction> transactions;

            //properties
            public int AccountNumber { get; }
            public string OwnerName { get; }
            public double Balance
            {
                get { return balance; }
            }
            public string AccountType { get; protected set; }

            //Constructor

            public BankAccount(string ownerName)
            {
                OwnerName = ownerName;
                AccountNumber = nextAccountNumber++;

                balance = 0;
                transactions = new List<Transaction>();
            }

            //Methods

            public static int GetTotalTransactions()
            {
                return totalTransactionsProcessed;
            }

            //Overloading 
            public void Deposit(double amount)
            {
                if (amount <= 0)
                {
                    Console.WriteLine("Deposit amount must be greater than zero.");
                    return;
                }

                balance += amount;

                Transaction transaction = new Transaction("Deposit", amount);
                transactions.Add(transaction);

                totalTransactionsProcessed++;

                Console.WriteLine("Deposit successful:"+ amount);
            }

            public void Deposit(double amount, string note)
            {
                if (amount <= 0)
                {
                    Console.WriteLine("Deposit amount must be greater than zero.");
                    return;
                }

                balance += amount;

                Transaction transaction = new Transaction("Deposit", amount, note);
                transactions.Add(transaction);

                totalTransactionsProcessed++;

                Console.WriteLine($"Deposit successful: {amount:C} | Note: {note}");
            }

            public abstract void Withdraw(double amount);

            public virtual void PrintStatement()
            {
                Console.WriteLine("=================================");
                Console.WriteLine($"Account Number : {AccountNumber}");
                Console.WriteLine($"Owner Name     : {OwnerName}");
                Console.WriteLine($"Account Type   : {AccountType}");
                Console.WriteLine($"Balance        : {Balance:C}");
                Console.WriteLine("=================================");
                Console.WriteLine("TRANSACTIONS:");

                if (transactions.Count == 0)
                {
                    Console.WriteLine("No transactions available.");
                }
                else
                {
                    foreach (Transaction transaction in transactions)
                    {
                        Console.WriteLine(transaction);
                    }
                }

                Console.WriteLine("=================================");
            }





        }


    }
}
