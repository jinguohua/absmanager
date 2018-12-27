import React from 'react';

export interface ISectionTitleProps {
  title: string;
  className?: string;
}
 
export interface ISectionTitleState {
  
}
 
class SectionTitle extends React.Component<ISectionTitleProps, ISectionTitleState> {
  render() { 
    const { title, className } = this.props;
    const text = title ? title : '';
    return (
      <div className={className}>
        <p>{text}</p>
      </div>
    );
  }
}
 
export default SectionTitle;