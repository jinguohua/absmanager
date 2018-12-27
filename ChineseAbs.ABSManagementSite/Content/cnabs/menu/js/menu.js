/*global $ */
$(document).ready(function () {
    //未登录时隐藏导航栏
    if (isLogin != "True") {
        $("#navigator").hide();
    }

    if (isLogin == "True") {
        //获取未读消息
        $.ajax({
            type: 'GET',
            url: '/menu/GetUnReadMsgCount',
            dataType: 'json',
            success: function (data) {
                if (data.Count > 0) {
                    $("#allMsgCount").text(data.Count);
                    $("#allMsgCount").show();
                }
                for (var index in data.MsgList) {
                    var item = data.MsgList[index];
                    if (item.Count > 0) {
                        $("#" + item.MenuKey + " .message-informer").text(item.Count);
                        $("#" + item.MenuKey + " .message-informer").show();
                    }
                }
            },
            error: function () { },
        })
    }

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

    /*$(".menu > ul > li").hover(function (e) {
        if ($(window).width() > 943) {
            var isVisible = $(this).children("ul").is(":visible");
            if (!isVisible && e.type == "mouseenter"
                || isVisible && e.type == "mouseleave") {
                $(this).children("ul").stop(true, false).toggle();
                e.preventDefault();
            }
        }
    });*/
    $(document).click(function () {
        $(".menu > ul > li").css("background-color", "#3B3831");
        $(".menu > ul > li").siblings().children("ul").fadeOut();
    });

    //菜单点击下拉事件
    $(".menu > ul > li").click(function (e) {
        if ($(this).attr("id") == "liLogin") {
            return;
        }
        e.stopPropagation();

        var windowWidth = $(window).width();
        var rightWidth = windowWidth - $(this).offset().left - (windowWidth - 1050) / 2;
        var ulWidth = $(this).children("ul").width();
        if (ulWidth > rightWidth) {
            $(this).children("ul").css("right","0");
        }

        $(this).css("background-color", "#443F39");
        $(this).siblings().css("background-color", "#3B3831");
        $(this).children("ul").fadeToggle();
        $(this).siblings().children("ul").fadeOut();
        $(this).children("ul").click(function (e) {
            e.stopPropagation();
        });
      
        return false;
    });

    //上锁菜单点击事件
    $("#cnabsMenu .lock").click(function () {
        if (isLogin == "True") {
            var menuName = $(this).text();
            var msg = "我们将逐步开放这些功能给机构用户";
            switch (typeId) {
                case "1":
                    msg = "此功能为数据版或专业版功能";
                    break;
                case "2":
                    msg = "此功能为专业版功能";
                    break;
                case "3":
                    var iconName = $(this).parent().parent().siblings().html()
                    if (iconName == "发行协作平台" || iconName == "存续期管理平台") {
                        msg = "您尚未获得访问该功能的权限";
                    }
                    break;
            }
            alertify.alert("您好，" + msg + "，如您对此功能有兴趣请联系客服，电话：<a href='tel:021-31156258'>021-31156258</a>，邮箱：" +
                "<a href='mailto:feedback@cn-abs.com'>feedback@cn-abs.com</a>。我们的销售经理会稍后联系您，感谢您的理解与关注！");

            $.ajax({
                type: 'POST',
                url: '/menu/RecordPotentialUser',
                dataType: 'json',
                data: { menuName: menuName },
                success: function (data) { },
                error: function () { },
            })
        } else {
            alertify.alert('请先登录');
        }
        return false;
    });

    $("#deallist").click(function () {
        $(".menu > ul > li").children("ul").fadeOut();
    })
    //If width is more than 943px dropdowns are displayed on hover

    //    $(".menu > ul > li").click(function (e) {
    //        if ($(window).width() <= 943) {
    //            $(this).children("ul").fadeToggle(150);
    //}
    //});
    //If width is less or equal to 943px dropdowns are displayed on click (thanks Aman Jain from stackoverflow)

    $(".menu-mobile").click(function (e) {
        $(".menu > ul").toggleClass('show-on-mobile');
        e.preventDefault();
    });
    //when clicked on mobile-menu, normal menu is shown as a list, classic rwd menu story (thanks mwl from stackoverflow)

 
});

function BindingDealHref($id, href, $dealId) {
    $("#" + $id).attr("href", href + $dealId);
}

//滚动时当前位置固定到顶部 
window.onscroll = function () {
    var scrollTop = document.body.scrollTop + document.documentElement.scrollTop;
    if (scrollTop >= 65 ) { 
        $("#navigator").addClass("cnabsNavigatorFixed");
        //$("#navigator").css("background-color", "transparent"); 
    }


    if (scrollTop+36< 65) { 
        $("#navigator").removeClass("cnabsNavigatorFixed");
        //$("#navigator").css("background-color", "#36322D");  
    } 

    //if (36 < scrollTop && scrollTop <= 65) {
    //    $("#navigator").css("background-color", "#36322D");
    //} 
}


