//jQuery对象级的插件
;(function($){
    $.fn.tree = function(options){
        var defaults = {
            //各种参数，各种属性
            targettreeid:'',
            treeurl:'',
           
        }
     
        //利用extend方法把 defaults对象的方法属性全部整合到 options里，
        //也就是options继承了defaults对象的方法以及属性
        var options = $.extend(defaults,options);
        $('#'+options.targettreeid).jstree({
            'plugins': options.plugins,
            "checkbox": {
                "three_state": false,//父子级别级联选择
            },
            'core' : {
                'data' : {
                    "url" :options.treeurl,
                    "dataType" : "json",
                    "data" : function (node) {
                    
                    }
                }
            }
        });
        
        return this;
    }
})(jQuery);
