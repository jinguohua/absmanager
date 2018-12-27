import React from 'react';
import './index.less';

export interface IABSEmphasizeTextProps {
  title: string;
  style?: React.CSSProperties;
}
 
export interface IABSEmphasizeTextState {
  
}
 
class ABSEmphasizeText extends React.Component<IABSEmphasizeTextProps, IABSEmphasizeTextState> {
  render() { 
    const { style, title } = this.props;
    return <span className="abs-emphasize-text" style={{ ...style }}>{title}</span>;
  }
}
 
export default ABSEmphasizeText;