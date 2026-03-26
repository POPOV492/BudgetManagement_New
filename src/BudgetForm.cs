using System;
using System.Windows.Forms;
using System.Drawing;

namespace BudgetManagement
{
    public class BudgetForm : Form
    {
        private BudgetManager budgetManager;
        private TextBox descriptionTextBox;
        private TextBox amountTextBox;
        private ComboBox typeComboBox;
        private DateTimePicker datePicker;
        private Button addTransactionButton;
        private Button removeTransactionButton;
        private Button updateTransactionButton;
        private ListBox transactionsListBox;
        private Label totalBudgetLabel;

        public BudgetForm()
        {
            InitializeComponents();
            budgetManager = new BudgetManager();
            UpdateTransactionsList();
            UpdateTotalBudget();
        }

        private void InitializeComponents()
        {
            this.Text = "Управление бюджетом";
            this.Width = 600;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;

            descriptionTextBox = new TextBox { Location = new Point(10, 10), Width = 150 };
            amountTextBox = new TextBox { Location = new Point(170, 10), Width = 100 };

            typeComboBox = new ComboBox { Location = new Point(280, 10), Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
            typeComboBox.Items.Add("Доход");
            typeComboBox.Items.Add("Расход");
            typeComboBox.SelectedIndex = 0;

            datePicker = new DateTimePicker { Location = new Point(390, 10), Width = 150, Format = DateTimePickerFormat.Short };

            addTransactionButton = new Button { Location = new Point(10, 40), Text = "Добавить", Width = 100 };
            addTransactionButton.Click += AddTransactionButton_Click;

            removeTransactionButton = new Button { Location = new Point(120, 40), Text = "Удалить", Width = 100 };
            removeTransactionButton.Click += RemoveTransactionButton_Click;

            updateTransactionButton = new Button { Location = new Point(230, 40), Text = "Обновить", Width = 100 };
            updateTransactionButton.Click += UpdateTransactionButton_Click;

            transactionsListBox = new ListBox { Location = new Point(10, 80), Width = 560, Height = 250 };

            totalBudgetLabel = new Label
            {
                Location = new Point(10, 340),
                Width = 300,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Text = "Общий бюджет: 0 руб."
            };

            this.Controls.AddRange(new Control[] {
                descriptionTextBox, amountTextBox, typeComboBox, datePicker,
                addTransactionButton, removeTransactionButton, updateTransactionButton,
                transactionsListBox, totalBudgetLabel
            });
        }

        private void UpdateTransactionsList()
        {
            transactionsListBox.Items.Clear();
            foreach (var transaction in budgetManager.Transactions)
            {
                string type = transaction.Type == TransactionType.Доход ? "+" : "-";
                transactionsListBox.Items.Add($"{transaction.Description} | {type} {transaction.Amount} руб. | {transaction.Date:dd.MM.yyyy}");
            }
        }

        private void UpdateTotalBudget()
        {
            totalBudgetLabel.Text = $"Общий бюджет: {budgetManager.TotalBudget} руб.";
            totalBudgetLabel.ForeColor = budgetManager.TotalBudget >= 0 ? Color.Green : Color.Red;
        }

        private void ClearInputFields()
        {
            descriptionTextBox.Clear();
            amountTextBox.Clear();
            typeComboBox.SelectedIndex = 0;
            datePicker.Value = DateTime.Now;
        }

        private void AddTransactionButton_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(amountTextBox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите положительную сумму!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TransactionType type = typeComboBox.SelectedIndex == 0 ? TransactionType.Доход : TransactionType.Расход;
            Transaction newTransaction = new Transaction(descriptionTextBox.Text, amount, type, datePicker.Value);

            budgetManager.AddTransaction(newTransaction);
            UpdateTransactionsList();
            UpdateTotalBudget();
            ClearInputFields();
        }

        private void RemoveTransactionButton_Click(object sender, EventArgs e)
        {
            if (transactionsListBox.SelectedIndex == -1) return;
            var transaction = budgetManager.Transactions[transactionsListBox.SelectedIndex];
            budgetManager.RemoveTransaction(transaction);
            UpdateTransactionsList();
            UpdateTotalBudget();
        }

        private void UpdateTransactionButton_Click(object sender, EventArgs e)
        {
            if (transactionsListBox.SelectedIndex == -1) return;
            if (!decimal.TryParse(amountTextBox.Text, out decimal newAmount) || newAmount <= 0) return;

            TransactionType newType = typeComboBox.SelectedIndex == 0 ? TransactionType.Доход : TransactionType.Расход;
            var transaction = budgetManager.Transactions[transactionsListBox.SelectedIndex];
            budgetManager.UpdateTransaction(transaction, descriptionTextBox.Text, newAmount, newType);

            UpdateTransactionsList();
            UpdateTotalBudget();
            ClearInputFields();
        }
    }
}
