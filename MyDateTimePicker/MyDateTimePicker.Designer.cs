namespace MyDateTimePicker
{
    sealed partial class MyDateTimePicker
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
            this.picDropDown = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picDropDown)).BeginInit();
            this.SuspendLayout();
            // 
            // picDropDown
            // 
            this.picDropDown.BackColor = System.Drawing.Color.Transparent;
            this.picDropDown.Image = global::Escape.Controls.Properties.Resources._1;
            this.picDropDown.Location = new System.Drawing.Point(127, 1);
            this.picDropDown.Margin = new System.Windows.Forms.Padding(1);
            this.picDropDown.Name = "picDropDown";
            this.picDropDown.Padding = new System.Windows.Forms.Padding(2);
            this.picDropDown.Size = new System.Drawing.Size(19, 20);
            this.picDropDown.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDropDown.TabIndex = 1;
            this.picDropDown.TabStop = false;
            this.picDropDown.MouseLeave += new System.EventHandler(this.picDropDown_MouseLeave);
            this.picDropDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picDropDown_MouseDown);
            this.picDropDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picDropDown_MouseUp);
            this.picDropDown.MouseEnter += new System.EventHandler(this.picDropDown_MouseEnter);
            // 
            // MyDateTimePicker
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.picDropDown);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MyDateTimePicker";
            this.Size = new System.Drawing.Size(147, 21);
            this.Click += new System.EventHandler(this.MyDateTimePicker_Click);
            this.FontChanged += new System.EventHandler(this.MyDateTimePicker_FontChanged);
            ((System.ComponentModel.ISupportInitialize)(this.picDropDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picDropDown;



    }
}
