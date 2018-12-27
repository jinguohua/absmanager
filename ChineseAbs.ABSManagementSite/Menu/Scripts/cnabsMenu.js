/*global $ */
$(document).ready(function () {

    "use strict";

    $('.menu > ul > li:has( > ul)').addClass('menu-dropdown-icon');
    //Checks if li has sub (ul) and adds class for toggle icon - just an UI


    $('.menu > ul > li > ul:not(:has(ul))').addClass('normal-sub');
    //Checks if drodown menu's li elements have anothere level (ul), if not the dropdown is shown as regular dropdown, not a mega menu (thanks Luka Kladaric)

    $(".menu > ul").before("<a href=\"#\" class=\"menu-mobile\">Navigation</a>");

    //Adds menu-mobile class (for mobile toggle menu) before the normal menu
    //Mobile menu is hidden if width is more then 959px, but normal menu is displayed
    //Normal menu is hidden if width is below 959px, and jquery adds mobile menu
    //Done this way so it can be used with wordpress without any trouble

    //$(".menu > ul > li").hover(function (e) {
    //    if ($(window).width() > 943) {
    //        var isVisible = $(this).children("ul").is(":visible");
    //        if (!isVisible && e.type == "mouseenter"
    //            || isVisible && e.type == "mouseleave") {
    //            $(this).children("ul").stop(true, false).toggle();
    //            e.preventDefault();
    //        }
    //    }
    //});
    $(document).click(function () {
        $(".menu > ul > li").css("background-color", "#3B3831");
        $(".menu > ul > li").siblings().children("ul").fadeOut();
    });
    $(".menu > ul > li").click(function (e) {
        e.stopPropagation();
        $(this).css("background-color", "#443F39");
        $(this).siblings().css("background-color", "#3B3831");
        $(this).children("ul").show();
        $(this).siblings().children("ul").fadeOut();
        $(this).children("ul").click(function (e) {
            e.stopPropagation();
        })
    });
    $("#deallist").click(function () {
        $(".menu > ul > li").children("ul").fadeOut();
    })
    //If width is more than 943px dropdowns are displayed on hover

    $(".menu > ul > li").click(function (e) {
        if ($(window).width() <= 943) {
            $(this).children("ul").fadeToggle(150);
        }
    });
    //If width is less or equal to 943px dropdowns are displayed on click (thanks Aman Jain from stackoverflow)

    $(".menu-mobile").click(function (e) {
        $(".menu > ul").toggleClass('show-on-mobile');
        e.preventDefault();
    });
    //when clicked on mobile-menu, normal menu is shown as a list, classic rwd menu story (thanks mwl from stackoverflow)

});

//** portfolio news
function getLatestDealNews() {
    $.ajax({
        type: "get",
        url: "/Menu/Ajax/LatestDealNewsHandler.ashx",
        dataType: "json",
        data: { method: "global", dealID: 0 },
        success: function (data) {
            if (data.OK && data.News != null) {
                alertify.error("你的组合管理有新的相关负面消息: <a href=\"" + data.News.url + "\">" + data.News.title + "</a> 涉及机构: " + data.News.agency, 7)
            }
        }
    })
}

function BindingDealHref($id, href, $dealId) {
    $("#" + $id).attr("href", href + $dealId);
}

//** msg function **
function getUnreadMsgCount(cancel) {
    if ($(".message-informer").length == 0) {
        window.clearInterval(intervalId);
        return;
    }

    $.get("Ajax/MessageHandler.ashx", { action: "getunread" }, function (data) {
        if (data.OK) {
            if (data.managecount != 0) {
                $(".message-informer.management").text(data.managecount);
                $(".message-informer.management").show();
            }
            if (data.abscount != 0) {
                $(".message-informer.cnabs").text(data.abscount);
                $(".message-informer.cnabs").show();
            }
            if (data.total != 0) {
                $(".message-informer.all").text(data.total);
                $(".message-informer.all").show();
            }
            if (data.ticketCount != 0) {
                $(".message-informer.ticket").text(data.ticketCount);
                $(".message-informer.ticket").show();
            }
        } else {
            window.clearInterval(intervalId);
        }
    });
}

