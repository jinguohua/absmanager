function OrganizationAlert() {
    alertify.alert("该功能已开通给数据版与专业版机构用户，您如对此功能有兴趣请联系客服。联系电话：<a href='tel:021-31156258'>021-31156258</a>，邮箱："
        + "<a href='mailto:feedback@cn-abs.com' style='color:#ffc446;'>feedback@cn-abs.com</a>。");
}
function openDialog(chart, dlgWidth, dlgHeight, title) {
    $(chart).dialog({
        closeText: "",
        title: title,
        height: dlgHeight,
        width: dlgWidth,
        autoOpen: true,
        closeOnEscape: true,
        show: true,
        hide: true,
        modal: true,
        position: { my: "center", at: "center" }
    });
}
function formatNumber(num) {
    var parts = num.toString().split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return parts.join(".");
};
function setInputText(chart,text) {
    $("#" + chart).val(text);
}

function DateToString(date) {
    return date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
}

function checkCBL(cbl) {
    var checkedValue = "";
    $(cbl).find(':checked').each(function () {
        checkedValue += "," + $.trim($(this).next().text());
    });
    return checkedValue.substring(1);
}
function checkDDL(ddl) {
    return $(ddl).find("option")[$(ddl).val()].text;
}
function drawingColumnCheck() {
    var columnCheck = true;
    $(".drawingColumn select").each(function () {
        if ($(this).val() === "0") {
            columnCheck = false;
            $(this).addClass("nopeCheck");
        }
        else {
            $(this).removeClass("nopeCheck");
        }
    });
    return columnCheck;
}
function checkAxisColumn() {
    var columnCheck = true;
    $(".drawingColumn select").each(function () {
        if ($(this).val() === "0") {
            columnCheck = false;
            $(this).addClass("nopeCheck");
        } else {
            $(this).removeClass("nopeCheck");
        }
    });
    return columnCheck;
}

function getColumnsList() {
    var requestColumns = "";
    $(".column-note-check img").each(function () {
        requestColumns += this.alt + ",";
    });
    return requestColumns.replace(/(,*$)/, "");
}
function getTextByValue(value) {
    return $("img[alt='" + value + "']").next()[0].innerText;
}
function getValueByText(text) {
    var value = "nopeFind";
    $(".column-note-check span,.column-note-check span,.column-note-orga span").each(function () {
        if (this.innerText === text) {
            value = $(this).prev("img")[0].alt;
        }
    });
    return value;
}
function getColHeadersList() {
    var headersList = [];
    $(".column-note-check span").each(function () {
        headersList.push(this.innerText);
    });
    return headersList;
}
function getColHeaderIndex(handson, header) {
    var headerIndex = -1;
    $.each(handson.getColHeader(), function (index, content) {
        if (content === header) headerIndex = index;
    });
    return headerIndex;
}
function getYearfilterByValue(handson, header, year) {
    var index = getColHeaderIndex(handson, header);
    var dateList = handson.getSourceDataAtCol(index);
    var filterDate = [];
    $.each(year.split(","), function (i, content) {
        $.each(dateList, function (j, date) {
            if (null !== date && undefined !== date) {
                if (date.substring(0, 4) === content) {
                    filterDate.push(date);
                }
            }
        });
    });
    $.unique(filterDate);
    filterDate.sort();
    return filterDate;
}
function getDrawingType(chart) {
    if ($("#" + chart).val() === "1") {
        return "scatter";
    } else if ($("#" + chart).val() === "2") {
        return "pie";
    } else {
        return null;
    }
}

