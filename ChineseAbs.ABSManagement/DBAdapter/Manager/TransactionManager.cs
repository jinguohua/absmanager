using System;
using System.Linq;
using ABSMgrConn;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Loggers;

namespace ChineseAbs.ABSManagement
{
    public class TransactionManager: BaseManager
    {
        public TransactionManager() { }
      
        protected override AbstractLogger GetLogger()
        {
            return new LoggerTransaction(UserInfo);
        }

        private AbstractLogger Logger
        {
            get { return GetLogger(); }
        }

        #region Transactions

        public void AddAccountTransaction(AccountTransaction accountTransaction)
        {
            m_db.Insert("AccountTransactions", "transaction_id", true, accountTransaction.GetTableObject());
            var account = new AccountManager().GetAccount(accountTransaction.AccountId);
            Logger.Log(account.ProjectId, "增加交易流水记录");
        }

        public void RemoveAccountTransactions(int accountId)
        {
            m_db.Delete<TableAccountTransactions>("where account_id = @0 ", accountId);
        }

        public AccountTransactions GetTransactions(int accountId)
        {
            var tableTrans = m_db.Query<TableAccountTransactions>(
                "Select * from dbo.AccountTransactions where account_id = @0 ORDER BY transaction_time, transaction_id", accountId);
            return new AccountTransactions(tableTrans);
        }

        public AccountTransactions GetTransactions(string accountGuid)
        {
            var accountId = Utils.DbUtils.GetIdByGuid(accountGuid, "dbo.Accounts", "account_guid");
            return GetTransactions(accountId);
        }

        public AccountTransactions GetTransactions(int accountId, bool hasChecked)
        {
            var tableTrans = m_db.Query<TableAccountTransactions>(
                "Select * from dbo.AccountTransactions where account_id = @0 and has_checked=@1", accountId, hasChecked);
            return new AccountTransactions(tableTrans);
        }

        public AccountTransactions GetTransactions(string accountGuid, bool hasChecked)
        {
            var accountId = Utils.DbUtils.GetIdByGuid(accountGuid, "dbo.Accounts", "account_guid");
            return GetTransactions(accountId, hasChecked);
        }

        public AccountTransactions GetTransactionsBeforeDate(int accountId, DateTime asOfDate)
        {
            var tableTransactions = m_db.Query<TableAccountTransactions>(
                @"select * from dbo.AccountTransactions 
                    where account_id = @0 and transaction_time >= @1 ORDER BY transaction_time", accountId, asOfDate);
            var rt = new AccountTransactions(tableTransactions);
            return rt;
        }

        public void CompleteCheckTransaction(AccountTransaction t)
        {
            t.HasChecked = true;
            t.TimeStamp = DateTime.Now;
            m_db.Update(t);
        }

        public bool CheckAccountBalanceConsistency(int accountId, DateTime asOfDate)
        {
            var am = new AccountManager();
            decimal endBalance = am.GetAccountBalances(accountId)
                .Where(x => x.AsOfDate <= asOfDate)
                .OrderByDescending(x => x.AsOfDate)
                .FirstOrDefault().EndBalance;
            var transactions = GetTransactionsBeforeDate(accountId, asOfDate);
            return CheckBalanceAndTransactions(endBalance, transactions);
        }

        public bool CheckBalanceAndTransactions(decimal balance, AccountTransactions transactions)
        {
            return balance == transactions.Sum(x => x.Amount);
        }

        #endregion
    }
}
