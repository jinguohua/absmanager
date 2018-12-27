import React from 'react';
import ABSPanel from '../../../../components/ABSPanel';
// import ABSList from '../../../../components/ABSList';
import ABSImgae from '../../../../components/ABSImage';
import ABSContainer from '../../../../components/ABSContainer';
import { connect } from 'dva';
import { cashflowChart } from './jsplumb';
import './index.less';
import ABSMinContainer from '../../../../components/ABSMinContainer';
import ABSNoContent from '../../../../components/ABSNoContent';
import CommonUtils from '../../../../utils/commonUtils';
import ABSLoading from '../../../../components/ABSLoading';

export interface IABSProductStructureProps {
  structureImage: any;
  dispatch: any;
  // triggerEventList: any;
  dealID: string;
  paymentModel: object;
  loading?: boolean;
}

export interface IABSProductStructureState {
  columnsData: Array<any>;
}

class Structure extends React.Component<IABSProductStructureProps, IABSProductStructureState> {
  constructor(props: IABSProductStructureProps) {
    super(props);
    this.state = {
      columnsData: [
        {
          title: '',
          dataIndex: 'name',
          key: 'name',
          name: 'name',
        }, {
          className: 'abs-left',
          title: '当前产品',
          dataIndex: 'is_has_current_product',
          key: 'is_has_current_product',
          is_has_current_product: 'is_has_current_product',
          render: (text) => {
            if (text === true) {
              return '有';
            } else {
              return '无';
            }
          },
        }, {
          className: 'abs-right',
          title: '同类产品',
          dataIndex: 'similar_product_percent',
          key: 'similar_product_percent',
          similar_product_percent: 'similar_product_percent',
          render: (text) => {
            return CommonUtils.formatContent(text, true, true);
          },
        }, {
          className: 'abs-left',
          title: '触发条件',
          key: 'trigger_condition',
          dataIndex: 'trigger_condition',
          trigger_condition: 'trigger_condition',
          width: 100
        }, {
          className: 'abs-right',
          title: '当前值',
          dataIndex: 'current',
          key: 'current',
          current: 'current',
        }, {
          className: 'abs-left',
          title: '结果',
          dataIndex: 'result',
          key: 'result',
          result: 'result',
          render: (text) => {
            if (text === true) {
              return '通过';
            } else {
              return '失败';
            }
          },
        },
      ],
    };
  }

  componentDidMount() {
    const { dealID } = this.props;

    this.props.dispatch({
      type: 'product/getStructureImage',
      payload: { deal_id: dealID },
    });

    this.props.dispatch({
      type: 'product/getPaymentModel',
      payload: { deal_id: dealID },
    });
  }

  componentDidUpdate(prevProps: any) {
    const { paymentModel } = this.props;
    if (paymentModel !== prevProps.paymentModel && paymentModel !== null) {
      cashflowChart(paymentModel);
    }
  }

  absImage() {
    const { structureImage, loading } = this.props;
    if (loading) {
      return <ABSLoading />;
    }
    // render得人时候接口未返回 给默认图
    // const imageurls = structureImage ? `${url}${(structureImage.img_url).substring(2)}` : imageurl;
    if (!structureImage || structureImage.img_url == null) {
      return <ABSNoContent />;
    }

    const structureImageUrl = require(`../../../../assets/images/dealStructure/${structureImage.img_url}`);

    return (
      <ABSPanel title="交易结构" >
        <ABSImgae logo={structureImageUrl} style={{ width: '85%' }} />
      </ABSPanel>
    );
  }

  render() {
    // const { columnsData } = this.state;
    // const { dealID } = this.props;
    const { paymentModel } = this.props;
    return (
      <ABSContainer>
        <ABSMinContainer>
          {this.absImage()}
          {/* <ABSPanel title="触发事件" removePadding={true} comment={<span className="abs-hasStar">同类产品:  基础资产类型中产品拥有该结构设计的占比</span>}>
            <ABSList
              actionType="product/getTriggerEventList"
              payload={{ deal_id: dealID }}
              columnsData={columnsData}
              model="product.triggerEventList.trigger_events"
            />
          </ABSPanel> */}
          {paymentModel ?
            <ABSPanel title="偿付模型">
              <div id="waterfall" />
            </ABSPanel>
            :
            null
          }
        </ABSMinContainer>
      </ABSContainer>
    );
  }
}

function mapStateToProps({ product, global, loading }: any) {
  return {
    structureImage: product.structureImage,
    // triggerEventList: product.triggerEventList,
    dealID: global.params.dealID,
    paymentModel: product.paymentModel,
    loading: loading.models.product
  };
}

export default connect(mapStateToProps)(Structure);