var filterRecord;
function getFilterRecord() {
    return this.filterRecord;
}
function recordFilters(handson) {
    handson.addHook('beforeFilter', function (formulasStack) {
       // return;

        var filterList = [];
        var filterListCn = [];
        $.each(formulasStack, function(i, formulasContent) {
            var header = getColHeadersList()[formulasContent.column];
            var column = getValueByText(header);
            if ("FrequencyCn" === column) {
                column = "Frequency";
            }
            if ("IsFixedCouponCn" === column) {
                column = "IsFixedCoupon";
            }
            $.each(formulasContent.formulas, function(j, content) {
                if (content.name === "empty") {
                    filterList.push({ Column: column, Formula: " IS NULL" });
                    filterListCn.push(header + "为空");
                } else if (content.name === "not_empty") {
                    filterList.push({ Column: column, Formula: " IS NOT NULL" });
                    filterListCn.push(header + "不为空");
                } else if (content.name === "eq") {
                    filterList.push({ Column: column, Formula: " = '" + content.args.toString() + "'" });
                    filterListCn.push(header + "等于" + content.args.toString());
                } else if (content.name === "neq") {
                    filterList.push({ Column: column, Formula: " <> '" + content.args.toString() + "'" });
                    filterListCn.push(header + "不等于" + content.args.toString());
                } else if (content.name === "begins_with") {
                    filterList.push({ Column: column, Formula: " LIKE '" + content.args.toString() + "%'" });
                    filterListCn.push(header + "以" + content.args.toString() + "开头");
                } else if (content.name === "ends_with") {
                    filterList.push({ Column: column, Formula: " LIKE '%" + content.args.toString() + "'" });
                    filterListCn.push(header + "以" + content.args.toString() + "结尾");
                } else if (content.name === "contains") {
                    filterList.push({ Column: column, Formula: " LIKE '%" + content.args.toString() + "%'" });
                    filterListCn.push(header + "包含" + content.args.toString());
                } else if (content.name === "not_contains") {
                    filterList.push({ Column: column, Formula: " NOT LIKE '%" + content.args.toString() + "%'" });
                    filterListCn.push(header + "不包含" + content.args.toString());
                } else if (content.name === "gt") {
                    filterList.push({ Column: column, Formula: " > '" + content.args.toString() + "'" });
                    filterListCn.push(header + "大于" + content.args.toString());
                } else if (content.name === "gte") {
                    filterList.push({ Column: column, Formula: " >= '" + content.args.toString() + "'" });
                    filterListCn.push(header + "大于等于" + content.args.toString());
                } else if (content.name === "lt") {
                    filterList.push({ Column: column, Formula: " < '" + content.args.toString() + "'" });
                    filterListCn.push(header + "小于" + content.args.toString());
                } else if (content.name === "lte") {
                    filterList.push({ Column: column, Formula: " <= '" + content.args.toString() + "'" });
                    filterListCn.push(header + "小于等于" + content.args.toString());
                } else if (content.name === "between") {
                    filterList.push({ Column: column, Formula: " >= '" + content.args[0].toString() + "'" });
                    filterList.push({ Column: column, Formula: " <= '" + content.args[1].toString() + "'" });
                    filterListCn.push(header + "从" + content.args[0].toString() + "到" + content.args[1].toString());
                } else if (content.name === "not_between") {
                    filterList.push({ Column: column, Formula: " < '" + content.args[0].toString() + "'" });
                    filterList.push({ Column: column, Formula: " > '" + content.args[1].toString() + "'" });
                    filterListCn.push(header + "不在" + content.args[0].toString() + "到" + content.args[1].toString());
                } else if (content.name === "date_yesterday") {
                    var dateYesterday = new Date().getFullYear() + "-" + (new Date().getMonth() + 1) + "-" + (new Date().getDate() - 1);
                    filterList.push({ Column: column, Formula: " = '" + dateYesterday + "'" });
                    filterListCn.push(header + dateYesterday);
                } else if (content.name === "date_today") {
                    var dateToday = new Date().getFullYear() + "-" + (new Date().getMonth() + 1) + "-" + new Date().getDate();
                    filterList.push({ Column: column, Formula: " = '" + dateToday + "'" });
                    filterListCn.push(header + dateToday);
                } else if (content.name === "date_tomorrow") {
                    var dateTomorrow = new Date().getFullYear() + "-" + (new Date().getMonth() + 1) + "-" + (new Date().getDate() + 1);
                    filterList.push({ Column: column, Formula: " = '" + dateTomorrow + "'" });
                    filterListCn.push(header + dateTomorrow);
                } else if (content.name === "date_before") {
                    filterList.push({ Column: column, Formula: " < '" + content.args.toString() + "' " });
                    filterListCn.push(header + "在" + content.args.toString() + "之前");
                } else if (content.name === "date_after") {
                    filterList.push({ Column: column, Formula: " > '" + content.args.toString() + "' " });
                    filterListCn.push(header + "在" + content.args.toString() + "之后");
                } else {
                    var byValue = content.args.toString();
                    if (column.toLowerCase().indexOf("date") !== -1) {
                        filterList.push({ Column: column, Formula: " in( '" + byValue.replace(/[,]/g, "','") + "') " });
                        var cblYear = checkCBL("[id$='cblYear']").toString();

                        if (cblYear === "") {
                            var dateTemp = [];
                            $.each(byValue.split(','), function(n, value) {
                                dateTemp.push(value.substr(0,4));
                            });
                            $.unique(dateTemp);
                            dateTemp.sort();
                            cblYear = dateTemp.toString();
                        }

                        if (1) {
                            var filterDate = getYearfilterByValue(handson, header, cblYear).toString();
                            if (byValue === filterDate) {
                                $.each(cblYear.split(','), function(n, value) { filterListCn.push(value); });
                            } else {
                                $.each(byValue.split(','), function(n, value) { filterListCn.push(value); });
                                if (column === "ClosingDate") {
                                    $("[id$='cblYear']").find(":checked").each(function() { $(this).click(); });
                                }
                            }
                        }
                    } else if (column === "IsFixedCoupon") {
                        filterListCn.push("固息等于" + byValue);
                        if ("是" === byValue) filterList.push({ Column: column, Formula: " =1 " });
                        else filterList.push({ Column: column, Formula: " =0 " });
                    } else {
                        filterList.push({ Column: column, Formula: " in( '" + byValue.replace(/[,]/g, "','") + "') " });
                        $.each(byValue.split(','), function(n, value) {
                            if (isNaN(value)) {
                                value = value.replace(/<[^>]*href[^>]*>|<\/a>/g, "");
                            }
                            filterListCn.push(value);
                        });
                        if (column === "ProductType") {
                            resetCBLCheched("cblProductType", byValue);
                        } else if (column === "DealType") {
                            resetCBLCheched("cblDealType", byValue);
                        } else if (column === "Regulator") {
                            resetCBLCheched("cblRegulator", byValue);
                        } else if (column === "OrigRating1") {
                            resetCBLCheched("cblRating", byValue);
                        } else if (column === "SecurityType") {
                            resetCBLCheched("cblSecurityType", byValue);
                        }
                    }
                }

                if (column === "Originator" && checkDDL("[id$='ddlOriginator']") !== content.args.toString()) {
                    $("[id$='ddlOriginator']").val(0);
                } else if (column === "Issuer" && checkDDL("[id$='ddlIssuer']") !== content.args.toString()) {
                    $("[id$='ddlIssuer']").val(0);
                }
            });
        });
        showFilterRecord(filterListCn);
        filterRecord = filterList;
    });
}
function showFilterRecord(filterList) {
    $(".filter-record").empty();
    $(".filter-record").html(function () {
        var filters = "";
        $.each(filterList, function (index, content) {
            filters += "<span>" + content + "</span>";
        });
        if (filters === "") {
            filters = "<span>无筛选</span>";
        }
        return "<span class='filter-title fixedColor'>筛选条件：</span>" + filters;
    });
}
function clearHandson(handson) {
    if (undefined !== handson && null !== handson) {
        handson.updateSettings({
            dropdownMenu: false,
            data: null
        });
    }
}
function clearFilterHandson(handson) {
    $("[id$='ddlOriginator']").val(0);
    $("[id$='ddlIssuer']").val(0);
    $(":checked").each(function () { $(this).click(); });
    handson.getPlugin('filters').clearFormulas();
    handson.getPlugin('filters').filter();
    
    $("[part=filterInput]").each(function(i,e) {
        var $scope = angular.element(e).scope();
        $scope.text = "全部";
        $scope.previousValue = "全部";
        $scope.$apply();
    });
}
function setScatterAxisColumn(series, xAxis, yAxis, toolTip, columnClass) {
    $("#" + series).empty();
    $("#" + xAxis).empty();
    $("#" + yAxis).empty();
    $("#" + toolTip).empty();
    document.getElementById(xAxis).options.add(new Option("-请选择 -", 0));
    document.getElementById(yAxis).options.add(new Option("-请选择 -", 0));
    $(columnClass[0]).each(function () {
        document.getElementById(series).options.add(new Option(getTextByValue(this.alt), this.alt));
        if ("DealNameLink" !== this.alt && "DealNameChinese" !== this.alt) {
            document.getElementById(toolTip).options.add(new Option(getTextByValue(this.alt), this.alt));
        }
    });
    $(columnClass[1]).each(function () {
        document.getElementById(xAxis).options.add(new Option(getTextByValue(this.alt), this.alt));
        document.getElementById(toolTip).options.add(new Option(getTextByValue(this.alt), this.alt));
    });
    $(columnClass[2]).each(function () {
        document.getElementById(xAxis).options.add(new Option(getTextByValue(this.alt), this.alt));
        document.getElementById(yAxis).options.add(new Option(getTextByValue(this.alt), this.alt));
        document.getElementById(toolTip).options.add(new Option(getTextByValue(this.alt), this.alt));
    });
    $("#" + series).val("DealType");
}
function resetCBLCheched(chart, data) {
    if (data !== checkCBL("[id$='" + chart + "']").toString()) {
        $("[id$='" + chart + "']").find(":checked").each(function () { $(this).click(); });
        $.each(data.split(','), function (index, content) {
            $("[id$='" + chart + "'] label").each(function () {
                if ($(this)[0].innerText === content) {
                    $(this).click();
                }
            }
            );
        });
    }
}
function reSetColumnFilter(handson, header, value) {
    var index = getColHeaderIndex(handson, header);
    if (index > -1) {
        handson.getPlugin('filters').removeFormulas(index);
        if ("" !== value && "全部" !== value && value.length > 0) {
            handson.getPlugin('filters').addFormula(index, 'by_value', [value.split(",")]);
        }
    }
    if (index < 0 && "" !== value && "全部" !== value) {
        alertify.error("未选中列：" + header + "；筛选已失效，请重新选择", 5);
    }
}
function reSetYearFilter(handson, header, year) {
    var index = getColHeaderIndex(handson, header);
    var filterDate = getYearfilterByValue(handson, header, year);
    if (filterDate.length > 0) {
        handson.getPlugin('filters').addFormula(index, 'by_value', [filterDate]);
    }
    if (index < 0 && "" !== year) {
        alertify.error("未选中列：" + header + "；筛选已失效，请重新选择", 5);
    }
}
function resetHandsonColumnSetting(data) {
    var index = getColHeadersList().indexOf("当前状态");
    if (index > 0) {
        data.columns[index] = {
            editor: false, data: 'CurrentStatus', className: 'htCenter', type: 'text', renderer: function (instance, TD, row, col, prop, value, cellProperties) {
                if (value === '发行期') {
                    TD.style = 'text-align:center;color:#f45b5b';
                } else if (value === '已清算') {
                    TD.style = 'text-align:center;color:#B7AFA5';
                } else {
                    TD.style = 'text-align:center';
                }
                TD.innerHTML = value;
            }
        };
    }
}
function resetDrawingTypeParameter(chart) {
    if ("pie" === getDrawingType(chart)) {
        $(".drawingScatter").hide();
        $(".drawingPie").show();
    } else {
        $(".drawingScatter").show();
        $(".drawingPie").hide();
    }
}

