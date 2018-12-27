import React from 'react';
import classNames from 'classnames';
import ABSComment from '../ABSComment';
import './index.less';
import ABSLoading from '../ABSLoading';

export interface IABSPanelProps {
  title: string;
  // 是否去除内边距
  removePadding?: boolean;
  className?: string;
  style?: React.CSSProperties;
  comment?: React.ReactNode|string;
  // 是否正在加载
  loading?: boolean;
}
 
class ABSPanel extends React.Component<IABSPanelProps> {
  static defaultProps = {
    removePadding: false,
    id: 'abs-panel',
  };

  renderLoading() {
    return <ABSLoading size="large" />;
  }

  renderContent() {
    const { loading, children } = this.props;
    if (loading) {
      return this.renderLoading();
    }
    return children;
  }

  render() {
    const { title, style, className, removePadding, comment, loading } = this.props;
    const classes = classNames('abs-panel', className, {
      'abs-panel-no-padding': removePadding,
      'abs-panel-loading': loading,
    });
    
    return (
      <div className={classes} style={{ ...style }}>
        <div className="abs-panel-header">
          <div className="abs-panel-title">
            {title}
          </div>
          {comment ? <ABSComment>{comment}</ABSComment> : null}
        </div>
        <div className="abs-panel-content">
          {this.renderContent()}
        </div>
      </div>
    );
  }
}
 
export default ABSPanel;