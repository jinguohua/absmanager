import commonUtils from '../../../../utils/commonUtils';
import defaultOption from '../../../../components/ABSChart/defaultOption';
import { specialColor } from '../../../../components/ABSChart/chartTheme';

export function getLineChartConfig(chartDatas: any) {
  if (!chartDatas || !chartDatas.line_series || chartDatas.line_series.length === 0) {
    return {};
  }
  const data = chartDatas;
  const seriesList: any = commonUtils.getChartSeries(data);
  const plotValue = new Date(data.plot_value).getTime();
  seriesList.map((item) => {
    if (item.name.indexOf('损失率') > -1) {
      item.yAxis = 1;
    }
  });
  var option = {
    ...defaultOption,
    title: {
      text: data.title,
    },
    xAxis: {
      plotBands: {
        color: 'rgba(33,38,50, 0.6)',
        from: 0,
        to: plotValue,
      },
      plotLines: [{
        zIndex: 999,
        color: '#374150',
        width: 1,
        value: plotValue,
        dashStyle: 'dash',
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
    yAxis: [{
      title: {
        enabled: true,
        useHTML: true,
        text: commonUtils.titleReserver('剩余本金', true)
      },
      labels: {
        format: '{value:.0f}'
      },
    }, {
      title: {
        enabled: true,
        useHTML: true,
        text: commonUtils.titleReserver('资产损失率', false)
      },
      labels: {
        formatter: function () {
          const that = this as any;
          return (that.value * 100).toFixed(2) + '%';
        }
      },
      opposite: true,
    }],
    plotOptions: {
      series: {
        marker: {
          enabled: false
        }
      }
    },
    tooltip: {
      formatter: function () {
        const chart = (this as any);
        const e = new Date(chart.x);
        if (chart.series.name.indexOf('损失率') > 0) {
          return e.getFullYear() + '-' + (e.getMonth() + 1) + '-' + e.getDate() + '<br/>' + chart.series.name + ': <b>' + Math.round(10000 * chart.y) / 100 + '</b>%';
        } else {
          return e.getFullYear() + '-' + (e.getMonth() + 1) + '-' + e.getDate() + '<br/>' + chart.series.name + '<br/>剩余本金:' + ': <b>' + Math.round(100 * chart.y) / 100 + '</b>(亿)';
        }

      }
    },
    series: seriesList,
  };
  return option;
}

export function getPrepaymentChart(chartDatas: any) {
  if (!chartDatas || !chartDatas.line_series || chartDatas.line_series.length === 0) {
    return {};
  }
  const data = chartDatas;
  const seriesList: any = commonUtils.getChartSeries(data);
  for (let i = 0; i < seriesList.length; i++) {
    if (seriesList[i].type === 'spline') {
      seriesList[i].zIndex = 4;
      seriesList[i].color = specialColor;
    }
  }
  var option = {
    ...defaultOption,
    title: {
      text: data.title,
      useHTML: false
    },
    xAxis: {
      title: {
        text: '(月)',
        align: 'high',
        offset: 6,
        y: 14
      },
      tickInterval: 5
    },
    labels: {
      rotation: 0
    },
    yAxis: {
      title: {
        enabled: !0,
        text: null
      },
      labels: {
        formatter:  function () {
          const that = this as any;
          return (that.value * 100).toFixed(2) + '%';
        }
      },
    },
    plotOptions: {
      series: {
        turboThreshold: 10000,
        marker: {
          enabled: 1
        }
      }
    },
    tooltip: {
      useHTML: true,
      style: {
        zIndex: 1000000,
      },
      formatter: function () {
        var s = '';
        var t = seriesList[0].name;
        const chart = (this as any);
        var count = 0;
        seriesList.map((item) => {
          let transferBr = 1;
          for (var i = 0; i < item.data.length; i++) {
            if (item.data[i].x === chart.x && item.data[i].y === chart.y) {
              if (item.data[i].x > 0 || item.data[i].y > 0) {
                s += (transferBr % 3 === 0 ? '<br/>' : '') +
                  '<span style="color:' + chart.series.color + '">●</span>' + item.data[i].tooltip;
                transferBr++;
              } else {
                if (t.indexOf(item.data[i].tooltip) < 0) {
                  count = count + 1;
                }
              }
              t += item.data[i].tooltip;
            }
          }
        });
        var namey = data.title === '资产池累计提前偿还率走势图' ? '累计提前偿还率' : '累计违约率';
        if (count === 0) {
          return '<span style="color:' + chart.series.color + '">存续时间：</span>' + chart.x.toFixed(0) + '月<br/><span style="color:' + chart.series.color + '">' + namey + '：</span>' + (chart.y * 100).toFixed(2) + '%<br/>' + s;
        } else {
          return '<span style="color:' + chart.series.color + '">存续时间：</span>' + chart.x.toFixed(0) + '月<br/><span style="color:' + chart.series.color + '">' + namey + '：</span>' + (chart.y * 100).toFixed(2) + '%' + '<br/><span style="color:' + chart.series.color + '">●  </span>共有' + count.toString() + '单产品';
        }
      },
    },
    exporting: {
      enabled: false,
    },
    series: seriesList,
  };
  return option;
}