function declareHandson(chart, data) {
    resetHandsonColumnSetting(data);
    var container = document.querySelector("#" + chart);
    var handsontable = new Handsontable(container, {
        width: 1010,
        height: 533,
        rowHeaders: true,
        dropdownMenu: ["filter_by_condition", "filter_by_value", "filter_action_bar"],
        colHeaders: getColHeadersList(),
        data: data.dataResult,
        columns: data.columns,
        filters: true,
        columnSorting: true,
        sortIndicator: true,
        autoColumnSize: true,
        manualColumnResize: true,
        readOnly: true,
        wordWrap: false,
        copyable: false,
        manualColumnMove: true,
        renderAllRows: false,
        fixedColumnsLeft: 1,
        fixedRowsBottom: 1,
        viewportColumnRenderingOffset: 100,
        maxRows: container.length,
        beforeKeyDown: function (e) {
            if (e.keyCode === 8 || e.keyCode === 46) {
                Handsontable.Dom.stopImmediatePropagation(e);
            }
        }
    });
    return handsontable;
}
function columnCustom(chart) {
    $(".column-note-check img").each(function () { this.src = "../Images/Icons/pass.png"; });
    $(".column-note-uncheck img,.column-note-orga img").each(function () { this.src = "../Images/Icons/unpass.png"; });
    $(".column-note-orga").click(function () { OrganizationAlert(); });
    $(".column-note-check:not(.column-note-fixed),.column-note-uncheck").click(function () {
        if ($(this).hasClass("column-note-check")) {
            $(this).removeClass("column-note-check");
            $(this).addClass("column-note-uncheck");
            $(this).find("img")[0].src = "../Images/Icons/unpass.png";
        } else {
            $(this).removeClass("column-note-uncheck");
            $(this).addClass("column-note-check");
            $(this).find("img")[0].src = "../Images/Icons/pass.png";
        }
    });
    $(".column-button-allOrNot").click(function () {
        if (this.innerText === "清空筛选") {
            this.innerText = "全选";
            $(".column-note-check:not(.column-note-fixed)").each(function () {
                $(this).removeClass("column-note-check");
                $(this).addClass("column-note-uncheck");
                $(this).find("img")[0].src = "../Images/Icons/unpass.png";
            });
        } else {
            this.innerText = "清空筛选";
            $(".column-note-uncheck").each(function () {
                $(this).removeClass("column-note-uncheck");
                $(this).addClass("column-note-check");
                $(this).find("img")[0].src = "../Images/Icons/pass.png";
            });
        }
    });
    $(".column-button-ok").click(function () {
        $("#" + chart).empty();
        $("#" + chart).html("<img src='../Images/ajaxloader.gif' style='margin: 100px 500px;' />");
        $(".column-custom").dialog("close");
        loadData();
    });
    $(".column-button-cancel").click(function () { $(".column-custom").dialog("close"); });
}

