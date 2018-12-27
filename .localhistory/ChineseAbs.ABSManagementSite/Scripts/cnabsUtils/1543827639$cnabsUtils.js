

function cnabsDlgOk(id, title, callback, dialogHeight, dialogWidth) {
    if (dialogHeight == undefined) {
        dialogHeight = 300;
    };
    if (dialogWidth == undefined) {
        dialogWidth = 400;
    };

    var dlg = $('#' + id);
    dlg.dialog({
        closeText: "",
        title: title,
        autoOpen: false,
        closeOnEscape: true,
        show: true,
        hide: false,
        height: dialogHeight,
        width: dialogWidth,
        resizable: false,
        modal: true,
        open: function () {
            $(this).parents('.ui-dialog').attr('tabindex', -1)[0].focus();
            $(this).addClass("cnabs_scrollbar");
            var dialogHead = $('#' + id).prev();
            dialogHead.addClass("cnabsDlgYesNoHeader");
            dialogHead.find(".ui-button").html("×");
            dialogHead.find(".ui-button").addClass("cnabsDlgYesNoClose")
            $(this).parents().css("padding", "0px");
        },
        buttons: {
            "确定": function () {
                if (callback != undefined && callback != null) {
                    var val = callback();
                    if (val == undefined || val == null || val == true) {
                        $(this).dialog("close");
                    }
                }
                else {
                    $(this).dialog("close");
                }
            },
        }
    });

    dlg.dialog("open");
    return dlg;
}

function cnabsDlg(id, title, dialogHeight, dialogWidth, dialogPosition) {
    if (dialogHeight == undefined) {
        dialogHeight = 300;
    };
    if (dialogWidth == undefined) {
        dialogWidth = 400;
    };

    if (dialogPosition == undefined) {
        var dialogPosition = {
            my: "center center",
            at: "center center",
            of: window,
        };
    }

    var dlg = $('#' + id);
    dlg.dialog({
        closeText: "",
        title: title,
        position: dialogPosition,
        autoOpen: false,
        closeOnEscape: true,
        show: true,
        hide: false,
        height: dialogHeight,
        width: dialogWidth,
        resizable: false,
        modal: true,
        open: function () {
            $(this).parents('.ui-dialog').attr('tabindex', -1)[0].focus();
            $(this).addClass("cnabs_scrollbar");
            var dialogHead = $('#' + id).prev();
            dialogHead.addClass("cnabsDlgYesNoHeader");
            dialogHead.find(".ui-button").html("×");
            dialogHead.find(".ui-button").addClass("cnabsDlgYesNoClose")
            $(this).parents().css("padding", "0px");
        },
    });
    dlg.dialog("open");
    return dlg;
}

function cnabsDlgDragStop(id, title, dialogHeight, dialogWidth) {
    if (dialogHeight == undefined) {
        dialogHeight = 300;
    };
    if (dialogWidth == undefined) {
        dialogWidth = 400;
    };
    var dlg = $('#' + id);
    dlg.dialog({
        closeText: "",
        title: title,
        autoOpen: false,
        closeOnEscape: true,
        show: true,
        hide: false,
        height: dialogHeight,
        width: dialogWidth,
        resizable: false,
        modal: true,
        open: function () {
            $(this).parents('.ui-dialog').attr('tabindex', -1)[0].focus();
            $(this).addClass("cnabs_scrollbar");
            var dialogHead = $('#' + id).prev();
            dialogHead.addClass("cnabsDlgYesNoHeader");
            dialogHead.find(".ui-button").html("×");
            dialogHead.find(".ui-button").addClass("cnabsDlgYesNoClose")
            $(this).parents().css("padding", "0px");
        },
        dragStop: function () {
            $(this).dialog("option", "height", 'auto');
        }
    });
    dlg.dialog("open");
    return dlg;
}

function cnabsDlgYesNo(id, title, callback, dialogHeight, dialogWidth) {
    if (dialogHeight == undefined) {
        dialogHeight = 300;
    };
    if (dialogWidth == undefined) {
        dialogWidth = 400;
    };
    var dlg = $('#' + id);
    dlg.dialog({
        closeText: "",
        title: title,
        autoOpen: false,
        closeOnEscape: true,
        show: true,
        hide: false,
        height: dialogHeight,
        width: dialogWidth,
        resizable: false,
        modal: true,
        open: function () {
            $(this).parents('.ui-dialog').attr('tabindex', -1)[0].focus();
            $(this).addClass("cnabs_scrollbar");
            var dialogHead = $('#' + id).prev();
            dialogHead.addClass("cnabsDlgYesNoHeader");
            dialogHead.find(".ui-button").html("×");
            dialogHead.find(".ui-button").addClass("cnabsDlgYesNoClose")
            $(this).parents().css("padding", "0px");
        },
        buttons: {
            "确定": function () {
                var val = callback();
                if (val == undefined || val == null || val == true) {
                    $(this).dialog("close");
                }
            },
            "取消": function () {
                $(this).dialog("close");
            }
        },
    });

    dlg.dialog("open");
    return dlg;
}

function cnabsMsgSuccess(msg, reload) {
    //alertify.success(g_cnabsMsgTitle, msg);
    var message = msg;
    if (msg.Message != null) message = msg.Message;

    if (reload != undefined && reload) {
        alertify.alert(g_cnabsMsgTitle, message + "，页面将在3秒后自动跳转...");
        setTimeout("window.location.reload()", 3000);
    }
    else {
        if (msg.Message != null) { cnabsAlertMore(msg.Message, msg.StackTrace); }
        else { alertify.alert(g_cnabsMsgTitle, msg); }
        // alertify.alert(g_cnabsMsgTitle, msg);
    }
}

function cnabsMsgError(msg, reload) {
    var message = msg;
    if (msg.Message != null) message = msg.Message;
    if (reload != undefined && reload) {
        alertify.alert(g_cnabsMsgTitle, message + "，页面将在3秒后自动跳转...");
        setTimeout("window.location.reload()", 3000);
    }
    else {
        if (msg.Message != null) { cnabsAlertMore(msg.Message, msg.StackTrace); }
        else { alertify.alert(g_cnabsMsgTitle, msg); }
    }
}

function cnabsGetColorByTaskStatus(taskStatus) {
    if (taskStatus == '完成' || taskStatus == 'Finished') {
        return '#55FF55';
    } else if (taskStatus == '准备' || taskStatus == 'Ready') {
        return "#b7afa5";
    }
    else if (taskStatus == '等待' || taskStatus == 'Waitting' || taskStatus == '进行中' || taskStatus == 'Running') {
        return '#3995cd';
    }
    else if (taskStatus == 'Skipped' || taskStatus == '跳过') {
        return '#ffffff';
    }
    else {
        return '#ff4a4a';
    }
}

function cnabsGetChineseStatusByTaskStatus(taskStatus) {
    if (taskStatus == 'Finished') {
        return '完成';
    } else if (taskStatus == 'Waitting') {
        return '等待';
    } else if (taskStatus == 'Running') {
        return '进行中';
    } else if (taskStatus == 'Overdue') {
        return '逾期';
    } else {
        return '错误';
    }
}

function cnabsTranslateTaskExCheckItemStatus(checkItemStatus) {
    if (checkItemStatus == 'Checked') {
        return '完成';
    } else {
        return '等待';
    }
}

function cnabsGetColorByTaskExCheckItemStatus(checkItemStatus) {
    if (checkItemStatus == 'Checked') {
        return 'color:#55FF55';
    } else {
        return 'color:#ffc446';
    }
}

function cnabsIsInt(val) {
    return !isNaN(parseInt(val))
}


function cnabsIsFloat(val) {
    //    ^        # start of string
    //    -{0,1}   # 0 or 1 char of '-'
    //    \d+      # this matches at least 1 digit (and is greedy; it matches as many as possible)

    //    (        # start of capturing group
    //        [\,] # matcher group with an escaped comma inside
    //        \d+  # same thing as above; matches at least 1 digit and as many as possible
    //    )*       # end of capturing group, which is repeated 0 or more times
    //             # this allows prices with and without commas.

    //    (        # start of capturing group
    //        [\.] # matcher group with an escaped fullstop inside
    //        \d+  # same thing; refer to above
    //    )?       # end of capturing group, which is optional.
    //             # this allows a decimal to be optional
    //    $        # end of string

    return /^-{0,1}\d+([\,]\d+)*([\.]\d+)?$/.test(val) && !isNaN(parseFloat(val));
}

function cnabsGetHexColor(colorval) {
    var parts = colorval.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
    delete (parts[0]);
    for (var i = 1; i <= 3; ++i) {
        parts[i] = parseInt(parts[i]).toString(16);
        if (parts[i].length == 1) parts[i] = '0' + parts[i];
    }
    return '#' + parts.join('');
}

