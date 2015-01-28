using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using MyMonthCalendar.Properties;

namespace MyMonthCalendar
{
    [ToolboxBitmap(typeof (MyMonthCalendar), "MyMonthCalendar"), ToolboxItem(true),
     ToolboxItemFilter("MyMonthCalendar"), Description("MonthCalendar Control")]
    public sealed partial class MyMonthCalendar : UserControl
    {
        #region 用到的类型

        #region SolarHolidayStruct

        /// <summary>
        ///     公历节日
        /// </summary>
        private struct SolarHolidayStruct
        {
            public int Day;
            public string HolidayName;
            public int Month;

            public SolarHolidayStruct(int month, int day, string name)
            {
                Month = month;
                Day = day;
                HolidayName = name;
            }
        }

        #endregion

        #region LunarHolidayStruct

        /// <summary>
        ///     农历节日
        /// </summary>
        private struct LunarHolidayStruct
        {
            public int Day;
            public string HolidayName;
            public int Month;

            public LunarHolidayStruct(int month, int day, string name)
            {
                Month = month;
                Day = day;
                HolidayName = name;
            }
        }

        #endregion

        #region DayInfo

        /// <summary>
        ///     月历中天的信息
        /// </summary>
        private struct DayInfo
        {
            public string day; //用来显示公历
            public bool hightLightL; //高亮显示农历部分
            public bool hightLightS; //高亮显示公历部分
            public string holidayInfo; //节日信息
            public string lunarDay; //用来显示农历或节日

            public DayInfo(string _day, string _lunarDay, string _holidayInfo, bool _hightLightS, bool _hightLightL)
            {
                day = _day;
                lunarDay = _lunarDay;
                holidayInfo = _holidayInfo;
                hightLightS = _hightLightS;
                hightLightL = _hightLightL;
            }
        }

        #endregion

        #region SplitLStyle

        /// <summary>
        ///     分割线的风格
        /// </summary>
        public enum SplitLStyle
        {
            None, //没有行和列
            OnlyRow, //只有行
            RowAndColumn, //行和列
        }

        #endregion

        #endregion

        #region 节日数据

        /// <remarks>
        ///     这里只显示部分重要的节日
        /// </remarks>
        private static readonly SolarHolidayStruct[] solarHolidayInfo =
        {
            new SolarHolidayStruct(1, 1, "元旦"),
            new SolarHolidayStruct(5, 1, "劳动"),
            new SolarHolidayStruct(10, 1, "国庆"),
            new SolarHolidayStruct(12, 25, "圣诞")
        };

        private static readonly LunarHolidayStruct[] lunarHolidayInfo =
        {
            new LunarHolidayStruct(1, 1, "春节"),
            new LunarHolidayStruct(1, 15, "元宵"),
            new LunarHolidayStruct(5, 5, "端午"),
            new LunarHolidayStruct(7, 7, "七夕"),
            new LunarHolidayStruct(8, 15, "中秋"),
            new LunarHolidayStruct(12, 29, "除夕"),
            new LunarHolidayStruct(12, 30, "除夕")
            //除夕可能是廿九也可能是三十
        };

        #endregion

        #region 用到的变量

        private static readonly string[] ChineseWeekName =
        {
            "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"
        };

        private readonly DateTime MaxSolarDate = new DateTime(2100, 12, 31); //所支持的最大公历日期，这里包括了要显示部分下个月信息而减去的预留。
        private readonly DateTime MinSolarDate = new DateTime(1901, 3, 1); //所支持的最小公历日期，这里包括了要显示部分上个月信息而加上的预留。
        private SolidBrush brushFestival = new SolidBrush(Color.Crimson);
        private ChineseCalendar.ChineseCalendar date = new ChineseCalendar.ChineseCalendar(DateTime.Today); //月历所在的日期
        private Color dayForeColor = Color.Black; //用于显示在月历上出现的这个月的颜色
        private DayInfo[] dayInfo = new DayInfo[42]; //该月日历信息
        private int endIndex = 42; //最大日期所在的索引，42表示不在那一页
        private DateTime firstDay; //一页月历中的第一天的日期
        private Font fontDay = new Font("微软雅黑", (float) 6.75);
        private Font fontToday = new Font("微软雅黑", 9);
        private Font fontWeek = new Font("微软雅黑", 9);
        private Graphics graphics; //Graphics对象

