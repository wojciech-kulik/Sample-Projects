namespace EmbeddedLibraries
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
            this.btnMethod1 = new System.Windows.Forms.Button();
            this.btnMethod2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnMethod1
            // 
            this.btnMethod1.Location = new System.Drawing.Point(12, 12);
            this.btnMethod1.Name = "btnMethod1";
            this.btnMethod1.Size = new System.Drawing.Size(125, 31);
            this.btnMethod1.TabIndex = 0;
            this.btnMethod1.Text = "Method from 1st DLL";
            this.btnMethod1.UseVisualStyleBackColor = true;
            this.btnMethod1.Click += new System.EventHandler(this.btnMethod1_Click);
            // 
            // btnMethod2
            // 
            this.btnMethod2.Location = new System.Drawing.Point(143, 12);
            this.btnMethod2.Name = "btnMethod2";
            this.btnMethod2.Size = new System.Drawing.Size(125, 31);
            this.btnMethod2.TabIndex = 1;
            this.btnMethod2.Text = "Method from 2nd DLL";
            this.btnMethod2.UseVisualStyleBackColor = true;
            this.btnMethod2.Click += new System.EventHandler(this.btnMethod2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 61);
            this.Controls.Add(this.btnMethod2);
            this.Controls.Add(this.btnMethod1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Embedded Libraries";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMethod1;
        private System.Windows.Forms.Button btnMethod2;
    }
}