//** fast pass icon function **
function togl() {

    if (window.loginParam == 0) {
        $("#fancyTree").html("<div style='margin:20px auto;width:120px;height:30px;vertical-align:middle;'><span><a href='../Account/login.aspx'>登录</a>查看产品列表</span></div>");
    }
    else if (fancytreeFlag == 0 && window.loginParam == 1) {
        toglFancyTree();
        fancytreeFlag = 1;
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

function feedback() {
    $("body").removeClass("pushable");
    if (loginParam == 0) {
        var $name = $("<label for=\"name\">姓名：</label><input id=\"name\" />");
        var $email = $("<label for=\"email\">邮箱：</label><input id=\"email\" />");
        if ($("#name")[0] == null && $("#email")[0] == null)
            $("#submit_categary").after($email).after($name);
        $("#dialog").dialog("open").css("overflow", "hidden");
        $(".ui-dialog.ui-front").css("z-index", 101);
    }
    else if (loginParam == 1) {
        $("#dialog").dialog("open").css("overflow", "hidden");
        $(".ui-dialog.ui-front").css("z-index", 101);
    }
    else
        window.location.href = "Account/Login.aspx";
}

function toScrollTop() {
    $('body,html').animate({ scrollTop: 0 }, 1000);
    return false;
}

function toScrollBottom() {
    $('body,html').animate({ scrollTop: $(".scrollBottom").offset().top }, 1000);
    return false;
}

//** feedback dialog template **
function FeedbackTemplate() {
    $("#dialog").dialog({
        closeText: "",
        autoOpen: false,
        open: function (event, ui) {
            $("#content").focus();
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
                    if (window.loginParam == 0) {
                        var name = $("#name").val().trim();
                        var email = $("#email").val().trim();
                        var reg = /^[-a-z0-9~!$%^&*_=+}{\'?]+(\.[-a-z0-9~!$%^&*_=+}{\'?]+)*@([a-z0-9_][-a-z0-9_]*(\.[-a-z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-z][a-z])|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$/i;
                        if (name == "") {
                            alertify.warning("姓名不能为空！", 5);
                            return false;
                        }
                        if (reg.test(email) != true) {
                            alertify.warning("邮箱格式不正确", 5);
                            return false;
                        }
                        data = data + "&name=" + name + "&email=" + email;
                    }
                    $.ajax({
                        type: "post",
                        url: "/Menu/Ajax/UsersFeedback.ashx",
                        dataType: "json",
                        data: data,
                        beforeSend: function (XHR) {
                            $("#dialog").dialog("close");
                            $("#submitTips").dialog("open");
                        },
                        success: function (data) {
                            $("#submitTips").dialog("close");
                            if (data["success"] == 1)
                                alertify.success("我们已经收到您的意见，感谢您的反馈！", 5);
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
}

//** feedback tips
function SubmitTips() {
    $("#submitTips").dialog({
        closeText: "",
        autoOpen: false,
        closeOnEscape: false,
        height: 80,
        width: 60,
        resizable: false,
        modal: true,
    });
}

//** highlight menu **
function BindingElementActive($navIndex, $subIndex, $menu) {
    return false;
    var $nav = $(".menu>ul>li:eq(" + $navIndex + ")");
    $nav.children("a").css("color", "#ffc446");
    //$nav.children('a').children('i').css('color', '#b7afa5');
    var $link = $nav.find("ul>div:eq(" + $subIndex + ")>li>ul>li>ul>li:eq(" + $menu + ")");
    $link.children("a").css("color", "#ffc446");
    $link.parent().siblings("ul").find("li").each(function () {
        $(this).children("a").css("color", "#D6D6D6");
    });
}

//** lock and unlock menu **
function MenuLock() {
    $.ajax({
        async: false,
        url: "/Menu/Ajax/MenuHandler.ashx?fresh=" + Math.random(),
        dataType: "json",
        success: function (data) {
            console.log();
            $.each(data, function (index, content) {
                if (data != null) {
                    var platform = ["产品在线设计", "发行协作平台", "存续期管理平台"];
                    if (content["type"] == "lock") {
                        $.each(content["names"], function (ind, con) {
                            if (platform.indexOf(con) != -1) {
                                var $platform = $("div[data-lock='" + con + "']");
                                var $list = $platform.parent("li").children("ul").children("li");
                                $.each($list, function (i, c) {
                                    $(c).append($("<i class='lock icon'></i>"));
                                });
                            }
                            else {
                                var $lock = $("a[data-lock='" + con + "']");
                                $lock && $lock
                                .parent("li").append($("<i class='lock icon'></i>"));
                            }
                        });
                    }
                    else if (content["type"] == "unlock") {
                        $.each(content["names"], function (ind, con) {
                            if (platform.indexOf(con) != -1) {
                                var $platform = $("div[data-lock='" + con + "']");
                                var $list = $platform.parent("li").children("ul").children("li");
                                $.each($list, function (i, c) {
                                    $(c).append($("<i class='unlock icon'></i>"));
                                });
                            }
                            else {
                                var $unlock = $("a[data-lock='" + con + "']");
                                $unlock && $unlock
                                .parent("li").append($("<i class='unlock icon'></i>"));
                            }
                        });
                    }
                }
            });
        },
        error: function (a, b, c) {
            alert(c);
        }
    });
}

function MenuLockTip() {
    $(".lock.icon").siblings("a").click(function () {
        alertify.alert("我们将逐步开放这些功能给机构用户，您如对此功能有兴趣请联系客服。联系电话：<a href='tel:021-31156258'>021-31156258</a>，邮箱：" +
            "<a href='mailto:feedback@cn-abs.com' style='color:#ffc446;'>feedback@cn-abs.com</a>，或者直接下载<a href='/机构账户开通申请表.docx'>申请表</a>。");
        return false;
    });
}

//** sidebar initial **
function getFontCss(treeId, treeNode) {
    return (treeNode.selected == "selected") ? { color: "orange", "font-weight": "bold" } : { color: "white" };
}

function ExpandDealTree() {
    var ztreeObj;
    var t = $("#dealTree");
    var dealid = $("#hidMasterDealId").val();
    var setting = {
        view: {
            dblClickExpand: false,
            showLine: true,
            showIcon: false,
            selectedMulti: false,
            fontCss: getFontCss,
        },
        async: {
            enable: true,
            url: "/Menu/Ajax/DealTreeSearchHandler.ashx?dealid=" + dealid + "&from=" + window.location.href,
            type: "get",
            dataType: "json",
        },
        data: {
            simpleData: {
                enable: true,
            }
        },
        callback: {
            beforeClick: function (treeId, treeNode) {
                var zTree = $.fn.zTree.getZTreeObj(treeId);
                if (treeNode.isParent) {
                    zTree.expandNode(treeNode, null, false, false);
                    return false;
                }
            },
        }
    };
    ztreeObj = $.fn.zTree.init(t, setting);
}

function toglFancyTree() {
    var dealid = $("#hidMasterDealId").val();
    var tantree = $("#fancyTree").fancytree({
        clickFolderMode: 4,// 1:activate, 2:expand, 3:activate and expand, 4:activate (dblclick expands)
        autoCollapse: true,// Automatically collapse all siblings, when a node is expanded
        autoScroll: true,// Automatically scroll nodes into visible area
        //minExpandLevel: 0,
        icons: false,
        source: {
            url: "/Menu/Ajax/DealTreeSearchHandler.ashx?dealid=" + dealid + "&from=" + window.location.href,
            type: "get",
            dataType: "json",
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
        }
    });
    $("#fancyTree").fancytree("getRootNode").visit(function (node) {
        node.setExpanded(true);
    });
}

//** navigator hover event **
function NavigatorFocus() {
    $(document).on('mouseover mouseout', '.navIcon', function (event) {
        var that = $(this);
        if (event.type == 'mouseover')
            that.css('background-color', '#47423C').css('cursor', 'pointer')//.children('i').hide();
            //that.children('div').show();
        else if (event.type == 'mouseout')
            that.css('background-color', 'transparent')//.children('i').show();
        //that.children('div').hide();
    });
}

//** custom-menu icon function **
function CustomMenuInitial() {
    $('.custom-menu-content').dialog({
        closeText: "",
        autoOpen: false,
        open: function (event, ui) {
            //initial menu list
            if (window.loginParam == 0)
                alertify.warning('请登录后添加自定义菜单');
            else
                $.post('/Menu/Ajax/CustomMenuHandler.ashx', { action: 'menuGet' }, function (data) {
                    if (data == '') {
                        $('.my-custom-menu').html('').append('<div>暂无自定义菜单，请在下方列表中选择添加。</div>')
                        $('div.more-custom-menu table td.disabled').removeClass('disabled');
                    }
                    else {
                        var menu = data.split(',');
                        getCustomMenu(menu);
                    }
                });
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
        buttons: {
            "保存": function () {
                var $result = $('.my-custom-menu>span>span');
                var name = [];
                $.each($result, function (i, c) {
                    name.push($(c).html());
                });
                var that = $(this);
                $.post('/Menu/Ajax/CustomMenuHandler.ashx', { action: 'menuSave', names: name.join(',') }, function (data) {
                    if (data == 1) {
                        alertify.success('自定义菜单已保存');
                        that.dialog("close");
                        window.location.reload();
                    }
                    else alertify.error('服务器错误，请返回重试');
                });

            }
        },
    });
    //initial menu
    if (window.loginParam == 1)
        $.post('/Menu/Ajax/CustomMenuHandler.ashx', { action: 'menuGet' }, function (data) {
            if (data != "") {
                var menu = data.split(',');
                createCustomMenu(menu);
            }
            menuPositionAjust();
        });
    else
        menuPositionAjust();
}

//my custom menu
$(document).on('click', '.my-custom-menu>span', function () {
    var that = $(this);
    var $selected = 'selected';
    that.addClass($selected);
    that.siblings('.' + $selected).removeClass($selected);
    if (that.parent('.my-custom-menu').children('span').hasClass($selected) &&
        $('.my-custom-menu-operation').length == 0) {
        var $customOperation = $('<div></div>');
        $customOperation.addClass('my-custom-menu-operation');
        $customOperation.append('<button>上移</button>').append('<button>下移</button>').append('<button>删除</button>');
        $customOperation.find('button').addClass('button');
        $('.my-custom-menu').after($customOperation);
    }
});

$(document).on('click', '.my-custom-menu-operation>button', function () {
    var $text = $(this).html();
    var textFlag = $text.indexOf('上') >= 0 ? 1 : $text.indexOf('下') >= 0 ? 2 : $text.indexOf('删') >= 0 ? 3 : 0;
    var $selected = $('.my-custom-menu>span.selected')
    if ($selected.length == 1) {
        switch (textFlag) {
            case 1:
                moveUp($selected.prev(), $selected);
                break;
            case 2:
                moveDown($selected, $selected.next());
                break;
            case 3:
                deleteItem($selected);
                break;
            default: alertify.error('操作错误，请返回重试');
        }
    }
    else
        alertify.error('操作错误，请选择菜单项');
});

function moveUp(preDom, curDom) {
    var $curDom = $(curDom);
    if (preDom.length > 0) {
        $(preDom).before($curDom);
    }
}

function moveDown(curDom, nextDom) {
    var $curDom = $(curDom);
    if (nextDom.length > 0) {
        $(nextDom).after($curDom);
    }
}

function deleteItem(curDom) {
    var that = $(curDom);
    var $text = that.find('span').html();
    var $container = $('.more-custom-menu table td>span');
    $.each($container, function (idex, content) {
        if ($text == $(content).html()) {
            $(content).parent('td').removeClass('disabled');
            return false;
        }
    });
    that.remove();
    if ($('.my-custom-menu>span').length == 0) {
        $('.my-custom-menu').html('').append('<div>暂无自定义菜单，请在下方列表中选择添加。</div>');
        $('.my-custom-menu-operation').remove();
    }
}

//more custom menu
$(document).on('click', ".more-custom-menu table td:has(i):not('.disabled')", function (event) {
    //add to my custom-menu
    var $item = $('<span></span>').append($(this).html() + '&nbsp;&nbsp;&nbsp;&nbsp;');
    var $myMenu = $('.my-custom-menu');
    if ($myMenu.html().indexOf('选择添加') > 0)
        $myMenu.html('');
    $myMenu.append($item);
    //add tag for disabled selected
    $(this).addClass('disabled');
});

//add custom ment to master
function createCustomMenu(menu) {
    var $customMenu = $('#sub-operation>div.custom-menu');
    $.each(menu, function (i, c) {
        var $div = $('<div class="navIcon"></div>');
        $div.attr('title', c);
        //add icon
        var menuIconList = $('div.more-custom-menu table td:has(i)>span');
        $.each(menuIconList, function (ind, con) {
            if ($(con).html() == c) {
                $div.append($(con).parent('td').children('i').clone().addClass('big grey iconStyle'));
                return false;
            }
        });
        //add url click
        var menuUrlList = $('div.menu-container a[data-lock]');
        $.each(menuUrlList, function (ind, con) {
            if ($(con).attr('data-lock') == c) {
                $div.attr('onclick', 'javascript:window.location.href=\'' + $(con).attr('href') + '\'')
                return false;
            }
        })
        $customMenu.before($div);
    });
}

//add custom menu for select
function getCustomMenu(menu) {
    $('.my-custom-menu').html('');
    var menuIconList = $('div.more-custom-menu table td:has(i)>span');

    var flag = 0;
    $.each(menu, function (i, c) {
        $.each(menuIconList, function (ind, con) {
            if ($(con).html() == menu[i]) {
                //add to my custom-menu
                var $item = $('<span></span>').append($(con).parent('td').html() + '&nbsp;&nbsp;&nbsp;&nbsp;');
                $('.my-custom-menu').append($item);
                //add tag for disabled selected
                $(con).parent('td').addClass('disabled');
                return false;
            }
            if (ind == menuIconList.length - 1) {
                $(con).parent('td').removeClass('disabled');
            }
        });
    });
}

//adjust the custom menu position
function menuPositionAjust() {
    var menuHeight = $('#operationNavigator').height();
    var windowHeight = $(window).height();
    var n = Math.floor(menuHeight / windowHeight);
    if (n > 0) {
        $('#operationNavigator').css('top', 0);
        var perNumber = Math.floor(windowHeight / 50);
        var totalNumber = $('#sub-operation>div').length;
        var pages = Math.ceil(totalNumber / perNumber);
        for (var i = 2; i <= pages; i++) {
            var $div = $('<div></div>');
            $div.css({
                'position': 'fixed',
                width: '50px',
                right: (i - 1) * 50 + 10 + 'px',
                top: 0
            });
            var sss$ = $('#sub-operation>div');
            if (i == pages) {
                var $extendMenu = $('#sub-operation>div:gt(' + (perNumber - 1) + ')');
                $div.append($extendMenu);
                $('#operationNavigator').parent().append($div);
            }
            else {
                var $extendMenu = $('#sub-operation>div:gt(' + (perNumber - 1) + ')');
                $extendMenu.length = perNumber;
                $div.append($extendMenu);
                $('#operationNavigator').parent().append($div);
            }
        }
    }
    if (n == 0) {
        $('#operationNavigator').css('top', (windowHeight - menuHeight) / 2);
    }

}

