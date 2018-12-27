/**
 * embed panel 1.0.0 https://www.cn-abs.com
 */

var embedPanelParam;

function nullFunction() {

}

function embedPanel(options, runNow, keepShow) {
    var panel = options.panel;
    var parent = options.parent;
    var direction = options.direction ? options.direction : 'bottom';
    var toggleButton = options.toggleButton;
    var onVisibleCallback = options.onVisibleCallback ? options.onVisibleCallback : nullFunction;
    var backgroundColor = options.backgroundColor ? options.backgroundColor : '#333';
    var dimPage = options.dimPage ? option.dimPage : false;
    var closable = options.closable ? options.closable : false;
    var returnScroll = options.returnScroll ? options.returnScroll : true;
    var hideButton = options.hideButton;
    var onHideButtonClick = options.onHideButtonClick;

    var settings = {
        context: $(parent),
        dimPage: dimPage,
        closable: closable,
        returnScroll: true
    };

    var $sidebar = $(panel);
    $sidebar.addClass('ui sidebar uncover');
    $sidebar.addClass(direction);
    if (direction == 'left' || direction == 'right') {
        $sidebar.addClass('vertical');
    }
    else
    {
        $sidebar.removeClass('vertical');
    }

    $sidebar.css('background-color', backgroundColor);
    //$sidebar.css('z-index', '99');
    $(parent).css('overflow-y', 'hidden');

    var callback = function () {
        var settings = {
            context: $(parent),
            dimPage: false,
            closable: false,
            returnScroll: true
        };
        embedPanelParam = $(toggleButton).data('content');

        var setVisiable = function (isShow) {
            if (isShow) {
                $(panel).data("lastShow", toggleButton);
                $.extend(settings, { onVisible: onVisibleCallback });

                $sidebar.sidebar(settings).sidebar('show');
            } else {
                if ($(panel).data("lastShow") != toggleButton) {
                    $(panel).data("lastShow", toggleButton);
                    $.extend(settings, { onHidden: onhide });
                }

                $sidebar.sidebar(settings).sidebar('hide');
            }
        };

        if (keepShow != undefined) {
            setVisiable(keepShow);
            return;
        }

        var isvisible = $sidebar.sidebar('is visible');
        setVisiable(!isvisible);
    };

    //$(toggleButton).click(callback);

    if (runNow) {
        callback();
    }

    $sidebar.sidebar(settings);

    function onhide() {
        $.extend(settings, { onVisible: onVisibleCallback });
        $sidebar.sidebar(settings).sidebar('show');
    }

    if (hideButton != null) {
        $(hideButton).click(function () {
            $sidebar.sidebar(settings).sidebar('hide');
            if (onHideButtonClick != undefined && onHideButtonClick != null) {
                onHideButtonClick();
            }
        });
    }
}

