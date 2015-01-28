//作者：刘典武
//时间：2010-12-01

using System;
using System.Globalization;

namespace ChineseCalendar
{

    #region 异常处理

    /// <summary>
    ///     中国日历异常处理
    /// </summary>
    public class ChineseCalendarException : Exception
    {
        public ChineseCalendarException(string msg)
            : base(msg)
        {
        }
    }

    #endregion

    /// <summary>
    ///     中国农历类 农历范围1901-1-1～2100-12-29 公历范围1901-2-19～2101-1-28
    /// </summary>
    /// <remarks>
    ///     2010-2-22
    /// </remarks>
    public class ChineseCalendar
    {
        #region 内部结构

        private struct LunarHolidayStruct
        {
            public int Day;
            public string HolidayName;
            public int Month;
            public int Recess;

            public LunarHolidayStruct(int month, int day, int recess, string name)
            {
                Month = month;
                Day = day;
                Recess = recess;
                HolidayName = name;
            }
        }

        private struct SolarHolidayStruct
        {
            public int Day;
            public string HolidayName;
            public int Month;
            public int Recess; //假期长度

            public SolarHolidayStruct(int month, int day, int recess, string name)
            {
                Month = month;
                Day = day;
                Recess = recess;
                HolidayName = name;
            }
        }

        private struct WeekHolidayStruct
        {
            public string HolidayName;
            public int Month;
            public int WeekAtMonth;
            public int WeekDay;

            public WeekHolidayStruct(int month, int weekAtMonth, int weekDay, string name)
            {
                Month = month;
                WeekAtMonth = weekAtMonth;
                WeekDay = weekDay;
                HolidayName = name;
            }
        }

        #endregion

        #region 内部变量

        private static ChineseLunisolarCalendar calendar = new ChineseLunisolarCalendar();

        private string _animal;
        private string _lunarDayString;
        private string _lunarMonthString;
        private string _lunarYearString;
        private string _sexagenary;
        private DateTime _solarDate;

        #endregion

        #region 基础数据

        #region 基本常量

        private const int GanZhiStartYear = 1864; //干支计算起始年
        private const string ChineseNumber = "〇一二三四五六七八九";
        private const int AnimalStartYear = 1900; //1900年为鼠年
        private static DateTime GanZhiStartDay = new DateTime(1899, 12, 22); //起始日
        private static DateTime ChineseConstellationReferDay = new DateTime(2007, 9, 13); //28星宿参考值,本日为角
        public readonly int maxLunarYear = 2100;
        public readonly DateTime maxSolarDate = new DateTime(2101, 1, 28); //最大公历日期
        public readonly int minLunarYear = 1901;
        public readonly DateTime minSolarDate = new DateTime(1901, 2, 19); //最小公历日期

        #endregion

        #region 星座和诞生石

        private static readonly string[] ConstellationName =
        {
            "白羊座", "金牛座", "双子座",
            "巨蟹座", "狮子座", "处女座",
            "天秤座", "天蝎座", "射手座",
            "摩羯座", "水瓶座", "双鱼座"
        };

        private static readonly string[] BirthStoneName =
        {
            "钻石", "蓝宝石", "玛瑙", "珍珠", "红宝石", "红条纹玛瑙",
            "蓝宝石", "猫眼石", "黄宝石", "土耳其玉", "紫水晶", "月长石，血石"
        };

        //白羊座：    3月21日------4月20日   诞生石：   钻石
        //金牛座：    4月21日------5月20日   诞生石：   蓝宝石
        //双子座：    5月21日------6月21日   诞生石：   玛瑙
        //巨蟹座：    6月22日------7月22日   诞生石：   珍珠
        //狮子座：    7月23日------8月22日   诞生石：   红宝石
        //处女座：    8月23日------9月22日   诞生石：   红条纹玛瑙
        //天秤座：    9月23日-----10月23日   诞生石：   蓝宝石
        //天蝎座：   10月24日-----11月21日   诞生石：   猫眼石
        //射手座：   11月22日-----12月21日   诞生石：   黄宝石
        //摩羯座：   12月22日------1月19日   诞生石：   土耳其玉
        //水瓶座：    1月20日------2月18日   诞生石：   紫水晶
        //双鱼座：    2月19日------3月20日   诞生石：   月长石，血石

        #endregion

        #region 二十八星宿

        private static readonly string[] ChineseConstellationName =
        {
            //四        五      六         日        一      二      三
            "角木蛟", "亢金龙", "女土蝠", "房日兔", "心月狐", "尾火虎", "箕水豹",
            "斗木獬", "牛金牛", "氐土貉", "虚日鼠", "危月燕", "室火猪", "壁水獝",
            "奎木狼", "娄金狗", "胃土彘", "昴日鸡", "毕月乌", "觜火猴", "参水猿",
            "井木犴", "鬼金羊", "柳土獐", "星日马", "张月鹿", "翼火蛇", "轸水蚓"
        };

        #endregion

        #region 二十四节气

        private static readonly string[] SolarTerms =
        {
            "小寒", "大寒", "立春", "雨水", "惊蛰", "春分",
            "清明", "谷雨", "立夏", "小满", "芒种", "夏至",
            "小暑", "大暑", "立秋", "处暑", "白露", "秋分",
            "寒露", "霜降", "立冬", "小雪", "大雪", "冬至"
        };