function cnabsLightenChanel(strChannel, percent) {
    var v = parseInt(strChannel, 16);

    if (percent > 0) {
        v = parseInt(v + (255 - v) / 100 * percent);
    }
    else {
        v = parseInt(v + (v - 0) / 100 * percent);
    }

    v = ((v < 255) ? v : 255);
    v = ((v > 0) ? v : 0);
    return (v.toString(16).length == 1) ? "0" + v.toString(16) : v.toString(16);
}

function cnabsLightenColor(color, percent) {
    var R = cnabsLightenChanel(color.substring(1, 3), percent);
    var G = cnabsLightenChanel(color.substring(3, 5), percent);
    var B = cnabsLightenChanel(color.substring(5, 7), percent);
    return "#" + R + G + B;
}

function cnabsGetUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) {
        return unescape(r[2]);
    }
    return null;
}

var cnabsBind = function (object, fun) {
    return function () {
        return fun.apply(object, arguments);
    }
}

function cnabsGetUnique(list) {
    var uniqueList = [];
    for (var i = 0; i < list.length; i++) {
        if (uniqueList.indexOf(list[i]) == -1) {
            uniqueList.push(list[i]);
        }
    }
    return uniqueList;
}

function CnabsAutoComplete(obj, autoObj, arr, callback) {
    this.obj = $("#" + obj)[0];//获取输入框id
    this.autoObj = $("#" + autoObj)[0];//获取下拉列表的id
    this.value_arr = arr; //不要包含重复值
    this.index = -1; //当前选中的DIV的索引
    this.search_value = ""; //保存当前搜索的字符
    if (callback != undefined && callback != null) {
        this.callback = callback;
    }
}

CnabsAutoComplete.prototype = {
    //初始化DIV的位置
    init: function () {
        this.autoObj.style.left = this.obj.offsetLeft + "px";
        this.autoObj.style.top = this.obj.offsetTop + this.obj.offsetHeight + "px";
        this.autoObj.style.width = this.obj.offsetWidth - 2 + "px";//减去边框的长度2px
    },
    //删除自动完成需要的所有DIV
    deleteDIV: function () {
        while (this.autoObj.hasChildNodes()) {
            this.autoObj.removeChild(this.autoObj.firstChild);
        }
        this.autoObj.className = "cnabs_auto_complete_hidden";
    },
    //设置值
    setValue: function (_this) {
        return function () {
            _this.obj.value = this.seq;
            _this.autoObj.className = "cnabs_auto_complete_hidden";
        }
    },
    //模拟鼠标移动至DIV时，DIV高亮
    autoOnmouseover: function (_this, _div_index) {
        return function () {
            _this.index = _div_index;
            _this.autoObj.childNodes[_this.index].className = 'cnabs_auto_complete_onmouseover';
        }
    },
    autoOnmouseleave: function (_this, _div_index) {
        return function () {
            _this.index = _div_index;
            _this.autoObj.childNodes[_this.index].className = 'cnabs_auto_complete_onmouseout';
        }
    },
    //更改classname
    changeClassname: function (length) {
        for (var i = 0; i < length; i++) {
            if (i != this.index) {
                this.autoObj.childNodes[i].className = 'cnabs_auto_complete_onmouseout';
            } else {
                this.autoObj.childNodes[i].className = 'cnabs_auto_complete_onmouseover';
                this.obj.value = this.autoObj.childNodes[i].seq;
            }
        }
    },

    //响应键盘
    pressKey: function (event) {
        var length = this.autoObj.children.length;
        //光标键"↓"
        if (event.keyCode == 40) {
            ++this.index;
            if (this.index > length) {
                this.index = 0;
            } else if (this.index == length) {
                this.obj.value = this.search_value;
            }
            this.changeClassname(length);
        }
        //光标键"↑"
        else if (event.keyCode == 38) {
            this.index--;
            if (this.index < -1) {
                this.index = length - 1;
            } else if (this.index == -1) {
                this.obj.value = this.search_value;
            }
            this.changeClassname(length);
        }
        //回车键
        else if (event.keyCode == 13) {
            this.autoObj.className = "cnabs_auto_complete_hidden";
            this.index = -1;
            if (this.callback != undefined && this.callback != null) {
                this.callback();
            }
        } else {
            this.index = -1;
        }
    },
    //程序入口
    start: function (event) {
        if (event.keyCode != 13 && event.keyCode != 38 && event.keyCode != 40) {
            this.init();
            this.deleteDIV();
            this.search_value = this.obj.value;
            var valueArr = this.value_arr;
            valueArr.sort();
            //if (this.obj.value.replace(/(^\s*)|(\s*$)/g, '') == "") { return; }//值为空，退出
            try { var reg = new RegExp("(" + this.obj.value + ")", "i"); }
            catch (e) { return; }
            var div_index = 0;//记录创建的DIV的索引

            var resultList = new Array();
            var subKey = this.obj.value.toLowerCase();
            $.each(valueArr, function (i, item) {
                $.each(item.keys, function (j, key) {
                    if (key.toLowerCase().indexOf(subKey) >= 0) {
                        resultList.push(item.text);
                        return false;
                    }
                });
            });
            resultList.sort(function (l, r) { return l.toLowerCase().indexOf(subKey) - r.toLowerCase().indexOf(subKey); });

            for (var i = 0; i < resultList.length && div_index < 20; i++) {
                var div = document.createElement("div");//添加一个div元素
                div.className = "cnabs_auto_complete_onmouseout";
                div.seq = resultList[i];
                div.onclick = this.setValue(this);
                div.onmouseover = this.autoOnmouseover(this, div_index);
                div.onmouseleave = this.autoOnmouseleave(this, div_index);
                div.innerHTML = resultList[i].replace(reg, "<strong>$1</strong>");//搜索到的字符粗体显示.replace(reg, "<strong>$1</strong>")
                this.autoObj.appendChild(div);
                this.autoObj.className = "cnabs_auto_complete_show";
                div_index++;
            }
        }
        this.pressKey(event);
        window.onresize = cnabsBind(this, function () { this.init(); });
        window.onscroll = cnabsBind(this, function () { this.init(); });
    },
    hide: function () {
        this.autoObj.className = "cnabs_auto_complete_hidden";
    },
    isFocused: function () {
        return $('.cnabs_auto_complete_onmouseover').length > 0;
    },
    getIdByText: function (text) {
        var id = null;
        $.each(this.value_arr, function (i, item) {
            if (item.text == text) {
                id = item.id;
                return false;
            }
        }
        );
        return id;
    }
}

////控制当前位置导航栏开始
//var am_site_map_panelOffsetTop = 66;
//$(document).ready(function () {
//    if ($(".am_site_map_panel").length > 0 && $(".am_site_map_panel").css("position") == "static") {
//        am_site_map_panelOffsetTop = $(".am_site_map_panel")[0].offsetTop;
//    }
//});

//function cnabsfixNavigationBar() {
//    var pageScrollTop = $(window).scrollTop();
//    var tableclothLeft = $(".tablecloth")[0].offsetLeft;
//    var am_site_map_panelLeft = tableclothLeft;
//    if (pageScrollTop > am_site_map_panelOffsetTop) {
//        $(".am_site_map_panel").css({
//            "z-index": "104",
//            "width": 1010 + "px",
//            "position": "fixed",
//            "top": 0 + "px",
//            "left": am_site_map_panelLeft + "px",
//            'background-color': '#2c2c2c',
//        })
//        $(".tablecloth").css({ "paddingTop": 28 + "px" })
//    } else {
//        $(".am_site_map_panel").css({
//            "position": "static",
//            'background-color': '#2c2c2c',
//        })
//        $(".tablecloth").css({ "paddingTop": 1 + "px" })
//    }
//}

//$(window).scroll(function () {
//    if ($(".am_site_map_panel").length > 0) {
//        cnabsfixNavigationBar();
//    }
//})
//$(window).resize(function () {
//    if ($(".am_site_map_panel").length > 0) {
//        var am_site_map_panelWidth = $(".am_site_map_panel")[0].clientWidth - 10;
//        var am_site_map_panelOffsetTop = $(".am_site_map_panel")[0].offsetTop;
//        cnabsfixNavigationBar();
//    }
//})
////控制当前位置导航栏结束

//点击文字checkbox可勾选代码开始
function TextControlCheckbox(checkboxId, checkboxTextId) {
    $("#" + checkboxTextId).click(function () {
        var obj = $("#" + checkboxId);
        obj.prop("checked", !obj.is(":checked"));
    });
};
//点击文字checkbox可勾选代码结束

function cnabsRandom(max) {
    return parseInt(Math.random() * max);
}

