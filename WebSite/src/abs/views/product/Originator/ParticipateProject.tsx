import React, { Component } from 'react';
import { connect } from 'dva';
// import { Link } from 'dva/router';
import './ParticipateProject.less';
import ABSPanel from '../../../../components/ABSPanel';
import commonUtils from '../../../../utils/commonUtils';
import ABSList from '../../../../components/ABSList';
import '../../../../components/ABSBubble/index.less';
import routeConfig from '../../../config/routeConfig';
import ABSLink from '../../../../components/ABSLink';
import { generateInnerHTML } from './utils';

export interface IParticipateProjectProps {
  dispatch?: any;
  loading?: boolean;
  organizationID?: any;
}
 
const columnsData = [
  {
    title: '产品简称',
    dataIndex: 'deal_name',
    key: 'deal_name',
    render: (text) => commonUtils.formatContent(text, true)
  }, {
    title: '交易场所',
    dataIndex: 'exchange',
    key: 'exchange',
    render: (text) => commonUtils.formatContent(text, true)
  }, {
    title: '产品分类',
    dataIndex: 'deal_type',
    key: 'deal_type',
    render: (text) => commonUtils.formatContent(text, true)
  }, {
    className: 'abs-right',
    title: '发行金额(亿)',
    key: 'total_offering',
    dataIndex: 'total_offering',
    render: (text) => commonUtils.formatContent(text, true, null, null, null, 8),
  }, {
    title: '起息日',
    dataIndex: 'closing_date',
    key: 'closing_date',
    render: (text) => commonUtils.formatContent(text, true)
  }, {
    title: '当前状态',
    dataIndex: 'current_status',
    key: 'current_status',
    render: (text) => commonUtils.formatContent(text, true)
  }, {
    title: '发行/管理人',
    dataIndex: 'deal_issuer',
    key: 'deal_issuer',
    render: (dealIssuerList) => {
      // dealIssuerList: ["广发证券股份有限公司", "广发证券资产管理（广东）有限公司"]
      const innerHTML = generateInnerHTML(dealIssuerList);
      return <div className="abs-multi-line-container" dangerouslySetInnerHTML={{__html: innerHTML }}/>;
    }
  }, {
    title: '主承销商',
    dataIndex: 'deal_lead_underwriter',
    key: 'deal_lead_underwriter',
    render: (dealIssuerList) => {
      const innerHTML = generateInnerHTML(dealIssuerList);
      return <div className="abs-multi-line-container" dangerouslySetInnerHTML={{__html: innerHTML }}/>;
    }
  }
];

class ParticipateProject extends Component<IParticipateProjectProps> {

  // isShowFirstOrder = (datas) => {
  //   datas.map((item) => {
  //     if (item.key === 'deal_name') {
  //       item.render = (text, record) => {
  //         if (record.is_first_deal === true) {
  //           if (record.first_deal_descript !== undefined || record.first_deal_descript !== '' || record.first_deal_descript.length > 0) {
  //             return this.isFirstDealDescript(text, record);
  //           }
  //           return this.showImageText(text, record);
  //         } else {
  //           return this.showText(text, record);
  //         }
  //       };
  //     }
  //   });
  //   return datas;
  // }

  // isFirstDealDescript = (text, record) => {
  //   return (
  //       <div>
  //         <ABSLink to={routeConfig.productDealInfo + '?deal_id=' + record.deal_id} className="abs-firstOrder" target="_blank">
  //           {text}
  //         </ABSLink>
  //         <div className="abs-expert-first-deal-img">
  //           <Tooltip title={record.first_deal_description} autoAdjustOverflow={true}  placement="topLeft">
  //             <div className="abs-tooltip">
  //             <ABSLink to={routeConfig.firstDealRank} className="abs-firstOrder" target="_blank">
  //               <ABSImage logo={logo} width={18} height={22} />
  //             </ABSLink>
  //             </div>
  //           </Tooltip>
  //         </div>
  //       </div>
  //   );
  // }

  // showImageText = (text, record) => {
  //   return (
  //       <div>
  //         <ABSLink to={routeConfig.productDealInfo + '?deal_id=' + record.deal_id} className="abs-firstOrder" target="_blank">
  //           {text}
  //         </ABSLink>
  //         <div className="abs-expert-first-deal-img">
  //           <ABSLink to={routeConfig.firstDealRank} className="abs-firstOrder" target="_blank">
  //             <ABSImage logo={logo} width={18} height={22} />
  //           </ABSLink>
  //         </div>
  //       </div>
  //   );
  // }

  showText = (text, record) => {
    return (
      <ABSLink to={routeConfig.productDealInfo + '?deal_id=' + record.deal_id} target="_blank">
        {text}
      </ABSLink>
    );
  }

  render() {
    const { organizationID } = this.props;
    return (
      <div className="participate-project">
        <ABSPanel title="参与项目" removePadding={true}>
          <div className="abs-participate-project">
            <ABSList
              actionType="product/getParticipateProject"
              payload={{ organization_id: organizationID }}
               columnsData={columnsData}
              model="product.participateProject"
            />
          </div>
        </ABSPanel>
      </div>
    );
  }
}

function mapStateToProps({ global }: any) {
  return { organizationID: global.params.organizationID };
}

export default connect(mapStateToProps)(ParticipateProject); 