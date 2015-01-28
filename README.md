# C# 中带有中国农历的日期选择控件 

C# 本身没有提供农历的日期控件，因此在刚开始接触 C# 之后，就开发了一款带有农历的日期选择控件。

包含二个部分：

名称                | 描述
--------------------|----------------
MyMonthCalendar     | 对应 C# 本身的 MonthCalendar
MyDateTimePicker    | 对应 C# 本身的 DateTimePicker

开发这样一款带有农历信息的日期选择控件，需要有一个处理农历的类，由于已经有很多人做了，就不再重复造轮子了。我选择的 ChineseCalendar 本身提供的信息如下：

1. 农历范围1901-01-01～2100-12-29
2. 公历范围1901-02-19～2101-01-28
3. 生肖
4. 时辰
5. 星座
6. 二十四节气
7. 公历节假日
8. 农历节假日

## 显示效果

日期显示的背景颜色是可以自定义的

![此处输入图片的描述][1]

## 使用方法

1. 如果仅仅只是使用 MyMonthCalendar，需要引用ChineseCalendar.dll和MyMonthCalendar.dll。
2. 如果是使用MyDateTimePicker，还需要引用MouseKeyboardLibrary.dll和MyDateTimePicker.dll。MyDateTimePicker使用了MouseKeyboardLibrary来判断鼠标是否点击选择了日期。

然后在工具箱中拖动控件即可。


  [1]: http://oxygen.qiniudn.com/img2015012829.png