//为Ajax加锁（同一Ajax请求不能同时发出两次）
var g_cnabsAjaxLock = [];

function cnabsIsAjaxLocked(queryUrl) {
    for (var i = 0; i < g_cnabsAjaxLock.length; ++i) {
        var lockObj = g_cnabsAjaxLock[i];
        if (lockObj.queryUrl == queryUrl && lockObj.isLocked) {
            if (lockObj.isShowLockedMsg == undefined || lockObj.isShowLockedMsg) {
                alertify.error(lockObj.action + '已经开始，请稍后再试');
            }
            return true;
        }
    }
    return false;
}

function cnabsLockAjax(queryUrl) {
    for (var i = 0; i < g_cnabsAjaxLock.length; ++i) {
        var lockObj = g_cnabsAjaxLock[i];
        if (lockObj.queryUrl == queryUrl) {
            lockObj.isLocked = true;
        }
    }
}

function cnabsUnlockAjax(queryUrl) {
    for (var i = 0; i < g_cnabsAjaxLock.length; ++i) {
        var lockObj = g_cnabsAjaxLock[i];
        if (g_cnabsAjaxLock[i].queryUrl == queryUrl) {
            lockObj.isLocked = false;
        }
    }
}

//为Ajax请求增加等待UI动画
var g_cnabsUIPending = [];

function CnabsUIPending(element, content, insertAfter) {
    this.element = element;
    this.pendingDivId = cnabsRandomHtmlId('ui_pending');
    this.content = content;
    this.insertAfter = insertAfter;
}

CnabsUIPending.prototype = {
    process: function () {
        var html = "<div id='" + this.pendingDivId + "' style='overflow:hidden;width:100%;margin:0px auto 0px;'>";
        html += "<div style='width:53px;margin:0 auto;padding: 35px 0px 20px 0px;'><img src='/Images/Common/running.gif?date=20170426' align='center' alt='Alternate Text'></div>";
        html += "<div style='color:#b7afa5;font-size:12px;text-align:center;'>" + this.content + "</div>";
        html += "</div>";

        if (this.insertAfter != undefined) {
            $(this.insertAfter).after(html);
        } else {
            $(this.element).after(html);
        }
        $(this.element).hide();
    },
    done: function () {
        $('#' + this.pendingDivId).remove();
        $(this.element).show();
    }
}

function cnabsTriggerUIPending(queryUrl) {
    for (var i = 0; i < g_cnabsUIPending.length; ++i) {
        if (g_cnabsUIPending[i].queryUrl == queryUrl) {
            g_cnabsUIPending[i].process();
        }
    }
}

function cnabsReleaseUIPending(queryUrl) {
    for (var i = 0; i < g_cnabsUIPending.length; ++i) {
        if (g_cnabsUIPending[i].queryUrl == queryUrl) {
            g_cnabsUIPending[i].done();
        }
    }
}
//为Ajax请求失败填充错误信息
var g_cnabsFillErrorMessage = [];

function CnabsFillErrorMessage(element){
    this.element = element;
    this.fillErrorMessageDivId = cnabsRandomHtmlId('fill_error_message');
}

CnabsFillErrorMessage.prototype={
    showErrorMessage:function(){
        var html='<div class="cnabs_error_message" id='+this.fillErrorMessageDivId+'>'+this.errorMessage +'</div>';
        $(this.element).html(html);
    }
};

function cnabsTriggerFillErrorMessage(queryUrl, message) {
    for (var i = 0; i < g_cnabsFillErrorMessage.length; ++i) {
        if (g_cnabsFillErrorMessage[i].queryUrl == queryUrl) {
            g_cnabsFillErrorMessage[i].errorMessage = message.replace(/\r\n/g, '</br>');
            g_cnabsFillErrorMessage[i].showErrorMessage();
            return true;
        }
    }
    return false;
}
//通用的底层Ajax处理方法（构造、析构、处理响应）
function cnabsAjaxBase(action, url, param, onSuccess, onError, setting) {
    if (!cnabsAjaxInitialize(url)) {
        return;
    }
    if (setting == undefined) {
        setting = { isAsync: true };
    }

    var method = setting.method || "post"

    $.ajax({
        async: setting.isAsync,
        url: url,
        type: method,
        data: param,
        dataType: 'json',
        contentType: setting.isFile ? false : 'application/x-www-form-urlencoded; charset=UTF-8',
        processData: !setting.isFile,
        success: function (data) {
            cnabsAjaxOnReponseSuccess(action, url, data, onSuccess, onError);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            cnabsAjaxOnReponseError(action, url, XMLHttpRequest, textStatus, errorThrown, onError);
        }
    })
}

function cnabsAjaxInitialize(url) {
    if (cnabsIsAjaxLocked(url)) {
        return false;
    }

    cnabsLockAjax(url);
    cnabsTriggerUIPending(url);
    return true;
}

function cnabsAjaxFinalize(url) {
    cnabsUnlockAjax(url);
    cnabsReleaseUIPending(url);
}

function cnabsAjaxOnReponseSuccess(action, url, data, onSuccess, onError) {
    var dataObj;
    var success = false;
    var errorMessage = "";
    var detail = "";
    if (data["Key"] != undefined) {
        success = data.Key == 0;
        if (success)
            dataObj = data.Value;
        else {
            errorMessage = data.Value.Message

            detail = data.Value.StackTrace;
        }
    }
    else if (data["Code"] != undefined) {
        success = data.Code == 0;
        if (success)
            dataObj = data.Data;
        else {
            errorMessage = data.Message;
        }
    }
    if (success)
        onSuccess(dataObj);
    else {
        
        g_disableAlert = cnabsTriggerFillErrorMessage(url, errorMessage, detail);

        if (onError != undefined && onError != null) {
            onError(errorMessage);
        } else {
            cnabsAlertMore(action + '失败：' + errorMessage);
        }
    }
    cnabsAjaxFinalize(url);
}

function cnabsAjaxOnReponseError(action, url, XMLHttpRequest, textStatus, errorThrown, onError) {
    if (XMLHttpRequest.readyState == 4) {
        if (XMLHttpRequest.status == 401
        || XMLHttpRequest.status == 402)
        window.location = '/Account/Login';
    }
    g_disableAlert = cnabsTriggerFillErrorMessage(url,XMLHttpRequest.Value);
   
    if (onError != undefined && onError != null) {
        onError(XMLHttpRequest);
    } else if (XMLHttpRequest.readyState == 0) {
        cnabsAlert(action + '失败，服务器连接失败');
    } else {
        cnabsAlert(action + '失败，服务器错误');
    }

    cnabsAjaxFinalize(url);
}
var g_cnabsValidYearMax = 2050;
var g_cnabsValidYearMin = 1970;
