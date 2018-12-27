namespace ChineseAbs.ABSManagement.Reminder
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ABSMgrReminderServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.ABSMgrReminderServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // ABSMgrReminderServiceProcessInstaller
            // 
            this.ABSMgrReminderServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.ABSMgrReminderServiceProcessInstaller.Password = null;
            this.ABSMgrReminderServiceProcessInstaller.Username = null;
            // 
            // ABSMgrReminderServiceInstaller
            // 
            this.ABSMgrReminderServiceInstaller.ServiceName = "ABSManagerReminder";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ABSMgrReminderServiceProcessInstaller,
            this.ABSMgrReminderServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ABSMgrReminderServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller ABSMgrReminderServiceInstaller;
    }
}