var scatterChart;
function drawingScatterChart(data, contentName, xTitle, yTitle) {
    if (data.toString() === "") {
        $("#" + contentName).html("<br />暂无相关数据");
        return;
    }
    var xDate, xMax = 0, yMax = 0;
    var seriesList = [];
    if (data.length > 0) {
        if (data[0].Data.Data.length > 0) {
            if (data[0].Data.Data[0].XIsDate) { xDate = true; }
        }
    }
    var totalTitle = $("#ddlToolTip option:selected").text();

    $.each(data, function (index, content) {
        var dataList = [];
        $.each(content.Data.Data, function (n, point) {
            var totaldata;
            if (totalTitle.indexOf('日') !== -1) {
                totaldata = DateToString(new Date(point.DateTicks));
            } else if (totalTitle.indexOf('%') !== -1) {
                totaldata = (point.Name * 100).toFixed(2).replace(".00", "") + '%';
            } else {
                if (isNaN(point.Name)) {
                    totaldata = point.Name;
                } else {
                    totaldata = Highcharts.numberFormat(parseFloat(point.Name).toFixed(2), 0, ".", ",");
                }
            }
            if (xDate) {
                dataList.push({ x: point.DateTicks, y: point.Y, total: totaldata, name: point.Deal });
            } else {
                dataList.push({ x: point.X, y: point.Y, total: totaldata, name: point.Deal });
                if (point.X > xMax) { xMax = point.X; }
            }
            if (point.Y > yMax) { yMax = point.Y; }
        });
        seriesList.push({ name: content.Name, data: dataList, type: 'scatter', marker: { radius: 3 } });
    });
    var axisDate = {
        type: "datetime",
        dateTimeLabelFormats: {
            year: '%Y',
            month: '%Y.%m',
            week: "%Y.%m",
            day: '%Y.%m.%d'
        },
        labels: {
             style:
                {
                    "fontSize": "15px"
                }
        }
    }
    var axisDouble = {
        type: 'linear',
        labels: {
            formatter: function () {
                    if (xTitle.indexOf('%') !== -1) {
                        return (this.value * 100).toFixed(2).replace(".00", "") + '%';
                    } else {
                        return Highcharts.numberFormat(this.value, 0, ".", ",") + "";
                    } 
            },
            style:
            {
                "fontSize": "15px"
            }
        },
        floor:0
    }
    var options = {
        chart: {
            renderTo: contentName,
            zoomType: 'xy',
            events: {
                selection: function (event) {
                    if (!event.resetSelection) {
                        scatterResetZoom(event, xTitle, yTitle);
                    } else {
                        var minX, maxX, minY, maxY;

                        if (xTitle.indexOf('日') > 0) {
                            minX = DateToString(new Date(event.target.axes[0].oldMin));
                            maxX = DateToString(new Date(event.target.axes[0].oldMax));
                        } else if (xTitle.indexOf('%') > 0) {
                            minX = (event.target.axes[0].oldMin * 100).toFixed(2) + "%";
                            maxX = (event.target.axes[0].oldMax * 100).toFixed(2) + "%";
                        } else {
                            minX = event.target.axes[0].oldMin;
                            maxX = event.target.axes[0].oldMax;
                        }

                        if (yTitle.indexOf('日') > 0) {
                            minY = DateToString(new Date(event.target.axes[1].oldMin));
                            maxY = DateToString(new Date(event.target.axes[1].oldMax));
                        } else if (yTitle.indexOf('%') > 0) {
                            minY = (event.target.axes[1].oldMin * 100).toFixed(2) + "%";
                            maxY = (event.target.axes[1].oldMax * 100).toFixed(2) + "%";
                        } else {
                            minY = event.target.axes[1].oldMin;
                            maxY = event.target.axes[1].oldMax;
                        }

                        $("#deawingXMin").val(minX);
                        $("#deawingXMax").val(maxX);
                        $("#deawingYMin").val(minY);
                        $("#deawingYMax").val(maxY);
                    }
                    return ;
                }
            }
        },
        title: {
            text: ""
        },
        xAxis: xDate ? axisDate : axisDouble,
        yAxis: {
            type: 'linear',
            labels: {
                formatter: function () {
                    if (yTitle.indexOf('%') !== -1) {
                        return (this.value * 100).toFixed(2).replace(".00", "") + '%';
                    } else {
                        return Highcharts.numberFormat(this.value, 0, ".", ",") + "";
                    }
                },
                style:
                {
                    "fontSize": "15px"
                }
            }
        },
        tooltip: {
            formatter: function () {
                return this.key + "<br />" + totalTitle +"："+ this.total;
            }
        },
        exporting: {
            enabled: false
        },
        plotOptions: {
            series: {
                turboThreshold: 10000
            }
        },
        legend: {
            itemStyle: {
                fontSize: "15px"
            }
        },
        series: seriesList
};
    Highcharts.setOptions({
        lang: {
            resetZoom: "重置缩放比例"
        }
    });

    options.yAxis.type = $("#ddlAxiesType").val();

    var chart = new Highcharts.Chart(
        options,
        function (scatter) {
            scatterChart = scatter;

            scatterResetZoom(scatter, xTitle, yTitle);

        });
}

