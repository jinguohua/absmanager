
function ComTab(labels, tabDivPrefix) {
    if (labels != null && labels.length > 0) {
        var hasSelected = false;
        var selectedTabId = "";

        //tabId重校验
        for (var i = 0; i < labels.length - 1; i++) {
            for (var j = i + 1; j < labels.length; j++) {
                var hasRepeat = false;
                if (labels[i].tabId == labels[j].tabId) {
                    alert("tabId 不能有重复！");
                    hasRepeat = true;
                    break;
                }
                if (hasRepeat) break;
            }
        }

        //默认第一个出现标签this.selected 为选择项，如果设置selected属性，选择第一个
        $.each(labels, function () {
            this.style = "unselectedLabel";
            if (this.selected != undefined && this.selected) {
                hasSelected = true;
                selectedTabId = this.tabId;
                return false;
            }
        });

        if (!hasSelected) {
            labels[0].selected = true;
            selectedTabId = labels[0].tabId;
        }
    }


    if (tabDivPrefix == null) {
        tabDivPrefix = "divTabdEx";
    }
    this.labels = labels;
    this.selectedTabId = selectedTabId;
    this.acceptGetParams = {};
    if (typeof ComTab._initialized == "undefined") {
        ComTab.prototype.init = function () {
            setParentModule(this.labels, this.selectedTabId, tabDivPrefix);
        }
        ComTab.prototype.GetOutParams = function (key) {
            this.acceptGetParams[key] = cnabsGetUrlParam(key);
        }

        ComTab._initialized = true;
    }
}

function cnabsGetUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) {
        return unescape(r[2]);
    }
    return null;
}

function setParentModule(labels, selectedTabId, tabDivPrefix) {
    $.each(labels, function (index, item) {
        item.top = index * 84 + "px";
        item.controlLabelStyle = { top: item.top };
    });
    var tabMod = angular.module("comTab", [])
    tabMod.directive('cnabstabs', function ($rootScope, $location, locationURL) {
        return {
            restrict: 'E',
            template: '<div ng-cloak ng-repeat="label in labels" id="divLabel{{label.tabId}}" class="fontTranform {{label.style}}" ng-click="labelChange(label.tabId)" ng-style="label.controlLabelStyle"><span>{{label.title}}</span></div>',
            replace: true,
            transclude: true,
            link: function ($scope, iElm, iAttrs) {
                $scope.currentProjectGuid = "";
                $scope.labels = labels;

                $scope.labelChange = function (tabId, data) {
                    if (data == null) data = $location.search();
                    //页面首次加载，接收某个外部传入参数
                    for (var outParam in this.acceptGetParams) {
                        if (typeof data[outParam] == "undefined") {
                            data[outParam] = this.acceptGetParams[outParam];
                        }
                    }

                    $location.path(tabId);
                    $.each($scope.labels, function () {
                        var div = $("#" + tabDivPrefix + this.tabId);
                        this.translateX = -((this.index - 1) * 53 + 24) + "px";
                        if (this.tabId == tabId) {
                            var ss = [];
                            for (var i = 0; i < this.params.length; i++) {

                                var p = data[this.params[i]];
                                p = p == null ? "null" : '"' + p + '"';
                                ss.push(p)
                            }
                            var str = ss.join(',');

                            eval('$rootScope.$emit("UpdateTabEx' + tabId + '",' + str + ')');
                            this.style = 'selectedLabel';
                            div.show();
                        } else {
                            this.style = 'unselectedLabel';
                            div.hide();
                        }
                    })
                };
                $rootScope.$on("ChangeLabel", function (event, data) {
                    $scope.labelChange(data.tabId, data);
                });
            }
        }
    });

    function cnabsGetUrlParam(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) {
            return unescape(r[2]);
        }
        return null;
    }

    tabMod.service('locationURL', function ($location) {
        var hash = null;
        this.getInitURL = function () {
            if (hash == null) {
                hash = JSON.parse(JSON.stringify($location.search()));
                var tabIdstr;
                var path = $location.path();
                path = path.replace(/\//g, "");
                var tabIdstr = path == "" ? selectedTabId : path;
                hash.tabId = tabIdstr;
            }
            return hash;
        };

        this.setURLHash = function (hashKey, hashValue) {
            $location.search(hashKey, hashValue);
        }

    });
}