        private int lastMouseInIndex = -1; //上一个鼠标所在的日期的索引
        private int lastSelectedIndex = -1; //上次选择的日期的索引
        private Color lunarHolForeColor = Color.MediumVioletRed; //农历节日颜色
        private DateTime maxDate; //最大日期
        private DateTime minDate; //最小日期
        private int monthEndIndex; //该月最后一天在该月月历信息的索引
        private int monthStartIndex; //该月1号在该月月历信息的索引
        private int mouseInIndex = -1; //鼠标所在的日期在该页月历中的索引，-1表示未移到日期上
        private Pen penMouseIn = new Pen(Color.SkyBlue); //绘制鼠标移到的日期的矩形框
        private Pen penSelected = new Pen(Color.RoyalBlue); //绘制选择的矩形框
        private Pen penToday = new Pen(Color.Gray); //绘制今天的矩形框
        private ChineseCalendar.ChineseCalendar selectedDate; //选中的日期
        private int selectedIndex = -1; //选中的日期所在的索引，-1表示未选中

        private Size size; //控件的大小
        private SplitLStyle sls = SplitLStyle.None; //分割线风格
        private Color solarHolForeColor = Color.MediumVioletRed; //公历节日颜色
        private Color solarTermsForeColor = Color.MediumVioletRed; //24节气颜色
        private Color splitLinesColor = Color.OliveDrab; //日期分割线颜色
        private int startIndex = -1; //最小日期所在的索引，-1表示不在那一页
        private Color titleColor = Color.Black; //年份和月份的字体颜色
        private ChineseCalendar.ChineseCalendar todayDate = new ChineseCalendar.ChineseCalendar(DateTime.Today); //今天的日期
        private int todayIndex; //今天的索引
        private Color trailingForeColor = Color.LightGray; //用于显示在月历上出现的上个月和下个月的颜色
        private Color weekBackColor = Color.OliveDrab; //星期的背景色
        private Color weekForeColor = Color.White; //星期的前景色

        /// <summary>
        ///     用户选择了日期
        /// </summary>
        /// <remarks>
        ///     定义DateSelected事件，当用户选择了日期时触发
        [Description("用户选择了日期")]
        [Category("行为")]
        public event EventHandler DateSelected;

        #endregion

        #region 构造函数

        /// <summary>
        ///     构造函数
        /// </summary>
        public MyMonthCalendar()
        {
            InitializeComponent();

            //初始化最小日期和最大日期
            minDate = MinSolarDate;
            maxDate = MaxSolarDate;
            //选择的日期默认为今天
            selectedDate = new ChineseCalendar.ChineseCalendar(DateTime.Today);

            graphics = CreateGraphics(); //创建Graphics对象
            //设置为1号
            date.SolarDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            GetData(); //更新数据
            selectedIndex = todayIndex; //初始时，将选择的日期设置今天的日期。
            BackColor = Color.White; //默认背景颜色为白色
            Paint += MyMonthCalendar_Paint;
            Resize += MyMonthCalendar_Resize;
            DateSelected += MyMonthCalendar_DateSelected;
            size = Size; //记录当前大小
            penToday.Width = 2;
            penMouseIn.Width = 2;
            penSelected.Width = 2;
        }

        #endregion

        #region 事件

        #region MyMonthCalendar_Paint

        /// <summary>
        ///     绘制组件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyMonthCalendar_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            int i, LX, LY, x, y;

            #region 修改年份和月份

            if (date.SolarDate.Month > 10)
            {
                x = 34;
            }
            else
            {
                x = 38;
            }
            e.Graphics.DrawString(ConvertToChineseMonthName(date.SolarDate.Month),
                fontWeek, new SolidBrush(titleColor), new PointF(x, 4));

            e.Graphics.DrawString(date.SolarDate.Year.ToString(), fontWeek,
                new SolidBrush(titleColor), new PointF(237, 4));

            #endregion

            #region 绘制星期

            //绘制星期背景
            e.Graphics.FillRectangle(new SolidBrush(weekBackColor), 3, 22, 302, 18);

            //绘制星期
            for (i = 0; i < 7; i++)
            {
                e.Graphics.DrawString(ChineseWeekName[i], fontWeek,
                    new SolidBrush(weekForeColor), new PointF(5 + i*43, 23));
            }

