var cnabs_intervalSidebar = 0;
var cnabs_fancytreeFlag = 0;

$(function () {
    //调整导航栏位置
    CnabsNavPositionAjust();

    //验证是否同意用户协议
    //////CnabsCheckIsAgree();

    //验证是否签到
    //CnabsCheckIsSignIn();

    //注册用户统计弹出框
    var pageTitle = $("#thirdMenu").text().trim();
    CurrentPageStatistics(pageTitle);

    //注册意见反馈模板
    CnabsFeedbackTemplate();

    //注册导航栏点击事件
    CnabsInitNavEvent();

    ////注册侧边栏
    //CnabsInitSidebar();
});

//调整导航栏位置
function CnabsNavPositionAjust()
{
    var windowHeight = $(window).height();
    var navHeight = $('#operationNavigator').height();
    $("#operationNavigator").css('top', (windowHeight - navHeight) / 2);
}

//注册导航栏点击事件
function CnabsInitNavEvent()
{
    CnabsNavigatorFocus();//
    $(".cnabsNavIcon").click(function (event) {
        var $target = $(this).children("i");
        if ($target.hasClass("signal")) {
            cnabsGetLatestPageUse();
        }
        else if ($target.hasClass("list"))
            cnabsOnTogl();
        else if ($target.hasClass("daily"))
            cnabsOnDaily();
        else if ($target.hasClass("mail"))
            cnabsOnFeedBack();
        else if ($target.hasClass("up"))
            cnabsToScrollTop();
        else if ($target.hasClass("down"))
            cnabsToScrollBottom();
        else return false;
    });
}

//导航栏鼠标移上样式
function CnabsNavigatorFocus() {
    $(document).on('mouseover mouseout', '.cnabsNavIcon', function (event) {
        var that = $(this);
        if (event.type == 'mouseover')
            that.css('background-color', '#47423C').css('cursor', 'pointer')//.children('i').hide();
            //that.children('div').show();
        else if (event.type == 'mouseout')
            that.css('background-color', 'transparent')//.children('i').show();
        //that.children('div').hide();
    });
}

//验证是否同意用户协议
function CnabsCheckIsAgree()
{
    if (isLogin != "True") {
        return false;
    }

    $.ajax({
        type: 'get',
        url: '/menu/CheckIsUserAgreeWebAgreement',
        dataType: 'json',
        success: function (data) {
            if (data == undefined || data == null)
                return;
             
            if (data.IsAgree==false) {
                $(".agreement").dialog({
                    closeText: "",
                    autoOpen: true,
                    closeOnEscape: false,
                    show: true,
                    hide: true,
                    draggable: false,
                    resizable: false,
                    height: 450,
                    width: 700,
                    modal: true,
                    open: function (event, ui) {
                        $(".ui-dialog-titlebar-close").hide();
                    },
                    buttons: {
                        "同意": function () {
                            var $button = $(".ui-dialog-buttonpane.ui-widget-content.ui-helper-clearfix button:first");
                            $button.attr("disabled", "disabled");
                            var IsAccept = $("#agree").is(":checked");
                            if (IsAccept == false)
                                alert("请阅读并接受用户协议");
                            if (IsAccept == true) {
                                $.ajax({
                                    type:"post",
                                    url: "/menu/AddWebAgreementSigned",
                                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                                        alert(textStatus);
                                    },
                                })
                                $(this).dialog("close");
                            }
                        },
                        "拒绝": function () {
                            var loginOutUrl = $("#hidLoginOutUrl").val();
                            alert(loginOutUrl);
                            window.location.href = loginOutUrl;
                        }
                    },
                });
            }
        }
    });
}

//验证是否签到
function CnabsCheckIsSignIn() {
    if (isLogin != "True") {
        $("#daily-icon").removeClass().addClass("orange write icon");
        return false;
    }
    $.ajax({
        type: "get",
        url: "/menu/CheckIsDailySign",
        dataType: "json",
        data: {},
        success: function (data) {
            if (!data) {
                $("#daily-icon").removeClass().addClass("orange write icon");
            }
        }
    });
}

//点击用户统计
function cnabsGetLatestPageUse() {
    var url = window.location.pathname + window.location.search;
    url = url == "/" ? "/Market/MarketSummary.aspx" : url;
    $("#cnabsUserAccessStatis").dialog("open");
    $.ajax({
        type: 'post',
        url: '/menu/GetPageActiveUserList',
        dataType: 'json',
        data: { action: 'logInfo', url: url },
        success: function (data) {
            if (data.Message == 'fail') {
                $("#cnabsUserAccessStatis img").hide();
                alertify.error('服务器出错');
            } else if (data.Message == 'empty') {
                $("#lbUserTotalCount").text('0');
                $("#lbClickTotalCount").text('0');
                $("#cnabsUserAccessStatis img").hide();
                $("#divEmptyData").show();
            } else {
                $("#lbUserTotalCount").text(data.length.toString());
                var count = 0;
                for (var i = 0; i < data.length; i++) {
                    count += data[i].ClickCount;
                }
                $("#lbClickTotalCount").text(count.toString());
                bindTable(data);
            }
        },
        error: function () {
            alertify.error('服务器出错');
        }
    });
}

