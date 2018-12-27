// 触发日期选择框
$.ui.dialog.prototype._focusTabbable = $.noop;
$(document).on("focus", ".date", function () {
	$(".date").datepicker({
        prevText:"上月",
        nextText:"下月",
        changeMonth:true,
        changeYear: true,
        yearSuffix: '',
        yearRange: "1970:2050",
        dateFormat: "yy-mm-dd",
        monthNamesShort: ['一月','二月','三月','四月','五月','六月','七月','八月','九月','十月','十一月','十二月'],
        monthNames: ['一月','二月','三月','四月','五月','六月','七月','八月','九月','十月','十一月','十二月'],
        dayNames: ['星期日','星期一','星期二','星期三','星期四','星期五','星期六'],  
        dayNamesShort: ['周日','周一','周二','周三','周四','周五','周六'],  
        dayNamesMin: ['日','一','二','三','四','五','六']
	});
});

$(document).on("focus", ".datetime", function () {
 
    $(".datetime").datetimepicker({
        timeInput: true,
        timeFormat: "HH:mm",
        showHour: false,
        showMinute: false,
        showSecond: false,

        showMonthAfterYear: false,
        changeMonth: true,
        changeYear: true,
        yearSuffix: '',
        yearRange: "1970:2050",
        monthNamesShort: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
        firstDay: 0
    }) ;

});