            #endregion

            #region 绘制日期

            //上个月的
            for (i = 0; i < monthStartIndex; i++)
            {
                LX = 8 + i*43;
                LY = 41;
                e.Graphics.DrawString(dayInfo[i].day, fontDay,
                    new SolidBrush(trailingForeColor), new PointF(LX, LY));

                e.Graphics.DrawString(dayInfo[i].lunarDay, fontDay,
                    new SolidBrush(trailingForeColor), new PointF(LX + 14, LY));
            }

            //这个月的
            for (; i <= monthEndIndex; i++)
            {
                x = i%7;
                y = i/7;
                LX = 8 + x*43;
                LY = 41 + y*18;

                //修改字体颜色
                Color _solarColor = dayInfo[i].hightLightS ? solarHolForeColor : dayForeColor;

                Color _lunarColor = dayInfo[i].hightLightL ? lunarHolForeColor : dayForeColor;

                e.Graphics.DrawString(dayInfo[i].day, fontDay,
                    new SolidBrush(_solarColor), new PointF(LX, LY));

                e.Graphics.DrawString(dayInfo[i].lunarDay, fontDay,
                    new SolidBrush(_lunarColor), new PointF(LX + 14, LY));
            }
            //下个月的
            for (; i < 42; i++)
            {
                x = i%7;
                y = i/7;
                LX = 8 + x*43;
                LY = 43 + y*18;
                e.Graphics.DrawString(dayInfo[i].day, fontDay,
                    new SolidBrush(trailingForeColor), new PointF(LX, LY));

                e.Graphics.DrawString(dayInfo[i].lunarDay, fontDay,
                    new SolidBrush(trailingForeColor), new PointF(LX + 14, LY));
            }

            //绘制今天日期的信息
            //e.Graphics.DrawRectangle(new Pen(Color.Red), new Rectangle(new Point(3, 153),
            //new Size(43, 15)));

            e.Graphics.DrawString("今天：" + todayDate.SolarDate.ToShortDateString() +
                                  "   " + todayDate.ToSexAnimalString(), fontToday,
                new SolidBrush(dayForeColor), new Point(30, 151));

            #endregion

            #region 绘制分割线

            if (sls != SplitLStyle.None)
            {
                if (sls == SplitLStyle.RowAndColumn)
                {
                    for (i = 0; i <= 7; i++) //列
                    {
                        e.Graphics.DrawLine(new Pen(splitLinesColor), new Point(3 + i*43, 40),
                            new Point(3 + i*43, 148));
                    }

                    i = 0;
                }
                else
                {
                    i = 1;
                }

                for (; i <= 6; i++) //行
                {
                    e.Graphics.DrawLine(new Pen(splitLinesColor), new Point(3, 40 + i*18),
                        new Point(304, 40 + i*18));
                }
            }

            #endregion

            #region 今天日期和选择的日期的矩形框

            //绘制今天日期的矩形框
            PaintTodayRectangle();

            //绘制选择的日期的矩形框
            PaintSelectedRectangle();

            #endregion
        }

        #endregion

        #region MyMonthCalendar_Resize

        private void MyMonthCalendar_Resize(object sender, EventArgs e)
        {
            //默认不允许修改大小
            Size = size;
        }

        #endregion

        #region MyMonthCalendar_SizeChanged

        /// <summary>
        ///     控件大小默认不可通过设置Size属性来改变
        /// </summary>
        private void MyMonthCalendar_SizeChanged(object sender, EventArgs e)
        {
            Size = size;
        }

        #endregion

        #region MyMonthCalendar_MouseClick

