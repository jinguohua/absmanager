import React from 'react';
import { connect } from 'dva';
import './index.less';

interface IABSFooterProps {
  version?: string;
}

class ABSFooter extends React.Component<IABSFooterProps> {
  render() { 
    // const { version } = this.props;
    const version = '2.0.0';
    return (
      <div className="abs-footer">
        Copyright 2014 - 2018 沪ICP备15008941号  上海和逸金融信息服务有限公司版权所有   版本号：{version}
      </div>
    );
  }
}
 
const mapStateToProps = ({ global }) => {
  return { version: global.version };
};

export default connect(mapStateToProps)(ABSFooter);