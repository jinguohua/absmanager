import React, { Component } from 'react';
import ABSUpload from '../../../../components/ABSUpload';

class ABSUIUpload extends Component {

  render() {
    return (
      <div>
        <ABSUpload desc="（*文件大小在20M以内）"/>
        <div style={{ height: 20 }} />
        <ABSUpload isSingle={true} fileInfo={{ name: 'This is a demo for test.doc', size: 10 }} />
      </div>
    );
  }
}

export default ABSUIUpload;