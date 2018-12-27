using ABSMgrConn;
using ChineseAbs.ABSManagement.Loggers;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Linq;

namespace ChineseAbs.ABSManagement
{
    public class AccountManager: BaseManager
    {
        public AccountManager() { }

        protected override AbstractLogger GetLogger()
        {
            return new LoggerAccount(UserInfo);
        }

        private AbstractLogger Logger
        {
            get { return GetLogger(); }
        }

        #region Accounts

        public Accounts GetAccounts(int projectId, bool loadCurrentBalance = true) 
        {
            var tblAccounts = m_db.Query<TableAccounts>
                ("Select * from dbo.Accounts where project_id = @0", projectId);
            Accounts rt = new Accounts();
            foreach (var tbAccount in tblAccounts)
            {
                Account account = new Account(tbAccount);
                account = Decrypt(account);
                if (loadCurrentBalance)
                {
                    LoadCurrentBalance(account);
                }
                rt.Add(account);
            }
            return rt;
        }

        //Add
        public Accounts GetAccounts(bool loadCurrentBalance = true)
        {
            var tblAccounts = m_db.Query<TableAccounts>
                ("Select * from dbo.Accounts ");
            Accounts rt = new Accounts();
            foreach (var tbAccount in tblAccounts)
            {
                Account account = new Account(tbAccount);
                account = Decrypt(account);
                if (loadCurrentBalance)
                {
                    LoadCurrentBalance(account);
                }
                rt.Add(account);
            }
            return rt;
        }

        public Accounts GetAccounts(string projectGuid, bool loadCurrentBalance = true)
        {
            var projectId = Utils.DbUtils.GetIdByGuid(projectGuid, "dbo.Project", "project_guid");
            return GetAccounts(projectId, loadCurrentBalance);
        }

        public void LoadCurrentBalance(Account account)
        {
            var tblAccountBalance = m_db.SingleOrDefault<TableAccountBalances>
                (@"Select top 1 * from dbo.AccountBalances 
                    Where account_id = @0 Order by as_of_date desc"
                , account.AccountId);
            if (tblAccountBalance != null)
            {
                AccountBalance bal = new AccountBalance(tblAccountBalance);
                account.CurrentBalance = bal;
            }
        }

        public Account GetAccount(int accountId, bool loadCurrentBalance = true)
        {
            var tblAccount = m_db.Single<TableAccounts>(
                "Select top 1 * from dbo.Accounts where account_id = @0", accountId);
            var rt = new Account(tblAccount);
            if (loadCurrentBalance)
            {
                LoadCurrentBalance(rt);
            }
            rt = Decrypt(rt);
            return rt;
        }

        public Account GetAccount(string accountGuid, bool loadCurrentBalance = true)
        {
            var tblAccount = m_db.Single<TableAccounts>(
                "Select top 1 * from dbo.Accounts where account_guid = @0", accountGuid);
            var rt = new Account(tblAccount);
            if (loadCurrentBalance)
            {
                LoadCurrentBalance(rt);
            }
            rt = Decrypt(rt);
            return rt;
        }

        public void UpdateAccount(Account account)
        {
            account.TimeStamp = DateTime.Now;
            account =  Encrypt(account);
            m_db.Update("Accounts", "account_id", account.GetTableObject(), account.AccountId);
            m_logger.Log(account.ProjectId, "更新账户");
        }

        public void AddAccountBalance(AccountBalance accountBalance)
        {
            m_db.Insert("AccountBalances", "account_balance_id", true, accountBalance.GetTableObject());
            var account = GetAccount(accountBalance.AccountId);
            Logger.Log(account.ProjectId, "更新账户《" + account.Name + "》余额");
        }

        public void UpdateAccountBalance(AccountBalance accountBalance)
        {
            m_db.Update("AccountBalances", "account_balance_id", accountBalance.GetTableObject(), accountBalance.AccountBalanceId);
            var account = GetAccount(accountBalance.AccountId);
            Logger.Log(account.ProjectId, "更新账户《" + account.Name + "》余额");
        }

        public int GetProjectIdByAccountId(int accountId)
        {
            return m_db.Query<int>("SELECT project_id FROM dbo.Accounts WHERE account_id = @0", accountId).SingleOrDefault();
        }

        public void AddAccount(Account account)
        {
            account.TimeStamp = DateTime.Now;
            account = Encrypt(account);
            m_db.Insert(account.GetTableObject());
            AccountBalance accountBalance = new AccountBalance
            {
                AccountId = account.AccountId,
                EndBalance = 0,
                AsOfDate = DateTime.Now,
                TimeStamp = DateTime.Now,
                TimeStampUserName = account.TimeStampUserName
            };
            m_db.Insert("AccountBalances", "account_balance_id", true, accountBalance.GetTableObject());
            m_logger.Log(account.ProjectId, "新增账户 : " + account.Name);
        }

        public void RemoveAccount(Account account)
        {
            m_db.Delete(account.GetTableObject());
            m_logger.Log(account.ProjectId, "删除账户");
        }

        public void RemoveAccount(int accountId,int projectId)
        {
            m_db.Delete<TableAccounts>("where account_id = @0 ", accountId);
            m_logger.Log(projectId, "删除账户");
        }

        public int GetMaxAccountId() 
        {
            int id = m_db.Single<int>(" select ident_current('dbo.Accounts') ");
            return id;
        }

        public AccountBalances GetAccountBalances(int accountId)
        {
            var tableBalances = m_db.Query<TableAccountBalances>(
                "select * from dbo.AccountBalances where account_id = @0", accountId);
            var rt = new AccountBalances(tableBalances);
            return rt;
        }

        public AccountBalances GetAccountBalances(string accountGuid)
        {
            var accountId = Utils.DbUtils.GetIdByGuid(accountGuid, "dbo.Accounts", "account_guid");
            var tableBalances = m_db.Query<TableAccountBalances>(
                "select * from dbo.AccountBalances where account_id = @0", accountId);
            var rt = new AccountBalances(tableBalances);
            return rt;
        }

        #endregion

        private Account Encrypt(Account account)
        {
            account.Name = RsaUtils.Encrypt(account.Name);
            if (account.IssuerBank != null && account.IssuerBank != "")
                account.IssuerBank = RsaUtils.Encrypt(account.IssuerBank);
            if (account.BankAccount != null && account.BankAccount != "")
                account.BankAccount = RsaUtils.Encrypt(account.BankAccount);
            return account;
        }

        private Account Decrypt(Account account)
        {
            account.Name = RsaUtils.Decrypt<string>(account.Name);
            if (account.IssuerBank != null && account.IssuerBank != "")
                account.IssuerBank = RsaUtils.Decrypt<string>(account.IssuerBank);
            if (account.BankAccount != null && account.BankAccount != "")
                account.BankAccount = RsaUtils.Decrypt<string>(account.BankAccount);
            return account;
        }
    }
}
