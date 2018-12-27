import React, { Component } from 'react';
import '../../../../styles/coverAnt.less';
import { ABSUploadAvatar } from '../../../../components/ABSUploadAvatar';

class ABSUploadAvatarRoute extends Component<any, any> {

  handleFileCode = fileCode => {
    // console.log('fileCode: ', fileCode);
  }

  render() {
    return (
      <div>
        <ABSUploadAvatar updateFileCode={this.handleFileCode}/>
        <ABSUploadAvatar updateFileCode={this.handleFileCode} url="http://10.1.1.35:8000/filestore/common/downloadimg/cnabs/d29af383-5cb0-c983-68a0-08d56f9683d7/s" />
      </div>
    );
  }
}

export default ABSUploadAvatarRoute;