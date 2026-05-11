using System.Transactions;

namespace NovaPay
{
    internal class Program
    {

        public interface IDepositable
        {
            public void Deposit(double amount);
        }

        public interface IWithdrawable
        {
            public void Withdraw(double amount);
        }

        interface IStatementPrintable
        {
            public void PrintStatement();
        }

        public static void DisplayMenu()
        {
            Console.WriteLine("========== NovaPay Banking System ==========");
            Console.WriteLine("1. Open Savings Account");
            Console.WriteLine("2. Open Current Account");
            Console.WriteLine("3. Open Fixed Deposit Account");
            Console.WriteLine("4. Deposit");
            Console.WriteLine("5. Withdraw");
            Console.WriteLine("6. Print Account Statement");
            Console.WriteLine("7. Apply Interest (Savings Only)");
            Console.WriteLine("8. Bank Summary");
            Console.WriteLine("9. Exit");
        }
        static void Main(string[] args)
        {
            Bank bank = new Bank("NovaPay");

            int choice;

            do
            {
                DisplayMenu();

                Console.WriteLine("choose an Option: ");
                choice = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine();

                switch (choice)
                {
                    // OPEN SAVINGS ACCOUNT
                    case 1:
                        Console.Write("Enter owner name: ");
                        string savingsOwner = Console.ReadLine();

                        Console.Write("Enter interest rate (default 0.03): ");
                        string rateInput = Console.ReadLine();

                        double interestRate = 0.03;

                        if (!string.IsNullOrWhiteSpace(rateInput))
                        {
                            interestRate = Convert.ToDouble(rateInput);
                        }

                        SavingsAccount savings =
                            new SavingsAccount(savingsOwner, interestRate);

                        bank.OpenAccount(savings);

                        break;

                    // OPEN CURRENT ACCOUNT
                    case 2:
                        Console.Write("Enter owner name: ");
                        string currentOwner = Console.ReadLine();

                        Console.Write("Enter overdraft limit: ");
                        double overdraft =
                            Convert.ToDouble(Console.ReadLine());

                        CurrentAccount current =
                            new CurrentAccount(currentOwner, overdraft);

                        bank.OpenAccount(current);

                        break;

                    // OPEN FIXED DEPOSIT ACCOUNT
                    case 3:
                        Console.Write("Enter owner name: ");
                        string fixedOwner = Console.ReadLine();

                        Console.Write("Enter deposit amount: ");
                        double depositAmount =
                            Convert.ToDouble(Console.ReadLine());

                        FixedDepositAccount fixedAcc =
                            new FixedDepositAccount(fixedOwner, depositAmount);

                        bank.OpenAccount(fixedAcc);

                        break;

                    // DEPOSIT
                    case 4:
                        Console.Write("Enter account number: ");
                        int depAccNum =
                            Convert.ToInt32(Console.ReadLine());

                        BankAccount depFound =
                            bank.FindAccount(depAccNum);

                        if (depFound == null)
                        {
                            Console.WriteLine("Account not found.");
                            break;
                        }

                        Console.Write("Enter deposit amount: ");
                        double depAmount =
                            Convert.ToDouble(Console.ReadLine());

                        IDepositable dep =
                            depFound as IDepositable;

                        bank.ProcessDeposit(dep, depAmount);

                        break;

                    // WITHDRAW
                    case 5:
                        Console.Write("Enter account number: ");
                        int witAccNum =
                            Convert.ToInt32(Console.ReadLine());

                        BankAccount witFound =
                            bank.FindAccount(witAccNum);

                        if (witFound == null)
                        {
                            Console.WriteLine("Account not found.");
                            break;
                        }

                        Console.Write("Enter withdrawal amount: ");
                        double witAmount =
                            Convert.ToDouble(Console.ReadLine());

                        IWithdrawable wit =
                            witFound as IWithdrawable;

                        bank.ProcessWithdrawal(wit, witAmount);

                        break;

                    // PRINT STATEMENT
                    case 6:
                        Console.Write("Enter account number: ");
                        int statementAcc =
                            Convert.ToInt32(Console.ReadLine());

                        bank.PrintAccountStatement(statementAcc);

                        break;

                    // APPLY INTEREST
                    case 7:
                        Console.Write("Enter savings account number: ");
                        int savingsAccNum =
                            Convert.ToInt32(Console.ReadLine());

                        BankAccount acc =
                            bank.FindAccount(savingsAccNum);

                        if (acc == null)
                        {
                            Console.WriteLine("Account not found.");
                        }
                        else if (acc is SavingsAccount)
                        {
                            SavingsAccount savingsAcc =acc as SavingsAccount;
                            savingsAcc.ApplyInterest();
                        }
                        else
                        {
                            Console.WriteLine(
                                "Interest can only be applied to Savings Accounts."
                            );
                        }

                        break;

                    // BANK SUMMARY
                    case 8:
                        bank.DisplaySummary();
                        break;

                    // EXIT
                    case 0:
                        Console.WriteLine("Thank you for using NovaPay Banking System.");
                        break;

                    // INVALID CHOICE
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();

            } while (choice != 0);
        }

        public abstract class BankAccount : IDepositable, IWithdrawable, IStatementPrintable
        {
            //Static Fields
            private static int nextAccountNumber = 1001;
            protected static int totalTransactionsProcessed;

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

                Console.WriteLine("Deposit successful:" + amount);
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

        class SavingsAccount : BankAccount
        {
            // PRIVATE FIELDS
            private double interestRate;

            private static double MinBalance = 100;

            // CONSTRUCTOR

            public SavingsAccount(string ownerName, double interestRate = 0.03) : base(ownerName)
            {
                AccountType = "Savings";
                this.interestRate = interestRate;
            }

            //Overridden Methods

            public override void Withdraw(double amount)
            {
                // Validate amount
                if (amount <= 0)
                {
                    Console.WriteLine("Withdrawal amount must be greater than zero.");
                    return;
                }

                // Check minimum balance rule
                if ((balance - amount) < MinBalance)
                {
                    Console.WriteLine(
                        $"Withdrawal denied. Minimum balance of {MinBalance:C} must remain."
                    );
                    return;
                }

                // Process withdrawal
                balance -= amount;

                // Record transaction
                transactions.Add(new Transaction("Withdrawal", amount));

                // Increment total transactions
                totalTransactionsProcessed++;

                Console.WriteLine($"Withdrawal successful: {amount:C}");
            }


            public override void PrintStatement()
            {
                base.PrintStatement();

                Console.WriteLine($"Interest Rate : {interestRate:P}");
                
            }

            // APPLY INTEREST METHOD
            public void ApplyInterest()

            {
                double interest = balance * interestRate;

                Deposit(interest, "Interest Credit");

                Console.WriteLine($"Interest applied: {interest:C}");
            }








        }

        public class CurrentAccount : BankAccount
        {
            // PRIVATE FIELD
            private double overdraftLimit;

            // PROPERTY
            public double OverdraftLimit
            {
                get { return overdraftLimit; }
            }

            // CONSTRUCTOR
            public CurrentAccount(string ownerName, double overdraftLimit): base(ownerName)
            {
                if (overdraftLimit < 0)
                {
                    Console.WriteLine("Overdraft limit cannot be negative.");
                    overdraftLimit = 0;
                }

                this.overdraftLimit = overdraftLimit;

                AccountType = "Current";
            }

            // OVERRIDDEN WITHDRAW METHOD
            public override void Withdraw(double amount)
            {
                // Validate amount
                if (amount <= 0)
                {
                    Console.WriteLine("Withdrawal amount must be greater than zero.");
                    return;
                }

                // Check overdraft limit
                if ((balance - amount) < -overdraftLimit)
                {
                    Console.WriteLine("Withdrawal exceeds overdraft limit.");
                    return;
                }

                // Process withdrawal
                balance -= amount;

                // Record transaction
                transactions.Add(new Transaction("Withdrawal", amount));

                // Increment transaction counter

                totalTransactionsProcessed++;

                Console.WriteLine($"Withdrawal successful: {amount:C}");
            }

            // SEALED OVERRIDE METHOD
            public sealed override void PrintStatement()
            {
                base.PrintStatement();

                Console.WriteLine($"Overdraft Limit : {overdraftLimit:C}");
                
            }
        }

        public class FixedDepositAccount : BankAccount
        {
            // PRIVATE FIELD
            private double lockedAmount;

            // CONSTRUCTOR
            public FixedDepositAccount(string ownerName, double depositAmount)
                : base(ownerName)
            {
                AccountType = "Fixed Deposit";

                // Validate deposit amount
                if (depositAmount <= 0)
                {
                    Console.WriteLine("Initial deposit must be greater than zero.");
                    return;
                }

                // Store locked amount
                lockedAmount = depositAmount;

                // Deposit initial amount
                Deposit(depositAmount, "Initial Fixed Deposit");
            }

            // OVERRIDDEN WITHDRAW METHOD
            public override void Withdraw(double amount)
            {
                Console.WriteLine(
                    "Fixed Deposit accounts cannot be withdrawn before maturity."
                );
            }

            // OVERRIDDEN PRINT STATEMENT
            public override void PrintStatement()
            {
                base.PrintStatement();

                Console.WriteLine($"Locked Amount : {lockedAmount:C}");
                Console.WriteLine("---------------------------------");
            }
        }

        public class Transaction
        {
            // PRIVATE FIELDS
            private string type;
            private double amount;
            private DateTime date;
            private string note;

            // CONSTRUCTOR
            public Transaction(string type, double amount, string note = "")
            {
                this.type = type;
                this.amount = amount;
                this.note = note;

                // Automatically set current date/time
                date = DateTime.Now;
            }

            // METHOD
            public void DisplayInfo()
            {
                Console.Write($"{date.ToShortDateString()} ");
                Console.Write($"{type} ");
                Console.Write($"{amount:C} ");

                // Print note only if not empty
                if (!string.IsNullOrEmpty(note))
                {
                    Console.Write(note);
                }

                Console.WriteLine();
            }
        }


        public class Bank
        {
            // AUTO-IMPLEMENTED PROPERTY
            public string BankName { get; private set; }

            // PRIVATE COLLECTION
            private List<BankAccount> accounts;

            // CONSTRUCTOR
            public Bank(string name)
            {
                BankName = name;

                accounts = new List<BankAccount>();
            }

            // OPEN ACCOUNT
            public void OpenAccount(BankAccount account)
            {
                accounts.Add(account);

                Console.WriteLine($"Account created successfully. Account Number: {account.AccountNumber}");
            }

            // FIND ACCOUNT
            public BankAccount FindAccount(int accountNumber)
            {
                foreach (BankAccount account in accounts)
                {
                    if (account.AccountNumber == accountNumber)
                    {
                        return account;
                    }
                }

                return null;
            }

            // PROCESS DEPOSIT
            public void ProcessDeposit(IDepositable account, double amount)
            {
                account.Deposit(amount);
            }

            // PROCESS WITHDRAWAL
            public void ProcessWithdrawal(IWithdrawable account, double amount)
            {
                account.Withdraw(amount);
            }

            // PRINT ACCOUNT STATEMENT
            public void PrintAccountStatement(int accountNumber)
            {
                BankAccount account = FindAccount(accountNumber);

                if (account == null)
                {
                    Console.WriteLine("Account not found.");
                    return;
                }

                IStatementPrintable printable = account as IStatementPrintable;

                printable.PrintStatement();
            }

            // DISPLAY SUMMARY
            public void DisplaySummary()
            {
                int savingsCount = 0;
                int currentCount = 0;
                int fixedDepositCount = 0;

                double totalBalance = 0;

                foreach (BankAccount account in accounts)
                {
                    // Count account types
                    if (account is SavingsAccount)
                    {
                        savingsCount++;
                    }
                    else if (account is CurrentAccount)
                    {
                        currentCount++;
                    }
                    else if (account is FixedDepositAccount)
                    {
                        fixedDepositCount++;
                    }

                    // Add balances
                    totalBalance += account.Balance;
                }

                Console.WriteLine("BANK DETAILS :");
                Console.WriteLine($"Bank Name            : {BankName}");
                Console.WriteLine($"Total Accounts       : {accounts.Count}");
                Console.WriteLine($"Savings Accounts     : {savingsCount}");
                Console.WriteLine($"Current Accounts     : {currentCount}");
                Console.WriteLine($"Fixed Deposit Accs   : {fixedDepositCount}");
                Console.WriteLine($"Total Balance        : {totalBalance:C}");
                Console.WriteLine($"Total Transactions   : {BankAccount.GetTotalTransactions()}");
                
            }
        }



    }
}