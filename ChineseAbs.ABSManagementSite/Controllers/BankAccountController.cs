using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagementSite.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Utils;

namespace ABSManagement.Controllers
{
    [Authorize]
    public class BankAccountController : BaseController
    {
        public ActionResult Index()
        {
            var authorizedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
            var projects = m_dbAdapter.Project.GetProjects(authorizedProjectIds);
            List<object> projectList = new List<object>();
            foreach (Project project in projects)
            {
                projectList.Add(new { ProjectName = project.Name, ProjectGuid = project.ProjectGuid, Selected = false });
            }
            return View(projectList);
        }

        public ActionResult AddModDelDemo()
        {
            var authorizedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
            var projects = m_dbAdapter.Project.GetProjects(authorizedProjectIds);
        List<object> projectList = new List<object>();
            foreach (Project project in projects)
            {
                projectList.Add(new { ProjectName = project.Name, ProjectGuid = project.ProjectGuid, Selected = false });
            }
            return View(projectList);
        }

        private void CheckPermission(ProjectLogicModel project)
        {
            var projectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
            CommUtils.Assert(projectIds.Contains(project.Instance.ProjectId),
                "没有该产品[{0}]的操作权限", project.Instance.ProjectGuid);
        }

        [HttpPost]
        public ActionResult CreateBankAccount(string projectGuid, string bankAccountType,
            string bankAccountName, string issuerBank, string bankAccountNumber)
        {
            return ActionUtils.Json(() =>
            {
                var project = Platform.GetProject(projectGuid);
                CheckPermission(project);

                var accountType = CommUtils.ParseEnum<EAccountType>(bankAccountType);
                CommUtils.AssertHasContent(bankAccountName, "请输入账户名称");

                var account = new Account();
                account.AccountId = m_dbAdapter.BankAccount.GetMaxAccountId() + 1;
                account.AccountGuid = Guid.NewGuid().ToString();
                account.ProjectId = project.Instance.ProjectId;
                account.AccountType = accountType;
                account.AccountTypeId = (int)accountType;
                account.Name = bankAccountName;
                account.IssuerBank = issuerBank;
                account.BankAccount = bankAccountNumber;
                m_dbAdapter.BankAccount.AddAccount(account);
                return ActionUtils.Success(null);
            });
        }

        [HttpPost]
        public ActionResult EditBankAccount(string projectGuid, string bankAccountGuid,
            string bankAccountType, string bankAccountName,
            string issuerBank, string bankAccountNumber)
        {
            return ActionUtils.Json(() =>
            {
                var project = Platform.GetProject(projectGuid);
                CheckPermission(project);

                var accountType = CommUtils.ParseEnum<EAccountType>(bankAccountType);
                CommUtils.AssertHasContent(bankAccountName, "请输入账户名称");

                var account = m_dbAdapter.BankAccount.GetAccount(bankAccountGuid);
                CommUtils.Assert(project.Instance.ProjectId == account.ProjectId,
                    "传入参数错误：[ProjectGuid={0}][BankAccountGuid={1}]",
                    projectGuid, bankAccountGuid);

                account.AccountType = accountType;
                account.AccountTypeId = (int)accountType;
                account.Name = bankAccountName;
                account.IssuerBank = issuerBank;
                account.BankAccount = bankAccountNumber;
                m_dbAdapter.BankAccount.UpdateAccount(account);
                return ActionUtils.Success(null);
            });
        }

