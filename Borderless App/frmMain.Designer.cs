namespace Borderless_App
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lstProcesses = new System.Windows.Forms.ListBox();
            this.btnPatch = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRefreshList = new System.Windows.Forms.Button();
            this.toolHelper = new System.Windows.Forms.ToolTip(this.components);
            this.chkClose = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lstProcesses
            // 
            this.lstProcesses.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstProcesses.FormattingEnabled = true;
            this.lstProcesses.ItemHeight = 15;
            this.lstProcesses.Location = new System.Drawing.Point(12, 35);
            this.lstProcesses.Name = "lstProcesses";
            this.lstProcesses.Size = new System.Drawing.Size(454, 184);
            this.lstProcesses.TabIndex = 2;
            this.toolHelper.SetToolTip(this.lstProcesses, "Select process to make borderless fullscreen.");
            // 
            // btnPatch
            // 
            this.btnPatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPatch.Location = new System.Drawing.Point(12, 252);
            this.btnPatch.Name = "btnPatch";
            this.btnPatch.Size = new System.Drawing.Size(456, 52);
            this.btnPatch.TabIndex = 3;
            this.btnPatch.Text = "(Un)Patch Process";
            this.toolHelper.SetToolTip(this.btnPatch, resources.GetString("btnPatch.ToolTip"));
            this.btnPatch.UseVisualStyleBackColor = true;
            this.btnPatch.Click += new System.EventHandler(this.BtnPatch_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Processes:";
            // 
            // btnRefreshList
            // 
            this.btnRefreshList.Location = new System.Drawing.Point(149, 6);
            this.btnRefreshList.Name = "btnRefreshList";
            this.btnRefreshList.Size = new System.Drawing.Size(317, 23);
            this.btnRefreshList.TabIndex = 1;
            this.btnRefreshList.Text = "Refresh Process List";
            this.toolHelper.SetToolTip(this.btnRefreshList, "Refresh the process list.");
            this.btnRefreshList.UseVisualStyleBackColor = true;
            this.btnRefreshList.Click += new System.EventHandler(this.BtnRefreshList_Click);
            // 
            // toolHelper
            // 
            this.toolHelper.AutoPopDelay = 32766;
            this.toolHelper.InitialDelay = 500;
            this.toolHelper.ReshowDelay = 100;
            // 
            // chkClose
            // 
            this.chkClose.AutoSize = true;
            this.chkClose.Checked = true;
            this.chkClose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkClose.Location = new System.Drawing.Point(224, 224);
            this.chkClose.Name = "chkClose";
            this.chkClose.Size = new System.Drawing.Size(244, 22);
            this.chkClose.TabIndex = 4;
            this.chkClose.Text = "Close Borderless App after patch";
            this.toolHelper.SetToolTip(this.chkClose, "Choose whether this program will exit after patch\r\ncompletion.");
            this.chkClose.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(478, 313);
            this.Controls.Add(this.chkClose);
            this.Controls.Add(this.btnRefreshList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPatch);
            this.Controls.Add(this.lstProcesses);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Borderless App";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox lstProcesses;
        private System.Windows.Forms.Button btnPatch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRefreshList;
        private System.Windows.Forms.ToolTip toolHelper;
        private System.Windows.Forms.CheckBox chkClose;
    }
}