        /// <summary>
        ///     按住，并释放后发生。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyMonthCalendar_MouseClick(object sender, MouseEventArgs e)
        {
            if (mouseInIndex != -1 && e.Button == MouseButtons.Left)
            {
                //修改被选择的日期
                selectedDate.SolarDate = date.SolarDate.AddDays(mouseInIndex -
                                                                monthStartIndex - date.SolarDate.Day + 1);

                IsDateSelected = true; //用户通过单击日期选择了日期
                EventHandler handler = DateSelected;
                handler(this, new EventArgs()); //触发DateSelected事件

                if (mouseInIndex < monthStartIndex)
                {
                    //上个月
                    if (date.SolarDate.AddMonths(-1).Year >= minDate.Year)
                    {
                        date.SolarDate = date.SolarDate.AddMonths(-1);
                        GetData();
                        selectedIndex = monthStartIndex + selectedDate.SolarDate.Day - 1;

                        Refresh(); //刷新控件
                    }
                }
                else if (mouseInIndex > monthEndIndex)
                {
                    //下个月
                    if (date.SolarDate.AddMonths(1).Year <= maxDate.Year)
                    {
                        date.SolarDate = date.SolarDate.AddMonths(1);
                        GetData();
                        selectedIndex = monthStartIndex + selectedDate.SolarDate.Day - 1;

                        Refresh(); //刷新控件
                    }
                }
                else
                {
                    lastSelectedIndex = selectedIndex; //记录上次选择的区域
                    selectedIndex = mouseInIndex; //选择的日期的索引等于这时鼠标所在日期的索引。
                    //这次选择的与上次选择的为同一个月，恢复上次选择的区域
                    if (lastSelectedIndex != selectedIndex)
                    {
                        Repaint(lastSelectedIndex);
                    }
                    PaintSelectedRectangle(); //绘制选择的区域
                    PaintTodayRectangle(); //绘制今天的区域
                }
            }
            /*else if (e.X >= 3 && e.X <= 46 && e.Y >= 153 && e.Y <= 168 &&
                e.Button == MouseButtons.Left)
            {
                //转到今天所在的月历
                date.SolarDate = new DateTime(todayDate.SolarDate.Year,
                    todayDate.SolarDate.Month, 1);
                GetData();
                this.Refresh();
            }*/
        }

        #endregion

        #region MyMonthCalendar_MouseMove

        /// <summary>
        ///     鼠标移到某一天时，绘制一个外框表示鼠标进入了该天。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyMonthCalendar_MouseMove(object sender, MouseEventArgs e)
        {
            if (GetDay(e.Location))
            {
                if (mouseInIndex != lastMouseInIndex)
                {
                    //节日提示
                    if (dayInfo[mouseInIndex].holidayInfo != "")
                    {
                        toolTip.Show(dayInfo[mouseInIndex].holidayInfo, this, e.X, e.Y + 25);
                    }
                    else
                    {
                        toolTip.Hide(this);
                    }

                    PaintMouseInRectangle(); //绘制边框
                }
            }
            else
            {
                //鼠标从日期区域内移到日期区域外
                if (lastMouseInIndex != -1)
                {
                    Repaint(lastMouseInIndex);

                    //之所以要绘制时因为恢复绘制过的区域时可能将这两处破坏
                    PaintTodayRectangle(); //绘制今天的矩形框
                    PaintSelectedRectangle(); //绘制被选择的矩形框
                }

                toolTip.Hide(this);
            }

            lastMouseInIndex = mouseInIndex;
        }

        #endregion

        #endregion

        #region 自定义事件

