
import '../../../../components/ABSStructure/index.less';
import commonUtils from '../../../../utils/commonUtils';
import defaultChart from '../../../../components/ABSChart/chartData';
import defaultOption from '../../../../components/ABSChart/defaultOption';

interface IDealIssuer {
  fullname: string;
  name: string;
}
export function getColumnChartConfig(chartData: any, isFirst: boolean = false) {
  if (!chartData || !chartData.line_series || chartData.line_series.length === 0) {
    return defaultChart;
  }
  const data = chartData;
  const { xList, seriesList } = commonUtils.getChartSeries(data, (point) => point.x, (point) => Number.parseFloat(String(point.y)) * 100);
  const plotValue = data.plot_value;
  const width = (document.body.offsetWidth - 200 - 14) / 2;
  const offest = ((width - 40) / xList.length) / 2;
  var option = {
    ...defaultOption,
    title: {
      text: data.title,
    },
    xAxis: {
      labels: {
        x: offest,
        rotation: -30,
      },
      showFirstLabel: true,
      categories: xList,
      plotLines: [{
        zIndex: 999,
        color: '#374150',
        width: 1,
        value: plotValue < 0 ? plotValue + 0.5 : plotValue - 0.5,
        dashStyle: 'dash',
        label: {
          // text: data.plot_label,
          verticalAlign: 'middle',
          textAlign: 'center',
          style: {
            color: 'white'
          }
        },
      }],
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
      column: {
        borderWidth: 0,
        pointWidth: 20
      },
      series: {
        marker: {
          enabled: !1
        }
      }
    },
    tooltip: {
      formatter: function () {
        const that = this as any;
        return `${that.series.name}:${getTooltip(xList, that.x)}~${that.x}, 占比${commonUtils.formatContent(that.y)}`;
      }
    },
    series: seriesList,
  };
  return option;
}

export function getTooltip(xList: Array<any>, item: any) {
  const index = xList.indexOf(item);
  if (index <= 0) {
    return String(xList).indexOf('%') > 0 ? '0%' : 0;
  }
  if (index - 1 === 0) {
    return xList[0];
  }
  return xList[index - 1];
}

export function generateInnerHTML(dealIssuerList: IDealIssuer[]) {
  const innerHTML = dealIssuerList.reduce((acc, dealIssuer, index) => {
    if (index !== dealIssuerList.length - 1) {
      return `${acc}<p>${dealIssuer.fullname}</p><br>`;
    }
    // 最后一个元素不加<br>
    return `${acc}<p>${dealIssuer.fullname}</p>`;
  }, '');
  return innerHTML;
}