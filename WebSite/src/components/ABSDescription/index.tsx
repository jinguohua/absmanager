import React from 'react';
import './index.less';

export interface IABSDescriptionProps {
  children: React.ReactNode;
  style?: React.CSSProperties;
}
 
class ABSDescription extends React.Component<IABSDescriptionProps> {
  render() { 
    const { children, style } = this.props;
    return (
      <h2 className="abs-description" style={style}>
        {children}
      </h2>
    );
  }
}
 
export default ABSDescription;