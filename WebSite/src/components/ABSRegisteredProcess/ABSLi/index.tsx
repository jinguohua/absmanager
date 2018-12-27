import React from 'react';
import './index.less';

export interface Iprops {
  text: string;
}
 
class ABSLi extends React.Component<Iprops, any> {
  render() {
    const { text } = this.props;
    return (
      <div className={'abs-ul'}>
        <div className="abs-ul-dot" />
        <div className="abs-ul-li">{text}</div>
      </div>
    );
  }
}
 
export default ABSLi;