        private static readonly int[] sTermsInfo =
        {
            0, 21208, 42467, 63836, 85337, 107014,
            128867, 150921, 173149, 195551, 218072, 240693,
            263343, 285989, 308563, 331033, 353350, 375494,
            397447, 419210, 440795, 462224, 483532, 504758
        };

        #endregion

        #region 农历相关数据

        private const string CelestialStem = "甲乙丙丁戊己庚辛壬癸";
        private const string TerrestrialBranch = "子丑寅卯辰巳午未申酉戌亥";
        private const string AnimalsStr = "鼠牛虎兔龙蛇马羊猴鸡狗猪";

        private static readonly string[] ChineseDayName =
        {
            "初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十",
            "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十",
            "廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十"
        };

        private static readonly string[] ChineseMonthName =
        {
            "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "冬", "腊"
        };

        #endregion

        #region 按公历计算的节日

        private static readonly SolarHolidayStruct[] solarHolidayInfo =
        {
            new SolarHolidayStruct(1, 1, 1, "元旦"),
            new SolarHolidayStruct(2, 2, 0, "世界湿地日"),
            new SolarHolidayStruct(2, 10, 0, "国际气象节"),
            new SolarHolidayStruct(2, 14, 0, "情人节"),
            new SolarHolidayStruct(3, 1, 0, "国际海豹日"),
            new SolarHolidayStruct(3, 5, 0, "学雷锋纪念日"),
            new SolarHolidayStruct(3, 8, 0, "妇女节"),
            new SolarHolidayStruct(3, 12, 0, "植树节 孙中山逝世纪念日"),
            new SolarHolidayStruct(3, 14, 0, "国际警察日"),
            new SolarHolidayStruct(3, 15, 0, "消费者权益日"),
            new SolarHolidayStruct(3, 17, 0, "中国国医节 国际航海日"),
            new SolarHolidayStruct(3, 21, 0, "世界森林日 消除种族歧视国际日 世界儿歌日"),
            new SolarHolidayStruct(3, 22, 0, "世界水日"),
            new SolarHolidayStruct(3, 24, 0, "世界防治结核病日"),
            new SolarHolidayStruct(4, 1, 0, "愚人节"),
            new SolarHolidayStruct(4, 7, 0, "世界卫生日"),
            new SolarHolidayStruct(4, 22, 0, "世界地球日"),
            new SolarHolidayStruct(5, 1, 1, "劳动节"),
            new SolarHolidayStruct(5, 2, 1, "劳动节假日"),
            new SolarHolidayStruct(5, 3, 1, "劳动节假日"),
            new SolarHolidayStruct(5, 4, 0, "青年节"),
            new SolarHolidayStruct(5, 8, 0, "世界红十字日"),
            new SolarHolidayStruct(5, 12, 0, "国际护士节"),
            new SolarHolidayStruct(5, 31, 0, "世界无烟日"),
            new SolarHolidayStruct(6, 1, 0, "国际儿童节"),
            new SolarHolidayStruct(6, 5, 0, "世界环境保护日"),
            new SolarHolidayStruct(6, 26, 0, "国际禁毒日"),
            new SolarHolidayStruct(7, 1, 0, "建党节 香港回归纪念 世界建筑日"),
            new SolarHolidayStruct(7, 11, 0, "世界人口日"),
            new SolarHolidayStruct(8, 1, 0, "建军节"),
            new SolarHolidayStruct(8, 8, 0, "中国男子节 父亲节"),
            new SolarHolidayStruct(8, 15, 0, "抗日战争胜利纪念"),
            new SolarHolidayStruct(9, 9, 0, "毛泽东逝世纪念"),
            new SolarHolidayStruct(9, 10, 0, "教师节"),
            new SolarHolidayStruct(9, 18, 0, "九·一八事变纪念日"),
            new SolarHolidayStruct(9, 20, 0, "国际爱牙日"),
            new SolarHolidayStruct(9, 27, 0, "世界旅游日"),
            new SolarHolidayStruct(9, 28, 0, "孔子诞辰"),
            new SolarHolidayStruct(10, 1, 1, "国庆节 国际音乐日"),
            new SolarHolidayStruct(10, 2, 1, "国庆节假日"),
            new SolarHolidayStruct(10, 3, 1, "国庆节假日"),
            new SolarHolidayStruct(10, 6, 0, "老人节"),
            new SolarHolidayStruct(10, 24, 0, "联合国日"),
            new SolarHolidayStruct(11, 10, 0, "世界青年节"),
            new SolarHolidayStruct(11, 12, 0, "孙中山诞辰纪念"),
            new SolarHolidayStruct(12, 1, 0, "世界艾滋病日"),
            new SolarHolidayStruct(12, 3, 0, "世界残疾人日"),
            new SolarHolidayStruct(12, 20, 0, "澳门回归纪念"),
            new SolarHolidayStruct(12, 24, 0, "平安夜"),
            new SolarHolidayStruct(12, 25, 0, "圣诞节"),
            new SolarHolidayStruct(12, 26, 0, "毛泽东诞辰纪念")
        };

        #endregion

        #region 按农历计算的节日