//弹出用户统计页面
function CurrentPageStatistics(title) {
    $('#cnabsUserAccessStatis').dialog({
        closeText: "",
        autoOpen: false,
        open: function (event, ui) {
        },
        closeOnEscape: true,
        show: true,
        hide: true,
        height: 650,
        width: 800,
        minWidth: 300,
        minHeight: 350,
        resizable: false,
        modal: true,
        title: title + ' 最近一周用户使用情况',
        buttons: {
            "确定": function () {
                $(this).dialog("close");
            }
        },
    });
}

//绑定用户统计列表
function bindTable(list) {
    var tbl = $("#tblPageLogInfo")
    if ($(tbl).find("tr").length <= 1) {
        var tr = $(tbl).find("tr:eq(0)");
        var newtr, tds = null;
        for (var i = 0; i < list.length; i++) {
            newtr = tr.clone();
            tds = $(newtr).find("td");
            $(tds[0]).text(list[i].UserName);
            $(tds[1]).text(list[i].ClickCount);
            $(tds[2]).text(list[i].RealName);
            $(tds[3]).text(list[i].Company);
            $(tds[4]).text(list[i].CellPhone);
            $(tbl).append(newtr);
        }
        $("#cnabsUserAccessStatis img").hide();
    }
}

//注册侧边栏
function CnabsInitSidebar()
{
    var $sidebar = $(".ui.sidebar");
    var $body = $("body");

    cnabs_intervalSidebar = window.setInterval(function () {
        !$sidebar.hasClass("visible") && ($body.removeClass("pushable") && $sidebar.hide())
        if ($sidebar.hasClass("uncover"))
            $sidebar.removeClass("animating");
    }, 3 * 1000);
}
//点击产品菜单
function cnabsOnTogl() {
    if (isLogin == "False") {
        $("#fancyTree").html($("#cnabsNavigator #unLoginHtml").html());
    }
    else if (cnabs_fancytreeFlag == 0 && isLogin == "True") {
        toglFancyTree();
        cnabs_fancytreeFlag = 1;
    }

    $("body").addClass("pushable");
    var $toglSidebar = $('.ui.sidebar');
    $toglSidebar.sidebar('toggle');
    //** solve the sidebar problem which not hidden in IE start
    $('div.ui.navigator').css('position', 'relative');
    //** solve the sidebar problem which not hidden in IE end

    if ($("div.pusher").hasClass("dimmed"))
        $("body").removeClass("pushable");
}

//弹出产品菜单页面
function toglFancyTree() {
    //var dealid = $("#hidMasterDealId").val();
    var tantree = $("#fancyTree").fancytree({
        clickFolderMode: 4,// 1:activate, 2:expand, 3:activate and expand, 4:activate (dblclick expands)
        autoCollapse: true,// Automatically collapse all siblings, when a node is expanded
        autoScroll: true,// Automatically scroll nodes into visible area
        //minExpandLevel: 0,
        icons: false,
        source: {
            url: "/menu/GetDealTree?dealid=" + dealId + "&from=" + window.location.href,
            //url:"",
            type: "get",
            dataType: "json"
        },
        activate: function (event, data) {
            var node = data.node;
            if (node.data.href) {
                window.open(node.data.href, node.data.target);
            }
            var allChildren = data.node.getParent().getChildren();
            for (var i = 0; i < allChildren.length; i++) {
                if (allChildren[i].key != data.node.key)
                    allChildren[i].setExpanded(false);
                allChildren[i].visit(function (allnode) {
                    allnode.setExpanded(false);
                })
            }
        },
        beforeExpand: function (event, data) {
            var allChildren = data.node.getParent().getChildren();
            for (var i = 0; i < allChildren.length; i++) {
                if (allChildren[i].key != data.node.key)
                    allChildren[i].setExpanded(false);
                allChildren[i].visit(function (allnode) {
                    allnode.setExpanded(false);
                })
            }
        }
    });
    $("#fancyTree").fancytree("getRootNode").visit(function (node) {
        node.setExpanded(true);
    });
}

