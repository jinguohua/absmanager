import React from 'react';
import './index.less';
import ABSProcess from './ABSProcess';

export interface IABSRegisteredProcessProps {
  className?: string;
}
 
export interface IABSRegisteredProcessState {
  
}
 
class ABSRegisteredProcess extends React.Component<IABSRegisteredProcessProps, IABSRegisteredProcessState> {
  render() {
    const { className } = this.props;
    return (
      <div className={`abs-registered-process ${className}`}>
        <div className="abs-registered-process-section">
          <ABSProcess className="abs-registered-process-space" title="上传名片" description="JPEG,BMP,PNG" icon={'abs-process-icon'} />
          <ABSProcess className="abs-registered-process-space" title="审核阶段" description="3个工作日" />
          <ABSProcess title="开通账号" description="邮件形式通知" />
        </div>
      </div>
    );
  }
}
 
export default ABSRegisteredProcess;