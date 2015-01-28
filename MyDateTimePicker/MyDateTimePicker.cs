using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Escape.Controls.Properties;
using MouseKeyboardLibrary;

namespace MyDateTimePicker
{
    public sealed partial class MyDateTimePicker : UserControl
    {
        #region 用到的类型

        /// <summary>
        ///     日期字符串的显示模式
        /// </summary>
        public enum ShowMode
        {
            LunarDate, //农历
            SolarDate //公历
        }

        #endregion

        #region 用到的变量

        private Color colorBorder = Color.FromArgb(127, 157, 185); //日期的边框
        private Color colorStr; //字体的颜色
        private ChineseCalendar.ChineseCalendar date = new ChineseCalendar.ChineseCalendar(DateTime.Today); //选择的日期
        private string dateStr; //显示的日期
        private bool enabled = true; //控件是否可用
        private Form formCalendar = new Form(); //新建一个窗体用来显示monthCalendar
        private bool isDrag = true; //是否拖动修改大小
        private bool isEnterPicDropDown; //是否进入picDropDown
        private bool leaveMonthCalendar = true; //判断是否离开monthCalendar
        private Point locationForm;
        private MyMonthCalendar.MyMonthCalendar monthCalendar = new MyMonthCalendar.MyMonthCalendar(); //月历选择控件
        private MouseHook mouseHook = new MouseHook(); //鼠标钩子
        private Size preSize; //记录控件之前的大小
        private ShowMode showDateMode = ShowMode.SolarDate; //日期默认显示为公历

        #endregion

        #region 构造函数

        public MyDateTimePicker()
        {
            InitializeComponent();
            CreateGraphics(); //创建Graphics对象
            BackColor = Color.White;
            preSize = Size; //记录控件之前的大小
            Paint += MyDateTimePicker_Paint;
            Resize += MyDateTimePicker_Resize;
            monthCalendar.MouseEnter += monthCalendar_MouseEnter; //鼠标进入monthCalendar
            monthCalendar.MouseLeave += monthCalendar_MouseLeave; //鼠标离开monthCalendar
            monthCalendar.DateSelected += monthCalendar_DateSelected; //用户选择了日期

            mouseHook.MouseDown += mouseHook_MouseDown; //捕捉鼠标按下事件

            formCalendar.Visible = false;
            formCalendar.TopMost = true;
            formCalendar.Controls.Add(monthCalendar);
            formCalendar.Size = monthCalendar.Size;
            formCalendar.FormBorderStyle = FormBorderStyle.None;
            formCalendar.StartPosition = FormStartPosition.Manual;
            formCalendar.ShowInTaskbar = false;
        }

        #endregion

        #region 事件

        #region mouseHook_MouseDown

        /// <summary>
        ///     在控件外按下鼠标时（最先调用此函数）formCalendar和monthCalendar都将隐藏
        /// </summary>
        private void mouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            if (leaveMonthCalendar)
            {
                if (isEnterPicDropDown == false)
                {
                    //鼠标不在picDropDown上面时才需要隐藏
                    formCalendar.Visible = false;
                    monthCalendar.Visible = false;
                    mouseHook.Stop(); //不再捕捉鼠标事件
                }
            }
        }

        #endregion

        #region monthCalendar_DateSelected

        /// <summary>
        ///     用户选择了日期，修改选择的日期，并将月历隐藏
        /// </summary>
        private void monthCalendar_DateSelected(object sender, EventArgs e)
        {
            date = monthCalendar.SelectedLunarDate; //修改选择的日期
            formCalendar.Visible = false;
            monthCalendar.Visible = false;
            Refresh();
        }

        #endregion

        #region monthCalendar_MouseLeave

        private void monthCalendar_MouseLeave(object sender, EventArgs e)
        {
            leaveMonthCalendar = true; //鼠标离开monthCalendar
        }

        #endregion

        #region monthCalendar_MouseEnter

        private void monthCalendar_MouseEnter(object sender, EventArgs e)
        {
            leaveMonthCalendar = false; //鼠标进入monthCalendar
        }

        #endregion

        #region MyDateTimePicker_Paint

