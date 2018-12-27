import React, { Component } from 'react';
import { connect } from 'dva';
import ABSPanel from '../../../../components/ABSPanel';
import ABSListView from '../../../../components/ABSList';
import { chartColors } from '../../../../components/ABSChart/chartTheme';
import './relationAndResult.less';
import PerfectScrollbar from 'react-perfect-scrollbar';

const colors = chartColors;
const nowDate = new Date();

export interface IRelationAndResultProps {
  paymentList: any;
  dispatch: any;
  scenarioID: number;
}

class RelationAndResult extends Component<IRelationAndResultProps, any> {
  componentDidMount() {
    const { dispatch, scenarioID } = this.props;
    dispatch({ type: 'product/getPaymentList', payload: { scenario_id: scenarioID } });
  }

  // table表头数据处理
  restructureColumnsData = (paymentList) => {
    let columnsData: any = [];
    let num: number = 0;
    paymentList.map((dom, key) => {
      num = (num === colors.length) ? 0 : num;
      const color = colors[num];
      num++;
      const columns = {
        title: dom.title,
        key: dom.index,
        dataIndex: dom.index,
        className: dom.index !== 0 ? 'abs-center' : '',
        render: (text, record, index) => {
          if (dom.index === 0) {
            return text;
          } else if (dom.is_event || dom.is_event) {
            return this.isPassColor(text, key);
          } else {// 模块颜色通过顺序筛选颜色
            return this.moduleBgColor(text, color, key);
          }
        }
      };
      columnsData.push(columns);
    });
    
    return columnsData;
  }

  // 设置是否通过的文字颜色  
  isPassColor = (text, key) => {
    if (text) {
      return (
        <span className="abs-list-col-pass" key={key}>通过</span>
      );
    }
    return (
      <span className="abs-list-col-noPass" key={key}>未通过</span>
    );
  }
  // 设置证券模块颜色
  moduleBgColor = (condition, color, key) => {
    return (
      <div key={key} className="abs-list-color-col" style={{ backgroundColor: (condition ? color : '') }} />
    );
  }
  // 设置遮罩层的高
  setMaskHeight = (list) => {
    const maskCount = list.reduce((acc, dom) => {
      if (nowDate >= new Date(dom['0'])) {
        return acc + 1;
      }
      return acc;
    }, 0);
    return maskCount;
  }

  render() {
    const { paymentList } = this.props;
    const paymentLists = paymentList ? paymentList : {};
    const maskHeight = this.setMaskHeight(paymentLists.data || []);
    const maskDisplay = maskHeight ? 'block' : 'none';
    return (
      <div className="relationAndResultTable">
        <ABSPanel title="偿付关系与事件结果" removePadding={true}>
          <div className="abs-list-mask" style={{ height: (maskHeight * 32), display: maskDisplay }} />
          <PerfectScrollbar>
            <ABSListView
              type="default"
              tableWidth="100%"
              bordered={false}
              columnsData={this.restructureColumnsData(paymentLists.lables || [])}
              contentData={paymentLists.data || []}
              loading={false}
            />
          </PerfectScrollbar>
        </ABSPanel>
      </div>
    );
  }
}

function mapStateToProps({ product, global }: any) {
  return { paymentList: product.paymentList, scenarioID: product.scenarioID };
}

export default connect(mapStateToProps)(RelationAndResult);