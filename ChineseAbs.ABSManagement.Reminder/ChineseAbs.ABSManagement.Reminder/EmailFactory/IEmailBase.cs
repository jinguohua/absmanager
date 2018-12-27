using System;
namespace ChineseAbs.ABSManagement.Reminder.EmailFactory
{
    interface IEmailBase
    {
        string GenerateContent();
        void LoadData();
    }
}
