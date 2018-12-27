import React from 'react';
import './index.less';
import { Alert } from 'antd';

export interface IABSAlertData {
  message?: string;
  type?: 'success' | 'info' | 'warning' | 'error';
}

export class ABSAlert extends React.Component<IABSAlertData, any> {
  render() {
    const { type, message } = this.props;
    return (
      <div className="abs-alert">
        <Alert 
          message={message} 
          type={type} 
          closable={true} 
          showIcon={true}
        />
      </div>
    );
  }
}
