import '../../../../components/ABSStructure/index.less';
import commonUtils from '../../../../utils/commonUtils';
import defaultOption from '../../../../components/ABSChart/defaultOption';
import { chartColors } from '../../../../components/ABSChart/chartTheme';
import { IProgressNode } from '../../../../components/ABSProgress/ABSProgressItem';

export function formatProductCycleData(response: any): null | IProgressNode[] {
  if (!response) {
    return null;
  }
  const circleList = response.circles;
  if (!Array.isArray(circleList)) {
    return null;
  }
  const productCycle: IProgressNode[] = [];
  for (const item of circleList) {
    if (!item) {
      continue;
    }
    const obj: IProgressNode = {
      date: '',
      title: '',
      time: 0,
    };
    if (item.tag_name) {
      obj.title = item.tag_name;
    }
    if (!item.date_value) {
      continue;
    }
    obj.date = item.date_value;
    const timeValue = Date.parse(obj.date);
    if (isNaN(timeValue)) {
      continue;
    }
    obj.time = timeValue;
    productCycle.push(obj);
  }
  return productCycle;
}

export function formatExpertListData(response: any): null | any[] {
  if (!response) {
    return null;
  }
  const professorList = response.professors;
  if (!Array.isArray(professorList)) {
    return null;
  }
  const expertList: any[] = [];
  for (const item of professorList) {
    if (!item) {
      continue;
    }
    const obj: any = {};
    obj.id = item.user_id;
    obj.imageUrl = item.avatar_path;
    obj.name = item.name;
    obj.company = item.company;
    obj.department = item.department;
    obj.position = item.title;
    expertList.push(obj);
  }
  return expertList;
}

export function formatOrganizationData(response: any): null | any[] {
  if (!response) {
    return null;
  }
  const { organizations } = response;
  if (!Array.isArray(organizations)) {
    return null;
  }
  return organizations;
}

export function getBasicData(basicInfo: any) {
  const { deal_sub_type } = basicInfo ? basicInfo : '';
  if (deal_sub_type) {
    return getABNData(basicInfo);
  }
  return getABSData(basicInfo);
}

export function getABSData(basicInfo: any) {
  const { deal_name, product_type, exchange, is_reinvestment } = basicInfo ? basicInfo : '';
  const list = [
    {
      title: '产品简称',
      content: commonUtils.formatContent(deal_name),
    },
    {
      title: '市场分类',
      content: commonUtils.formatContent(product_type),
    },
    {
      title: '交易场所',
      content: commonUtils.formatContent(exchange),
    },
    {
      title: '循环购买',
      content: commonUtils.formatContent(is_reinvestment),
    },
  ];
  const { current_status, deal_type, issue_type, register } = basicInfo ? basicInfo : '';
  const registerName = register ? register.map((item) => { return item.register_name + ' <br> '; }) : '';
  const list1 = [
    {
      title: '当前状态',
      content: commonUtils.formatContent(current_status),
    },
    {
      title: '产品分类',
      content: commonUtils.formatContent(deal_type),
    },
    {
      title: '发行方式',
      content: commonUtils.formatContent(issue_type),
    },
    {
      title: '许准予字',
      content: commonUtils.formatContent(String(registerName)),
    },
  ];
  const { total_offering, asset_subcategory, regulator } = basicInfo ? basicInfo : '';
  const list2 = [
    {
      title: '发行规模(亿)',
      content: commonUtils.formatContent(total_offering, null, null, true, 2, 8),
      contentStyle: { color: '#EA7600' }
    },
    {
      title: '产品细分',
      content: commonUtils.formatContent(asset_subcategory),
    },
    {
      title: '监管机构',
      content: commonUtils.formatContent(regulator),
    },
  ];

  return { list, list1, list2 };
}