function scatterResetZoom(chart, xTitle, yTitle) {
    var minX, maxX, minY, maxY;

    if (xTitle.indexOf('日') > 0) {
        minX = DateToString(new Date(chart.xAxis[0].min));
        maxX = DateToString(new Date(chart.xAxis[0].max));
    } else if (xTitle.indexOf('%') > 0) {
        minX = (chart.xAxis[0].min * 100).toFixed(2) + "%";
        maxX = (chart.xAxis[0].max * 100).toFixed(2) + "%";
    } else {
        minX = (chart.xAxis[0].min).toFixed(2);
        maxX = (chart.xAxis[0].max).toFixed(2);
    }

    if (yTitle.indexOf('日') > 0) {
        minY = DateToString(new Date(chart.yAxis[0].min));
        maxY = DateToString(new Date(chart.yAxis[0].max));
    } else if (yTitle.indexOf('%') > 0) {
        minY = (chart.yAxis[0].min * 100).toFixed(2) + "%";
        maxY = (chart.yAxis[0].max * 100).toFixed(2) + "%";
    } else {
        minY = (chart.yAxis[0].min).toFixed(2);
        maxY = (chart.yAxis[0].max).toFixed(2);
    }

    $("#deawingXMin").val(minX);
    $("#deawingXMax").val(maxX);
    $("#deawingYMin").val(minY);
    $("#deawingYMax").val(maxY);
}

