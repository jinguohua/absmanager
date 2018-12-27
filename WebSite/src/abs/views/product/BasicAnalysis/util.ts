import commonUtils from '../../../../utils/commonUtils';
import defaultOption from '../../../../components/ABSChart/defaultOption';

export function formatComputationResult(result: any): null | any[] {
  if (!Array.isArray(result)) {
    return null;
  }
  const computationResult: any[] = [];
  for (const item of result) {
    if (!item) {
      continue;
    }
    const obj: any = {};
    obj.key = item.security_id;
    obj.name1 = item.short_name;
    obj.name2 = item.loss;
    obj.name3 = item.irr;
    obj.name4 = item.npv;
    obj.name5 = item.wal_for_principal;
    obj.name6 = item.payback_period;
    obj.name7 = item.loss_threshold;
    computationResult.push(obj);
  }
  return computationResult;
}

export function formatDate(text: string): string {
  const emptyString = '';
  if (!text) {
    return emptyString;
  }
  const dateTimeValue = Date.parse(text);
  if (isNaN(dateTimeValue)) {
    return emptyString;
  }
  const date = new Date(text);
  const monthValue = date.getMonth() + 1;
  const month = monthValue < 10 ? `0${monthValue}` : `${monthValue}`;
  const monthDateValue = date.getDate();
  const monthDay = monthDateValue < 10 ? `0${monthDateValue}` : `${monthDateValue}`;
  const dateString = `${date.getFullYear()}-${month}-${monthDay}`;
  return dateString;
}

export function getCommonChartData(chartData: any, itemStyle?: any, isPlotBands?: boolean) {
  if (!chartData || !chartData.line_series || chartData.line_series.length === 0) {
    return;
  }
  const data = chartData;
  const seriesList: any = commonUtils.getChartSeries(data, null, null, null, itemStyle);
  const plotValue = new Date(data.plot_value).getTime();

  return {
    ...defaultOption,
    title: {
      text: chartData.title,
    },
    xAxis: {
      tickInterval: 1000 * 60 * 60 * 24 * 365,
      plotBands: {
        color: 'rgba(33,38,50, 0.6)',
        from: 0,
        to: isPlotBands ? plotValue : 0,
      },
      plotLines: [{
        zIndex: 999,
        color: '#374150',
        width: 1,
        value: plotValue,
        dashStyle: 'dash',
        label: {
          // text: data.plot_label,
          verticalAlign: 'middle',
          textAlign: 'center',
          style: {
            color: 'white'
          }
        }
      }],
      labels: {
        formatter: function () {
          const that = this as any;
          let e = new Date(that.value);
          return e.getFullYear() + '年';
        },
        rotation: 0,
      },
    },
    plotOptions: {
      series: {
        marker: {
          enabled: !1
        }
      }
    },
    series: seriesList,
  };
}

export function getYAxisLabel(series: any, index: number) {
  if (series.name.indexOf('损失率') > 0) {
    return {
      yAxis: 1
    };
  }
  return null;
}

// 资产池偿付走势图
export function getAPPaymentTrendChartData(chartData: any) {
  const option = {
    ...getCommonChartData(chartData, (series, index) => getYAxisLabel(series, index), true),
    yAxis: [
      {
        title: {
          enabled: true,
          useHTML: true,
          text: '(' + commonUtils.titleReserver('亿', true) + ')'
            + commonUtils.titleReserver('剩余本金', true),
        },
        labels: {
          format: '{value:.0f}'
        },
      },
      {
        opposite: true,
        title: {
          enabled: true,
          useHTML: true,
          text: commonUtils.titleReserver('资产损失率', false),
        },
        labels: {
          format: '{value:.0f}%'
        },
      },
    ],
    tooltip: {
      formatter: function () {
        const chart = (this as any);
        if (chart.series.name.indexOf('损失率') > 0) {
          return commonUtils.formatDate(chart.x) + '<br/>' + chart.series.name + ': <b>' + Math.round(100 * chart.y) / 100 + '</b>%';
        } else {
          return commonUtils.formatDate(chart.x) + '<br/>' + chart.series.name + '<br/>剩余本金' + ': <b>' + Math.round(100 * chart.y) / 100 + '</b>(亿)';
        }
      }
    },
  };
  return option;
}

// IOOC走势图
export function getCOCChartData(chartData: any) {
  const option = {
    ...getCommonChartData(chartData, null, true),
    yAxis: {
      title: {
        enabled: true,
        useHTML: true,
        text: commonUtils.titleReserver('百分比', true),
      },
      labels: {
        // format: '{value:.0f}%'
        formatter: function () {
          const chart = (this as any);
          return Math.round(100 * chart.value) + '%';
        }
      },
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
        const chart = (this as any);
        return commonUtils.formatDate(chart.x) + '<br/>' + chart.series.name + ': <b>' + Math.round(100 * chart.y) + '</b>%';
      }
    },
  };
  return option;
}

// 资产池利息分布走势图
export function getAPInterestRateTrendChartData(chartData: any) {

  const option = {
    ...getCommonChartData(chartData, null, true),
    yAxis: {
      title: {
        enabled: true,
        useHTML: true,
        text: '(' + commonUtils.titleReserver('百万', true) + ')'
          + commonUtils.titleReserver('利息总量', true),
      },
      labels: {
        formatter: function () {
          const that = this as any;
          return commonUtils.formatContent(that.value, null, null, null, 0, 6);
        },
      },
    },
    tooltip: {
      formatter: function () {
        const chart = (this as any);
        return commonUtils.formatDate(chart.x) +
          '<br/>' + chart.series.name + ':<b>' + commonUtils.formatContent(chart.y, null, null, null, null, 6) +
          '</b>(百万)';
      }
    },
  };
  return option;
}