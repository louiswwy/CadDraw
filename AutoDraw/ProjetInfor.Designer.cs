namespace AutoDraw
{
    partial class ProjetInfor
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
            this.T_Project_Name = new System.Windows.Forms.TextBox();
            this.T_Print_Name = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.T_Project_Phase = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.T_Print_Chapter = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.T_Print_Pattren = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "工程名称";
            // 
            // T_Project_Name
            // 
            this.T_Project_Name.Location = new System.Drawing.Point(62, 16);
            this.T_Project_Name.Name = "T_Project_Name";
            this.T_Project_Name.Size = new System.Drawing.Size(223, 21);
            this.T_Project_Name.TabIndex = 1;
            // 
            // T_Print_Name
            // 
            this.T_Print_Name.Location = new System.Drawing.Point(62, 70);
            this.T_Print_Name.Name = "T_Print_Name";
            this.T_Print_Name.Size = new System.Drawing.Size(223, 21);
            this.T_Print_Name.TabIndex = 2;
            this.T_Print_Name.TextChanged += new System.EventHandler(this.T_Print_Name_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "图纸名称";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(29, 158);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "确认";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(171, 157);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 24);
            this.button2.TabIndex = 5;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.T_Print_Pattren);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.T_Project_Phase);
            this.panel1.Controls.Add(this.statusStrip1);
            this.panel1.Controls.Add(this.T_Print_Chapter);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.T_Project_Name);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.T_Print_Name);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(297, 206);
            this.panel1.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "阶    段";
            // 
            // T_Project_Phase
            // 
            this.T_Project_Phase.Location = new System.Drawing.Point(62, 43);
            this.T_Project_Phase.Name = "T_Project_Phase";
            this.T_Project_Phase.Size = new System.Drawing.Size(223, 21);
            this.T_Project_Phase.TabIndex = 8;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 184);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(297, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // T_Print_Chapter
            // 
            this.T_Print_Chapter.Location = new System.Drawing.Point(62, 97);
            this.T_Print_Chapter.Name = "T_Print_Chapter";
            this.T_Print_Chapter.Size = new System.Drawing.Size(223, 21);
            this.T_Print_Chapter.TabIndex = 3;
            this.T_Print_Chapter.TextChanged += new System.EventHandler(this.T_Print_Chapter_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "单    元";
            // 
            // T_Print_Pattren
            // 
            this.T_Print_Pattren.Location = new System.Drawing.Point(62, 124);
            this.T_Print_Pattren.Name = "T_Print_Pattren";
            this.T_Print_Pattren.Size = new System.Drawing.Size(223, 21);
            this.T_Print_Pattren.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 127);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "图号规则";
            // 
            // ProjetInfor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 206);
            this.Controls.Add(this.panel1);
            this.MaximumSize = new System.Drawing.Size(313, 245);
            this.MinimumSize = new System.Drawing.Size(313, 245);
            this.Name = "ProjetInfor";
            this.Text = "项目信息";
            this.Load += new System.EventHandler(this.ProjetInfor_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox T_Project_Name;
        private System.Windows.Forms.TextBox T_Print_Name;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox T_Print_Chapter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox T_Project_Phase;
        private System.Windows.Forms.TextBox T_Print_Pattren;
        private System.Windows.Forms.Label label5;
    }
}