        private static readonly LunarHolidayStruct[] lunarHolidayInfo =
        {
            new LunarHolidayStruct(1, 1, 1, "春节"),
            new LunarHolidayStruct(1, 15, 0, "元宵节"),
            new LunarHolidayStruct(5, 5, 0, "端午节"),
            new LunarHolidayStruct(7, 7, 0, "七夕情人节"),
            new LunarHolidayStruct(7, 15, 0, "中元节 盂兰盆节"),
            new LunarHolidayStruct(8, 15, 0, "中秋节"),
            new LunarHolidayStruct(9, 9, 0, "重阳节"),
            new LunarHolidayStruct(12, 8, 0, "腊八节"),
            new LunarHolidayStruct(12, 23, 0, "北方小年(扫房)"),
            new LunarHolidayStruct(12, 24, 0, "南方小年(掸尘)")
            //new LunarHolidayStruct(12, 30, 0, "除夕")  //注意除夕需要其它方法进行计算
            //除夕可能是廿九也可能是三十
        };

        #endregion

        #region 按某月第几个星期几

        private static readonly WeekHolidayStruct[] weekHolidayInfo =
        {
            new WeekHolidayStruct(5, 2, 0, "母亲节"),
            new WeekHolidayStruct(5, 3, 0, "全国助残日"),
            new WeekHolidayStruct(6, 3, 0, "父亲节"),
            new WeekHolidayStruct(9, 3, 2, "国际和平日"),
            new WeekHolidayStruct(9, 4, 0, "国际聋人节"),
            new WeekHolidayStruct(10, 1, 1, "国际住房日"),
            new WeekHolidayStruct(10, 1, 3, "国际减轻自然灾害日"),
            new WeekHolidayStruct(11, 4, 4, "感恩节")
        };

        #endregion

        #endregion

        #region 构造函数

        #region ChineseCalendar <公历日期初始化>

        /// <summary>
        ///     用一个标准的公历日期来初使化
        /// </summary>
        /// <param name="dt">公历日期</param>
        public ChineseCalendar(DateTime dt)
        {
            //检查日期是否在限制范围内
            if (dt < minSolarDate || dt > maxSolarDate)
            {
                throw new ChineseCalendarException("公历日期只支持在1901-2-19～2101-1-28范围内");
            }
            _solarDate = dt;
            LoadFromSolarDate();
        }

        #endregion

        #region ChineseCalendar <农历日期初始化>

        /// <summary>
        ///     用农历的日期来初使化
        /// </summary>
        /// <param name="year">农历年</param>
        /// <param name="month">农历月</param>
        /// <param name="day">农历日</param>
        /// <param name="IsLeapMonth">是否闰月</param>
        public ChineseCalendar(int year, int month, int day, bool IsLeapMonth)
        {
            _solarDate = GetDateFromLunarDate(year, month, day, IsLeapMonth);
            LoadFromSolarDate();
        }

        #endregion

        #endregion

        #region 私有函数

        #region LoadFromSolarDate

        /// <summary>
        ///     利用ChineseLunisolarCalendar类初始化农历数据
        /// </summary>
        private void LoadFromSolarDate()
        {
            IsLunarLeapMonth = false;

            LunarYear = calendar.GetYear(_solarDate);
            LunarMonth = calendar.GetMonth(_solarDate);
            LunarDay = calendar.GetDayOfMonth(_solarDate);

            _sexagenary = null;
            _animal = null;
            _lunarYearString = null;
            _lunarMonthString = null;
            _lunarDayString = null;

            int leapMonth = calendar.GetLeapMonth(LunarYear);
            if (leapMonth != 0)
            {
                if (leapMonth == LunarMonth)
                {
                    IsLunarLeapMonth = true;
                    LunarMonth -= 1;
                }
                else if (leapMonth > 0 && leapMonth < LunarMonth)
                {
                    LunarMonth -= 1;
                }
            }
        }

        #endregion

        #region GetDateFromLunarDate

        /// <summary>
        ///     阴历转阳历
        /// </summary>
        /// <param name="year">阴历年</param>
        /// <param name="month">阴历月</param>
        /// <param name="day">阴历日</param>
        /// <param name="isLeapMonth">是否闰月</param>
        private DateTime GetDateFromLunarDate(int year, int month, int day, bool isLeapMonth)
        {
            if (year < 1901 || year > 2100)
                throw new ChineseCalendarException("只支持1901～2100期间的农历年");
            if (month < 1 || month > 12)
                throw new ChineseCalendarException("表示月份的数字必须在1～12之间");

            int leapMonth = calendar.GetLeapMonth(year);

            if (isLeapMonth && leapMonth != month + 1)
            {
                throw new ChineseCalendarException("该月不是闰月");
            }

            if (isLeapMonth || (leapMonth > 0 && leapMonth <= month))
            {
                if (day < 1 || day > calendar.GetDaysInMonth(year, month + 1))
                    throw new ChineseCalendarException("农历日期输入有误");
                return calendar.ToDateTime(year, month + 1, day, 0, 0, 0, 0);
            }
            if (day < 1 || day > calendar.GetDaysInMonth(year, month))
                throw new ChineseCalendarException("农历日期输入有误");
            return calendar.ToDateTime(year, month, day, 0, 0, 0, 0);
        }

        #endregion

        #region GetSexagenaryHour