        /// <summary>
        ///     绘制组件
        /// </summary>
        private void MyDateTimePicker_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            //绘制边框
            if (enabled)
            {
                e.Graphics.DrawRectangle(new Pen(colorBorder), new Rectangle(0, 0,
                    Size.Width - 1, Size.Height - 1));
                colorStr = ForeColor;
            }
            else
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(175, 175, 175)), new Rectangle(0, 0,
                    Size.Width - 1, Size.Height - 1));
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(102, Color.LightGray)), 1, 1, Size.Width - 3,
                    Size.Height - 3);
                colorStr = Color.FromArgb(109, 109, 109);
            }

            //修改下拉按钮的大小和位置
            picDropDown.Size = new Size(Size.Height - 4, Size.Height - 2);
            picDropDown.Location = new Point(Size.Width - 1 - picDropDown.Width, 1);

            if (showDateMode == ShowMode.SolarDate)
            {
                dateStr = date.SolarDate.Year + "年 " +
                          date.SolarDate.Month + "月" +
                          date.SolarDate.Day + "日";
            }
            else
            {
                dateStr = date.ToString();
            }
            //绘制日期字符串
            int y = Font.Name == "微软雅黑" ? 3 : 4;
            e.Graphics.DrawString(dateStr, Font, new SolidBrush(colorStr),
                new Point(5, y));
        }

        #endregion

        #region MyDateTimePicker_Resize

        /// <summary>
        ///     拖动时修改长度
        /// </summary>
        private void MyDateTimePicker_Resize(object sender, EventArgs e)
        {
            //编辑时拖动改变大小 
            if (isDrag)
            {
                preSize.Width = Size.Width;
                Size = preSize; //不可修改高度
                Refresh();
            }
            else
            {
                isDrag = true;
            }
        }

        #endregion

        #region MyDateTimePicker_Click

        /// <summary>
        ///     在控件上的非月历区域单击鼠标将隐藏月历
        /// </summary>
        private void MyDateTimePicker_Click(object sender, EventArgs e)
        {
            if (leaveMonthCalendar)
            {
                monthCalendar.Visible = false;
                formCalendar.Visible = false;
                mouseHook.Stop(); //不再捕捉鼠标事件
            }
        }

        #endregion

        #region 下拉按钮鼠标事件

        #region picDropDown_MouseEnter

        private void picDropDown_MouseEnter(object sender, EventArgs e)
        {
            picDropDown.Image = Resources._2;
            isEnterPicDropDown = true;
        }

        #endregion

        #region picDropDown_MouseLeave

        private void picDropDown_MouseLeave(object sender, EventArgs e)
        {
            picDropDown.Image = Resources._1;
            isEnterPicDropDown = false;
        }

        #endregion

        #region picDropDown_MouseUp

        private void picDropDown_MouseUp(object sender, MouseEventArgs e)
        {
            picDropDown.Image = Resources._2;
        }

        #endregion

        #region picDropDown_MouseDown

        /// <summary>
        ///     按下下拉按钮
        /// </summary>
        private void picDropDown_MouseDown(object sender, MouseEventArgs e)
        {
            picDropDown.Image = Resources._3; //鼠标按下时修改外观
            Point locationControl = Location; //控件在父容器的位置

            if (monthCalendar.Visible)
            {
                //在monthCalendar显示的情况下按下就隐藏
                monthCalendar.Visible = false;
                formCalendar.Visible = false;
            }
            else
            {
                Point mouseP = MousePosition; //鼠标在屏幕上的坐标
                Rectangle screenSize = Screen.PrimaryScreen.WorkingArea; //当前屏幕的大小不包括任务栏
                Point controlInScreen = PointToScreen(new Point(0, 0)); //控件在屏幕上的坐标
                if (mouseP.X > Size.Width)
                {
                    if (screenSize.Width - mouseP.X > monthCalendar.Size.Width -
                        Size.Width)
                    {
                        locationForm.X = controlInScreen.X;
                    }
                    else
                    {
                        //右边界
                        locationForm.X = screenSize.Width - monthCalendar.Size.Width - 2;
                    }
                }
                else
                {
                    //左边界
                    locationForm.X = 2;
                }

                if ((screenSize.Height + 30 - mouseP.Y) >
                    (Size.Height + monthCalendar.Size.Height))
                {
                    locationForm.Y = controlInScreen.Y + Size.Height + 2;
                }
                else
                {
                    //下边界
                    locationForm.Y = controlInScreen.Y - monthCalendar.Size.Height - 2;
                }

                formCalendar.Location = locationForm; //月历窗口的坐标

                monthCalendar.Visible = true;
                formCalendar.Visible = true;
                mouseHook.Start(); //启动鼠标钩子
            }
        }

        #endregion

        #endregion

        #endregion

        #region 属性

        #region 行为

        #region 日期显示为农历还是是公历

        /// <summary>
        ///     日期显示为公历还是是农历
        /// </summary>
        [Description("日期显示为公历还是是农历")]
        [Category("行为")]
        public ShowMode ShowDateMode
        {
            get { return showDateMode; }
            set
            {
                if (showDateMode != value)
                {
                    showDateMode = value;
                    Refresh();
                }
            }
        }

        #endregion

        #region 选中的农历日期

        /// <summary>
        ///     选中的农历日期
        /// </summary>
        [Description("选中的农历日期")] //显示在属性设计视图中的描述
        [Category("行为")] //类别
        public ChineseCalendar.ChineseCalendar SelectedLunarDate
        {
            set
            {
                date = value;
                Refresh();
            }
            get { return date; }
        }

        #endregion

        #region 选中的公历日期

        /// <summary>
        ///     选中的阳历日期
        /// </summary>
        [Description("选中的公历日期")]
        [Category("行为")]
        public DateTime SelectedSolarDate
        {
            set
            {
                date.SolarDate = value;
                Refresh();
            }
            get { return date.SolarDate; }
        }

        #endregion

        #region 最小日期

        /// <summary>
        ///     最小日期
        /// </summary>
        [Description("最小日期")]
        [Category("行为")]
        public DateTime MinDate
        {
            set { monthCalendar.MinDate = value; }
            get { return monthCalendar.MinDate; }
        }

        #endregion

        #region 最大日期

        /// <summary>
        ///     最大日期
        /// </summary>
        [Description("最大日期")]
        [Category("行为")]
        public DateTime MaxDate
        {
            set { monthCalendar.MaxDate = value; }
            get { return monthCalendar.MaxDate; }
        }

        #endregion

        #region Enabled

        /// <summary>
        ///     控件是否可用
        /// </summary>
        [Description("指示控件是否可用")]
        [Category("行为")]
        public new bool Enabled
        {
            set
            {
                enabled = value;
                if (enabled == false)
                {
                    monthCalendar.Visible = false;
                    picDropDown.Enabled = false;
                    picDropDown.Image = Resources.enabled; //写在paint里会导致不停的重绘
                    if (mouseHook.IsStarted)
                        mouseHook.Stop();
                }
                else
                {
                    picDropDown.Enabled = true;
                    picDropDown.Image = Resources._1; //写在paint里会导致不停的重绘
                }

                Refresh();
            }
            get { return enabled; }
        }

        #endregion

        #endregion

        #region 外观

        #region 星期的前景色

        /// <summary>
        ///     星期的前景色
        /// </summary>
        [Description("星期的前景色")]
        [Category("外观")]
        public Color WeekForeColor
        {
            set { monthCalendar.WeekForeColor = value; }
            get { return monthCalendar.WeekForeColor; }
        }

        #endregion

        #region 星期的背景色

        /// <summary>
        ///     星期的背景色
        /// </summary>
        [Description("星期的背景色")]
        [Category("外观")]
        public Color WeekBackColor
        {
            set { monthCalendar.WeekBackColor = value; }
            get { return monthCalendar.WeekBackColor; }
        }

        #endregion

        #region 用于显示在月历上出现的上个月和下个月的颜色

        /// <summary>
        ///     用于显示在月历上出现的上个月和下个月的颜色
        /// </summary>
        [Description("用于显示在月历上出现的上个月和下个月的颜色")]
        [Category("外观")]
        public Color TrailingForeColor
        {
            set { monthCalendar.TrailingForeColor = value; }
            get { return monthCalendar.TrailingForeColor; }
        }

        #endregion

        #region 用于显示在月历上出现的这个月的颜色

        /// <summary>
        ///     用于显示在月历上出现的这个月的颜色
        /// </summary>
        [Description("用于显示在月历上出现的这个月的颜色")]
        [Category("外观")]
        public Color DayForeColor
        {
            set { monthCalendar.DayForeColor = value; }
            get { return monthCalendar.DayForeColor; }
        }

        #endregion

        #region 年份和月份的字体颜色

        /// <summary>
        ///     年份和月份的字体颜色
        /// </summary>
        [Description("年份和月份的字体颜色")]
        [Category("外观")]
        public Color TitleColor
        {
            set { monthCalendar.TitleColor = value; }
            get { return monthCalendar.TitleColor; }
        }

        #endregion

        #region 日期分割线颜色

        /// <summary>
        ///     日期分割线颜色
        /// </summary>
        [Description("日期分割线颜色")]
        [Category("外观")]
        public Color SplitLinesColor
        {
            set { monthCalendar.SplitLinesColor = value; }
            get { return monthCalendar.SplitLinesColor; }
        }

        #endregion

        #region 分割线风格

        /// <summary>
        ///     分割线风格
        /// </summary>
        [Description("分割线风格")]
        [Category("外观")]
        public MyMonthCalendar.MyMonthCalendar.SplitLStyle SplitLinesStyle
        {
            set { monthCalendar.SplitLinesStyle = value; }
            get { return monthCalendar.SplitLinesStyle; }
        }

        #endregion

        #region MyDateTimePicker_FontChanged

        /// <summary>
        ///     字体发生变化时改变控件大小
        /// </summary>
        private void MyDateTimePicker_FontChanged(object sender, EventArgs e)
        {
            isDrag = false; //不是拖动修改大小
            int width = Size.Width;
            Size = new Size(width, FontHeight + 7);
        }

        #endregion

        #endregion

        #endregion
    }
}