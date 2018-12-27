import React from 'react';
import './index.less';
import { connect } from 'dva';
import { Tooltip } from 'antd';
import ABSLink from '../ABSLink';
import ABSImage from '../ABSImage';
import commonUtils from '../../utils/commonUtils';
import ABSLabelValueList from '../ABSLabelValueList';

export interface IABSORganizationPageTitleProps {
  dispatch: ({ }: any) => void;
  organizationDetail: any;
}

export interface IABSORganizationPageTitleState {

}

class ABSORganizationPageTitle extends React.Component<IABSORganizationPageTitleProps, IABSORganizationPageTitleState> {

  renderPrizes() {
    const { organizationDetail } = this.props;
    const { prize_models } = organizationDetail;
    if (!prize_models) {
      return null;
    }
    const data: Array<any> = [];
    prize_models.map((item, index) => {
      const { name, description_page, icon_url, tooltip } = item;
      const url = description_page;
      data.push(
        <div className="prizes" key={index}>
          <Tooltip placement="bottomLeft" title={<div dangerouslySetInnerHTML={{ __html: '<span class="prizes-tooltip-title">' + name + '</span>' + tooltip }} />}>
            <a href={url} target="_blank">
              <ABSImage className="img-title" style={{ marginLeft: 6 }} logo={icon_url} width={24} height={28} />
            </a>
          </Tooltip>
        </div>);
    });
    return [...data];
  }

  renderIconAndName() {
    const { organizationDetail } = this.props;
    const { info } = organizationDetail;
    const { logo_file_url = '', full_name, short_name } = info ? info : '';
    return (
      <div className="abs-page-head">
        { 
          logo_file_url === null ? null :
          <ABSImage className="abs-organization-logo" logo={logo_file_url} width={142} height={50} />
        }
        <div className="abs-page-content">
          <div className="abs-page-content-head">
            <h2 className="abs-page-content-title">{full_name}</h2>
            {this.renderPrizes()}
          </div>
          <p className="abs-page-content-subtitle">{short_name}</p>
        </div>
      </div>
    );
  }

  renderContect() {
    const { organizationDetail } = this.props;
    const { info } = organizationDetail;
    const { contact = '' } = info ? info : '';
    const item = [{ title: '联系方式', content: commonUtils.formatContent(contact, false, false, false, 0, 0) }];
    return (
      <ABSLabelValueList list={item} />
    );
  }

  renderLink() {
    const { organizationDetail } = this.props;
    const { info } = organizationDetail;
    const { website } = info ? info : '';
    const item = [{ title: '机构网址', content: website ? <ABSLink target="_blank" to={website} >{commonUtils.formatContent(website)}</ABSLink> : <span style={{display: 'inline-block', minWidth: 250}}>{commonUtils.formatContent(website)}</span> }];
    return (
      <ABSLabelValueList list={item} />
    );
  }

  render() {
    return (
      <div className="abs-organization">
        {this.renderIconAndName()}
        {this.renderContect()}
        {this.renderLink()}
      </div>
    );
  }
}

const mapStateToProps = state => {
  const { organization } = state;
  const { organizationDetail } = organization;
  return { organizationDetail };
};

export default connect(mapStateToProps)(ABSORganizationPageTitle);