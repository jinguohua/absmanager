export const chartColors = [
  '#2A9CFF', '#4258EB', '#00BBD5', '#00C773', '#BFD52A', '#F19603',
  '#EB5F03', '#C761FF', '#4664E3', '#00B8DF', '#29DA8C', '#AFD52A',
  '#FBC902', '#CF50CD', '#713FFF', '#1486F0', '#00C354', '#DBC92C'
];
export const specialColor = '#FC9023';

const ChartTheme = {
  colors: chartColors,
  chart: {
    backgroundColor: {
      linearGradient: {
        x1: 0,
        y1: 0,
        x2: 1,
        y2: 1,
      },
      stops: [
        [0, '#0A0C0F'],
        [1, '#0A0C0F'],
      ],
    },
    style: {
      fontFamily: '"Microsoft YaHei", 微软雅黑, 宋体, Verdana, Arial, Helvetica, sans-serif',
      height: '400px',
    },
    plotBorderColor: '#606063',
    spacingTop: 20
  },
  title: {
    style: {
      color: '#FFE2B2',
      fontSize: '14px',
      lineHeight: '14px',
      fontFamily: 'Microsoft YaHei-Bold !important',
      fontWeight: 'bold',
      letterSpacing: '0.61px',
      textAlign: 'center',
      padding: '20px 0',
    },
    margin: 20
  },
  subtitle: {
    style: {
      color: '#E0E0E3',
      textTransform: 'uppercase',
    },
  },
  xAxis: {
    gridLineColor: '#1F232B',
    labels: {
      style: {
        color: '#FFFFFF',
        fontSize: '14px',
      },
      autoRotation: [-30]
      // rotation: 0
    },
    lineColor: '#232323',
    minorGridLineColor: '#505053',
    tickColor: '#999999',
    tickLength: 5,
    title: {
      style: {
        color: '#A0A0A3',
        fontSize: '14px',
      },
    },
    plotBands: {
      color: '#202020'
    }
  },
  yAxis: {
    gridLineColor: '#1F232B',
    gridLineWidth: 0.5,
    labels: {
      style: {
        color: '#FFFFFF',
        fontSize: '14px',
      },
    },
    lineColor: '#707073',
    minorGridLineColor: '#505053',
    tickColor: '#1F232B',
    tickWidth: 1.5,
    tickLength: 0,
    title: {
      enabled: !1,
      style: {
        color: '#A0A0A3',
        fontSize: '14px',
      },
    },
  },
  tooltip: {
    backgroundColor: 'rgba(33,38,45,0.90)',
    style: {
      color: '#F0F0F0',
      fontSize: '12px',
    },
  },
  plotOptions: {
    // series: {
    //   marker: {
    //     enabled: true,
    //     lineColor: '#202020',
    //     symbol: 'circle',
    //     radius: 3,
    //     lineWidth: 0
    //   },
    //   // lineWidth: 1
    // },
    scatter: {
      marker: {
        enabled: true,
        lineColor: '#202020',
        symbol: 'circle',
        radius: 3,
        lineWidth: 0
      },
      // lineWidth: 1
    },
    spline: {
      marker: {
        enabled: false,
        lineColor: '#202020',
        symbol: 'circle',
        radius: 3,
        lineWidth: 0
      },
      lineWidth: 3
    },
    line: {
      marker: {
        enabled: false,
        lineColor: '#202020',
        symbol: 'circle',
        radius: 3,
        lineWidth: 0
      },
    },
    boxplot: {
      fillColor: '505053',
    },
    candlestick: {
      lineColor: 'white',
    },
    errorbar: {
      color: 'white',
    },
  },
  legend: {
    margin: 20,
    itemStyle: {
      color: '#E0E0E3',
      fontSize: '12px',
      fontWeight: null,
    },
    itemHoverStyle: {
      color: '#FFF',
    },
    itemHiddenStyle: {
      color: '#606063',
    },
    maxHeight: 72,
    symbolRadius: 6,
    itemMarginTop: 5,
    navigation: {
      arrowSize: 8,
      style: {
        color: 'white'
      }
    }
  },
  credits: {
    style: {
      color: '#666',
      fontSize: '12px',
    },
  },
  labels: {
    style: {
      color: '#707073',
      fontSize: '12px',
    },
  },
  drilldown: {
    activeAxisLabelStyle: {
      color: '#F0F0F3',
    },
    activeDataLabelStyle: {
      color: '#F0F0F3',
    },
  },
  navigation: {
    buttonOptions: {
      symbolStroke: '#DDDDDD',
      theme: {
        fill: '#47423C',
      },
    },
  },
  rangeSelector: {
    buttonTheme: {
      fill: '#514C44',
      stroke: '#000000',
      style: {
        color: '#CCC',
      },
      states: {
        hover: {
          fill: '#707073',
          stroke: '#000000',
          style: {
            color: 'white',
          },
        },
        select: {
          fill: '#727272',
          stroke: '#000000',
          style: {
            color: 'white',
          },
        },
      },
    },
    inputBoxBorderColor: '#707073',
    inputStyle: {
      backgroundColor: '#202020',
      color: 'silver',
    },
    labelStyle: {
      color: 'silver',
    },
  },
  navigator: {
    handles: {
      backgroundColor: '#666',
      borderColor: '#AAA',
    },
    outlineColor: '#CCC',
    maskFill: 'rgba(255,255,255,0.1)',
    series: {
      color: '#7798BF',
      lineColor: '#B7AFA5',
    },
    xAxis: {
      gridLineColor: '#505053',
    },
  },
  scrollbar: {
    barBackgroundColor: '#727272',
    barBorderColor: '#727272',
    buttonArrowColor: '#CCC',
    buttonBackgroundColor: '#606063',
    buttonBorderColor: '#606063',
    rifleColor: '#FFF',
    trackBackgroundColor: '#514C44',
    trackBorderColor: '#514C44',
  },
  legendBackgroundColor: 'rgba(0, 0, 0, 0.5)',
  background2: '#505053',
  dataLabelsColor: '#B0B0B3',
  textColor: '#C0C0C0',
  contrastTextColor: '#F0F0F3',
  maskColor: 'rgba(255,255,255,0.3)',
};

export default ChartTheme;
