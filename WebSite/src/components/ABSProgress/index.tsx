import React from 'react';
import './index.less';
import ABSProgressItem, { IProgressNode } from './ABSProgressItem';
import classNames from 'classnames';

export interface IABSProgressProps {
  className?: string;
  dataSource: IProgressNode[];
}
 
export interface IABSProgressState {
  
}
 
class ABSProgress extends React.Component<IABSProgressProps, IABSProgressState> {
  render() {
    const { className } = this.props;
    const progressStyle = classNames('abs-progress', className);
    return (
      <div className={progressStyle}>
        {this.renderTimelineItems()}
      </div>
    );
  }

  renderTimelineItems() {
    const { dataSource } = this.props;
    if (!Array.isArray(dataSource)) {
      return null;
    }
    const items = dataSource.map((value, index, array) => {
      const prev = index > 0 ? array[index - 1] : null;
      const next = index < array.length - 1 ? array[index + 1] : null;
      return <ABSProgressItem key={index} current={value} prev={prev} next={next} />;
    });
    return items;
  }
}
 
export default ABSProgress;