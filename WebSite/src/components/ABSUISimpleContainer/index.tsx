import React from 'react';
import './index.less';

export interface IABSUISimpleContainerProps {
  children: React.ReactNode;
  style?: React.CSSProperties;
}
 
class ABSUISimpleContainer extends React.Component<IABSUISimpleContainerProps> {
  render() { 
    const { children, style } = this.props;
    return (
      <div className="absui-simple-container" style={style}>{children}</div>
    );
  }
}
 
export default ABSUISimpleContainer;