function drawingPieChart(data, contentName, title, totalType) {
    if (data.toString() === "") {
        $("#" + contentName).html("<br />暂无相关数据");
        return;
    }

    var dataList = [];
    $.each(data, function (index, content) {
        unitText = totalType == "balance" ? "亿元" : "笔";
        lengendText = content.X + "：" + Math.floor(content.Y) + unitText;
        dataList.push({ name: lengendText, y: content.Y, z: content.X });
    });

    var options = {
        chart: {
            renderTo: contentName,
            type: "pie"
        },
        title: {
            text: title
        },
        tooltip: {
            headerFormat: "",
            pointFormat: "{point.z}: <b>{point.percentage:.1f}%</b>"
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle',
            squareSymbol: true,
            symbolHeight: 12,
            symbolWidth: 12,
            symbolRadius: 0,
            itemMarginTop: 2,
            floating:false,
            navigation: {
                enabled: true
            }
            ,itemStyle: {
                'fontSize': '15px'
            }
        },
        plotOptions: {
            series: {
                turboThreshold: 10000
            },
            pie: {
                allowPointSelect: true,
                showInLegend: true,
                cursor: "pointer",
                borderWidth:0,
                dataLabels: {
                    enabled: false
                }
            }
        },
        exporting: {
            enabled: false
        },
        series: [{ data: dataList }]
    };
    var chart = new Highcharts.Chart(options);
}

