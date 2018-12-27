import React from 'react';
import './index.less';
import classnames from 'classnames';

export interface IABSPageTitleProps {
  style?: React.CSSProperties;
  className?: any;
  title: string;
  subTitle?: string;
  renderRight?: any;
}

class ABSPageTitle extends React.Component<IABSPageTitleProps> {

  renderSubTitle() {
    const { subTitle } = this.props;
    if (subTitle) {
      return <p className="abs-header-page-content-subtitle">{subTitle}</p>;
    }
    return null;
  }

  render() {
    const { style, className, title, renderRight } = this.props;
    const clazs = classnames('abs-header-page', className);
    return (
      <div className={clazs} style={style}>
        <div className="abs-header-page-content">
          <h2 className="abs-header-page-content-title">{title}</h2>
          {this.renderSubTitle()}
        </div>
        {renderRight}
      </div>
    );
  }
}

export default ABSPageTitle;