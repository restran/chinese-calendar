namespace MyMonthCalendar
{
    sealed partial class MyMonthCalendar
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.picToday = new System.Windows.Forms.PictureBox();
            this.picYearNext = new System.Windows.Forms.PictureBox();
            this.picMonthNext = new System.Windows.Forms.PictureBox();
            this.picYearPrevious = new System.Windows.Forms.PictureBox();
            this.picMonthPrevious = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picToday)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picYearNext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMonthNext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picYearPrevious)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMonthPrevious)).BeginInit();
            this.SuspendLayout();
            // 
            // picToday
            // 
            this.picToday.BackColor = System.Drawing.Color.Transparent;
            this.picToday.Image = global::MyMonthCalendar.Properties.Resources.btnChange1;
            this.picToday.Location = new System.Drawing.Point(13, 153);
            this.picToday.Name = "picToday";
            this.picToday.Size = new System.Drawing.Size(13, 13);
            this.picToday.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picToday.TabIndex = 7;
            this.picToday.TabStop = false;
            this.toolTip.SetToolTip(this.picToday, "转到今天");
            this.picToday.MouseLeave += new System.EventHandler(this.picToday_MouseLeave);
            this.picToday.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picToday_MouseClick);
            this.picToday.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picToday_MouseDown);
            this.picToday.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picToday_MouseUp);
            this.picToday.MouseEnter += new System.EventHandler(this.picToday_MouseEnter);
            // 
            // picYearNext
            // 
            this.picYearNext.Image = global::MyMonthCalendar.Properties.Resources.btn_right1;
            this.picYearNext.Location = new System.Drawing.Point(279, 4);
            this.picYearNext.Name = "picYearNext";
            this.picYearNext.Size = new System.Drawing.Size(16, 16);
            this.picYearNext.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picYearNext.TabIndex = 4;
            this.picYearNext.TabStop = false;
            this.toolTip.SetToolTip(this.picYearNext, "下一年");
            this.picYearNext.MouseLeave += new System.EventHandler(this.picRight_MouseLeave);
            this.picYearNext.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picYearNext_MouseClick);
            this.picYearNext.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picRight_MouseDown);
            this.picYearNext.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picRight_MouseUp);
            this.picYearNext.MouseEnter += new System.EventHandler(this.picRight_MouseEnter);
            // 
            // picMonthNext
            // 
            this.picMonthNext.Image = global::MyMonthCalendar.Properties.Resources.btn_right1;
            this.picMonthNext.Location = new System.Drawing.Point(78, 4);
            this.picMonthNext.Name = "picMonthNext";
            this.picMonthNext.Size = new System.Drawing.Size(16, 16);
            this.picMonthNext.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picMonthNext.TabIndex = 0;
            this.picMonthNext.TabStop = false;
            this.toolTip.SetToolTip(this.picMonthNext, "下一月");
            this.picMonthNext.MouseLeave += new System.EventHandler(this.picRight_MouseLeave);
            this.picMonthNext.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMonthNext_MouseClick);
            this.picMonthNext.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picRight_MouseDown);
            this.picMonthNext.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picRight_MouseUp);
            this.picMonthNext.MouseEnter += new System.EventHandler(this.picRight_MouseEnter);
            // 
            // picYearPrevious
            // 
            this.picYearPrevious.Image = global::MyMonthCalendar.Properties.Resources.btn_left1;
            this.picYearPrevious.Location = new System.Drawing.Point(215, 4);
            this.picYearPrevious.Name = "picYearPrevious";
            this.picYearPrevious.Size = new System.Drawing.Size(16, 16);
            this.picYearPrevious.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picYearPrevious.TabIndex = 5;
            this.picYearPrevious.TabStop = false;
            this.toolTip.SetToolTip(this.picYearPrevious, "上一年");
            this.picYearPrevious.MouseLeave += new System.EventHandler(this.picLeft_MouseLeave);
            this.picYearPrevious.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picYearPrevious_MouseClick);
            this.picYearPrevious.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picLeft_MouseDown);
            this.picYearPrevious.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picLeft_MouseUp);
            this.picYearPrevious.MouseEnter += new System.EventHandler(this.picLeft_MouseEnter);
            // 
            // picMonthPrevious
            // 
            this.picMonthPrevious.Image = global::MyMonthCalendar.Properties.Resources.btn_left1;
            this.picMonthPrevious.Location = new System.Drawing.Point(12, 4);
            this.picMonthPrevious.Name = "picMonthPrevious";
            this.picMonthPrevious.Size = new System.Drawing.Size(16, 16);
            this.picMonthPrevious.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picMonthPrevious.TabIndex = 1;
            this.picMonthPrevious.TabStop = false;
            this.toolTip.SetToolTip(this.picMonthPrevious, "上一月");
            this.picMonthPrevious.MouseLeave += new System.EventHandler(this.picLeft_MouseLeave);
            this.picMonthPrevious.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMonthPrevious_MouseClick);
            this.picMonthPrevious.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picLeft_MouseDown);
            this.picMonthPrevious.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picLeft_MouseUp);
            this.picMonthPrevious.MouseEnter += new System.EventHandler(this.picLeft_MouseEnter);
            // 
            // MyMonthCalendar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.picToday);
            this.Controls.Add(this.picYearNext);
            this.Controls.Add(this.picMonthNext);
            this.Controls.Add(this.picYearPrevious);
            this.Controls.Add(this.picMonthPrevious);
            this.Name = "MyMonthCalendar";
            this.Size = new System.Drawing.Size(307, 170);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MyMonthCalendar_MouseMove);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MyMonthCalendar_MouseClick);
            this.SizeChanged += new System.EventHandler(this.MyMonthCalendar_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.picToday)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picYearNext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMonthNext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picYearPrevious)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMonthPrevious)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picMonthNext;
        private System.Windows.Forms.PictureBox picMonthPrevious;
        private System.Windows.Forms.PictureBox picYearNext;
        private System.Windows.Forms.PictureBox picYearPrevious;
        private System.Windows.Forms.PictureBox picToday;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
