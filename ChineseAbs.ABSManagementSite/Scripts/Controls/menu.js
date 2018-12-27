
(function ($) {
    $.widget("ui.menuStrip", {
        options: {
            main: null,
            sub: null
        },

        _create: function () {
            var self = this;
            var header = this.element;
            var mainMenu = this.options.main;
            var sub = this.options.sub;

            $('.nav-strip a', mainMenu).mouseenter(function (evt) {
                var menuItem = $(evt.target).parents('.nav-item');
                self.ShowSubMenu(menuItem);
            });

            $(header).mouseleave(function () {
                var activeSub = $('.nav-strip .active', sub).parents('.nav-strip');
                if (activeSub.length > 0 &&
                    $(activeSub).css('display') != 'none')
                    return;

                $('.nav-strip', sub).hide();
                self.ShowSubMenu($('.nav-strip .active', mainMenu));
            });
        },
        
        ShowSubMenu: function (menuItem, noEffect) {
            var subs = this.options.sub;
            var subId = $(menuItem).attr('sub');
            if (subId == null)
                return;

            if ($('#' + subId + ' .active', subs).length > 0)
                if ($('#' + subId, subs).css('display') != 'none')
                    return;

            $('.nav-strip', subs).hide();
            var sub = $('#' + subId, subs);
            var left = $(menuItem).position().left + 10;
            var top = $(menuItem).position().top + $(menuItem).height() - 6;
            $(sub).css('left', left).css('top', top);

            if (noEffect != null)
                $(sub).show();
            else
                $(sub).show('blind', null, 250);   
        },

        SelectMenuItem: function (item1, item2) {
            var mainMenu = this.options.main;
            var sub = this.options.sub;
            var self = this;
            $(mainMenu).show();
            if (item1 != null) {
                $(".nav-item", mainMenu).removeClass('active');
                $(".nav-item", sub).removeClass('active');
                $("#" + item1).addClass('active');
            }
            
            if (item2 != null) {
                self.ShowSubMenu($("#" + item1), true);
                $("#" + item2).addClass('active');
            }
        },

        SelectMainMenu: function (item) {
            if (item != null) {
                $("#" + item).addClass('active');
                $(".nav-item", sub).removeClass('active');
            }
        },

        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);