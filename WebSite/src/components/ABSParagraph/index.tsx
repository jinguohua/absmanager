import React from 'react';
import classNames from 'classnames';
import './index.less';

export interface IABSParagraphProps {
  className?: string;
  children: React.ReactNode | string;
  style?: React.CSSProperties;
}
 
class ABSParagraph extends React.Component<IABSParagraphProps> {
  render() {
    const { className, children, style } = this.props;
    const classes = classNames('abs-paragraph', className);
    if (typeof children === 'string') {
      return (
        <p className={classes} dangerouslySetInnerHTML={{__html: children }} style={style} />
      );
    }
    return (
      <div className={classes} style={style}>{children}</div>
    );
  }
}
 
export default ABSParagraph;