function cnabsRandomHtmlId(name) {
    //生成随机页面元素 id
    var random = cnabsRandom(100000);
    var prefix = 'cnabs_' + name + '_id_';
    var id = prefix + random;
    while ($("#" + id).length != 0) {
        random = cnabsRandom(100000);
        id = prefix + random;
    }
    return id;
}

function cnabsAutoDialog(controls, title, callback, message, eventCallback) {

    var showControls = controls != undefined && controls != null && controls.length != 0;
    var showMessage = message != undefined && message != null && message.length != 0;

    var dlgId = cnabsRandomHtmlId('gen_dlg');

    //插入新的dialog div
    var divId = cnabsRandomHtmlId('gen_dlg_div');
    var div = '<div id="' + dlgId + '" class="cnabs_dialogCloth" style="display:none;">';

    if (showMessage) {
        div += '<div>' + message + '</div>';
    }

    if (showControls) {
        div += '<div id="' + divId + '"></div>';
    }

    div += '</div>';
    $('body').append(div);

    //初始化Dialog
    var autoDiv = null;
    if (showControls) {
        autoDiv = new CnabsAutoDiv(divId);
        autoDiv.init(controls, eventCallback);
    }

    var dlg = $('#' + dlgId);
    dlg.dialog({
        closeText: "",
        title: title,
        autoOpen: false,
        closeOnEscape: true,
        show: true,
        hide: false,
        height: 'auto',
        width: 'auto',
        resizable: false,
        modal: true,
        open: function () {
            $(this).parents('.ui-dialog').attr('tabindex', -1)[0].focus();
            $(this).addClass("cnabs_scrollbar");
            var dialogHead = $('#' + dlgId).prev();
            dialogHead.addClass("cnabsDlgYesNoHeader");
            dialogHead.find(".ui-button").html("×");
            dialogHead.find(".ui-button").addClass("cnabsDlgYesNoClose")
            $(this).parents().css("padding", "0px");
        },
        buttons: {
            "确定": function () {
                var uiValue = null;
                if (showControls) {
                    if (!autoDiv.validate(controls, true)) {
                        return;
                    };
                    uiValue = autoDiv.getUIValue();
                }

                var val = callback(uiValue);            
                //if (val == undefined || val == null || val == true) {
                //    $(this).dialog("close");
                //}
            },
            "取消": function () {
                $(this).dialog("close");
            }
        },
        //dialog关闭时，页面中移除dialog元素（防止反复调用dialog导致网页内容无限增加）
        close: function () {
            $('#' + dlgId).remove();
        }
    });

    dlg.dialog("open");
}

function cnabsAutoDlgYesNo(controls, title, callback, message, eventCallback) {

    var showControls = controls != undefined && controls != null && controls.length != 0;
    var showMessage = message != undefined && message != null && message.length != 0;

    var dlgId = cnabsRandomHtmlId('gen_dlg');

    //插入新的dialog div
    var divId = cnabsRandomHtmlId('gen_dlg_div');
    var div = '<div id="' + dlgId + '" class="cnabs_dialogCloth" style="display:none;">';

    if (showMessage) {
        div += '<div>' + message + '</div>';
    }

    if (showControls) {
        div += '<div id="' + divId + '"></div>';
    }

    div += '</div>';
    $('body').append(div);

    //初始化Dialog
    var autoDiv = null;
    if (showControls) {
        autoDiv = new CnabsAutoDiv(divId);
        autoDiv.init(controls, eventCallback);
    }

    var dlg = $('#' + dlgId);
    dlg.dialog({
        closeText: "",
        title: title,
        autoOpen: false,
        closeOnEscape: true,
        show: true,
        hide: false,
        height: 'auto',
        width: 'auto',
        resizable: false,
        modal: true,
        open: function () {
            $(this).parents('.ui-dialog').attr('tabindex', -1)[0].focus();
            $(this).addClass("cnabs_scrollbar");
            var dialogHead = $('#' + dlgId).prev();
            dialogHead.addClass("cnabsDlgYesNoHeader");
            dialogHead.find(".ui-button").html("×");
            dialogHead.find(".ui-button").addClass("cnabsDlgYesNoClose")
            $(this).parents().css("padding", "0px");
        },
        buttons: {
            "确定": function () {
                var uiValue = null;
                if (showControls) {
                    if (!autoDiv.validate(controls, true)) {
                        return;
                    };
                    uiValue = autoDiv.getUIValue();
                }

                var val = callback(uiValue);
                if (val == undefined || val == null || val == true) {
                    $(this).dialog("close");
                }
            },
            "取消": function () {
                $(this).dialog("close");
            }
        },
        //dialog关闭时，页面中移除dialog元素（防止反复调用dialog导致网页内容无限增加）
        close: function () {
            $('#' + dlgId).remove();
        }
    });

    dlg.dialog("open");
}

function cnabsDlgAjaxUploadWord(action, url, param, onSuccess, onError) {
    var option = { maxFileCount: 1, limitFileTypes: ['doc', 'docx'] };
    _cnabsDlgAjaxUploadFile(action, url, param, onSuccess, onError, option);
}

function cnabsDlgAjaxUploadExcel(action, url, param, onSuccess, onError) {
    var option = { maxFileCount: 1, limitFileTypes: ['xls', 'xlsx'] };
    _cnabsDlgAjaxUploadFile(action, url, param, onSuccess, onError, option);
}

function cnabsDlgAjaxUploadWord2003(action, url, param, onSuccess, onError) {
    var option = { maxFileCount: 1, limitFileTypes: ['doc'] };
    _cnabsDlgAjaxUploadFile(action, url, param, onSuccess, onError, option);
}

function cnabsDlgAjaxUploadExcel2003(action, url, param, onSuccess, onError) {
    var option = { maxFileCount: 1, limitFileTypes: ['xls'] };
    _cnabsDlgAjaxUploadFile(action, url, param, onSuccess, onError, option);
}

function cnabsDlgAjaxUploadWord2007(action, url, param, onSuccess, onError) {
    var option = { maxFileCount: 1, limitFileTypes: ['docx'] };
    _cnabsDlgAjaxUploadFile(action, url, param, onSuccess, onError, option);
}

function cnabsDlgAjaxUploadExcel2007(action, url, param, onSuccess, onError) {
    var option = { maxFileCount: 1, limitFileTypes: ['xlsx'] };
    _cnabsDlgAjaxUploadFile(action, url, param, onSuccess, onError, option);
}

function cnabsDlgAjaxUploadZip(action, url, param, onSuccess, onError) {
    var option = { maxFileCount: 1, limitFileTypes: ['zip'] };
    _cnabsDlgAjaxUploadFile(action, url, param, onSuccess, onError, option);
}

function cnabsDlgAjaxUploadFile(action, url, param, onSuccess, onError) {
    _cnabsDlgAjaxUploadFile(action, url, param, onSuccess, onError, { maxFileCount: 1 });
}

function cnabsDlgAjaxUploadFiles(action, url, param, onSuccess, onError) {
    _cnabsDlgAjaxUploadFile(action, url, param, onSuccess, onError, null);
}

function _cnabsDlgAjaxUploadFile(action, url, param, onSuccess, onError, filePickOption) {

    var dlgId = cnabsRandomHtmlId('gen_dlg');

    //插入新的dialog div
    var divId = cnabsRandomHtmlId('gen_dlg_div');
    var filePickerContainerId = cnabsRandomHtmlId('file_picker_container');
    var div = '<div id="' + dlgId + '" class="cnabs_dialogCloth" style="display:none;">';
    div += '<div id="' + filePickerContainerId + '" style="padding-top:10px"></div>';
    div += '</div>';
    $('body').append(div);

    var filePicker = new CnabsFilePicker();
    filePicker.init(filePickerContainerId, filePickOption);

    var title = action;
    var dlg = $('#' + dlgId);
    dlg.dialog({
        closeText: "",
        title: title,
        autoOpen: false,
        closeOnEscape: true,
        show: true,
        hide: false,
        height: 'auto',
        width: 'auto',
        resizable: false,
        modal: true,
        open: function () {
            $(this).parents('.ui-dialog').attr('tabindex', -1)[0].focus();
            $(this).addClass("cnabs_scrollbar");
            var dialogHead = $('#' + dlgId).prev();
            dialogHead.addClass("cnabsDlgYesNoHeader");
            dialogHead.find(".ui-button").html("×");
            dialogHead.find(".ui-button").addClass("cnabsDlgYesNoClose")
            $(this).parents().css("padding", "0px");
        },
        buttons: {
            "确定": function () {
                if (filePicker.totalFileCount() == 0) {
                    cnabsMsgError('请选择文件');
                    return;
                }

                var formData = filePicker.getFormData();
                $.each(param, function (name, value) {
                    formData.append(name, value);
                });

                var selfDialog = $(this);
                cnabsAjaxUploadFileSync(action, url, formData, function (data) {
                    onSuccess(data);
                    selfDialog.dialog("close");
                }, onError);
            },
            "取消": function () {
                $(this).dialog("close");
            }
        },
        //dialog关闭时，页面中移除dialog元素（防止反复调用dialog导致网页内容无限增加）
        close: function () {
            $('#' + dlgId).remove();
        }
    });

    dlg.dialog("open");
}

