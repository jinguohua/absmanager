namespace ChineseAbs.ABSManagement.ReminderHost
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
            this.reminderHostServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.reminderHostServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // reminderHostServiceProcessInstaller
            // 
            this.reminderHostServiceProcessInstaller.Password = null;
            this.reminderHostServiceProcessInstaller.Username = null;
            // 
            // reminderHostServiceInstaller
            // 
            this.reminderHostServiceInstaller.ServiceName = "ReminderHostService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.reminderHostServiceProcessInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller reminderHostServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller reminderHostServiceInstaller;
    }
}