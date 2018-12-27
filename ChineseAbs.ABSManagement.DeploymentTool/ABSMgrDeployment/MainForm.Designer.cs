namespace ABSMgrDeployment
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.textBoxDestinationWebsitePath = new System.Windows.Forms.TextBox();
            this.buttonStartDeployment = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonDestinationWebsitePath = new System.Windows.Forms.Button();
            this.buttonClearLog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxDestinationWebsitePath
            // 
            this.textBoxDestinationWebsitePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDestinationWebsitePath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.textBoxDestinationWebsitePath.Location = new System.Drawing.Point(17, 35);
            this.textBoxDestinationWebsitePath.Name = "textBoxDestinationWebsitePath";
            this.textBoxDestinationWebsitePath.Size = new System.Drawing.Size(500, 20);
            this.textBoxDestinationWebsitePath.TabIndex = 4;
            // 
            // buttonStartDeployment
            // 
            this.buttonStartDeployment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStartDeployment.Location = new System.Drawing.Point(523, 74);
            this.buttonStartDeployment.Name = "buttonStartDeployment";
            this.buttonStartDeployment.Size = new System.Drawing.Size(75, 23);
            this.buttonStartDeployment.TabIndex = 6;
            this.buttonStartDeployment.Text = "开始部署";
            this.buttonStartDeployment.UseVisualStyleBackColor = true;
            this.buttonStartDeployment.Click += new System.EventHandler(this.buttonStartDeployment_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "目标网站路径：";
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxLog.Location = new System.Drawing.Point(17, 74);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.ReadOnly = true;
            this.richTextBoxLog.Size = new System.Drawing.Size(500, 349);
            this.richTextBoxLog.TabIndex = 9;
            this.richTextBoxLog.Text = "";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "日志：";
            // 
            // buttonDestinationWebsitePath
            // 
            this.buttonDestinationWebsitePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDestinationWebsitePath.Location = new System.Drawing.Point(523, 33);
            this.buttonDestinationWebsitePath.Name = "buttonDestinationWebsitePath";
            this.buttonDestinationWebsitePath.Size = new System.Drawing.Size(75, 23);
            this.buttonDestinationWebsitePath.TabIndex = 20;
            this.buttonDestinationWebsitePath.Text = "浏览";
            this.buttonDestinationWebsitePath.UseVisualStyleBackColor = true;
            this.buttonDestinationWebsitePath.Click += new System.EventHandler(this.buttonDestinationWebsitePath_Click);
            // 
            // buttonClearLog
            // 
            this.buttonClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearLog.Location = new System.Drawing.Point(523, 400);
            this.buttonClearLog.Name = "buttonClearLog";
            this.buttonClearLog.Size = new System.Drawing.Size(75, 23);
            this.buttonClearLog.TabIndex = 21;
            this.buttonClearLog.Text = "清空日志";
            this.buttonClearLog.UseVisualStyleBackColor = true;
            this.buttonClearLog.Click += new System.EventHandler(this.buttonClearLog_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 435);
            this.Controls.Add(this.buttonClearLog);
            this.Controls.Add(this.buttonDestinationWebsitePath);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.richTextBoxLog);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.buttonStartDeployment);
            this.Controls.Add(this.textBoxDestinationWebsitePath);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ABSMgrDeployer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxDestinationWebsitePath;
        private System.Windows.Forms.Button buttonStartDeployment;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.RichTextBox richTextBoxLog;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonDestinationWebsitePath;
        private System.Windows.Forms.Button buttonClearLog;
    }
}

