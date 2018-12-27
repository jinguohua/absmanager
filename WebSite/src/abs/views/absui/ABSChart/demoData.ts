import '../../../../components/ABSStructure/index.less';
// 处理接口数据，返回chart图的config数据
export default function getDemoConfig() {
    // 所有chart图接口返回数据的格式都是↓↓↓这个样子
    const demoApiData = {
        code: 200,
        status: 'ok',
        data: {
            title: '各级证券偿付',
            plot_label: '',
            plot_value: '2018-07-12',
            line_series: [
                {
                    name: '15开元6A1',
                    type: 'line',
                    step: 'right',
                    dash_style: 'solid',
                    range: {
                        max_x: null,
                        max_y: 0,
                        min_x: null,
                        min_y: 0
                    },
                    data: {
                        data: [
                            {
                                x: '2015-11-17',
                                y: 100,
                                tooltip: null
                            },
                            {
                                x: '2016-01-12',
                                y: 35.44,
                                tooltip: null
                            },
                            {
                                x: '2016-04-12',
                                y: 0.00,
                                tooltip: null
                            }
                        ]
                    }
                },
                {
                    name: '15开元6A2',
                    type: 'line',
                    step: 'right',
                    dash_style: 'solid',
                    range: {
                        max_x: null,
                        max_y: 0,
                        min_x: null,
                        min_y: 0
                    },
                    data: {
                        data: [
                            {
                                x: '2015-11-17',
                                y: 100,
                                tooltip: null
                            },
                            {
                                x: '2016-01-12',
                                y: 100,
                                tooltip: null
                            },
                            {
                                x: '2016-04-12',
                                y: 100,
                                tooltip: null
                            },
                            {
                                x: '2016-07-12',
                                y: 21.98,
                                tooltip: null
                            },
                            {
                                x: '2016-10-12',
                                y: 0.00,
                                tooltip: null
                            }
                        ]
                    }
                },
                {
                    name: '15开元6A1(说明书)',
                    type: 'scatter',
                    step: 'right',
                    dash_style: 'solid',
                    range: {
                        max_x: null,
                        max_y: 0,
                        min_x: null,
                        min_y: 0
                    },
                    data: {
                        data: [
                            {
                                x: '2015-11-17',
                                y: 100,
                                tooltip: null
                            },
                            {
                                x: '2016-01-12',
                                y: 35.44,
                                tooltip: null
                            },
                            {
                                x: '2016-04-12',
                                y: 0.00,
                                tooltip: null
                            }
                        ]
                    }
                },
                {
                    name: '15开元6A2(说明书)',
                    type: 'scatter',
                    step: 'right',
                    dash_style: 'solid',
                    range: {
                        max_x: null,
                        max_y: 0,
                        min_x: null,
                        min_y: 0
                    },
                    data: {
                        data: [
                            {
                                x: '2015-11-17',
                                y: 100,
                                tooltip: null
                            },
                            {
                                x: '2016-01-12',
                                y: 100,
                                tooltip: null
                            },
                            {
                                x: '2016-04-12',
                                y: 100,
                                tooltip: null
                            },
                            {
                                x: '2016-07-12',
                                y: 21.98,
                                tooltip: null
                            },
                            {
                                x: '2016-10-12',
                                y: 0.00,
                                tooltip: null
                            }
                        ]
                    }
                }

            ]
        }

    };
    // 默认图表配置
    const defaultConfig = {
        title: {
            text: '暂无数据'
        },
        credits: {
            href: '',
            text: 'CNABS'
        }
    };
    if (demoApiData.status !== 'ok') {
        return;
    }
    if (demoApiData === null || demoApiData.data.line_series.length === 0) {
        return defaultConfig;
    }
    const data = demoApiData.data;
    const minDate = Date.UTC(2e3, 1, 1);
    let seriesList: any = [];
    data.line_series.map((series, index) => {
        let dataList: any = [];
        series.data.data.forEach((point) => {
            dataList.push([new Date(point.x).getTime(), point.y]);
        });
        let seriesData = {
            name: series.name,
            type: series.type,
            data: dataList,
            step: series.step,
            dashStyle: series.dash_style
        };
        seriesList.push(seriesData);
    });
    let plotLabel = data.plot_label;
    let plotValue = new Date(data.plot_value).getTime();

    var option = {
        title: {
            text: data.title,
        },
        xAxis: {
            plotBands: [{
                from: minDate,
                to: plotValue,
                color: '#000'
            }],
            plotLines: [{
                color: '#232323',
                width: .8,
                value: plotValue,
                dashStyle: 'dash',
                label: {
                    text: plotLabel,
                    verticalAlign: 'middle',
                    textAlign: 'left',  
                    style: {
                        color: '#E0E0E3'
                    }
                }
            }],
            labels: {
                formatter: function () {
                    const that = this as any;
                    let e = new Date(that.value);
                    let month = e.getMonth() + 1;
                    let day = e.getDate();
                    return e.getFullYear() + '.' +
                        (month < 10 ? '0' + month : month) + '.' +
                        (day < 10 ? '0' + day : day);
                },
                rotation: -30,
            },
        },
        yAxis: {
            title: {
                enabled: !0,
                text: ''
            },
            labels: {
                format: '{value:.0f}%'
            },
            min: 0,
            max: 100
        },
        plotOptions: {
            series: {
                marker: {
                    enabled: !1
                }
            }
        },
        tooltip: {
            formatter: function () {
                var cont = (this as any);
                let t,
                    e = new Date(cont.x);
                return t = e.getFullYear() + '-' + (e.getMonth() + 1) + '-' + e.getDate() +
                    '<br/>' +
                    cont.series.name +
                    '剩余本金:<br/>' +
                    Math.round(100 * cont.y) / 100 + '%';
            }
        },
        credits: {
            href: '',
            text: 'CNABS'
        },
        series: seriesList,
    };
    return option;
}