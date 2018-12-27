using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ABSMgrDeployment
{
    public class ActionExecuter
    {
        public RichTextBox richTextBoxLog { get; set; }

        private void ShowLog(string msg)
        {
            richTextBoxLog.Text += msg + Environment.NewLine;
        }

        private bool ExecuteAction(string actionName, Action action)
        {
            try
            {
                action();
                ShowLog(actionName + "成功。");
                return true;
            }
            catch (Exception e)
            {
                ShowLog(actionName + "失败。");
                ShowLog(e.Message);
                ShowLog(e.StackTrace);
                return false;
            }
        }

        public void Add(string name, Action action)
        {
            actions.Add(new Tuple<string, Action>(name, action));
        }

        public void RunAll()
        {
            foreach (var acton in actions)
            {
                if (!ExecuteAction(acton.Item1, acton.Item2))
                {
                    return;
                }
            }
        }

        List<Tuple<string, Action>> actions = new List<Tuple<string, Action>>();
    }
}