        /// <summary>
        ///     获得指定时间的时辰
        /// </summary>
        /// <returns>时辰</returns>
        /// 子：zi （晚上 11 时正至凌晨 1 时正） 鼠 鼠在这时间最活跃。 （一说为0：00~2：00，以此类推）
        /// 丑：chou （凌晨 1 时正至凌晨 3 时正） 牛 牛在这时候吃完草，准备耕田。
        /// 寅：yin （凌晨 3 时正至早上 5 时正） 虎 老虎在此时最猛。 
        /// 卯：mao （早上 5 时正至早上 7 时正 ）兔 月亮又称玉兔，在这段时间还在天上。 
        /// 辰： chen （早上 7 时正至上午 9 时正） 龙 相传这是「群龙行雨」的时候 
        /// 巳：si （上午 9 时正至上午11时正） 蛇 在这时候隐蔽在草丛中 
        /// 午：wu （上午11时正至下午 1 时正） 马 这时候太阳最猛烈，相传这时阳气达到极限，阴气将会产生，而马是阴类动物。 
        /// 未：wei （ 下午 1 时正至下午 3 时正 ） 羊 羊在这段时间吃草 
        /// 申：shen （ 下午 3 时正至下午 5 时正） 猴 猴子喜欢在这时候啼叫 
        /// 酉：you （下午 5 时正至晚上 7 时正） 鸡 鸡於傍晚开始归巢 
        /// 戌：xu （晚上 7 时正至晚上 9 时正 ） 狗 狗开始守门口 
        /// 亥：hai （晚上 9 时正至晚上 11 时正） 猪 夜深时分猪正在熟睡
        private string GetSexagenaryHour()
        {
            if (_solarDate.Hour == 23 || _solarDate.Hour == 0)
            {
                return TerrestrialBranch.Substring(0);
            }
            return TerrestrialBranch.Substring((_solarDate.Hour + 1)/2);
        }

        #endregion

        #region ConvertToChineseNum

        /// <summary>
        ///     将0-9转成汉字形式
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private string ConvertToChineseNum(string n)
        {
            switch (n)
            {
                case "0":
                    return ChineseNumber.Substring(0, 1);
                case "1":
                    return ChineseNumber.Substring(1, 1);
                case "2":
                    return ChineseNumber.Substring(2, 1);
                case "3":
                    return ChineseNumber.Substring(3, 1);
                case "4":
                    return ChineseNumber.Substring(4, 1);
                case "5":
                    return ChineseNumber.Substring(5, 1);
                case "6":
                    return ChineseNumber.Substring(6, 1);
                case "7":
                    return ChineseNumber.Substring(7, 1);
                case "8":
                    return ChineseNumber.Substring(8, 1);
                case "9":
                    return ChineseNumber.Substring(9, 1);
                default:
                    return "";
            }
        }

        #endregion

        #region CompareWeekDayHoliday