function scatterSetExtremes() {
    var titleXAxis = $("#ddlXAxis option:selected").text();
    var titleYAxis = $("#ddlYAxis option:selected").text();

    var minX, maxX, minY, maxY;
    if (titleXAxis.indexOf('日') > 0) {
        minX = new Date($("#deawingXMin").val().split('-')).getTime();
        maxX = new Date($("#deawingXMax").val().split('-')).getTime();
    } else if (titleXAxis.indexOf('%') > 0) {
        minX = parseFloat($("#deawingXMin").val().replace(/%/g, "")) / 100;
        maxX = parseFloat($("#deawingXMax").val().replace(/%/g, "")) / 100;
    } else {
        minX = parseFloat($("#deawingXMin").val().replace(/%/g, ""));
        maxX = parseFloat($("#deawingXMax").val().replace(/%/g, ""));
    }

    if (titleYAxis.indexOf('%') > 0) {
        minY = parseFloat($("#deawingYMin").val().replace(/%/g, "")) / 100;
        maxY = parseFloat($("#deawingYMax").val().replace(/%/g, "")) / 100;
    } else {
        minY = parseFloat($("#deawingYMin").val().replace(/%/g, ""));
        maxY = parseFloat($("#deawingYMax").val().replace(/%/g, ""));
    }

    scatterChart.xAxis[0].setExtremes(minX, maxX);
    scatterChart.yAxis[0].setExtremes(minY, maxY);
}
