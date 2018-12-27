
import '../../../../components/ABSStructure/index.less';
import commonUtils from '../../../../utils/commonUtils';
import defaultOption from '../../../../components/ABSChart/defaultOption';

export function getColumnChartConfig(chartData: any) {
  if (!chartData || !chartData.line_series || chartData.line_series.length === 0) {
    return {};
  }
  const data = chartData;
  const seriesList: any = commonUtils.getChartSeries(data, (point) => point.x, (point) => Number.parseFloat(String(point.y)) * 100);
  var option = {
    ...defaultOption,
    title: {
      text: null
    },
    plotOptions: {
      pie: {
        borderWidth: 0
      },
      series: {
        dataLabels: {
          enabled: false
        },
        showInLegend: true,
        innerSize: '50%'
      }
    },
    tooltip: {
      formatter: function () {
        const chart = (this as any);
        return chart.point.name + '<br/>金额占比:' + Math.round(100 * chart.y) / 100 + '%';
      }
    },
    series: seriesList.seriesList,
  };
  return option;
}

export function getlineColumnChartConfig(chartData: any) {
  if (!chartData || !chartData.line_series || chartData.line_series.length === 0) {
    return {};
  }
  const data = chartData;
  let { seriesList, xList } = commonUtils.getChartSeries(data, (point) => point.x, null);
  var xAxisLabelsOffset = 0;
  if (xList) {
    const width = (document.body.offsetWidth - 200 - 14) / 3;
    xAxisLabelsOffset = ((width - 40) / xList.length) / 2;
  }

  var option = {
    ...defaultOption,
    title: {
      text: data.title,
    },
    xAxis: {
      title: {
        text: '集中度',
      },
      categories: xList,
      labels: {
        step: 1,
        x: -xAxisLabelsOffset
      }
    },
    yAxis: {
      title: {
        enabled: true,
        useHTML: true,
        text: commonUtils.titleReserver('百分比', true),
        style: {
          fontSize: '12px'
        }
      },
      labels: {
        format: '{value:.0f}%'
      },
      min: 0,
      max: 100
    },
    plotOptions: {
      column: {
        borderWidth: 0
      },
      series: {
        showInLegend: false,
      }
    },
    tooltip: {
      formatter: function () {
        var cont = (this as any);
        return '金额占比:' + Math.round(100 * cont.y) / 100 + '%';
      }
    },
    series: seriesList,
  };
  return option;
}
