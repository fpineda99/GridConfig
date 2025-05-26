namespace GridConfigV2
{
    partial class Form1
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
            this.gridInputGroupBox = new System.Windows.Forms.GroupBox();
            this.xGridInputLabel = new System.Windows.Forms.Label();
            this.yGridInputLabel = new System.Windows.Forms.Label();
            this.zGridInputLabel = new System.Windows.Forms.Label();
            this.xGridInputTextBox = new System.Windows.Forms.TextBox();
            this.yGridInputTextBox = new System.Windows.Forms.TextBox();
            this.zGridInputTextBox = new System.Windows.Forms.TextBox();
            this.CreateGridbtn = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.gridInputGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridInputGroupBox
            // 
            this.gridInputGroupBox.Controls.Add(this.xGridInputLabel);
            this.gridInputGroupBox.Controls.Add(this.yGridInputLabel);
            this.gridInputGroupBox.Controls.Add(this.zGridInputLabel);
            this.gridInputGroupBox.Controls.Add(this.xGridInputTextBox);
            this.gridInputGroupBox.Controls.Add(this.yGridInputTextBox);
            this.gridInputGroupBox.Controls.Add(this.zGridInputTextBox);
            this.gridInputGroupBox.Controls.Add(this.CreateGridbtn);
            this.gridInputGroupBox.Location = new System.Drawing.Point(12, 12);
            this.gridInputGroupBox.Name = "gridInputGroupBox";
            this.gridInputGroupBox.Size = new System.Drawing.Size(450, 155);
            this.gridInputGroupBox.TabIndex = 8;
            this.gridInputGroupBox.TabStop = false;
            this.gridInputGroupBox.Text = "Custom Grid Input";
            // 
            // xGridInputLabel
            // 
            this.xGridInputLabel.AutoSize = true;
            this.xGridInputLabel.Location = new System.Drawing.Point(17, 22);
            this.xGridInputLabel.Name = "xGridInputLabel";
            this.xGridInputLabel.Size = new System.Drawing.Size(14, 13);
            this.xGridInputLabel.TabIndex = 0;
            this.xGridInputLabel.Text = "X";
            // 
            // yGridInputLabel
            // 
            this.yGridInputLabel.AutoSize = true;
            this.yGridInputLabel.Location = new System.Drawing.Point(17, 51);
            this.yGridInputLabel.Name = "yGridInputLabel";
            this.yGridInputLabel.Size = new System.Drawing.Size(14, 13);
            this.yGridInputLabel.TabIndex = 1;
            this.yGridInputLabel.Text = "Y";
            // 
            // zGridInputLabel
            // 
            this.zGridInputLabel.AutoSize = true;
            this.zGridInputLabel.Location = new System.Drawing.Point(17, 80);
            this.zGridInputLabel.Name = "zGridInputLabel";
            this.zGridInputLabel.Size = new System.Drawing.Size(14, 13);
            this.zGridInputLabel.TabIndex = 2;
            this.zGridInputLabel.Text = "Z";
            // 
            // xGridInputTextBox
            // 
            this.xGridInputTextBox.Location = new System.Drawing.Point(37, 19);
            this.xGridInputTextBox.Name = "xGridInputTextBox";
            this.xGridInputTextBox.Size = new System.Drawing.Size(390, 20);
            this.xGridInputTextBox.TabIndex = 3;
            // 
            // yGridInputTextBox
            // 
            this.yGridInputTextBox.Location = new System.Drawing.Point(37, 48);
            this.yGridInputTextBox.Name = "yGridInputTextBox";
            this.yGridInputTextBox.Size = new System.Drawing.Size(390, 20);
            this.yGridInputTextBox.TabIndex = 4;
            // 
            // zGridInputTextBox
            // 
            this.zGridInputTextBox.Location = new System.Drawing.Point(37, 77);
            this.zGridInputTextBox.Name = "zGridInputTextBox";
            this.zGridInputTextBox.Size = new System.Drawing.Size(390, 20);
            this.zGridInputTextBox.TabIndex = 5;
            // 
            // CreateGridbtn
            // 
            this.CreateGridbtn.Location = new System.Drawing.Point(37, 112);
            this.CreateGridbtn.Name = "CreateGridbtn";
            this.CreateGridbtn.Size = new System.Drawing.Size(79, 28);
            this.CreateGridbtn.TabIndex = 6;
            this.CreateGridbtn.Text = "Create Grid";
            this.CreateGridbtn.UseVisualStyleBackColor = true;
            this.CreateGridbtn.Click += new System.EventHandler(this.CreateGridbtn_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 178);
            this.Controls.Add(this.gridInputGroupBox);
            this.Name = "Form1";
            this.Text = "Grid Configuration";
            this.gridInputGroupBox.ResumeLayout(false);
            this.gridInputGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gridInputGroupBox;
        private System.Windows.Forms.Label xGridInputLabel;
        private System.Windows.Forms.Label yGridInputLabel;
        private System.Windows.Forms.Label zGridInputLabel;
        private System.Windows.Forms.TextBox xGridInputTextBox;
        private System.Windows.Forms.TextBox yGridInputTextBox;
        private System.Windows.Forms.TextBox zGridInputTextBox;
        private System.Windows.Forms.Button CreateGridbtn;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}