        /// <summary>
        ///     判断某个日期是不是指定的第几个月第几周第几天，星期天是第0天。
        /// </summary>
        /// <param name="date">指定的日期</param>
        /// <param name="month">第几个月</param>
        /// <param name="week">第几个星期</param>
        /// <param name="day">第几天</param>
        /// <returns>true或false</returns>
        private bool CompareWeekDayHoliday(DateTime date, int month, int week, int day)
        {
            if (date.Month == month) //月份相同
            {
                if ((int) date.DayOfWeek == day) //星期几相同
                {
                    var firstDay = new DateTime(date.Year, date.Month, 1); //生成当月第一天
                    int firWeekDays = 7 - (int) firstDay.DayOfWeek; //计算第一周有多少天

                    if (day + firWeekDays + (week - 2)*7 + 1 == date.Day)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #endregion

        #region  属性

        #region 最小最大日期

        /// <summary>
        ///     所支持的最小农历年份
        /// </summary>
        public static int MinLunarYear
        {
            get { return 1901; }
        }

        /// <summary>
        ///     所支持的最大农历年份
        /// </summary>
        public static int MaxLunarYear
        {
            get { return 2100; }
        }

        /// <summary>
        ///     所支持的最小公历日期
        /// </summary>
        public static DateTime MinSolarDate
        {
            get { return new DateTime(1901, 2, 19); }
        }

        /// <summary>
        ///     所支持的最大公历日期
        /// </summary>
        public static DateTime MaxSolarDate
        {
            get { return new DateTime(2101, 1, 28); }
        }

        #endregion

        #region 节日

        #region LunarDateHoliday

        /// <summary>
        ///     中国农历节日
        /// </summary>
        public string LunarDateHoliday
        {
            get
            {
                string tempStr = "";
                if (IsLunarLeapMonth == false) //闰月不计算节日
                {
                    foreach (LunarHolidayStruct lh in lunarHolidayInfo)
                    {
                        if ((lh.Month == LunarMonth) && (lh.Day == LunarDay))
                        {
                            tempStr = lh.HolidayName;
                            break;
                        }
                    }

                    //对除夕进行特别处理
                    if (LunarMonth == 12)
                    {
                        int leapMonth = calendar.GetLeapMonth(LunarYear);
                        int days;
                        //计算当年农历12月的总天数
                        if (leapMonth > 0 && LunarMonth > leapMonth - 1)
                        {
                            days = calendar.GetDaysInMonth(LunarYear, 13);
                        }
                        else
                        {
                            days = calendar.GetDaysInMonth(LunarYear, 12);
                        }

                        if (LunarDay == days) //如果为最后一天
                        {
                            tempStr = "除夕";
                        }
                    }
                }
                return tempStr;
            }
        }

        #endregion

        #region WeekDayHoliday

        /// <summary>
        ///     按某月第几周第几日计算的节日
        /// </summary>
        public string WeekDayHoliday
        {
            get
            {
                string tempStr = "";
                foreach (WeekHolidayStruct wh in weekHolidayInfo)
                {
                    if (CompareWeekDayHoliday(_solarDate, wh.Month, wh.WeekAtMonth, wh.WeekDay))
                    {
                        tempStr = wh.HolidayName;
                        break;
                    }
                }
                return tempStr;
            }
        }

        #endregion

        #region SolarDateHoliday

        /// <summary>
        ///     按公历日计算的节日
        /// </summary>
        public string SolarDateHoliday
        {
            get
            {
                string tempStr = "";

                foreach (SolarHolidayStruct sh in solarHolidayInfo)
                {
                    if ((sh.Month == _solarDate.Month) && (sh.Day == _solarDate.Day))
                    {
                        tempStr = sh.HolidayName;
                        break;
                    }
                }
                return tempStr;
            }
        }

        #endregion

        #endregion

        #region 公历日期

        #region SolarDate

        /// <summary>
        ///     取对应的公历日期
        /// </summary>
        public DateTime SolarDate
        {
            get { return _solarDate; }
            set
            {
                if (_solarDate.Equals(value))
                {
                }
                else
                {
                    //检查日期是否在限制范围内
                    if (value < minSolarDate || value > maxSolarDate)
                    {
                        throw new ChineseCalendarException("公历日期只支持在1901-2-19～2101-1-28范围内");
                    }
                    _solarDate = value;
                    LoadFromSolarDate();
                }
            }
        }

        #endregion

        #region WeekDay

        /// <summary>
        ///     星期几的英文表示
        /// </summary>
        public DayOfWeek WeekDay
        {
            get { return _solarDate.DayOfWeek; }
        }

        #endregion

        #region WeekDayString

        /// <summary>
        ///     星期几的中文表示
        /// </summary>
        public string WeekDayString
        {
            get
            {
                switch (_solarDate.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        return "星期日";
                    case DayOfWeek.Monday:
                        return "星期一";
                    case DayOfWeek.Tuesday:
                        return "星期二";
                    case DayOfWeek.Wednesday:
                        return "星期三";
                    case DayOfWeek.Thursday:
                        return "星期四";
                    case DayOfWeek.Friday:
                        return "星期五";
                    default:
                        return "星期六";
                }
            }
        }

        #endregion

        #region DateString

        /// <summary>
        ///     公历日期中文表示法 如一九九七年七月一日
        /// </summary>
        public string DateString
        {
            get
            {
                string str = "";
                int i;
                string num = _solarDate.Year.ToString();
                for (i = 0; i < num.Length; i++)
                {
                    str += ConvertToChineseNum(num.Substring(i, 1));
                }
                str += "年";
                num = _solarDate.Month.ToString();
                for (i = 0; i < num.Length; i++)
                {
                    str += ConvertToChineseNum(num.Substring(i, 1));
                }
                str += "月";
                num = _solarDate.Day.ToString();
                for (i = 0; i < num.Length; i++)
                {
                    str += ConvertToChineseNum(num.Substring(i, 1));
                }
                str += "日";
                return str;
            }
        }

        #endregion

        #region ChineseConstellation

        /// <summary>
        ///     28星宿计算
        /// </summary>
        public string ChineseConstellation
        {
            get
            {
                TimeSpan ts = _solarDate - ChineseConstellationReferDay;
                int offset = ts.Days;
                int modStarDay = offset%28;
                return (modStarDay >= 0
                    ? ChineseConstellationName[modStarDay]
                    : ChineseConstellationName[27 + modStarDay]);
            }
        }

        #endregion

        #endregion

        #region 农历日期

        #region IsLunarLeapMonth

        /// <summary>
        ///     是否闰月
        /// </summary>
        public bool IsLunarLeapMonth { get; private set; }

        #endregion

        #region LunarDay

        /// <summary>
        ///     农历日
        /// </summary>
        public int LunarDay { get; private set; }

        #endregion

        #region LunarDayString

        /// <summary>
        ///     农历日的中文表示
        /// </summary>
        public string LunarDayString
        {
            get
            {
                if (string.IsNullOrEmpty(_lunarDayString))
                    _lunarDayString = ChineseDayName[LunarDay - 1];
                return _lunarDayString;
            }
        }

        #endregion

        #region LunarMonth

        /// <summary>
        ///     农历的月份
        /// </summary>
        public int LunarMonth { get; private set; }

        #endregion

        #region LunarMonthString

        /// <summary>
        ///     农历月份中文表示
        /// </summary>
        public string LunarMonthString
        {
            get
            {
                if (string.IsNullOrEmpty(_lunarMonthString))
                    _lunarMonthString = ChineseMonthName[LunarMonth - 1];
                return _lunarMonthString;
            }
        }

        #endregion

        #region LunarYear

        /// <summary>
        ///     农历年份
        /// </summary>
        public int LunarYear { get; private set; }

        #endregion

        #region LunarYearString

        /// <summary>
        ///     农历年的中文表示
        /// </summary>
        public string LunarYearString
        {
            get
            {
                if (string.IsNullOrEmpty(_lunarYearString))
                {
                    _lunarYearString = "";
                    string num = LunarYear.ToString();
                    for (int i = 0; i < 4; i++)
                    {
                        _lunarYearString += ChineseNumber.Substring(Convert.ToInt32(num.Substring(i, 1)), 1);
                    }
                }
                return _lunarYearString;
            }
        }

        #endregion

        #endregion

        #region 24节气

        /// <summary>
        ///     定气法计算二十四节气
        /// </summary>
        /// <remarks>
        ///     节气的定法有两种。古代历法采用的称为"恒气"，即按时间把一年等分为24份，
        ///     每一节气平均得15天有余，所以又称"平气"。现代农历采用的称为"定气"，即
        ///     按地球在轨道上的位置为标准，一周360°，两节气之间相隔15°。由于冬至时地
        ///     球位于近日点附近，运动速度较快，因而太阳在黄道上移动15°的时间不到15天。
        ///     夏至前后的情况正好相反，太阳在黄道上移动较慢，一个节气达16天之多。采用
        ///     定气时可以保证春、秋两分必然在昼夜平分的那两天。
        ///     立春：立是开始的意思,立春就是春季的开始。
        ///     雨水：降雨开始，雨量渐增。
        ///     惊蛰：蛰是藏的意思。惊蛰是指春雷乍动，惊醒了蛰伏在土中冬眠的动物。
        ///     春分：分是平分的意思。春分表示昼夜平分。
        ///     清明：天气晴朗，草木繁茂。
        ///     谷雨：雨生百谷。雨量充足而及时，谷类作物能茁壮成长。
        ///     立夏：夏季的开始。
        ///     小满：麦类等夏熟作物籽粒开始饱满。
        ///     芒种：麦类等有芒作物成熟。
        ///     夏至：炎热的夏天来临。
        ///     小暑：暑是炎热的意思。小暑就是气候开始炎热。
        ///     大署：一年中最热的时候。
        ///     立秋：秋季的开始。
        ///     处暑：处是终止、躲藏的意思。处暑是表示炎热的暑天结束。
        ///     白露：天气转凉，露凝而白。
        ///     秋分：昼夜平分。
        ///     寒露：露水以寒，将要结冰。
        ///     霜降：天气渐冷，开始有霜。
        ///     立冬：冬季的开始。
        ///     小雪：开始下雪。
        ///     大雪：降雪量增多，地面可能积雪。
        ///     冬至：寒冷的冬天来临。
        ///     小寒：气候开始寒冷。
        ///     大寒：一年中最冷的时候。
        /// </remarks>
        public string TwentyFourSolarTerms
        {
            get
            {
                var baseDateAndTime = new DateTime(1900, 1, 6, 2, 5, 0); //#1/6/1900 2:05:00 AM#
                string tempStr = "";

                int y = _solarDate.Year;

                for (int i = 0; i < 24; i++)
                {
                    double num = 525948.76*(y - 1900) + sTermsInfo[i];

                    DateTime newDate = baseDateAndTime.AddMinutes(num);
                    if (newDate.DayOfYear == _solarDate.DayOfYear)
                    {
                        tempStr = SolarTerms[i];
                        break;
                    }
                }
                return tempStr;
            }
        }

        /// <summary>
        ///     当前日期前一个最近节气的日期
        /// </summary>
        public DateTime PrevSolarTermsDate
        {
            get { return GetPrevSolarTermsDate(_solarDate); }
        }

        /// <summary>
        ///     当前日期后一个最近节气的日期
        /// </summary>
        public DateTime NextSolarTremsDate
        {
            get { return GetNextSolarTremsDate(_solarDate); }
        }

        #endregion

        #region 星座和诞生石

        #region Constellation

        /// <summary>
        ///     星座
        /// </summary>
        public string Constellation
        {
            get { return GetConstellation(_solarDate); }
        }

        #endregion

        #region BirthStone

        /// <summary>
        ///     诞生石
        /// </summary>
        public string BirthStone
        {
            get { return GetBirthStone(_solarDate); }
        }

        #endregion

        #endregion

        #region 属相

        /// <summary>
        ///     属相
        /// </summary>
        public string AnimalString
        {
            get
            {
                if (string.IsNullOrEmpty(_animal))
                {
                    int y = calendar.GetSexagenaryYear(_solarDate);
                    _animal = AnimalsStr.Substring((y - 1)%12, 1);
                }

                return _animal;
            }
        }

        #endregion

        #region 天干地支

        #region SexagenaryYear

        /// <summary>
        ///     农历年的干支表示，如乙丑年。
        /// </summary>
        public string SexagenaryYear
        {
            get
            {
                if (string.IsNullOrEmpty(_sexagenary))
                {
                    int y = calendar.GetSexagenaryYear(_solarDate);
                    _sexagenary = CelestialStem.Substring((y - 1)%10, 1) +
                                  TerrestrialBranch.Substring((y - 1)%12, 1);
                }
                return _sexagenary;
            }
        }

        #endregion

        #region SexagenaryMonth

        /// <summary>
        ///     干支的月表示，注意农历的闰月不记干支
        /// </summary>
        public string SexagenaryMonth
        {
            get
            {
                //每个月的地支总是固定的,而且总是从寅月开始
                int zhiIndex;
                if (LunarMonth > 10)
                {
                    zhiIndex = LunarMonth - 10;
                }
                else
                {
                    zhiIndex = LunarMonth + 2;
                }
                string zhi = TerrestrialBranch.Substring(zhiIndex - 1, 1);

                //根据当年的干支年的干来计算月干的第一个
                int ganIndex = 1;
                int i = (LunarYear - GanZhiStartYear)%60; //计算干支
                switch (i%10)
                {
                        #region ...

                    case 0: //甲
                        ganIndex = 3;
                        break;
                    case 1: //乙
                        ganIndex = 5;
                        break;
                    case 2: //丙
                        ganIndex = 7;
                        break;
                    case 3: //丁
                        ganIndex = 9;
                        break;
                    case 4: //戊
                        ganIndex = 1;
                        break;
                    case 5: //己
                        ganIndex = 3;
                        break;
                    case 6: //庚
                        ganIndex = 5;
                        break;
                    case 7: //辛
                        ganIndex = 7;
                        break;
                    case 8: //壬
                        ganIndex = 9;
                        break;
                    case 9: //癸
                        ganIndex = 1;
                        break;

                        #endregion
                }
                string gan = CelestialStem.Substring((ganIndex + LunarMonth - 2)%10, 1);

                return gan + zhi;
            }
        }

        #endregion

        #region SexagenaryDay

        /// <summary>
        ///     取干支日表示法
        /// </summary>
        public string SexagenaryDay
        {
            get
            {
                TimeSpan ts = _solarDate - GanZhiStartDay;
                int offset = ts.Days;
                int i = offset%60;
                return CelestialStem.Substring(i%10, 1) + TerrestrialBranch.Substring(i%12, 1);
            }
        }

        #endregion

        #region SexagenaryHour

        /// <summary>
        ///     时辰
        /// </summary>
        public string SexagenaryHour
        {
            get
            {
                _solarDate = new DateTime(_solarDate.Year, _solarDate.Month, _solarDate.Day,
                    DateTime.Today.Hour, DateTime.Today.Minute, DateTime.Today.Second);
                return GetSexagenaryHour();
            }
        }

        #endregion

        #region SexagenaryDateString

        /// <summary>
        ///     取当前日期的干支表示法如 甲子年乙丑月丙庚日
        /// </summary>
        public string SexagenaryDateString
        {
            get { return SexagenaryYear + "年" + SexagenaryMonth + "月" + SexagenaryDay + "日"; }
        }

        #endregion

        #endregion

        #endregion

        #region 公共方法

        #region GetPrevSolarTermsDate

        /// <summary>
        ///     获取指定日期前一个最近节气的日期
        /// </summary>
        public DateTime GetPrevSolarTermsDate(DateTime dt)
        {
            var baseDateAndTime = new DateTime(1900, 1, 6, 2, 5, 0); //#1/6/1900 2:05:00 AM#
            var newDate = new DateTime();

            int y = dt.Year;

            for (int i = 24; i >= 1; i--)
            {
                double num = 525948.76*(y - 1900) + sTermsInfo[i - 1];

                newDate = baseDateAndTime.AddMinutes(num); //按分钟计算

                if (newDate.DayOfYear < dt.DayOfYear)
                {
                    break;
                }
            }
            return newDate;
        }

        #endregion

        #region GetNextSolarTremsDate

        /// <summary>
        ///     获取指定日期后一个最近节气的日期
        /// </summary>
        public DateTime GetNextSolarTremsDate(DateTime dt)
        {
            var baseDateAndTime = new DateTime(1900, 1, 6, 2, 5, 0); //#1/6/1900 2:05:00 AM#
            var newDate = new DateTime();

            int y = dt.Year;

            for (int i = 1; i <= 24; i++)
            {
                double num = 525948.76*(y - 1900) + sTermsInfo[i - 1];

                newDate = baseDateAndTime.AddMinutes(num); //按分钟计算

                if (newDate.DayOfYear > dt.DayOfYear)
                {
                    break;
                }
            }
            return newDate;
        }

        #endregion

        #region GetConstellation

        /// <summary>
        ///     获取指定公历日期的星座
        /// </summary>
        /// <param name="dt">指定的日期</param>
        /// <returns>星座的字符串</returns>
        public string GetConstellation(DateTime dt)
        {
            int index;
            int m = dt.Month;
            int d = dt.Day;
            int y = m*100 + d;

            if (((y >= 321) && (y <= 420)))
            {
                index = 0;
            }
            else if ((y >= 421) && (y <= 520))
            {
                index = 1;
            }
            else if ((y >= 521) && (y <= 620))
            {
                index = 2;
            }
            else if ((y >= 621) && (y <= 722))
            {
                index = 3;
            }
            else if ((y >= 723) && (y <= 822))
            {
                index = 4;
            }
            else if ((y >= 823) && (y <= 922))
            {
                index = 5;
            }
            else if ((y >= 923) && (y <= 1022))
            {
                index = 6;
            }
            else if ((y >= 1023) && (y <= 1121))
            {
                index = 7;
            }
            else if ((y >= 1122) && (y <= 1221))
            {
                index = 8;
            }
            else if ((y >= 1222) || (y <= 119))
            {
                index = 9;
            }
            else if ((y >= 120) && (y <= 218))
            {
                index = 10;
            }
            else if ((y >= 219) && (y <= 320))
            {
                index = 11;
            }
            else
            {
                index = 0;
            }

            return ConstellationName[index];
        }

        #endregion

        #region GetBirthStone

        /// <summary>
        ///     获取指定公历日期的诞生石
        /// </summary>
        /// <param name="dt">指定的日期</param>
        /// <returns>诞生石字符串</returns>
        public string GetBirthStone(DateTime dt)
        {
            int index;
            int m = dt.Month;
            int d = dt.Day;
            int y = m*100 + d;

            if (((y >= 321) && (y <= 420)))
            {
                index = 0;
            }
            else if ((y >= 421) && (y <= 520))
            {
                index = 1;
            }
            else if ((y >= 521) && (y <= 620))
            {
                index = 2;
            }
            else if ((y >= 621) && (y <= 722))
            {
                index = 3;
            }
            else if ((y >= 723) && (y <= 822))
            {
                index = 4;
            }
            else if ((y >= 823) && (y <= 922))
            {
                index = 5;
            }
            else if ((y >= 923) && (y <= 1022))
            {
                index = 6;
            }
            else if ((y >= 1023) && (y <= 1121))
            {
                index = 7;
            }
            else if ((y >= 1122) && (y <= 1221))
            {
                index = 8;
            }
            else if ((y >= 1222) || (y <= 119))
            {
                index = 9;
            }
            else if ((y >= 120) && (y <= 218))
            {
                index = 10;
            }
            else if ((y >= 219) && (y <= 320))
            {
                index = 11;
            }
            else
            {
                index = 0;
            }

            return BirthStoneName[index];
        }

        #endregion

        #region ToString

        /// <summary>
        ///     农历日期的字符串显示，如：二〇〇九年十二月廿四
        /// </summary>
        /// <returns>农历日期的字符串显示</returns>
        public override string ToString()
        {
            if (IsLunarLeapMonth == false)
            {
                return LunarYearString + "年" + LunarMonthString +
                       "月" + LunarDayString;
            }
            return LunarYearString + "年闰" + LunarMonthString +
                   "月" + LunarDayString;
        }

        #endregion

        #region ToSexagenaryString

        /// <summary>
        ///     农历日期天干地支年加生肖表示法，如：己丑(牛)年腊月廿四
        /// </summary>
        /// <returns>农历日期天干地支年加生肖表示法</returns>
        public string ToSexAnimalString()
        {
            if (IsLunarLeapMonth == false)
            {
                return SexagenaryYear + "(" + AnimalString + ")年" +
                       LunarMonthString + "月" + LunarDayString;
            }
            return SexagenaryYear + "(" + AnimalString + ")年闰" +
                   LunarMonthString + "月" + LunarDayString;
        }

        #endregion

        #region GetLeapMonth

        /// <summary>
        ///     获取闰月的月份，如0表示无闰月，6表示闰5月。
        /// </summary>
        /// <param name="year">农历年份</param>
        /// <returns>闰月是第几个月</returns>
        public static int GetLeapMonth(int year)
        {
            if (year < 1901 || year > 2100)
                throw new ChineseCalendarException("只支持1901～2100期间的农历年");
            return calendar.GetLeapMonth(year);
        }

        #endregion

        #region GetDaysInMonth

        /// <summary>
        ///     计算该农历年份月份的天数
        /// </summary>
        /// <param name="year">农历年</param>
        /// <param name="month">农历月，如果没有闰月是（1～12），有闰月是（1～13）</param>
        /// <returns>天数</returns>
        public static int GetDaysInMonth(int year, int month)
        {
            if (year < 1901 || year > 2100)
                throw new ChineseCalendarException("只支持1901～2100期间的农历年");
            return calendar.GetDaysInMonth(year, month);
        }

        #endregion

        #region AddDays

        /// <summary>
        ///     将指定的天数加到此农历日期上
        /// </summary>
        /// <param name="n">天数</param>
        /// <returns>农历日期</returns>
        public ChineseCalendar AddDays(int n)
        {
            return new ChineseCalendar(SolarDate.AddDays(n));
        }

        #endregion

        #region AddMonths

        /// <summary>
        ///     将指定的月份数加到此农历日期上
        /// </summary>
        /// <param name="n">月份数</param>
        /// <returns>农历日期</returns>
        public ChineseCalendar AddMonths(int n)
        {
            var temp = new ChineseCalendar(_solarDate);
            for (int i = 0; i < n; i++)
            {
                temp.SolarDate = temp._solarDate.AddDays(29);
                if (temp.LunarMonth == LunarMonth &&
                    (IsLunarLeapMonth || (IsLunarLeapMonth == false &&
                                      temp.IsLunarLeapMonth == false)))
                {
                    temp.LunarMonth++;
                    temp._lunarMonthString = null;
                }

                int days = GetDaysInMonth(temp.LunarYear, temp.LunarMonth);
                temp.LunarDay = days < LunarDay ? days : LunarDay;

                temp._lunarDayString = null;
            }

            return temp;
        }

        #endregion

        #region AddYears

        /// <summary>
        ///     将指定的年份数加到此农历日期上
        /// </summary>
        /// <param name="n">年份</param>
        /// <returns>农历日期</returns>
        public ChineseCalendar AddYears(int n)
        {
            ChineseCalendar temp = this;
            temp.LunarYear += n;
            temp.IsLunarLeapMonth = false;
            temp._lunarYearString = null;
            temp._sexagenary = null;
            temp._animal = null;
            int days = GetDaysInMonth(temp.LunarYear, temp.LunarMonth);
            if (days < LunarDay)
            {
                temp.LunarDay = days;
                temp._lunarDayString = null;
            }

            return temp;
        }

        #endregion

        #endregion
    }
}