        [HttpPost]
        public ActionResult DeleteBankAccount(string projectGuid, string bankAccountGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = Platform.GetProject(projectGuid);
                CheckPermission(project);

                var account = m_dbAdapter.BankAccount.GetAccount(bankAccountGuid);
                CommUtils.Assert(project.Instance.ProjectId == account.ProjectId,
                    "传入参数错误：[ProjectGuid={0}][BankAccountGuid={1}]",
                    projectGuid, bankAccountGuid);

                m_dbAdapter.BankAccount.RemoveAccount(account.AccountId, account.ProjectId);

                m_dbAdapter.Transaction.RemoveAccountTransactions(account.AccountId);
                return ActionUtils.Success(null);
            });
        }

        public ActionResult ShowProjectAccounts(string projectGuid)
        {
            var selectedProject = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
            var authorizedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
            var projects = m_dbAdapter.Project.GetProjects(authorizedProjectIds);
            List<object> projectList = new List<object>();

            foreach (Project project in projects)
            {
                if (project.ProjectGuid == selectedProject.ProjectGuid)
                {
                    projectList.Add(new { ProjectName = project.Name, ProjectGuid = project.ProjectGuid, Selected = true });
                    continue;
                }
                projectList.Add(new { ProjectName = project.Name, ProjectGuid = project.ProjectGuid, Selected = false });
            }
            return View("Index", projectList);
        }

        public ActionResult GetBankAccounts(string projectGuid)
        {
            if (string.IsNullOrEmpty(projectGuid))
            {
                throw new ApplicationException("传入非法参数");
            }

            var accounts = m_dbAdapter.BankAccount.GetAccounts(projectGuid);

            List<object> viewModel = new List<object>();
            if (accounts != null)
            {
                foreach (var account in accounts)
                {
                    var accountBalance = account.CurrentBalance == null ? 0 : account.CurrentBalanceAmount;
                    var timestamp = account.CurrentBalance == null ? account.TimeStamp : account.CurrentBalance.TimeStamp;
                    var accountBalanceId = account.CurrentBalanceId;
                    viewModel.Add(new {
                        AccountGuid = account.AccountGuid,
                        AccountId = account.AccountId,
                        AccountBalanceId = accountBalanceId,
                        AccountName = account.Name,
                        AccountType = account.AccountType.ToString(),
                        IssuerBank = account.IssuerBank,
                        BankAccount = account.BankAccount,
                        AccountBalance = accountBalance,
                        Timestamp = timestamp
                    });
                }
            }
            return Content(Serialize(viewModel), "text/json");
        }

        public void UpdateAccountBalance(string accountId, string accountBalanceId, string updateAccountBalance, string asOfDate)
        {
            int account_Id;
            int account_Balance_Id;
            decimal update_Balance;
            DateTime as_Of_Date;

            if (!(int.TryParse(accountId, out account_Id) && decimal.TryParse(updateAccountBalance, out update_Balance) &&
                DateTime.TryParse(asOfDate, out as_Of_Date) && account_Id > 0 && update_Balance >= 0 && update_Balance <= 100000000000 &&
                int.TryParse(accountBalanceId, out account_Balance_Id) && account_Balance_Id > 0))
            {
                throw new ApplicationException("传入非法参数");
            }
            AccountBalance accountBalance = new AccountBalance()
            {
                AccountId = account_Id,
                EndBalance = update_Balance,
                AsOfDate = as_Of_Date,
                TimeStamp = DateTime.Now,
                TimeStampUserName = User.Identity.Name,
                AccountBalanceId = account_Balance_Id
            };
            m_dbAdapter.BankAccount.UpdateAccountBalance(accountBalance);
        }

        public ActionResult GetAccountTransactions(int accountId, string filterCondition)
        {
            if (accountId <= 0 || string.IsNullOrEmpty(filterCondition))
            {
                throw new ApplicationException("传入非法参数");
            }

            AccountTransactions accountTransactions = null;
            switch (filterCondition)
            {
                case "lastWeek":
                    accountTransactions = m_dbAdapter.Transaction.GetTransactionsBeforeDate(accountId, DateTime.Now.AddDays(-7));
                    break;
                case "lastMonth":
                    accountTransactions = m_dbAdapter.Transaction.GetTransactionsBeforeDate(accountId, DateTime.Now.AddDays(-30));
                    break;
                case "lastHalfYear":
                    accountTransactions = m_dbAdapter.Transaction.GetTransactionsBeforeDate(accountId, DateTime.Now.AddMonths(-6));
                    break;
                case "lastYear":
                    accountTransactions = m_dbAdapter.Transaction.GetTransactionsBeforeDate(accountId, DateTime.Now.AddYears(-1));
                    break;
                case "all":
                    accountTransactions = m_dbAdapter.Transaction.GetTransactions(accountId);
                    break;
            }

            List<object> transactions = new List<object>();

            if (accountTransactions != null)
            {
                foreach (var transaction in accountTransactions)
                {
                    transactions.Add(new { Amount = transaction.Amount, Description = transaction.Description, TransactionTime = transaction.TransactionTime.ToString("yyyy-MM-dd") });
                }
            }
            return Content(Serialize(transactions), "text/json");
        }

        public void AddAccountTransaction(int accountId, decimal transactionAmount, DateTime transactionTime, string operation, string transactionDescription)
        {
            if (accountId <= 0 || transactionTime > DateTime.Now || string.IsNullOrEmpty(operation) || transactionAmount < 0 || transactionAmount > 100000000000)
            {
                throw new ApplicationException("传入非法参数!");
            }
            decimal amount = 0;
            switch (operation)
            {
                case "into"://向账户存入金额
                    amount = transactionAmount;
                    break;
                case "tansferOut"://账户支出金额
                    amount = -transactionAmount;
                    break;
                default:
                    throw new ApplicationException("传入非法参数!");
            }
            AccountTransaction accountTransaction = new AccountTransaction
            {
                AccountId = accountId,
                TransactionGuid = Guid.NewGuid().ToString(),
                Amount = amount,
                TransactionTime = transactionTime,
                Description = transactionDescription,
                TimeStamp = DateTime.Now,
                TimeStampUserName = User.Identity.Name
            };
            m_dbAdapter.Transaction.AddAccountTransaction(accountTransaction);
        }

        public ActionResult GetTransactionBalance(int accountId)
        {
            decimal transactionBalance = m_dbAdapter.Transaction.GetTransactions(accountId).Sum(x => x.Amount);
            return Json(transactionBalance, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllAccountOperationLog(string projectGuid)
        {
            if (string.IsNullOrEmpty(projectGuid))
            {
                throw new ApplicationException("传入非法参数");
            }
            return null;
        }

        private string Serialize(object o)
        {
            var setting = new JsonSerializerSettings();
            var jsonConverter = new List<JsonConverter>()
            {
                new IsoDateTimeConverter(){ DateTimeFormat = "yyyy-MM-dd HH:mm:ss" }
            };
            setting.Converters = jsonConverter;
            return JsonConvert.SerializeObject(o, setting);
        }
    }
}
