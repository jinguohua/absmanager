import React from 'react';
import './index.less';
import { connect } from 'dva';
import { Tooltip } from 'antd';
import ABSLink from '../ABSLink';
import commonUtils from '../../utils/commonUtils';
import ABSLabelValueList from '../ABSLabelValueList';
import RouteConfig from '../../abs/config/routeConfig';

export interface IABSSecurityPageTitleProps {
  dispatch: any;
  pageHeader: any;
  securityID: number;
}

export interface IABSSecurityPageTitleState {
  bodyWidth: number;
}

class ABSSecurityPageTitle extends React.Component<IABSSecurityPageTitleProps, IABSSecurityPageTitleState> {

  constructor(props: any) {
    super(props);
    this.state = {
      bodyWidth: document.body.offsetWidth,
    };
  }

  componentDidMount() {
    const { securityID } = this.props;
    this.props.dispatch({ type: 'investment/getPageHeader', payload: { security_id: securityID } }).then((response) => {
      if (response.security_pricing_status) {
        this.props.dispatch({ type: 'investment/getPriceList', payload: { security_id: securityID } });
      }
    });
    window.addEventListener('resize', this.onHandleResize);
  }

  componentWillReceiveProps(nextProps: any) {
    if (nextProps.securityID !== null && nextProps.securityID !== this.props.securityID) {
      const { securityID } = nextProps;
      this.props.dispatch({ type: 'investment/getPageHeader', payload: { security_id: securityID } }).then((response) => {
        if (response.security_pricing_status) {
          this.props.dispatch({ type: 'investment/getPriceList', payload: { security_id: securityID } });
        }
      });
    }
  }

  componentWillUnmount() {
    window.removeEventListener('resize', this.onHandleResize);
  }

  renderTitle() {
    const { pageHeader } = this.props;
    const { security_short_name, security_code, rating, clean_price, dirty_price, deal_status } = pageHeader ? pageHeader : '';
    const numberList = deal_status === '已清算' ? '已清算' : `${commonUtils.formatContent(clean_price, null, null, null, 4)}/${commonUtils.formatContent(dirty_price, null, null, null, 4)}/${commonUtils.formatContent(pageHeader.yield, null, true, null, 4)}%`;
    return (
      <div className="abs-security-page-content-title">
        <p className="abs-security-page-content-title-name">{commonUtils.formatContent(security_short_name)}</p>
        <p className="abs-security-page-content-title-cl">{commonUtils.formatContent(security_code)}</p>
        <p className="abs-security-page-content-title-cl">{commonUtils.formatContent(rating)}</p>
        <ABSLink to={`${RouteConfig.investmentSecurityPricing}`}><p className="abs-security-page-content-title-number">{numberList}</p></ABSLink>
        <p className="abs-security-page-content-title-tips">{`（CNABS参考价）`}</p>
      </div>
    );
  }

  getData(title: string, content: string | React.ReactNode, contentStyle?: React.CSSProperties) {
    return [{
      title,
      content,
      contentStyle,
    }];
  }

  getBody() {
    const { pageHeader } = this.props;
    const { bodyWidth } = this.state;
    const numbers = Math.floor((bodyWidth - 750) / 185);
    const { originators } = pageHeader ? pageHeader : '';
    let data: string[] = [];
    if (originators) {
      originators.map((item) => {
        data.push(`<a target='_blank' href=${RouteConfig.organizationDetail}${item.id}>${item.full_name}</a>`);
      });
    }
    const title = data.join('<br/>');
    return originators ? originators.map((item, index) => {
      if (index < numbers) {
        return <ABSLink to={`${RouteConfig.organizationDetail}${item.id}`}>{index > 0 ? `、${item.full_name}` : `${item.full_name}`}</ABSLink>;
      }
      if (index === numbers) {
        return <div key={index}><ABSLink to={`${RouteConfig.organizationDetail}${item.id}`}>{index > 0 ? `、${item.full_name}` : `${item.full_name}`}</ABSLink><Tooltip placement="bottomRight" title={<div dangerouslySetInnerHTML={{ __html: title }} />}><a href={undefined}>{'...'}</a></Tooltip></div>;
      }
      return null;
    }) : '';
  }

  renderSubTitle() {
    const { pageHeader } = this.props;
    const { current_life, current_coupon, deal_type, deal_sub_category } = pageHeader ? pageHeader : '';
    return (
      <div className="abs-security-page-content-subtitle">
        <ABSLabelValueList list={this.getData('剩余期限', commonUtils.formatContent(current_life))} />
        <div className="abs-security-page-content-subtitle-margin" />
        <ABSLabelValueList list={this.getData('票面利率(%)', commonUtils.formatContent(current_coupon, null, true, null, 3))} />
        <div className="abs-security-page-content-subtitle-margin" />
        <ABSLabelValueList list={this.getData('产品类型', commonUtils.formatContent(deal_type))} />
        <div className="abs-security-page-content-subtitle-margin" />
        <ABSLabelValueList list={this.getData('产品细分', commonUtils.formatContent(deal_sub_category))} />
        <div className="abs-security-page-content-subtitle-margin" />
        <ABSLabelValueList list={this.getData('产品主体', <div className="abs-label-value-titles">{commonUtils.formatContent(this.getBody())}</div>)} />
        <div className="abs-security-page-content-subtitle-margin" />
      </div>
    );
  }

  render() {
    return (
      <div className="abs-security-page">
        <div className="abs-security-page-content">
          {this.renderTitle()}
          {this.renderSubTitle()}
        </div>
      </div>
    );
  }

  onHandleResize = () => {
    this.setState({ bodyWidth: document.body.offsetWidth });
  }
}

const mapStateToProps = ({ investment, global }) => {
  const { pageHeader } = investment;
  return {
    pageHeader,
    securityID: global.params.securityID,
  };
};

export default connect(mapStateToProps)(ABSSecurityPageTitle);