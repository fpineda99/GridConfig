namespace GridConfig
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtX = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCreateGrid = new System.Windows.Forms.Button();
            this.txtY = new System.Windows.Forms.TextBox();
            this.txtZ = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(113, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Coordinates and Labels";
            // 
            // txtX
            // 
            this.txtX.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.txtX.Location = new System.Drawing.Point(32, 35);
            this.txtX.Name = "txtX";
            this.txtX.Size = new System.Drawing.Size(308, 20);
            this.txtX.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "X";
            // 
            // btnCreateGrid
            // 
            this.btnCreateGrid.Location = new System.Drawing.Point(136, 120);
            this.btnCreateGrid.Name = "btnCreateGrid";
            this.btnCreateGrid.Size = new System.Drawing.Size(75, 24);
            this.btnCreateGrid.TabIndex = 14;
            this.btnCreateGrid.Text = "Create Grid";
            this.btnCreateGrid.UseVisualStyleBackColor = true;
            this.btnCreateGrid.Click += new System.EventHandler(this.btnCreateGrid_Click);
            // 
            // txtY
            // 
            this.txtY.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.txtY.Location = new System.Drawing.Point(32, 64);
            this.txtY.Name = "txtY";
            this.txtY.Size = new System.Drawing.Size(308, 20);
            this.txtY.TabIndex = 25;
            // 
            // txtZ
            // 
            this.txtZ.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.txtZ.Location = new System.Drawing.Point(32, 94);
            this.txtZ.Name = "txtZ";
            this.txtZ.Size = new System.Drawing.Size(308, 20);
            this.txtZ.TabIndex = 26;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Y";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label4.Location = new System.Drawing.Point(12, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Z";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(350, 156);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtZ);
            this.Controls.Add(this.txtY);
            this.Controls.Add(this.btnCreateGrid);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtX);
            this.Controls.Add(this.label1);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Name = "Form1";
            this.Text = "Grid Configuration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCreateGrid;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.TextBox txtZ;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