        private void MyMonthCalendar_DateSelected(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region 上一个 下一个 转到今天

        #region picYearNext_MouseClick

        private void picYearNext_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (date.SolarDate.Year < maxDate.Year)
                {
                    date.SolarDate = date.SolarDate.AddYears(1);
                    GetData();
                    Refresh(); //刷新控件
                }
            }
        }

        #endregion

        #region picYearPrevious_MouseClick

        private void picYearPrevious_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (date.SolarDate.Year > minDate.Year)
                {
                    date.SolarDate = date.SolarDate.AddYears(-1);
                    GetData();
                    Refresh(); //刷新控件
                }
            }
        }

        #endregion

        #region picMonthNext_MouseClick

        private void picMonthNext_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (date.SolarDate.Year < maxDate.Year)
                {
                    date.SolarDate = date.SolarDate.AddMonths(1);
                    GetData();
                    Refresh(); //刷新控件
                }
                else if (date.SolarDate.Year == maxDate.Year)
                {
                    if (date.SolarDate.Month < maxDate.Month)
                    {
                        date.SolarDate = date.SolarDate.AddMonths(1);
                        GetData();
                        Refresh(); //刷新控件
                    }
                }
            }
        }

        #endregion

        #region picMonthPrevious_MouseClick

        private void picMonthPrevious_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (date.SolarDate.Year > minDate.Year)
                {
                    date.SolarDate = date.SolarDate.AddMonths(-1);
                    GetData();
                    Refresh(); //刷新控件
                }
                else if (date.SolarDate.Year == minDate.Year)
                {
                    if (date.SolarDate.Month > minDate.Month)
                    {
                        date.SolarDate = date.SolarDate.AddMonths(-1);
                        GetData();
                        Refresh(); //刷新控件
                    }
                }
            }
        }

        #endregion

        #region picToday_MouseClick

        private void picToday_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //转到今天所在的月历
                date.SolarDate = new DateTime(todayDate.SolarDate.Year,
                    todayDate.SolarDate.Month, 1);
                GetData();
                Refresh();
            }
        }

        #endregion

        #region 按钮图标

        #region 左

        private void picLeft_MouseEnter(object sender, EventArgs e)
        {
            ((PictureBox) sender).Image = Resources.btn_left2;
            base.OnMouseEnter(e); //触发MouseEnter
        }

        private void picLeft_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ((PictureBox) sender).Image = Resources.btn_left3;
            }
        }

        private void picLeft_MouseLeave(object sender, EventArgs e)
        {
            ((PictureBox) sender).Image = Resources.btn_left1;
        }

        private void picLeft_MouseUp(object sender, MouseEventArgs e)
        {
            ((PictureBox) sender).Image = Resources.btn_left2;
        }

        #endregion

        #region 右

        private void picRight_MouseEnter(object sender, EventArgs e)
        {
            ((PictureBox) sender).Image = Resources.btn_right2;
            OnMouseEnter(e); //触发MouseEnter
        }

        private void picRight_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ((PictureBox) sender).Image = Resources.btn_right3;
            }
        }

        private void picRight_MouseLeave(object sender, EventArgs e)
        {
            ((PictureBox) sender).Image = Resources.btn_right1;
        }

        private void picRight_MouseUp(object sender, MouseEventArgs e)
        {
            ((PictureBox) sender).Image = Resources.btn_right2;
        }

        #endregion

        #region 转到今天

        private void picToday_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ((PictureBox) sender).Image = Resources.btnChange3;
            }
        }

        private void picToday_MouseEnter(object sender, EventArgs e)
        {
            ((PictureBox) sender).Image = Resources.btnChange2;
            OnMouseEnter(e); //触发MouseEnter
        }

        private void picToday_MouseLeave(object sender, EventArgs e)
        {
            ((PictureBox) sender).Image = Resources.btnChange1;
        }

        private void picToday_MouseUp(object sender, MouseEventArgs e)
        {
            ((PictureBox) sender).Image = Resources.btnChange2;
        }

        #endregion

        #endregion

        #endregion

        #region 私有函数

        #region ConvertToChineseMonthName

        /// <summary>
        ///     将阿拉伯数字表示的月份转换成汉字表示的月份
        /// </summary>
        /// <param name="month">输入的月份</param>
        private string ConvertToChineseMonthName(int month)
        {
            switch (month)
            {
                case 1:
                    return "一月";
                case 2:
                    return "二月";
                case 3:
                    return "三月";
                case 4:
                    return "四月";
                case 5:
                    return "五月";
                case 6:
                    return "六月";
                case 7:
                    return "七月";
                case 8:
                    return "八月";
                case 9:
                    return "九月";
                case 10:
                    return "十月";
                case 11:
                    return "十一月";
                case 12:
                    return "十二月";
                default:
                    return "";
            }
        }

        #endregion

        #region GetIndex

        /// <summary>
        ///     获取dt的日期在该页月历上的索引，要先判断dt是否在该页上。
        /// </summary>
        /// <param name="dt">给定的日期</param>
        /// <returns>dt的日期在该页月历上的索引</returns>
        private int GetIndex(DateTime dt)
        {
            if (DateTime.Compare(firstDay, dt) <= 0 &&
                DateTime.Compare(firstDay.AddDays(41), dt) >= 0)
            {
                if (date.SolarDate.Month == dt.Month)
                {
                    return monthStartIndex + dt.Day - 1;
                }
                if (date.SolarDate.Month > dt.Month)
                {
                    return dt.Day - firstDay.Day;
                }
                return monthEndIndex + dt.Day;
            }
            return -1; //-1表示不在该页
        }

        #endregion

        #region GetData

        /// <summary>
        ///     更新这一页的月历信息
        /// </summary>
        private void GetData()
        {
            var cld = new ChineseCalendar.ChineseCalendar(new DateTime(date.SolarDate.Year,
                date.SolarDate.Month, 1));

            monthStartIndex = (int) cld.WeekDay;
            monthEndIndex = monthStartIndex + cld.SolarDate.AddMonths(1).AddDays(-1).Day - 1;
            cld.SolarDate = cld.SolarDate.AddDays(0 - monthStartIndex);

            firstDay = cld.SolarDate; //这一页月历的第一天所在的日期

            //最小日期出现在该页
            startIndex = GetIndex(minDate);

            //最大日期出现在该页
            endIndex = GetIndex(maxDate);

            if (endIndex == -1) //endIndex用42表示不在该页
            {
                endIndex = 42;
            }

            //选择的日期出现在该页，更新selectedIndex
            selectedIndex = GetIndex(selectedDate.SolarDate);

            //今天的索引
            todayIndex = GetIndex(todayDate.SolarDate);

            //更新数据
            for (int i = 0; i < 42; i++)
            {
                FormatDayInfo(ref cld, i); //格式化DayInfo
                cld.SolarDate = cld.SolarDate.AddDays(1);
            }
        }

        #endregion

        #region FormatDayInfo

        /// <summary>
        ///     格式化日期中的信息
        /// </summary>
        /// <remarks>
        ///     为DayInfo赋值
        /// </remarks>
        private void FormatDayInfo(ref ChineseCalendar.ChineseCalendar date, int i)
        {
            string sh, lh, wh, st;

            dayInfo[i].day = date.SolarDate.Day.ToString();
            if (date.LunarDayString == "初一")
                dayInfo[i].lunarDay = date.LunarMonthString + "月";
            else
                dayInfo[i].lunarDay = date.LunarDayString;

            //获取所有节日
            dayInfo[i].holidayInfo = GetHoliday(ref date, out sh, out lh, out wh, out st);

            if (sh != "" || wh != "")
                dayInfo[i].hightLightS = true;
            else
                dayInfo[i].hightLightS = false;

            if (lh != "" || st != "")
                dayInfo[i].hightLightL = true;
            else
                dayInfo[i].hightLightL = false;

            //要显示的重要节日
            //优先级24节气 < 公历节日 < 农历节日

            //24节气
            if (st != "")
            {
                dayInfo[i].lunarDay = st;
                if (st == "清明")
                    dayInfo[i].hightLightS = true;
            }

            //公历节日
            if (sh != "")
            {
                foreach (SolarHolidayStruct shs in solarHolidayInfo)
                {
                    if ((shs.Month == date.SolarDate.Month) && (shs.Day == date.SolarDate.Day))
                    {
                        dayInfo[i].lunarDay = shs.HolidayName;
                        dayInfo[i].hightLightL = true;
                        break;
                    }
                }
            }

            //农历节日
            if (lh != "")
            {
                foreach (LunarHolidayStruct lhs in lunarHolidayInfo)
                {
                    if ((lhs.Month == date.LunarMonth) && (lhs.Day == date.LunarDay))
                    {
                        dayInfo[i].lunarDay = lhs.HolidayName;
                        dayInfo[i].hightLightS = true;
                        break;
                    }
                }
            }
        }

        #endregion

        #region GetHoliday

        /// <summary>
        ///     获得dt这一天的所有节日，
        /// </summary>
        private string GetHoliday(ref ChineseCalendar.ChineseCalendar date, out string sh,
            out string lh, out string wh, out string st)
        {
            string str = "";
            sh = date.SolarDateHoliday;
            lh = date.LunarDateHoliday;
            wh = date.WeekDayHoliday;
            st = date.TwentyFourSolarTerms;

            if (lh != "")
            {
                str += lh;
            }
            if (st != "")
            {
                if (str != "")
                    str += " " + st;
                else
                    str += st;
            }
            if (sh != "")
            {
                if (str != "")
                    str += " " + sh;
                else
                    str += sh;
            }
            if (wh != "")
            {
                if (str != "")
                    str += " " + wh;
                else
                    str += wh;
            }

            for (int i = 0; i < str.Length; i++)
            {
                if (str.Substring(i, 1) == " ")
                {
                    str = str.Remove(i, 1);
                    str = str.Insert(i, "\r\n");
                    i++;
                }
            }
            return str;
        }

        #endregion

        #region GetDay

        /// <summary>
        ///     根据鼠标的位置修改鼠标所在的日期的索引，若鼠标未停留在日期上则修改为-1
        /// </summary>
        /// <param name="location">鼠标的位置</param>
        private bool GetDay(Point location)
        {
            if (location.X < 3 || location.X > 304 || location.Y < 40 || location.Y > 148)
            {
                mouseInIndex = -1;
                return false; //处于日期范围外
            }
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (location.X > (3 + i*43) && location.X < (46 + i*43))
                    {
                        if (location.Y > (40 + j*18) && location.Y < (58 + j*18))
                        {
                            mouseInIndex = i + j*7;
                            if (mouseInIndex >= startIndex)
                            {
                            }
                            else
                            {
                                mouseInIndex = -1; //所在的区域属于不可用区域（小于最小日期）
                                return false;
                            }

                            if (mouseInIndex <= endIndex)
                            {
                            }
                            else
                            {
                                mouseInIndex = -1; //所在的区域属于不可用区域（大于最大日期）
                                return false;
                            }

                            return true;
                        }
                        continue; //行不符合
                    }
                    break; //列不符合
                }
            }

            mouseInIndex = -1;
            return false;
        }

        #endregion

        #region PaintMouseInrectangle

        /// <summary>
        ///     绘制鼠标所在日期的矩形框
        /// </summary>
        private void PaintMouseInRectangle()
        {
            if (lastMouseInIndex != -1)
            {
                Repaint(lastMouseInIndex); //恢复上次选择的区域
            }

            PaintTodayRectangle(); //绘制今天的矩形框
            PaintSelectedRectangle(); //绘制被选择的矩形框

            //绘制鼠标所在日期的区域
            graphics.DrawRectangle(penMouseIn, new Rectangle(new Point(4 + (mouseInIndex%
                                                                            7)*43, 41 + mouseInIndex/7*18),
                new Size(42, 17)));
        }

        #endregion

        #region Repaint

        /// <summary>
        ///     恢复绘制过矩形框的区域
        /// </summary>
        /// <param name="index">所在的位置的索引</param>
        private void Repaint(int index)
        {
            var pen = new Pen(BackColor) {Width = 2};

            int lx = 3 + (index%7)*43;
            int ly = 40 + index/7*18;

            graphics.DrawRectangle(pen, new Rectangle(new Point(lx + 1, ly + 1), new Size(42, 17)));
            pen.Width = 1;
            pen.Color = splitLinesColor;
            switch (sls)
            {
                case SplitLStyle.RowAndColumn:
                    graphics.DrawRectangle(pen, new Rectangle(new Point(lx, ly), new Size(43, 18)));
                    break;
                case SplitLStyle.OnlyRow:
                    graphics.DrawLine(pen, new Point(lx, ly), new Point(lx + 43, ly));
                    graphics.DrawLine(pen, new Point(lx, ly + 18), new Point(lx + 43, ly + 18));
                    break;
            }
        }

        #endregion

        #region PaintTodayRectangle

        /// <summary>
        ///     绘制今天的矩形框，在判断要不要绘制后才绘制。
        /// </summary>
        private void PaintTodayRectangle()
        {
            if (DateTime.Compare(firstDay, todayDate.SolarDate) <= 0 &&
                DateTime.Compare(firstDay.AddDays(41), todayDate.SolarDate) >= 0)
            {
                int x = todayIndex%7;
                int y = todayIndex/7;
                graphics.DrawRectangle(penToday, new Rectangle(new Point
                    (4 + x*43, 41 + y*18), new Size(42, 17)));
            }
        }

        #endregion

        #region PaintSelectedRectangle

        /// <summary>
        ///     绘制选择的日期的矩形框
        /// </summary>
        private void PaintSelectedRectangle()
        {
            if (DateTime.Compare(firstDay, selectedDate.SolarDate) <= 0 &&
                DateTime.Compare(firstDay.AddDays(41), selectedDate.SolarDate) >= 0)
            {
                int x = selectedIndex%7;
                int y = selectedIndex/7;
                graphics.DrawRectangle(penSelected, new Rectangle(new Point
                    (4 + x*43, 41 + y*18), new Size(42, 17)));
            }
        }

        #endregion

        #endregion

        #region 属性

        #region 行为

        #region 选中的农历日期

        /// <summary>
        ///     选中的农历日期
        /// </summary>
        [Description("选中的农历日期")] //显示在属性设计视图中的描述
        [Category("行为")] //类别
        public ChineseCalendar.ChineseCalendar SelectedLunarDate
        {
            set { selectedDate = value; }
            get { return selectedDate; }
        }

        #endregion

        #region 选中的公历日期

        /// <summary>
        ///     选中的公历日期
        /// </summary>
        [Description("选中的公历日期")]
        [Category("行为")]
        public DateTime SelectedSolarDate
        {
            set
            {
                if (DateTime.Compare(value, minDate) < 0)
                {
                    selectedDate.SolarDate = minDate;
                }
                else if (DateTime.Compare(value, maxDate) > 0)
                {
                    selectedDate.SolarDate = maxDate;
                }
                else
                {
                    selectedDate.SolarDate = value;
                }

                GetData();
                Refresh();
            }
            get { return selectedDate.SolarDate; }
        }

        #endregion

        #region 用户是否通过单击鼠标选择了日期

        /// <summary>
        ///     用户是否通过单击鼠标选择了日期
        /// </summary>
        [Description("用户是否通过单击鼠标选择了日期"), Category("行为")]
        public bool IsDateSelected { get; private set; }

        #endregion

        #region 最小日期

        /// <summary>
        ///     最小日期
        /// </summary>
        [Description("最小日期")]
        [Category("行为")]
        public DateTime MinDate
        {
            set
            {
                if (DateTime.Compare(value, maxDate) <= 0)
                {
                    minDate = value.Year < MinSolarDate.Year ? MinSolarDate : value;

                    if (DateTime.Compare(selectedDate.SolarDate, minDate) < 0)
                    {
                        //如果最小日期比选择的日期大，将选择的日期修改为最小日期
                        selectedDate.SolarDate = minDate;
                    }

                    if (DateTime.Compare(date.SolarDate, minDate) < 0)
                    {
                        //如果当前月历日期比最小的日期小，将当前月历日期修改为最小日期
                        date.SolarDate = minDate;
                    }

                    //刷新
                    GetData();
                    Refresh();
                }
            }
            get { return minDate; }
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
            set
            {
                if (DateTime.Compare(value, minDate) >= 0)
                {
                    maxDate = value.Year > MaxSolarDate.Year ? MaxSolarDate : value;

                    if (DateTime.Compare(selectedDate.SolarDate, maxDate) > 0)
                    {
                        //如果最大日期比选择的日期小，将选择的日期修改为最大日期
                        selectedDate.SolarDate = maxDate;
                    }

                    if (DateTime.Compare(date.SolarDate, maxDate) > 0)
                    {
                        //如果当前月历日期比最大的日期大，将当前月历日期修改为最大日期
                        date.SolarDate = maxDate;
                    }

                    //刷新
                    GetData();
                    Refresh();
                }
            }
            get { return maxDate; }
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
            set
            {
                weekForeColor = value;
                Refresh();
            }
            get { return weekForeColor; }
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
            set
            {
                weekBackColor = value;
                Refresh();
            }
            get { return weekBackColor; }
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
            set
            {
                trailingForeColor = value;
                Refresh();
            }
            get { return trailingForeColor; }
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
            set
            {
                dayForeColor = value;
                Refresh();
            }
            get { return dayForeColor; }
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
            set
            {
                titleColor = value;
                Refresh();
            }
            get { return titleColor; }
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
            set
            {
                splitLinesColor = value;
                Refresh();
            }
            get { return splitLinesColor; }
        }

        #endregion

        #region 分割线风格

        /// <summary>
        ///     分割线风格
        /// </summary>
        [Description("分割线风格")]
        [Category("外观")]
        public SplitLStyle SplitLinesStyle
        {
            set
            {
                sls = value;
                Refresh();
            }
            get { return sls; }
        }

        #endregion

        #endregion

        #endregion
    }
}