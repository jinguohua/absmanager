import React from 'react';
import { connect } from 'dva';
import './index.less';
// import { ABSButton } from '../ABSButton';
import commonUtils from '../../utils/commonUtils';

export interface IABSProductPageTitleProps {
  fullname: string;
  shortname: string;
  // buttons?: Array<React.ReactNode>;
  style?: React.CSSProperties;
  dispatch: any;
  product: any;
  dealID: number;
}

class ABSProductPageTitle extends React.Component<IABSProductPageTitleProps> {
  componentDidMount() {
    const id = this.getID();
    this.props.dispatch({
      type: 'project/getProjectBaseInfo',
      payload: { id: id }
    });
  }

  getID() {
    const { id = null } = commonUtils.getParams();
    return id;
  }

  render() {
    const { fullname, shortname, style } = this.props;
    return (
      <div className="abs-product-page-title" style={style}>
        <div className="abs-product-page-content">
          <h2 className="abs-product-page-content-title">{fullname || '-'}</h2>
          <p className="abs-product-page-content-subtitle">
            {shortname || '-'}
          </p>
        </div>
        <div className="abs-product-page-buttons" />
      </div>
    );
  }
}

const mapStateToProps = ({ project: { current } }) => ({
  fullname: current.fullName || '-',
  shortname: current.shortName || '-'
});
export default connect(mapStateToProps)(ABSProductPageTitle);
