namespace Demo
{
    partial class Demo
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.myDateTimePicker1 = new MyDateTimePicker.MyDateTimePicker();
            this.myMonthCalendar1 = new MyMonthCalendar.MyMonthCalendar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "MonthCalendar";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 236);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "DateTimePicker";
            // 
            // myDateTimePicker1
            // 
            this.myDateTimePicker1.BackColor = System.Drawing.Color.White;
            this.myDateTimePicker1.DayForeColor = System.Drawing.Color.Black;
            this.myDateTimePicker1.Location = new System.Drawing.Point(45, 253);
            this.myDateTimePicker1.Margin = new System.Windows.Forms.Padding(0);
            this.myDateTimePicker1.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            this.myDateTimePicker1.MinDate = new System.DateTime(1901, 3, 1, 0, 0, 0, 0);
            this.myDateTimePicker1.Name = "myDateTimePicker1";
            this.myDateTimePicker1.SelectedSolarDate = new System.DateTime(2015, 1, 28, 0, 0, 0, 0);
            this.myDateTimePicker1.ShowDateMode = MyDateTimePicker.MyDateTimePicker.ShowMode.SolarDate;
            this.myDateTimePicker1.Size = new System.Drawing.Size(172, 23);
            this.myDateTimePicker1.SplitLinesColor = System.Drawing.Color.OliveDrab;
            this.myDateTimePicker1.SplitLinesStyle = MyMonthCalendar.MyMonthCalendar.SplitLStyle.None;
            this.myDateTimePicker1.TabIndex = 1;
            this.myDateTimePicker1.TitleColor = System.Drawing.Color.Black;
            this.myDateTimePicker1.TrailingForeColor = System.Drawing.Color.LightGray;
            this.myDateTimePicker1.WeekBackColor = System.Drawing.Color.OliveDrab;
            this.myDateTimePicker1.WeekForeColor = System.Drawing.Color.White;
            // 
            // myMonthCalendar1
            // 
            this.myMonthCalendar1.BackColor = System.Drawing.Color.White;
            this.myMonthCalendar1.DayForeColor = System.Drawing.Color.Black;
            this.myMonthCalendar1.Location = new System.Drawing.Point(45, 41);
            this.myMonthCalendar1.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            this.myMonthCalendar1.MinDate = new System.DateTime(1901, 3, 1, 0, 0, 0, 0);
            this.myMonthCalendar1.Name = "myMonthCalendar1";
            this.myMonthCalendar1.SelectedSolarDate = new System.DateTime(2015, 1, 28, 0, 0, 0, 0);
            this.myMonthCalendar1.Size = new System.Drawing.Size(307, 170);
            this.myMonthCalendar1.SplitLinesColor = System.Drawing.Color.OliveDrab;
            this.myMonthCalendar1.SplitLinesStyle = MyMonthCalendar.MyMonthCalendar.SplitLStyle.None;
            this.myMonthCalendar1.TabIndex = 4;
            this.myMonthCalendar1.TitleColor = System.Drawing.Color.Black;
            this.myMonthCalendar1.TrailingForeColor = System.Drawing.Color.LightGray;
            this.myMonthCalendar1.WeekBackColor = System.Drawing.Color.OliveDrab;
            this.myMonthCalendar1.WeekForeColor = System.Drawing.Color.White;
            // 
            // Demo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 313);
            this.Controls.Add(this.myMonthCalendar1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.myDateTimePicker1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Demo";
            this.Text = "Demo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyDateTimePicker.MyDateTimePicker myDateTimePicker1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private MyMonthCalendar.MyMonthCalendar myMonthCalendar1;
    }
}

