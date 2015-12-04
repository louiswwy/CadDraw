namespace AutoDraw
{
    partial class MainInterface
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.比例尺ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.图签名称ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.比例尺ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.导入图块ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.button2 = new System.Windows.Forms.Button();
            this.TypeWayPoint = new System.Windows.Forms.ComboBox();
            this.B_Add = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.B_Draw = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.类型 = new System.Windows.Forms.Label();
            this.B_AddWayPoint = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 301);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(761, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.设置ToolStripMenuItem,
            this.导入图块ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(761, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 设置ToolStripMenuItem
            // 
            this.设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.比例尺ToolStripMenuItem});
            this.设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            this.设置ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.设置ToolStripMenuItem.Text = "设置";
            // 
            // 比例尺ToolStripMenuItem
            // 
            this.比例尺ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.图签名称ToolStripMenuItem,
            this.比例尺ToolStripMenuItem1});
            this.比例尺ToolStripMenuItem.Name = "比例尺ToolStripMenuItem";
            this.比例尺ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.比例尺ToolStripMenuItem.Text = "图纸属性";
            // 
            // 图签名称ToolStripMenuItem
            // 
            this.图签名称ToolStripMenuItem.Name = "图签名称ToolStripMenuItem";
            this.图签名称ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.图签名称ToolStripMenuItem.Text = "图签名称";
            this.图签名称ToolStripMenuItem.Click += new System.EventHandler(this.图签名称ToolStripMenuItem_Click);
            // 
            // 比例尺ToolStripMenuItem1
            // 
            this.比例尺ToolStripMenuItem1.Name = "比例尺ToolStripMenuItem1";
            this.比例尺ToolStripMenuItem1.Size = new System.Drawing.Size(124, 22);
            this.比例尺ToolStripMenuItem1.Text = "比例尺";
            this.比例尺ToolStripMenuItem1.Click += new System.EventHandler(this.比例尺ToolStripMenuItem1_Click);
            // 
            // 导入图块ToolStripMenuItem
            // 
            this.导入图块ToolStripMenuItem.Name = "导入图块ToolStripMenuItem";
            this.导入图块ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.导入图块ToolStripMenuItem.Text = "导入图块";
            this.导入图块ToolStripMenuItem.Click += new System.EventHandler(this.导入图块ToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(761, 276);
            this.panel1.TabIndex = 3;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.textBox2);
            this.splitContainer.Panel1.Controls.Add(this.label2);
            this.splitContainer.Panel1.Controls.Add(this.label1);
            this.splitContainer.Panel1.Controls.Add(this.textBox1);
            this.splitContainer.Panel1.Controls.Add(this.B_AddWayPoint);
            this.splitContainer.Panel1.Controls.Add(this.类型);
            this.splitContainer.Panel1.Controls.Add(this.treeView1);
            this.splitContainer.Panel1.Controls.Add(this.button2);
            this.splitContainer.Panel1.Controls.Add(this.TypeWayPoint);
            this.splitContainer.Panel1.Controls.Add(this.B_Add);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.button1);
            this.splitContainer.Panel2.Controls.Add(this.B_Draw);
            this.splitContainer.Size = new System.Drawing.Size(761, 276);
            this.splitContainer.SplitterDistance = 560;
            this.splitContainer.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(187, 216);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "？";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // TypeWayPoint
            // 
            this.TypeWayPoint.FormattingEnabled = true;
            this.TypeWayPoint.Location = new System.Drawing.Point(57, 16);
            this.TypeWayPoint.Name = "TypeWayPoint";
            this.TypeWayPoint.Size = new System.Drawing.Size(121, 20);
            this.TypeWayPoint.TabIndex = 1;
            // 
            // B_Add
            // 
            this.B_Add.Location = new System.Drawing.Point(34, 217);
            this.B_Add.Name = "B_Add";
            this.B_Add.Size = new System.Drawing.Size(75, 23);
            this.B_Add.TabIndex = 0;
            this.B_Add.Text = "添加";
            this.B_Add.UseVisualStyleBackColor = true;
            this.B_Add.Click += new System.EventHandler(this.B_Add_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(20, 159);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // B_Draw
            // 
            this.B_Draw.Location = new System.Drawing.Point(20, 217);
            this.B_Draw.Name = "B_Draw";
            this.B_Draw.Size = new System.Drawing.Size(75, 23);
            this.B_Draw.TabIndex = 0;
            this.B_Draw.Text = "绘图";
            this.B_Draw.UseVisualStyleBackColor = true;
            this.B_Draw.Click += new System.EventHandler(this.B_Draw_Click);
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(217, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(121, 207);
            this.treeView1.TabIndex = 3;
            // 
            // 类型
            // 
            this.类型.AutoSize = true;
            this.类型.Location = new System.Drawing.Point(10, 19);
            this.类型.Name = "类型";
            this.类型.Size = new System.Drawing.Size(29, 12);
            this.类型.TabIndex = 4;
            this.类型.Text = "类型";
            // 
            // B_AddWayPoint
            // 
            this.B_AddWayPoint.Location = new System.Drawing.Point(187, 16);
            this.B_AddWayPoint.Name = "B_AddWayPoint";
            this.B_AddWayPoint.Size = new System.Drawing.Size(20, 74);
            this.B_AddWayPoint.TabIndex = 5;
            this.B_AddWayPoint.Text = "+";
            this.B_AddWayPoint.UseVisualStyleBackColor = true;
            this.B_AddWayPoint.Click += new System.EventHandler(this.B_AddWayPoint_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(57, 42);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(121, 21);
            this.textBox1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "名称";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "里程";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(57, 69);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(121, 21);
            this.textBox2.TabIndex = 9;
            // 
            // MainInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 323);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainInterface";
            this.Text = "MainInterface";
            this.Load += new System.EventHandler(this.MainInterface_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 设置ToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Button B_Add;
        private System.Windows.Forms.Button B_Draw;
        private System.Windows.Forms.ToolStripMenuItem 比例尺ToolStripMenuItem;
        private System.Windows.Forms.ComboBox TypeWayPoint;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripMenuItem 图签名称ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 比例尺ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 导入图块ToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button B_AddWayPoint;
        private System.Windows.Forms.Label 类型;
    }
}