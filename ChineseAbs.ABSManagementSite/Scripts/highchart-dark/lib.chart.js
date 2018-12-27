function PieChart(data, title, div) {
    if (data != null) {
        var seriesList = [];
        $.each(data, function (index, content) {
            var dataList = [];
            $.each(content.Data.Data, function (index, content) {
                dataList.push([content.X, content.Y]);
            });
            seriesList.push({ name: content.Name, data: dataList });
        });

        var options = {
            chart: {
                renderTo: div,
                type: 'pie'
            },
            title: {
                text: title
            },
            tooltip: {
                headerFormat: '',
                pointFormat: '{point.name}: <b>{point.percentage:.1f}%</b>'
            },
            legend: {
                align: 'right',
                layout: 'vertical',
                verticalAlign: 'middle',
                x: 0,
                y: 10
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: false,
                        distance: 0,
                        color: 'white',
                        connectorColor: '#000000',
                        format: '<b>{point.name}</b>: {point.percentage:.1f} %'
                    },
                    showInLegend: true
                }
            },
            series: seriesList
        };

        chart = new Highcharts.Chart(options);
    }
}

function LineChart(data, title, div) {
    var minValue = 0;
    var interVal = 0.5;
    if (title.toLowerCase() == "irr") {
        minValue = -0.1;
        interVal = 0.1;
    }
    if (data != null) {
        var seriesList = [];
        $.each(data, function (index, content) {
            var dataList = [];
            $.each(content.Data.Data, function (index, content) {
                dataList.push([content.X, content.Y]);
            });
            seriesList.push({ name: content.Name, data: dataList });
        });

        var options = {
            chart: {
                renderTo: div,
                type: 'spline'
            },
            title: {
                text: title
            },
            xAxis: {
                type: 'number',
                tickLength: 5,
                tickPosition: "inside",
            },

            labels: {
                rotation: 0,
            },

            yAxis: {
                title: {
                    text: null
                },
                labels: {
                    formatter: function () {
                        return Math.round(this.value * 100) + '%';
                    }
                },
                min: minValue,
                tickInterval: interVal
            },
            plotOptions: {
                series: {
                    lineWidth: 3,
                    marker: {
                        enabled: false
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    var s;
                    s = '' + Math.round(this.y * 100) + '%';
                    return s;
                }
            },
            series: seriesList
        };
        chart = new Highcharts.Chart(options);
    }
}

function LineChartCoverage(data, title, div) {
    var minValue = 0;
    var interVal = 0.5;
    
    if (data != null) {
        var seriesList = [];
        $.each(data, function (index, content) {
            var dataList = [];
            $.each(content.Data.Data, function (index, content) {
                var date = parseInt(content.X.replace("/Date(", "").replace(")/", ""));
                dataList.push([date, content.Y]);
            });
            seriesList.push({ name: content.Name, data: dataList });
        });

        var options = {
            chart: {
                renderTo: div,
                type: 'spline'
            },
            title: {
                text: title
            },
            xAxis: {
                type: 'datetime',
                //tickInterval: 30 * 24 * 3600 * 1000,
                dateTimeLabelFormats: {
                    second: "%Y-%m-%d",
                    minute: "%Y-%m-%d",
                    hour: "%Y-%m-%d",
                    day: "%Y-%m-%d",
                    week: "%Y-%m-%d",
                    month: "%Y-%m",
                    year: "%Y"
                },
                tickLength: 5,
                tickPosition: "inside",
            },

            labels: {
                rotation: 0,
            },

            yAxis: {
                title: {
                    text: null
                },
                labels: {
                    formatter: function () {
                        return Math.round(this.value * 100) + '%';
                    }
                },
                max: 3.0,
                min: minValue,
                tickInterval: interVal
            },
            plotOptions: {
                series: {
                    lineWidth: 3,
                    marker: {
                        enabled: false
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    var s;
                    s = '' + Math.round(this.y * 100) + '%';
                    return s;
                }
            },
            series: seriesList
        };

        chart = new Highcharts.Chart(options);
    }
}

function BarChart(data, title, div) {
    if (data != null) {
        var categories = [];
        var seriesList = [];
        var i = 0;
        $.each(data, function (index, content) {
            var dataList = [];
            $.each(content.Data.Data, function (index, content) {
                var index = content.X.indexOf("%-");
                if (index > -1) {
                    categories.push(content.X.substr(1, index));
                    dataList.push(content.Y);
                }
                else {
                    categories.push(content.X);
                    dataList.push(content.Y);
                }
            });

            seriesList.push({
                name: content.Name, data: dataList
            });
        });

        var options = {
            chart: {
                renderTo: div,
                type: 'column',
            },
            title: {
                text: title
            },
            xAxis: {
                tickmarkPlacement: "on",
                categories: categories,
                tickLength: 0,
                labels: {
                    rotation: 0,
                    x: 0
                },
                tickPosition: "inside",
            },
            yAxis: [{
                title: {
                    enable: false
                },
                tickPixelInterval: 60,
                labels: {
                    formatter: function () {
                        return Math.round(this.value * 100) + '%';
                    }
                },
                max: 2
            }, {
                title: {
                    enable: false
                },
                opposite: true
            }],
            tooltip: {

                formatter: function () {
                    return Math.round(this.point.y * 10000) / 100 + '%';
                }
            },
            plotOptions: {
                column: {
                    pointPadding: 0.2,
                    borderWidth: 0
                },
                pointPlacement: "between"
            },
            series: seriesList
        };

        chart = new Highcharts.Chart(options);
    }
}

function BarPlotChart10Items(data, title, div, plotname, plotvalue) {
    if (data != null) {
        var categories = [];
        var seriesList = [];
        $.each(data, function (index, content) {
            var dataList = [];
            $.each(content.Data.Data, function (index, content) {
                categories.push(content.X);
                dataList.push(content.Y);
            });
            seriesList.push({ name: content.Name, data: dataList });
        });
        var n = categories.length;
        var max = categories[n - 1];
        if (isNaN(max))
            max = max.substr(0, max.length - 1);
        if (isNaN(max))
            max = max.substr(0, max.length -1);
        var reg = /[\u4e00-\u9fa5]/g;       
        var result = plotname.replace(reg, "").replace("：", "").replace("%", "");          
        var offset = 6 +(result / max) * 100;
        
        var options = {
            chart: {
                renderTo: div,
                type: 'column',
            },
            title: {
                text: title
            },
            xAxis: {
                tickmarkPlacement: "between",
                categories: categories,
                tickLength: 2,
                tickPosition: "inside",
                labels: {
                    x: -6,
                    y: 10,
                    rotation: 0,
                    step:2
                },
                plotLines: [{
                    color: 'black',
                    width: 1.5,
                    value: plotvalue,
                    dashStyle: 'dash',
                    label: {
                        rotation: 0,
                        x: 0,
                        y: -6,
                        text: plotname,
                        verticalAlign: 'top',
                        textAlign: 'left',
                        style: {
                            color: 'blue'
                        }
                    },
                    zIndex: 90
                }]
            },
            yAxis: [{
                title: {
                    enable: false
                },
                tickPixelInterval: 60,
                labels: {
                    formatter: function () {
                        return Math.round(this.value * 100) + '%';
                    }
                },
            }, {
                title: {
                    enable: false
                },
                opposite: true
            }],
            tooltip: {

                formatter: function () {
                    return Math.round(this.point.y * 10000) / 100 + '%';
                }
            },
            plotOptions: {
                column: {
                    pointPadding: 0.2,
                    borderWidth: 0
                },
                pointPlacement: "between"
            },
            series: seriesList
        };

        chart = new Highcharts.Chart(options, function (chart) {
            chart.renderer.path(['m', 40, 60, 'l', offset , 0, -5, 4, 5, -4, -5, -4, 5, 4]).attr({
                'stroke-width': 1.5,
                stroke: 'blue'
            }).add();
        });
    }
}

function LinePlotChartRating(data, title, div, plotname, plotvalue) {
    if (data != null)
    {
        var CNABSRating = ["5A", "4A+", "4A", "4A-", "AAA+", "AAA", "AAA-", "AA+", "AA", "AA-", "A+", "A", "A-", "BBB+", "BBB", "BBB-", "BB+", "BB", "BB-", "B+", "B", "B-"].reverse();
        var categories = [];
        var seriesList = [];
        $.each(data, function (index, content) {
            var dataList = [];
            categories = [];
            $.each(content.Data.Data, function (index, content) {
                categories.push(content.X);
                dataList.push(content.Y);
            });
            seriesList.push({ name: content.Name, data: dataList });
        });
        var options = {
            chart: {
                renderTo: div,
                type: 'spline',
            },
            title: {
                text: title
            },
            xAxis: {
                title: {
                    text: "有效资产数",
                    align: "high",
                },
                categories: categories,
                tickmarkPlacement: "on",
                tickInterval: 1,
                tickLength: 2,
                tickPosition: "inside",
                labels: {
                    x: 0,
                    y: 20,
                    rotation: 0,
                    step: 2,
                },
                plotLines: [{
                    color: '#B7AFA5',
                    width: 1.5,
                    value: plotvalue-1,
                    dashStyle: 'dash',
                    label: {
                        x: 6,
                        y: 36,
                        text: plotname,
                        verticalAlign: 'center',
                        textAlign: 'center',
                        style: {
                            color: '#B7AFA5'
                        }
                    },
                    zIndex: 90
                }]
            },
            yAxis: {
                max:20,
                title: {
                    enable: false
                },
                tickInterval: 1,
                labels: {
                    formatter: function () {
                        return CNABSRating[this.value + 1];
                    },
                    
                },
            },
            tooltip: {
                formatter: function () {
                    return this.point.y;
                }
            },
            plotOptions: {
                series: {
                    lineWidth: 3,
                    marker: {
                        enabled: false
                    }
                }
            },
            series: seriesList
        };
        chart = new Highcharts.Chart(options);
    }
}

           
           
        
            
     
