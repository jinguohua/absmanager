import React from 'react';
import ABSPanel from '../../../../components/ABSPanel';
import ABSList from '../../../../components/ABSList';
import { connect } from 'dva';
import ABSComment from '../../../../components/ABSComment';
import commonUtils from '../../../../utils/commonUtils';

export interface IComputationResultProps {
  // computationResultList: any[];
  analyzeDate: string;
  simulateDate: string;
  scenarioID: number;
  dealID: number;
}

class ComputationResult extends React.PureComponent<IComputationResultProps> {

  render() {
    const { dealID, analyzeDate, simulateDate, scenarioID } = this.props;
    const columnsData = [
      {
        title: '证券名称',
        key: 'name1',
        dataIndex: 'name1',
        render: text => <a href="javascript:;">{text}</a>,
      },
      {
        title: '损失',
        key: 'name2',
        dataIndex: 'name2',
        className: 'abs-right',
        render: (text) => commonUtils.formatContent(text, true, true)
      },
      {
        title: '内部收益',
        key: 'name3',
        dataIndex: 'name3',
        className: 'abs-right',
        render: (text) => commonUtils.formatContent(text, true, true)
      },
      {
        title: '净现值',
        key: 'name4',
        dataIndex: 'name4',
        className: 'abs-right',
        render: (text) => commonUtils.formatContent(text, true, true)
      },
      {
        title: 'WAL',
        key: 'name5',
        dataIndex: 'name5',
        className: 'abs-right',
        render: (text) => commonUtils.formatContent(text, true)
      },
      {
        title: '回收期(年)',
        key: 'name6',
        dataIndex: 'name6',
        className: 'abs-right',
        render: (text) => commonUtils.formatContent(text, true)
      },
      {
        title: '损失临界值',
        key: 'name7',
        dataIndex: 'name7',
        className: 'abs-right',
        render: (text) => commonUtils.formatContent(text, true, true)
      },
    ];
    return (
      <ABSPanel title="计算结果" removePadding={true} comment={this.renderTopRightView(simulateDate, analyzeDate)}>
        <ABSList
          columnsData={columnsData}
          actionType="product/getComputationResultList"
          payload={{ scenario_id: scenarioID, deal_id: dealID }}
          model="product.computationResultList"
        />
      </ABSPanel>
    );
  }

  renderTopRightView(simulateDate: string, analyzeDate: string) {
    if (!simulateDate && !analyzeDate) {
      return null;
    }
    return (
      <ABSComment>
        <span style={{ marginRight: '27px' }}>{`模拟日：${simulateDate}`}</span>
        <span>{`分析日：${analyzeDate}`}</span>
      </ABSComment>
    );
  }
}

function mapStateToProps({ product, global }: any) {
  const { simulateDate, analyzeDate, scenarioID } = product;
  return {
    // computationResultList,
    simulateDate,
    analyzeDate,
    scenarioID,
    dealID: global.params.dealID,
  };
}

export default connect(mapStateToProps)(ComputationResult);