//dialog框内容自动填充代码开始
function CnabsAutoDiv(divId) {
    this.divId = divId;
}

CnabsAutoDiv.prototype = {
    init: function (controls, eventCallback) {
        this.deleteElements();
        this.controls = controls;
        var obj = this;
        $.each(this.controls, function () {
            obj.initElement(obj.divId, this.title, this.type, this.elementId, this.value, this.OptionArray, this.placeHolder, this.ellipsis, this.disabled, this.checkboxStyle)
        });

        if (eventCallback != undefined && eventCallback != null && eventCallback.onInitialized != null) {
            eventCallback.onInitialized();
        }
    },
    deleteElements: function () {
        $("#" + this.divId).empty();
    },
    //某个控件是否显示错误信息
    setMessage: function (control, message) {
        if (message != undefined && message != null && message.length != 0) {
            $("#" + BoxId + " #" + control.elementId + "Prompt").css("display", "block");
            $("#" + BoxId + " #" + control.elementId + "Prompt" + " .cnabs_dialog_oPromptMsg").html(message);
        }
        else {
            $("#" + BoxId + " #" + control.elementId + "Prompt").css("display", "none");
        }
    },
    validate: function (controls, isShowMsg) {
        this.controls = controls;
        var cnabsAutoDiv = this;
        var BoxId = this.divId;
        var ret = [];
        $.each(this.controls, function () {
            //存在验证时
            if (this.limit != undefined) {
                //验证是否为正整数
                if (this.limit.type == "number") {
                    var obj = {};
                    obj.elementId = this.elementId;
                    obj.flag = true;

                    //判断是否是数值
                    var text = $("#" + BoxId + " #" + this.elementId).val();
                    var InputValue = cnabsParseFloat(text);
                    if (!cnabsIsFloat(text)) {
                        obj.flag = false;
                        obj.msg = this.title + "必须输入数字！";
                        ret.push(obj);
                        return true;
                    }

                    if (this.limit.isDigit != undefined && this.limit.isDigit) {
                        var reg = /^[-+]?\d+$/;
                        if (!reg.test(text)) {
                            obj.flag = false;
                            obj.msg = this.title + "必须输入整数！";
                            ret.push(obj);
                            return true;
                        }
                    }

                    var Max = cnabsParseFloat(this.limit.max);
                    if (!isNaN(Max) && InputValue > Max) {
                        obj.flag = false;
                        obj.msg = this.title + "不能大于" + Max + "！";
                        ret.push(obj);
                        return true;
                    }

                    var Min = cnabsParseFloat(this.limit.min);
                    if (!isNaN(Min) && InputValue < Min) {
                        obj.flag = false;
                        obj.msg = this.title + "不能小于" + Min + "！";
                        ret.push(obj);
                        return true;
                    }

                    ret.push(obj);
                }
                //验证输入字符数，可规定最大最小值
                else if (this.limit.type == "rangelength") {
                    var obj = {};
                    obj.elementId = this.elementId;
                    obj.flag = true;

                    var valueLength = $("#" + BoxId + " #" + this.elementId).val().length;
                    var Max = parseInt(this.limit.max);
                    if (!isNaN(Max) && valueLength > Max) {
                        obj.flag = false;
                        obj.msg = this.title + "不能超过" + Max + "字符数！";
                    }

                    var Min = parseInt(this.limit.min);
                    if (!isNaN(Min) && valueLength < Min) {
                        obj.flag = false;
                        if (Min == 1) {
                            obj.msg = "请填写" + this.title + "！";
                        } else {
                            obj.msg = this.title + "不能少于" + Min + "字符数！";
                        }
                    };
                    ret.push(obj);
                }
                //验证日期格式必须为YYYY-MM-DD
                else if (this.limit.type == "dateISO") {
                    if (this.limit.required == undefined || this.limit.required == 'true' || this.limit.required) {
                        this.limit.required = true;
                    } else {
                        this.limit.required = false;
                    };
                    var Required = this.limit.required;
                    var obj = {};
                    obj.elementId = this.elementId;
                    var value = $("#" + BoxId + " #" + this.elementId).val().replace(/\//g, '-');
                    $("#" + BoxId + " #" + this.elementId).val(value);
                    if (Required) {
                        if ($("#" + BoxId + " #" + this.elementId).val() == '') {
                            obj.flag = false;
                            obj.msg = "请填写" + this.title + "！";
                        } else {
                            var vaildObj = cnabsParseDateArray($("#" + BoxId + " #" + this.elementId).val());
                            obj.flag = vaildObj.isValidDate;
                            obj.msg = this.title + "格式：YYYY-MM-DD";
                            if (obj.flag) {
                                var date = new Date($("#" + BoxId + " #" + this.elementId).val());
                                if (date > new Date(g_cnabsValidYearMax.toString() + "-12-31")) {
                                    obj.flag = false;
                                    obj.msg = this.title + "输入时间不能大于" + g_cnabsValidYearMax + "-12-31";;
                                }
                                else if (date < new Date(g_cnabsValidYearMin.toString())) {
                                    obj.flag = false;
                                    obj.msg = this.title + "输入时间不能小于" + g_cnabsValidYearMin + "-01-01";
                                }
                            }
                        }
                    } else {
                        if ($("#" + BoxId + " #" + this.elementId).val() == '' || $("#" + BoxId + " #" + this.elementId).val() == '-') {
                            obj.flag = true;
                        } else {
                            var vaildObj = cnabsParseDateArray($("#" + BoxId + " #" + this.elementId).val());
                            obj.flag = vaildObj.isValidDate;
                            obj.msg = this.title + "格式：YYYY-MM-DD";
                            if (obj.flag) {
                                var date = new Date($("#" + BoxId + " #" + this.elementId).val());
                                if (date > new Date(g_cnabsValidYearMax.toString() + "-12-31")) {
                                    obj.flag = false;
                                    obj.msg = this.title + "输入时间不能大于" + g_cnabsValidYearMax + "-12-31";
                                }
                                else if (date < new Date(g_cnabsValidYearMin.toString())) {
                                    obj.flag = false;
                                    obj.msg = this.title + "输入时间不能小于" + g_cnabsValidYearMin + "-01-01";
                                }
                            }
                        }
                    }
                    ret.push(obj);
                }
                //验证日期格式必须为YYYY-MM-DD HH:mm
                else if (this.limit.type == "datetimeISO") {
                    if (this.limit.required == undefined || this.limit.required == 'true' || this.limit.required) {
                        this.limit.required = true;
                    } else {
                        this.limit.required = false;
                    };
                    var Required = this.limit.required;
                    var obj = {};
                    obj.elementId = this.elementId;
                    var value = $("#" + BoxId + " #" + this.elementId).val().replace(/\//g, '-');
                    $("#" + BoxId + " #" + this.elementId).val(value);
                    if (Required) {
                        if ($("#" + BoxId + " #" + this.elementId).val() == '') {
                            obj.flag = true;
                            obj.msg = "请填写" + this.title + "！";
                        } else {
                            var reg = /^[1-2]{1}([0-9]{3})-(0[1-9]|1[012])-(([0-2]){1}([0-9]{1})|([3]{1}[0-1]{1}))\s+(20|21|22|23|[0-1]\d):[0-5]\d$/;
                            obj.flag = reg.test($("#" + BoxId + " #" + this.elementId).val());
                            obj.msg = this.title + "格式：YYYY-MM-DD HH:mm";
                            if (obj.flag) {
                                var date = new Date($("#" + BoxId + " #" + this.elementId).val());
                                if (date > new Date(g_cnabsValidYearMax.toString() + "-12-31 23:59:59")) {
                                    obj.flag = false;
                                    obj.msg = this.title + "输入时间不能大于" + g_cnabsValidYearMax + "-12-31 23:59:59";;
                                }
                                else if (date < new Date(g_cnabsValidYearMin.toString() + "-01-01 00:00:00")) {
                                    obj.flag = false;
                                    obj.msg = this.title + "输入时间不能小于" + g_cnabsValidYearMin + "-01-01 00:00:00";
                                }
                            }
                        }
                    } else {
                        if ($("#" + BoxId + " #" + this.elementId).val() == '' || $("#" + BoxId + " #" + this.elementId).val() == '-') {
                            obj.flag = true;
                        } else {
                            var reg = /^[1-2]{1}([0-9]{3})-(0[1-9]|1[012])-(([0-2]){1}([0-9]{1})|([3]{1}[0-1]{1}))\s+(20|21|22|23|[0-1]\d):[0-5]\d$/;
                            obj.flag = reg.test($("#" + BoxId + " #" + this.elementId).val());
                            obj.msg = this.title + "格式：YYYY-MM-DD HH:mm";
                            if (obj.flag) {
                                var date = new Date($("#" + BoxId + " #" + this.elementId).val());
                                if (date > new Date(g_cnabsValidYearMax.toString() + "-12-31 23:59:59")) {
                                    obj.flag = false;
                                    obj.msg = this.title + "输入时间不能大于" + g_cnabsValidYearMax + "-12-31 23:59:59";;
                                }
                                else if (date < new Date(g_cnabsValidYearMin.toString() + "-01-01 00:00:00")) {
                                    obj.flag = false;
                                    obj.msg = this.title + "输入时间不能小于" + g_cnabsValidYearMin + "-01-01 00:00:00";
                                }
                            }
                        }
                    }
                    ret.push(obj);
                }
                //验证checkbox必须勾选。
                else if (this.limit.type == "check") {
                    var obj = {};
                    obj.elementId = this.elementId;
                    obj.flag = $("#" + BoxId + " #" + this.elementId).is(":checked");
                    obj.msg = this.title + "必须勾选！";
                    ret.push(obj);
                }
                else if (this.limit.type == "email") {
                    if (this.limit.required == undefined || this.limit.required == 'true' || this.limit.required) {
                        this.limit.required = true;
                    } else {
                        this.limit.required = false;
                    }
                    var Required = this.limit.required;
                    var obj = {};
                    obj.elementId = this.elementId;
                    if (Required) {
                        if ($("#" + BoxId + " #" + this.elementId).val() == '') {
                            obj.flag = false;
                            obj.msg = "请填写" + this.title + "！";
                        } else {
                            var reg = /\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}/;
                            var emailValueLength = $("#" + BoxId + " #" + this.elementId).val().length;
                            obj.flag = (reg.test($("#" + BoxId + " #" + this.elementId).val())) && (emailValueLength <= 38);
                            obj.msg = this.title + "不正确！";
                        }
                    } else {
                        if ($("#" + BoxId + " #" + this.elementId).val() == '') {
                            obj.flag = true;
                        } else {
                            var reg = /\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}/;
                            var emailValueLength = $("#" + BoxId + " #" + this.elementId).val().length;
                            obj.flag = (reg.test($("#" + BoxId + " #" + this.elementId).val())) && (emailValueLength <= 38);
                            obj.msg = this.title + "不正确！";
                        }
                    }
                    ret.push(obj);
                }
                else if (this.limit.type == "tel") {
                    if (this.limit.required == undefined || this.limit.required == 'true' || this.limit.required) {
                        this.limit.required = true;
                    } else {
                        this.limit.required = false;
                    }
                    var Required = this.limit.required;
                    var obj = {};
                    obj.elementId = this.elementId;
                    if (Required) {
                        if ($("#" + BoxId + " #" + this.elementId).val() == '') {
                            obj.flag = false;
                            obj.msg = "请填写" + this.title + "！";
                        } else {
                            var reg = /(13\d|14[57]|15[^4,\D]|17[678]|18\d)\d{8}|170[059]\d{7}/;
                            obj.flag = reg.test($("#" + BoxId + " #" + this.elementId).val());
                            obj.msg = this.title + "不正确！";
                        }
                    } else {
                        if ($("#" + BoxId + " #" + this.elementId).val() == '') {
                            obj.flag = true;
                        } else {
                            var reg = /(13\d|14[57]|15[^4,\D]|17[678]|18\d)\d{8}|170[059]\d{7}/;
                            obj.flag = reg.test($("#" + BoxId + " #" + this.elementId).val());
                            obj.msg = this.title + "不正确！";
                        }
                    }
                    ret.push(obj);
                }
                //自定义验证函数
                else if (this.limit.type == "custom") {
                    var obj = {};
                    obj.elementId = this.elementId;
                    var uncheckedElementValue = cnabsAutoDiv.getElementValue(cnabsAutoDiv.divId, this.elementId, this.type, this.OptionArray);
                    var customResult = this.limit.callback(uncheckedElementValue, this.title);
                    obj.flag = customResult.verdict;
                    obj.msg = customResult.msg;
                    ret.push(obj);
                }

            }
        })
        //显示警告
        if (isShowMsg) {
            var findFalse = 0;
            for (var i = 0; i < ret.length; i++) {
                if (ret[i].flag) {
                    $("#" + BoxId + " #" + ret[i].elementId + "Prompt").css("display", "none");
                } else {
                    $("#" + BoxId + " #" + ret[i].elementId + "Prompt").css("display", "block");
                    $("#" + BoxId + " #" + ret[i].elementId + "Prompt" + " .cnabs_dialog_oPromptMsg").html(ret[i].msg);
                    findFalse = 1;
                }
            };
            if (findFalse == 1) {
                return false;
            }
        }

        return ret;
    },
    getUIValue: function () {
        var ret = {};
        var obj = this;
        $.each(this.controls, function () {
            ret[this.elementId] = obj.getElementValue(obj.divId, this.elementId, this.type, this.OptionArray);
        });
        return ret;
    },
    getElementValue: function (BoxId, elementId, type, OptionArray) {
        var elementValue;
        if (type == "text" || type == "password" || type == "datetime") {
            elementValue = $("#" + BoxId + " #" + elementId).val();
        } else if (type == "date") {
            elementValue = $("#" + BoxId + " #" + elementId).val() == '' ? '' : cnabsParseDateArray($("#" + BoxId + " #" + elementId).val()).dateArray[0];
        } else if (type == "select") {
            elementValue = $("#" + BoxId + " #" + elementId + " option:selected").val();
        } else if (type == "checkbox") {
            elementValue = $("#" + BoxId + " #" + elementId).is(":checked");
        } else if (type == "radio") {
            for (var i = 0; i < OptionArray.length; i++) {
                if ($("#" + BoxId + " #" + elementId + i).is(":checked")) {
                    elementValue = $("#" + BoxId + " #" + elementId + i).val();
                    break;
                }
            }
        } else if (type == "textarea") {
            elementValue = $("#" + BoxId + " #" + elementId).val();
        } else if (type == "label") {
            elementValue = $("#" + BoxId + " #" + elementId).text();
        }
        return elementValue;
    },
    initElement: function (BoxId, title, type, elementId, value, OptionArray, placeHolder, ellipsis, disabled, checkboxStyle) {
        var box = document.getElementById(BoxId);
        var oDiv = document.createElement("div");
        oDiv.setAttribute("class", "cnabs_dialog_content_everyline");
        var oSpa = document.createElement("span");
        oSpa.innerHTML = title;
        var oIptPrompt = document.createElement("div");
        oIptPrompt.setAttribute("class", "left");
        var oPrompt = document.createElement("div");
        var oPromptImage = document.createElement("span");
        var oPromptMsg = document.createElement("span");
        oPromptImage.setAttribute("class", "ui-icon-alert cnabs_dialog_oPromptImage left");
        oPromptMsg.setAttribute("class", "left cnabs_dialog_oPromptMsg");
        oPrompt.appendChild(oPromptImage);
        oPrompt.appendChild(oPromptMsg);
        oPrompt.setAttribute("class", "cnabs_dialog_prompt cnabs_dialog_input_wid");
        oPrompt.setAttribute("id", elementId + "Prompt");
        if (type == "text" || type == "password") {
            var oIpt = document.createElement("input");
            oSpa.setAttribute("class", "cnabs_dialog_content_title");
            oIpt.setAttribute("id", elementId);
            oIpt.setAttribute("class", "cnabs_dialog_input_wid");
            oIpt.setAttribute("type", type);
            if (placeHolder != undefined) {
                oIpt.setAttribute("placeHolder", placeHolder);
            };
            if (value != undefined) {
                oIpt.setAttribute("value", value);
            };
            if (disabled != undefined) {
                oIpt.setAttribute("disabled", disabled);
            }
            oIptPrompt.appendChild(oIpt);
            oIptPrompt.appendChild(oPrompt);
            oDiv.appendChild(oSpa);
        } else if (type == "select") {
            var oSel = document.createElement("select");
            oSpa.setAttribute("class", "cnabs_dialog_content_title");
            oSel.setAttribute("id", elementId);
            oSel.setAttribute("class", "cnabs_dialog_input_wid");
            var defOpt = document.createElement("option");
            defOpt.value = "";
            defOpt.innerHTML = "请选择";
            oSel.appendChild(defOpt);
            for (var i = 0; i < OptionArray.length; i++) {
                var oOpt = document.createElement("option");
                var option = OptionArray[i];
                if (option instanceof Array && option.length == 2) {
                    oOpt.value = option[0];
                    oOpt.innerHTML = option[1];
                } else {
                    oOpt.value = option;
                    oOpt.innerHTML = option;
                }
                oSel.appendChild(oOpt);
                
                if (value != undefined && value == oOpt.value) {
                    oOpt.selected = true;
                }
            }
            if (disabled != undefined) {
                oSel.setAttribute("disabled", disabled);
            }
            oIptPrompt.appendChild(oSel);
            oIptPrompt.appendChild(oPrompt);
            oDiv.appendChild(oSpa);
        } else if (type == "checkbox") {
            if (checkboxStyle != undefined && checkboxStyle == 'checkboxText') {
                //选框在前，描述性文字在后
                var oIpt = document.createElement("input");
                oIpt.setAttribute("id", elementId);
                oIpt.setAttribute("class", "left");
                oIpt.setAttribute("type", type);
                if (value != undefined && value == true) {
                    oIpt.checked = true;
                }
                if (disabled != undefined) {
                    oIpt.setAttribute("disabled", disabled);
                }
                var oIptbox = document.createElement("div");
                oIptbox.appendChild(oIpt);
                oSpa.setAttribute("class", "cnabs_brown cnabs_pointer");
                var olabel = document.createElement("label");
                olabel.setAttribute("for", elementId);
                olabel.appendChild(oSpa);
                oIptbox.appendChild(olabel);
                oIptPrompt.appendChild(oIptbox);
                oIptPrompt.appendChild(oPrompt);
            } else {
                //选框在后，描述性文字在前
                var oIpt = document.createElement("input");
                oSpa.setAttribute("class", "cnabs_dialog_content_title");
                oIpt.setAttribute("id", elementId);
                oIpt.setAttribute("class", "left");
                oIpt.setAttribute("type", type);
                if (value != undefined && value == true) {
                    oIpt.checked = true;
                }
                if (disabled != undefined) {
                    oIpt.setAttribute("disabled", disabled);
                }
                var oIptbox = document.createElement("div");
                oIptbox.appendChild(oIpt);
                oIptPrompt.appendChild(oIptbox);
                oIptPrompt.appendChild(oPrompt);
                oDiv.appendChild(oSpa);
            }

        } else if (type == "radio") {
            oSpa.setAttribute("class", "cnabs_dialog_content_title");
            oDiv.appendChild(oSpa);
            var oIptRadio = document.createElement("div");
            oIptRadio.setAttribute("class", "cnabs_dialog_IptRadioBox");
            debugger
            for (var i = 0; i < OptionArray.length; i++) {
                var oIpt = document.createElement("input");
                var oRadio = document.createElement("span");
                oIpt.setAttribute("id", elementId + i);
                oIpt.setAttribute("class", "left");
                oIpt.setAttribute("type", type);
                oIpt.setAttribute("name", "1");
                oIpt.setAttribute("value", OptionArray[i]);
                oRadio.setAttribute("class", "left");
                oRadio.innerHTML = OptionArray[i];
                if (value != undefined && value == OptionArray[i]) {
                    oIpt.checked = true;
                }
                if (disabled != undefined) {
                    oIpt.setAttribute("disabled", disabled);
                }
                oIptRadio.appendChild(oIpt);
                oIptRadio.appendChild(oRadio);
            }
            oIptPrompt.appendChild(oIptRadio);
            oIptPrompt.appendChild(oPrompt);
            oDiv.appendChild(oSpa);
        } else if (type == "textarea") {
            var oTex = document.createElement("textarea");
            oSpa.setAttribute("class", "cnabs_dialog_content_title");
            oTex.setAttribute("id", elementId);
            oTex.setAttribute("class", "cnabs_dialog_input_wid cnabs_dialog_textarea_height");
            if (placeHolder != undefined) {
                oTex.setAttribute("placeHolder", placeHolder);
            };
            if (value != undefined) {
                oTex.value = value;
            };
            if (disabled != undefined) {
                oTex.setAttribute("disabled", disabled);
            }
            oIptPrompt.appendChild(oTex);
            oIptPrompt.appendChild(oPrompt);
            oDiv.appendChild(oSpa);
        } else if (type == "date") {
            var oIpt = document.createElement("input");
            oSpa.setAttribute("class", "cnabs_dialog_content_title");
            oIpt.setAttribute("id", elementId);
            oIpt.setAttribute("class", "date cnabs_dialog_input_wid");
            oIpt.setAttribute("type", "text");
            if (placeHolder != undefined) {
                oIpt.setAttribute("placeHolder", placeHolder);
            };
            if (value != undefined) {
                oIpt.setAttribute("value", value);
            };
            if (disabled != undefined) {
                oIpt.setAttribute("disabled", disabled);
            }
            oIptPrompt.appendChild(oIpt);
            oIptPrompt.appendChild(oPrompt);
            oDiv.appendChild(oSpa);
        }
        else if (type == "datetime") {
            var oIpt = document.createElement("input");
            oSpa.setAttribute("class", "cnabs_dialog_content_title");
            oIpt.setAttribute("id", elementId);
            oIpt.setAttribute("class", "datetime cnabs_dialog_input_wid");
            oIpt.setAttribute("type", "text");
            if (placeHolder != undefined) {
                oIpt.setAttribute("placeHolder", placeHolder);
            };
            if (value != undefined) {
                oIpt.setAttribute("value", value);
            };
            if (disabled != undefined) {
                oIpt.setAttribute("disabled", disabled);
            }
            oIptPrompt.appendChild(oIpt);
            oIptPrompt.appendChild(oPrompt);
            oDiv.appendChild(oSpa);
        } else if (type == "label") {
            var oText = document.createElement("span");
            oSpa.setAttribute("class", "cnabs_dialog_content_title");
            oText.setAttribute("id", elementId);
            if (ellipsis != undefined && ellipsis) {
                oText.setAttribute("class", "cnabs_dialog_input_wid left cnabs_ellipsis");
                if (value != undefined) {
                    oText.setAttribute("title", value)
                }
            } else {
                oText.setAttribute("class", "cnabs_dialog_input_wid left cnabs_breakWord");
            }
            if (value != undefined) {
                oText.innerHTML = value;
            };
            oIptPrompt.appendChild(oText);
            oIptPrompt.appendChild(oPrompt);
            oDiv.appendChild(oSpa);
        }
        else if (type == "multiselect") {
            oSpa.setAttribute("class", "cnabs_dialog_content_title");
            var sel2 = document.createElement("select");
            sel2.setAttribute("class", "cnabs_dialog_content_title");
            sel2.setAttribute("id", elementId);
            sel2.setAttribute("multiple", "multiple");
            oIptPrompt.appendChild(sel2);
            oIptPrompt.appendChild(oPrompt);
            oDiv.appendChild(oSpa);
           
        }
        oDiv.appendChild(oIptPrompt);
        box.appendChild(oDiv);
    }
}
//dialog框内容自动填充代码结束
//解析带“，”的浮点型数值
function cnabsParseFloat(num) {
    num = num + "";
    var val = num.replace(/,/g, "");
    return parseFloat(val);
}

