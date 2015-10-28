namespace SampleApp
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
            this.btnSerializeWrong = new System.Windows.Forms.Button();
            this.btnDeserialize = new System.Windows.Forms.Button();
            this.btnSerializeOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSerializeWrong
            // 
            this.btnSerializeWrong.Location = new System.Drawing.Point(26, 51);
            this.btnSerializeWrong.Name = "btnSerializeWrong";
            this.btnSerializeWrong.Size = new System.Drawing.Size(75, 23);
            this.btnSerializeWrong.TabIndex = 1;
            this.btnSerializeWrong.Text = "Serialize";
            this.btnSerializeWrong.UseVisualStyleBackColor = true;
            this.btnSerializeWrong.Click += new System.EventHandler(this.btnSerializeWrong_Click);
            // 
            // btnDeserialize
            // 
            this.btnDeserialize.Location = new System.Drawing.Point(26, 80);
            this.btnDeserialize.Name = "btnDeserialize";
            this.btnDeserialize.Size = new System.Drawing.Size(194, 23);
            this.btnDeserialize.TabIndex = 2;
            this.btnDeserialize.Text = "Deserialize";
            this.btnDeserialize.UseVisualStyleBackColor = true;
            this.btnDeserialize.Click += new System.EventHandler(this.btnDeserialize_Click);
            // 
            // btnSerializeOK
            // 
            this.btnSerializeOK.Location = new System.Drawing.Point(145, 51);
            this.btnSerializeOK.Name = "btnSerializeOK";
            this.btnSerializeOK.Size = new System.Drawing.Size(75, 23);
            this.btnSerializeOK.TabIndex = 3;
            this.btnSerializeOK.Text = "Serialize";
            this.btnSerializeOK.UseVisualStyleBackColor = true;
            this.btnSerializeOK.Click += new System.EventHandler(this.btnSerializeOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Exception:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(169, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "OK:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(252, 115);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSerializeOK);
            this.Controls.Add(this.btnDeserialize);
            this.Controls.Add(this.btnSerializeWrong);
            this.Name = "Form1";
            this.Text = "Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSerializeWrong;
        private System.Windows.Forms.Button btnDeserialize;
        private System.Windows.Forms.Button btnSerializeOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