export function getABNData(basicInfo: any) {
  const { deal_name, product_type, exchange, is_reinvestment, deal_sub_category } = basicInfo ? basicInfo : '';
  const list = [
    {
      title: '产品简称',
      content: commonUtils.formatContent(deal_name),
    },
    {
      title: '市场分类',
      content: commonUtils.formatContent(product_type),
    },
    {
      title: '资产描述',
      content: commonUtils.formatContent(deal_sub_category),
    },
    {
      title: '循环购买',
      content: commonUtils.formatContent(is_reinvestment),
    },
  ];
  const { current_status, deal_type, issue_type } = basicInfo ? basicInfo : '';
  const list1 = [
    {
      title: '当前状态',
      content: commonUtils.formatContent(current_status),
    },
    {
      title: '产品分类',
      content: commonUtils.formatContent(deal_type),
      contentStyle: { color: '#EA7600' }
    },
    {
      title: '交易场所',
      content: commonUtils.formatContent(exchange),
    },
    {
      title: '发行方式',
      content: commonUtils.formatContent(issue_type),
    },
  ];
  const { total_offering, asset_subcategory, regulator } = basicInfo ? basicInfo : '';
  const list2 = [
    {
      title: '发行规模(亿)',
      content: commonUtils.formatContent(total_offering, null, null, null, null, 8),
      contentStyle: { color: '#EA7600' }
    },
    {
      title: '产品细分',
      content: commonUtils.formatContent(asset_subcategory),
    },
    {
      title: '监管机构',
      content: commonUtils.formatContent(regulator),
    },
  ];

  return { list, list1, list2 };
}

export function getTableTitle() {
  return [
    {
      title: '证券简称',
      dataIndex: 'description',
      fixed: 'left',
      render: (text, record) => text,
    }, {
      title: '证券代码',
      dataIndex: 'security_code',
      render: (text, record) => text,
    }, {
      title: '发行量(万)',
      dataIndex: 'notional',
      className: 'abs-right',
      render: (text) => commonUtils.formatContent(text, true, false, true, 2, 4)
    }, {
      title: '存量(万)',
      dataIndex: 'principal',
      className: 'abs-right',
      render: (text) => commonUtils.formatContent(text, true, false, true, 2, 4)
    }, {
      title: '发行利率',
      dataIndex: 'coupon',
      className: 'abs-right',
      render: (text) => commonUtils.formatContent(text, true, true)
    }, {
      title: '当前利率',
      dataIndex: 'current_coupon',
      className: 'abs-right',
      render: (text) => commonUtils.formatContent(text, true, true)
    }, {
      title: '还本方式',
      dataIndex: 'repayment_of_principal',
      render: (text, record) => text,
    }, {
      title: '加权年限',
      dataIndex: 'initial_wal_legal',
      className: 'abs-right',
      render: (text) => commonUtils.formatContent(text, true)
    }, {
      title: '定价',
      dataIndex: 'clean_price',
      className: 'abs-right',
      render: (text) => commonUtils.formatContent(text, true, false, true, 4)
    }, {
      title: '当前评级',
      dataIndex: 'current_rating_pause',
      className: 'abs-center',
      render: (text, record) => text,
    }
  ];
}

export function getLineChartConfig(chartData: any) {
  if (!chartData || !chartData.line_series || chartData.line_series.length === 0) {
    return {};
  }
  const data = chartData;
  const seriesList: any = commonUtils.getChartSeries(data, null, null, (name, index) => commonUtils.getMarkerStyles(name, index));
  const plotValue = new Date(data.plot_value).getTime();
  // 添加对比颜色
  if (seriesList && seriesList.length > 0) {
    let n = seriesList.length / 2;
    seriesList.forEach((element, index) => {
      element.color = index >= n
        ? chartColors[((index - n) % chartColors.length)]
        : chartColors[index % chartColors.length];
    });
  }
  var option = {
    ...defaultOption,
    title: {
      text: data.title,
    },
    chart: {
      style: {
        height: '400px',
      }
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
          return commonUtils.formatDate(that.value);
        },
        rotation: -30,
      },
    },
    yAxis: {
      title: {
        enabled: true,
        useHTML: true,
        text: commonUtils.titleReserver('剩余本金占比', true),
      },
      labels: {
        format: '{value:.0f}%'
      },
      min: 0,
      max: 100
    },
    tooltip: {
      formatter: function () {
        const chart = (this as any);
        return commonUtils.formatDate(chart.x) + '<br/>' + chart.series.name + '<br/>剩余本金占比:' + Math.round(100 * chart.y) / 100 + '%';
      }
    },
    series: seriesList,
  };
  return option;
}