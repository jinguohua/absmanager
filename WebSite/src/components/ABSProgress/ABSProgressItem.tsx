import React from 'react';
import './absprogressItem.less';
import classNames from 'classnames';
import ABSDot from './ABSDot';
import ABSProgressItemLine from './ABSProgressItemLine';
import { getTopInactiveRatio, getBottomInactiveRatio } from './util';

export interface IProgressNode {
  date: string;
  title: string;
  time: number;
}

export interface IABSProgressItemProps {
  prev: IProgressNode | null;
  current: IProgressNode;
  next: IProgressNode | null;
}
 
export interface IABSProgressItemState {
  
}
 
class ABSProgressItem extends React.Component<IABSProgressItemProps, IABSProgressItemState> {

  render() {
    const { current } = this.props;
    if (!current) {
      return null;
    }
    const { date, title } = current;
    const currentTime = current && current.time ? current.time : 0;
    const active = currentTime > Date.now();
    const triangleStyle = classNames('progress-item-middle-triangle-left', active ? 'progress-item-middle-triangle-left-active' : 'progress-item-middle-triangle-left-inactive');
    const progressItemRightStyle = classNames('progress-item-right-bubble', active ? 'progress-item-right-bubble-active' : 'progress-item-right-bubble-inactive');
    const progressItemDateStyle = classNames('progress-item-right-bubble-date', active ? 'progress-item-right-bubble-date-active' : 'progress-item-right-bubble-date-inactive');
    return (
      <div className="progress-item">
        {this.renderLeftView(active)}
        <div className="progress-item-middle">
          <div className={triangleStyle}/>
        </div>
        <div className={progressItemRightStyle}>
          <div className="progress-item-right-bubble-content">
            <p className={progressItemDateStyle}>{date}</p>
            <p className="progress-item-right-bubble-title">{title}</p>
          </div>
        </div>
      </div>
    );
  }

  renderLeftView(active: boolean) {
    return (
      <div className="progress-item-left">
        {this.renderTopLine()}
        <ABSDot active={active} />
        {this.renderBottomLine()}
      </div>
    );
  }

  renderTopLine() {
    const { prev, current } = this.props;
    let hide = !prev;
    const currentTime = current.time;
    const prevTime = prev ? prev.time : 0;
    const inactiveRatio = getTopInactiveRatio(currentTime, prevTime);
    return <ABSProgressItemLine hide={hide} inactiveRatio={inactiveRatio} />;
  }

  renderBottomLine() {
    const { next, current } = this.props;
    let hide = !next;
    const currentTime = current.time;
    const nextTime = next ? next.time : 0;
    const inactiveRatio = getBottomInactiveRatio(currentTime, nextTime);
    return <ABSProgressItemLine hide={hide} inactiveRatio={inactiveRatio} />;
  }
}
 
export default ABSProgressItem;