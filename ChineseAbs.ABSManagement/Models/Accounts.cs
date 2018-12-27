using System;
using System.Collections.Generic;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    #region Accounts

    public enum EAccountType
    {
        募集账户 = 1,
        回款账户 = 2,
        监管账户 = 3,
        专项计划账户 = 4,
        登记机构账户 = 5,
        投资者账户 = 6,
        托管机构账户 = 7,
        原始权益人账户 = 8
    }

    public class Accounts : List<Account>
    {
        public Accounts() { }

        public Accounts(IEnumerable<TableAccounts> queryTable)
        {
            foreach (var item in queryTable)
            {
                this.Add(new Account(item));
            }
        }

        public List<TableAccounts> ToTableList()
        {
            var tableList = new List<TableAccounts>();
            foreach (var item in this)
            {
                tableList.Add(item.GetTableObject());
            }
            return tableList;
        }
    }

    public class Account: BaseDataContainer<TableAccounts>
    {
        #region Constructors

        public Account(TableAccounts acc): base(acc)
        {
        }

        public Account() 
        {
            AccountGuid = Guid.NewGuid().ToString();
        }

        #endregion

        #region Properties

        public int AccountId { get; set; }

        public string AccountGuid { get; set; }

        public int ProjectId { get; set; }

        public int AccountTypeId 
        {
            get
            {
                return (int)AccountType;
            }
            set
            {
                this.AccountType = (EAccountType)Enum.Parse(typeof(EAccountType), value.ToString());
            }
        }

        public EAccountType AccountType { get; set; }

        public string Name { get; set; }

        public string IssuerBank { get; set; }

        public string BankAccount { get; set; }

        public DateTime AsOfDate { get; set; }

        public DateTime? TimeStamp { get; set; }

        public string TimeStampUserName { get; set; }

        public AccountBalance CurrentBalance { get; set; }

        public int? CurrentBalanceId
        {
            get
            {
                return (CurrentBalance==null)?-1: CurrentBalance.AccountBalanceId;
            }
        }

        public decimal? CurrentBalanceAmount
        {
            get
            {
                return (CurrentBalance == null) ? -1 : CurrentBalance.EndBalance;
            }
        }

        #endregion

        public override TableAccounts GetTableObject()
        {
            var rt = new TableAccounts();
            rt.account_id = this.AccountId;
            rt.account_guid = this.AccountGuid;
            rt.project_id = this.ProjectId;
            rt.account_type_id = this.AccountTypeId;
            rt.name = this.Name;
            rt.issuer_bank = this.IssuerBank;
            rt.bank_account = this.BankAccount;
            rt.time_stamp = this.TimeStamp;
            rt.time_stamp_user_name = this.TimeStampUserName;
            return rt;
        }

        public override void FromTableObject(TableAccounts obj)
        {
            this.AccountId = obj.account_id;
            this.AccountGuid = obj.account_guid;
            this.ProjectId = obj.project_id;
            this.AccountTypeId = obj.account_type_id;
            this.Name = obj.name;
            this.IssuerBank = obj.issuer_bank;
            this.BankAccount = obj.bank_account;
            this.TimeStamp = obj.time_stamp;
            this.TimeStampUserName = obj.time_stamp_user_name;
        }
    }

    #endregion

    #region AccountBalances

    public class AccountBalances : List<AccountBalance> 
    {
        public AccountBalances() { }

        public AccountBalances(IEnumerable<TableAccountBalances> queryTable)
        {
            foreach (var item in queryTable)
            {
                this.Add(new AccountBalance(item));
            }
        }

        public List<TableAccountBalances> ToTableList()
        {
            var tableList = new List<TableAccountBalances>();
            foreach (var item in this)
            {
                tableList.Add(item.GetTableObject());
            }
            return tableList;
        }
    }

    public class AccountBalance: BaseDataContainer<TableAccountBalances>
    {
        #region Constructors

        public AccountBalance() { }

        public AccountBalance(TableAccountBalances bal): base(bal)
        {

        }
        #endregion

        #region Properties

        public int AccountBalanceId { get; set; }

        public int AccountId { get; set; }

        public DateTime AsOfDate { get; set; }

        public decimal EndBalance { get; set; }

        public DateTime TimeStamp { get; set; }

        public string TimeStampUserName { get; set; }

        #endregion

        public override TableAccountBalances GetTableObject()
        {
            var obj = new TableAccountBalances();
            obj.account_balance_id = this.AccountBalanceId;
            obj.account_id = this.AccountId;
            obj.as_of_date = this.AsOfDate;
            obj.end_balance = this.EndBalance;
            obj.time_stamp = this.TimeStamp;
            obj.time_stamp_user_name = this.TimeStampUserName;
            return obj;
        }

        public override void FromTableObject(TableAccountBalances obj)
        {
            this.AccountBalanceId = obj.account_balance_id;
            this.AccountId = obj.account_id;
            this.AsOfDate = obj.as_of_date;
            this.EndBalance = obj.end_balance;
            this.TimeStamp = obj.time_stamp;
            this.TimeStampUserName = obj.time_stamp_user_name;
        }
    }

    #endregion

    #region AccountTransactions

    public class AccountTransactions : List<AccountTransaction>
    {
        public AccountTransactions() { }

        public AccountTransactions(IEnumerable<TableAccountTransactions> queryTable)
        {
            foreach (var item in queryTable)
            {
                this.Add(new AccountTransaction(item));
            }
        }
    }

    public class AccountTransaction : BaseDataContainer<TableAccountTransactions>
    {
        public AccountTransaction()
        {
            this.TransactionGuid = Guid.NewGuid().ToString();
        }

        public AccountTransaction(TableAccountTransactions t): base(t)
        {
        }

        public int TransactionId { get; set; }

        public int AccountId { get; set; }

        public string TransactionGuid { get; set; }

        public DateTime TransactionTime { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public string Comment { get; set; }

        public bool HasChecked { get; set; }

        public DateTime TimeStamp { get; set; }

        public string TimeStampUserName { get; set; }


        public override TableAccountTransactions GetTableObject()
        {
            var t = new TableAccountTransactions();
            t.transaction_id = this.TransactionId;
            t.transaction_guid = this.TransactionGuid;
            t.transaction_time = this.TransactionTime;
            t.time_stamp = this.TimeStamp;
            t.time_stamp_user_name = this.TimeStampUserName;
            t.account_id = this.AccountId;
            t.amount = this.Amount;
            t.description = this.Description;
            t.comment = this.Comment;
            t.has_checked = this.HasChecked;
            return t;
        }

        public override void FromTableObject(TableAccountTransactions t)
        {
            this.TransactionId = t.transaction_id;
            this.TransactionGuid = t.transaction_guid;
            this.TransactionTime = t.transaction_time;
            this.TimeStamp = t.time_stamp;
            this.TimeStampUserName = t.time_stamp_user_name;
            this.AccountId = t.account_id;
            this.Amount = t.amount;
            this.Description = t.description;
            this.Comment = t.comment;
            this.HasChecked = t.has_checked;
        }
    }

    #endregion 
}