function cnabsInitTimeAgo() {
    jQuery("abbr.timeago").timeago();
}


function cnabsFormatAvatarPath(avatarPath) {
    if (avatarPath == undefined || avatarPath == null) {
        return '';
    }

    if (avatarPath.indexOf("data:") == -1) {
        avatarPath = 'https://www.cn-abs.com' + avatarPath;
    }
    return avatarPath;
}

function cnabsToolbar(selector, option) {
    $(selector).toolbar(option);
}

function cnabsReleaseAllToolbar() {
    $('.tool-container').remove();
}

function cnabsHasContent(str) {
    return str != undefined && str != null && str.length > 0;
}

function cnabsFormatUserName(userInfo) {
    var realName = '';
    if (cnabsHasContent(userInfo.realName)) {
        realName = userInfo.realName;
    } else if (cnabsHasContent(userInfo.RealName)) {
        realName = userInfo.RealName;
    }

    var userName = '';
    if (cnabsHasContent(userInfo.userName)) {
        userName = userInfo.userName;
    } else if (cnabsHasContent(userInfo.UserName)) {
        userName = userInfo.UserName;
    }

    if (cnabsHasContent(realName)) {
        return realName + '(' + userName + ')';
    }

    return userName;
}

//为数字每三位添加一个逗号函数
function cnabsAddCommasToNumber(num) {
    num += '';//改变成字符串
    x = num.split('.');
    x1 = x[0];//整数部分
    x2 = x.length > 1 ? '.' + x[1] : '';//小数部分
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

//Ajax
function cnabsAjax(action, url, param, onSuccess, onError, method) {
    cnabsAjaxBase(action, url, param, onSuccess, onError, {
        isFile: false, isAsync: true, method: method || 'post'
    });
}

function cnabsAjaxSync(action, url, param, onSuccess, onError, method) {
    cnabsAjaxBase(action, url, param, onSuccess, onError, {
        isFile: false, isAsync: false, method: method || 'post'
    });
}

function cnabsAjaxUploadFile(action, url, param, onSuccess, onError) {
    cnabsAjaxBase(action, url, param, onSuccess, onError, {
        isFile: true, isAsync: true, method: "post"
    });
}

function cnabsAjaxUploadFileSync(action, url, param, onSuccess, onError) {
    cnabsAjaxBase(action, url, param, onSuccess, onError, {
        isFile: true, isAsync: false, method: "post"
    });
}

function cnabsAjaxDownloadFile(action, url, param, onSuccess, onError) {
    var callback = function (guid) {
        window.location.href = '/Download/Index?guid=' + guid;

        if (onSuccess != undefined && onSuccess != null) {
            onSuccess();
        }
    };

    cnabsAjax(action, url, param, callback, onError);
}

//为Ajax请求增加等待UI动画
//element 要隐藏的元素
//content 等待的文字说明
function cnabsRegisterUIPending(queryUrl, element, content, insertAfter) {
    cnabsDeregisterUIPending(queryUrl);

    var uiPending = new CnabsUIPending(element, content, insertAfter);
    uiPending.queryUrl = queryUrl;
    g_cnabsUIPending.push(uiPending);
}

function cnabsDeregisterUIPending(queryUrl) {
    for (var i = 0; i < g_cnabsUIPending.length; ++i) {
        if (g_cnabsUIPending[i].queryUrl == queryUrl) {
            g_cnabsUIPending.splice(i, 1);
            break;
        }
    }
}
//为Ajax请求失败时填充错误信息
//element 要隐藏的元素
function cnabsRegisterErrorMessageArea(queryUrl, element) {
    cnabsDeregisterFillErrorMessage(queryUrl);
    var fillErrorMessage = new CnabsFillErrorMessage(element);
    fillErrorMessage.queryUrl = queryUrl;
    g_cnabsFillErrorMessage.push(fillErrorMessage);
}

function cnabsDeregisterFillErrorMessage(queryUrl) {
    for (var i = 0; i < g_cnabsFillErrorMessage.length; ++i) {
        if (g_cnabsFillErrorMessage[i].queryUrl == queryUrl) {
            g_cnabsFillErrorMessage.splice(i, 1);
            break;
        }
    }
}
//注册Ajax锁（同一Ajax请求不能同时发出两次）
function cnabsRegisterAjaxLock(queryUrl, isShowLockedMsg, action) {
    cnabsDeregisterAjaxLock(queryUrl);

    var lock = new Object();
    lock.queryUrl = queryUrl;
    lock.isLocked = false;
    lock.action = action;
    lock.isShowLockedMsg = isShowLockedMsg;
    g_cnabsAjaxLock.push(lock);
}

function cnabsDeregisterAjaxLock(queryUrl) {
    for (var i = 0; i < g_cnabsAjaxLock.length; ++i) {
        if (g_cnabsAjaxLock[i].queryUrl == queryUrl) {
            g_cnabsAjaxLock.splice(i, 1);
            break;
        }
    }
}

//获取日期
function cnabsGetDate() {
    var nowDate = new Date();
    var moment = nowDate.toISOString("yyyy-MM-dd").split("T")[0];
    return moment;
}


var g_cnabsAllUserInfo = new Object();

function cnabsLoadUserInfoTitle(userName) {
    if (!cnabsHasContent(userName)) {
        return '';
    }

    var param = { userName: userName };
    if (g_cnabsAllUserInfo[userName] == undefined) {
        g_cnabsAllUserInfo[userName] = userName;
        cnabsAjaxSync("GetUserInfo", "/ProjectSeries/GetUserInfo", param, function (data) {
            var userInfo = data;
            userInfoTitle = '姓名：' + userInfo.realName + '(' + userName + ')'
                + '\n电话：' + userInfo.cellphone + '\n邮箱：' + userInfo.email;
            g_cnabsAllUserInfo[userName] = userInfoTitle;
        }, function (data) {
            var errorMsg = "加载用户信息失败：";
            if (data != undefined && data != null && cnabsHasContent(data.Value)) {
                errorMsg += data.Value;
            } else {
                errorMsg += "未知错误";
            }

            g_cnabsAllUserInfo[userName] = errorMsg;
        });
    }
}

function cnabsGetUserInfoTitle(userName) {
    if (!cnabsHasContent(userName)) {
        return '';
    }

    return g_cnabsAllUserInfo[userName];
}

//去掉文件的扩展名
function cnabsGetFileNameWithoutExtension(fileName) {
    var index = fileName.lastIndexOf('.');
    return index < 0 ? fileName : fileName.substring(0, index);
}

function cnabsGetFileNameExtension(fileName) {
    var array = fileName.split('.');
    return array[array.length - 1];
}

function cnabsIsImage(fileName) {
    var extension = cnabsGetFileNameExtension(fileName).toLowerCase();
    return (extension == 'png' || extension == 'jpg' || extension == 'gif'
        || extension == 'jpeg' || extension == 'bmp');
}

function cnabsGetFileIconByFileName(fileName) {
    var array = fileName.split('.');
    var suffix = array[array.length - 1];
    if (suffix == 'zip' || suffix == 'rar') {
        return 'rarzip.png';
    } else if (suffix == 'docx' || suffix == 'doc') {
        return 'word.png';
    } else if (suffix == 'xlsx' || suffix == 'xls') {
        return 'excel.png';
    } else if (suffix == 'txt') {
        return 'txt.png';
    } else if (suffix == 'pdf') {
        return 'pdf.png';
    } else if (suffix == 'pptx' || suffix == 'ppt') {
        return 'ppt.png';
    } else if (suffix == 'html' || suffix == 'svg') {
        return 'html.png';
    } else if (suffix == 'png' || suffix == 'jpg' || suffix == 'gif' || suffix == 'jpeg' || suffix == 'bmp') {
        return 'image.png';
    } else {
        return 'undefined.png';
    }
}

var g_cnabsMsgTitle = "中国资产证券化分析网";

function cnabsAlert(message) {
    if (g_disableAlert) {
        return
    }

    if (message.Message != null) message = message.Message;
    return alertify.alert(g_cnabsMsgTitle, message);
}
function detailUpDown(aDetail) {
    var detailstackTrace = aDetail.nextSibling;
    if (detailstackTrace.style.display == "none") {
        detailstackTrace.style.display = "block";
        aDetail.innerHTML = "收起详情";
    } else {
        detailstackTrace.style.display = "none";
        aDetail.innerHTML = "展开详情";
    }
}

function cnabsAlertMore(message, stackTrace) {
    if (g_disableAlert) {
        return
    }
    if (!alertify.errorAlert) {
        alertify.dialog('errorAlert', function factory() {
            return {
                build: function () {
                    //var errorHeader = '<span class="fa fa-times-circle fa-2x" '
                    //+    'style="vertical-align:middle;color:#e10000;">'
                    //+ '</span> Application Error';
                    //this.setHeader(errorHeader);
                }
            };
        }, true, 'alert');
    }
    return alertify.errorAlert(message + "<br/><br/><br/>" +
        "<a id='a_detail' href='javascript:void(0);' onclick='detailUpDown(this)'>展开详情 </a>"
        + "<div id='div_stackTrace' style='display:none;word-break:break-word'>" + html2Escape(stackTrace) + "</div>"
    );

}
var g_disableAlert = false;



function html2Escape(sHtml) {
    if (sHtml == null || sHtml == "")
        return "";
    return sHtml.replace(/[<>&"]/g, function (c) { return { '<': '&lt;', '>': '&gt;', '&': '&amp;', '"': '&quot;' }[c]; });
}

function cnabsRedirect(url) {
    g_disableAlert = true;
    location.href = url;
}

function cnabsSetMsgTitle(title) {
    g_cnabsMsgTitle = title;
}

function cnabsIsValidDate(date) {
    if (!/^\d{4}-\d{1,2}-\d{1,2}$/.test(date)) {
        return { isVaild: false, errorMsg: "日期格式必须为yyyy-mm-dd" };
    }

    var parts = date.split("-");
    var y = parseInt(parts[0], 10);
    var m = parseInt(parts[1], 10);
    var d = parseInt(parts[2], 10);

    if (y < g_cnabsValidYearMin || y > g_cnabsValidYearMax) {
        return {
            isVaild: false,
            errorMsg: "年不在" + g_cnabsValidYearMin + "~" + g_cnabsValidYearMax + "之间"
        };
    }

    if (m <= 0 || m > 12) {
        return {
            isVaild: false,
            errorMsg: "月不在" + 1 + "~" + 12 + "之间"
        };
    }

    var daysPerMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    if (y % 400 == 0 || (y % 100 != 0 && y % 4 == 0)) {
        daysPerMonth[1] = 29;
    }

    if (d <= 0 || d > daysPerMonth[m - 1]) {
        return {
            isVaild: false,
            errorMsg: "日期范围错误"
        };
    }

    return {
        isVaild: true,
        errorMsg: ""
    };;
};

function cnabsParseDateArray(content) {

    content = content.replace(/\t/g, ',');
    content = content.replace(/\r\n/g, ',');
    content = content.replace(/\r/g, ',');
    content = content.replace(/\n/g, ',');
    content = content.replace(/，/g, ',');

    var errorMsg = '';
    var dateArray = content.split(',');
    var result = [];
    for (var i = 0; i < dateArray.length; ++i) {
        var originDate = dateArray[i];
        var date = dateArray[i];
        if (date.length == 0) {
            continue;
        }

        if (date.indexOf('-') != -1
            || date.indexOf('/') != -1
            || date.indexOf('\\') != -1) {
            date = date.replace(/\//g, '-');
            date = date.replace(/\\/g, '-');
        }
        else if (date.length == 8) {
            date = date.substring(0, 4) + '-' + date.substring(4, 6) + '-' + date.substring(6, 8);
        }

        var dateValidation = cnabsIsValidDate(date);
        if (!dateValidation.isVaild) {
            errorMsg = dateValidation.errorMsg + "：" + originDate;
        }
        if (dateValidation.isVaild && date.indexOf('-') != -1) {
            var m = '', d = '';
            if (date.split('-')[1] != undefined && date.split('-')[1].length == 1) {
                m = '0' + date.split('-')[1];
            } else {
                m = date.split('-')[1];
            }
            if (date.split('-')[2] != undefined && date.split('-')[2].length == 1) {
                d = '0' + date.split('-')[2];
            } else {
                d = date.split('-')[2];
            }
            date = date.split('-')[0] + '-' + m + '-' + d;
        }
        result.push(date);

        if (errorMsg.length != 0) {
            break;
        }
    }

    var obj = {};
    obj.isValidDate = errorMsg.length == 0;
    obj.errorMsg = errorMsg;
    obj.dateArray = obj.isValidDate ? result : [];
    return obj;
}

function IsFileRepeat(fileArray, file) {
    var isRepeat = false
    for (var i = 0; i < fileArray.length; i++) {
        if (file.lastModified == fileArray[i].lastModified
            && file.name == fileArray[i].name
            && file.size == fileArray[i].size
            && file.type == fileArray[i].type) {
            isRepeat = true
        }
    }
    return isRepeat
}
function eventPassOrFailInChinese(val) {
    val = val.replace('PASS', '未发生');
    val = val.replace('pass', '未发生');
    val = val.replace('FAIL', '发生');
    val = val.replace('fail', '发生');
    return val;
}
function paymentMethodInChinese(PaymentMethod) {
    var paymentMethod = "-";
    if (PaymentMethod == "EqualPrin") {
        paymentMethod = "等额本金";
    } else if (PaymentMethod == "EqualPmt") {
        paymentMethod = "等额本息";
    } else if (PaymentMethod == "UserDefined") {
        paymentMethod = "固定期限摊还";
    } else if (PaymentMethod == "SingleAmortization") {
        paymentMethod = "到期一次偿清";
    }
    return paymentMethod
}

function paymentFrequencyInChinese(PaymentFrequency) {
    var paymentFrequency = "未知的付款频率";
    if (PaymentFrequency == "TwoWeekly") {
        paymentFrequency = "两周付";
    } else if (PaymentFrequency == "Monthly") {
        paymentFrequency = "月付";
    } else if (PaymentFrequency == "Quarterly") {
        paymentFrequency = "季付";
    } else if (PaymentFrequency == "SemiAnnually") {
        paymentFrequency = "半年付";
    } else if (PaymentFrequency == "Annually") {
        paymentFrequency = "年付";
    }
    return paymentFrequency
}
function cnabsClearFrame() {
    $('iframe').remove();
}
function cnabsDownloadURL(url, count) {
    var hiddenIFrameID = 'hiddenDownloader';
    if (count != undefined) {
        hiddenIFrameID += count;
    }
    var iframe = document.createElement('iframe');
    iframe.id = hiddenIFrameID;
    iframe.style.display = 'none';
    document.body.appendChild(iframe);
    iframe.src = url;
}
//获取当前日期对应的前MonthNumber个月以及后MonthNumber个月的日期
function cnabsGetPastDateAndFutureDate(MonthNumber) {
    var now = new Date();
    var month = now.getMonth();
    var pastMonth, pastYear;
    var futureMonth, futureYear;
    if (month <= MonthNumber) {
        pastMonth = month - MonthNumber + 12;
        pastYear = now.getFullYear() - 1;
    } else {
        pastMonth = month - MonthNumber
        pastYear = now.getFullYear();
    }
    if (month >= 10) {
        futureMonth = month + MonthNumber - 12;
        futureYear = now.getFullYear() + 1;
    } else {
        futureMonth = month + MonthNumber
        futureYear = now.getFullYear();
    }
    var day = now.getDate();
    var pastDay = day;
    var futureDay = day;
    if (day == 31) {
        pastDay = new Date(pastYear, pastMonth + 1, 0).getDate();
        futureDay = new Date(futureYear, futureMonth + 1, 0).getDate();
    }
    if (pastDay <= 9) {
        pastDay = '0' + pastDay;
    }
    if (futureDay <= 9) {
        futureDay = '0' + futureDay;
    }
    pastMonth = pastMonth + 1;
    futureMonth = futureMonth + 1;
    if (pastMonth <= 9) {
        pastMonth = '0' + pastMonth;
    }
    if (futureMonth <= 9) {
        futureMonth = '0' + futureMonth;
    }
    return { past: pastYear + '-' + pastMonth + '-' + pastDay, future: futureYear + '-' + futureMonth + '-' + futureDay }
}
//阻止冒泡
function cnabsStopPropagation(even) {
    var event = even || window.event;
    if (event.stopPropagation) {
        event.stopPropagation()
    } else {
        event.cancelBubble = true;
    }
}

function cnabsInitObject(obj, value) {
    if (!cnabsHasContent(value)) {
        value = '';
    }

    if (obj != undefined && obj != null) {
        for (var propName in obj) {
            obj[propName] = value;
        }
    }
}

function cnabsIsPC() {
    var userAgentInfo = navigator.userAgent;
    var Agents = ["Android", "iPhone",
        "SymbianOS", "Windows Phone",
        "iPad", "iPod"];
    var flag = true;
    for (var v = 0; v < Agents.length; v++) {
        if (userAgentInfo.indexOf(Agents[v]) > 0) {
            flag = false;
            break;
        }
    }
    return flag;
}

function cnabsGetViewportInfoWidth() {
    var w = (window.innerWidth) ? window.innerWidth : (document.documentElement && document.documentElement.clientWidth) ? document.documentElement.clientWidth : document.body.offsetWidth;
    return w;
}
function cnabsGetViewportInfoHeight() {
    var h = (window.innerHeight) ? window.innerHeight : (document.documentElement && document.documentElement.clientHeight) ? document.documentElement.clientHeight : document.body.offsetHeight;
    return h;
}
//关于时间的格式
function cnabsFormatDate(date, type) {
    var result;
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hh = date.getHours();
    var mm = date.getMinutes();
    var ss = date.getSeconds();
    if (month < 10) {
        month = "0" + month;
    }
    if (day < 10) {
        day = "0" + day;
    }
    if (hh < 10) {
        hh = "0" + hh;
    }
    if (mm < 10) {
        mm = "0" + mm;
    }
    if (ss < 10) {
        ss = "0" + ss;
    }
    if (type == "yyyy-MM-dd hh:mm:ss") {
        result = year + "-" + month + "-" + day + " " + hh + ":" + mm + ":" + ss;
    }
    if (type == "yyyy-MM-dd hh:mm") {
        result = year + "-" + month + "-" + day + " " + hh + ":" + mm;
    }
    if (type == "yyyy-MM-dd") {
        result = year + "-" + month + "-" + day;
    }
    return result;
}