//点击签到
function cnabsOnDaily() {
    if ($("#liLogin").length > 0) {
        alertify.alert("请登录后再签到")
    }
    else if ($("#daily-icon").hasClass("grey")) {
        alertify.alert("您今天已经签过到了");
    } else {
        $.ajax({
            type: "post",
            url: "/menu/DailySign",
            dataType: "json",
            data: {},
            success: function (data) {
                if (data.OK) {
                    alertify.alert("<p><b>签到成功!</b></p> <p>你已连续签到" + data.Continuation + "天,积分增加" + data.Bonus + ",当前拥有总积分: " + data.Balance + ".</p>");
                    $("#daily-icon").removeClass().addClass("grey write icon");
                } else {
                    alertify.alert("<p><b>签到失败!</b></p> <p>" + data.Err + ".</p>");
                }
            }
        })
    }
}



//注册意见反馈模板
function CnabsFeedbackTemplate() {
    //正在提交
    $("#cnabsSubmitTips").dialog({
        closeText: "",
        autoOpen: false,
        closeOnEscape: false,
        height: 80,
        width: 60,
        resizable: false,
        modal: true,
    });

    $("#cnabsFeedback").dialog({
        closeText: "",
        autoOpen: false,
        open: function (event, ui) {
            $("#content").val('').focus();
            $("#submit_categary").val('function_consult');
        },
        closeOnEscape: true,
        show: true,
        hide: true,
        height: 450,
        width: 700,
        minWidth: 300,
        minHeight: 350,
        resizable: false,
        modal: true,
        buttons: {
            "提交": function () {
                var text = $("#content").val().trim();
                if (text != "") {
                    var categary = $("#submit_categary").find("option:selected").text();
                    var url = window.location.href;
                    var data = "type=feedback&categary=" + categary + "&url=" + url + "&text=" + text;
                    if (isLogin == "False") {
                        var name = $("#name").val().trim();
                        var email = $("#email").val().trim();
                        var reg = /^[-a-z0-9~!$%^&*_=+}{\'?]+(\.[-a-z0-9~!$%^&*_=+}{\'?]+)*@([a-z0-9_][-a-z0-9_]*(\.[-a-z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-z][a-z])|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$/i;
                        if (name == "") {
                            alertify.warning("姓名不能为空！", 5);
                            return false;
                        }
                        if (email.length == 0) {
                            alertify.warning("邮箱不能为空", 5);
                            return false;
                        } else if (reg.test(email) != true) {
                            alertify.warning("邮箱格式不正确", 5);
                            return false;
                        }
                        data = data + "&name=" + name + "&email=" + email;
                    }
                    $.ajax({
                        type: "post",
                        url: "/menu/SubmitUserFeedback",
                        dataType: "json",
                        data: data,
                        beforeSend: function (XHR) {
                            $("#cnabsFeedback").dialog("close");
                            $("#cnabsSubmitTips").dialog("open");
                        },
                        success: function (data) {
                            $("#cnabsSubmitTips").dialog("close");
                            if (data.success == 1) {
                                alertify.success("我们已经收到您的意见，感谢您的反馈！", 5);
                            }
                            else
                                alertify.error("服务器异常，请稍后再试", 5);
                        }
                    })
                }
                else {
                    alertify.warning("反馈内容不能为空！", 5);
                    return false;
                }
            },
            "取消": function () {
                $(this).dialog("close");
            }
        },
    });

    $(".feedback-text").on('input propertychange blur', function () {
        var _this = $(this);
        if (_this.val() == '') {
            $("span.palcehold").show();
        } else {
            $("span.palcehold").hide();
        }
    });

}

//点击问题反馈
function cnabsOnFeedBack() {
    $("body").removeClass("pushable");
    if (isLogin == "False") {
        var $name = $("<label for=\"name\"> 姓名：</label><input type=\"text\" id=\"name\" />");
        var $email = $("<label for=\"email\"> 邮箱：</label><input type=\"text\" id=\"email\" />");
        if ($("#name")[0] == null && $("#email")[0] == null)
            $("#submit_categary").after($email).after($name);
        $("#cnabsFeedback").dialog("open").css("overflow", "hidden");
        $(".ui-dialog.ui-front").css("z-index", 101);
    }
    else if (isLogin == "True") {
        $("#cnabsFeedback").dialog("open").css("overflow", "hidden");
        $(".ui-dialog.ui-front").css("z-index", 101);
    }
    else
        window.location.href = "Account/Login.aspx";
}

//回到顶部
function cnabsToScrollTop() {
    $('body,html').stop();
    $('body,html').animate({ scrollTop: 0 }, 1000);
    return false;
}

//回到底部
function cnabsToScrollBottom() {
    $('body,html').stop();
    $('body,html').animate({ scrollTop: $("#cnabsScrollBottom").offset().top }, 1000);
    return false;
}
