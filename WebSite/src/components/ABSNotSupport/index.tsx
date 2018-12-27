import React from 'react';
import ABSParagraph from '../ABSParagraph';

export interface IABSNotSupportProps {
  message?: string;
}
 
class ABSNotSupport extends React.Component<IABSNotSupportProps> {
  static defaultProps = {
    message: '',
  };
  
  render() { 
    const { message } = this.props;
    return (
      <ABSParagraph style={{ margin: 26 }}>
        {message}，如有需要请联系我们： 电话：021-31156258；邮箱 feedback@cn-abs.com
      </ABSParagraph>
    );
  }
}
 
export default ABSNotSupport;
