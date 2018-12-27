/* Simplified Chinese translation for the jQuery Timepicker Addon /
/ Written by Will Lu */
(function ($) {
    var isIE = !!window.ActiveXObject || "ActiveXObject" in window;

    $.timepicker.regional['zh-CN'] = {
        closeText: '关闭',
        prevText: '&#x3c;上月',
        nextText: '下月&#x3e;',
        currentText: '今天',
        monthNames: ['一月', '二月', '三月', '四月', '五月', '六月',
                '七月', '八月', '九月', '十月', '十一月', '十二月'],
        monthNamesShort: ['一', '二', '三', '四', '五', '六',
                '七', '八', '九', '十', '十一', '十二'],
        dayNames: ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'],
        dayNamesShort: ['周日', '周一', '周二', '周三', '周四', '周五', '周六'],
        dayNamesMin: ['日', '一', '二', '三', '四', '五', '六'],
        weekHeader: '周',
        dateFormat: 'yy-mm-dd',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: true,
        yearSuffix: '年',
        showSecond: true,
        timeFormat:"HH:mm:ss",
		timeOnlyTitle: '选择时间',
		timeText: '时间',
		hourText: '小时',
		minuteText: '分钟',
		secondText: '秒钟',
		millisecText: '毫秒',
		microsecText: '微秒',
		timezoneText: '时区',
		currentText: '现在时间',
		closeText: '关闭',
		timeSuffix: '',
		amNames: ['AM', 'A'],
		pmNames: ['PM', 'P'],
		isRTL: false,
        //Fix bug: datepicker doesn't hide after click close button
        //Reference: https://objectpartners.com/2012/06/18/jquery-ui-datepicker-ie-focus-fix/
		fixFocusIE: false,
		onSelect: function (dateText, inst) {
		    this.fixFocusIE = true;
		},

		onClose: function (dateText, inst) {
		    this.fixFocusIE = true;

		    $(this).blur().change().focus();
		    this.focus();
		},

		beforeShow: function (input, inst) {
		    var result = isIE ? !this.fixFocusIE : true;
		    this.fixFocusIE = false;
		    return result;
		},
	};
	$.timepicker.setDefaults($.timepicker.regional['zh-CN']);
})(jQuery);
