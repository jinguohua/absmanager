import React from 'react';
import './index.less';
import { Upload, Modal } from 'antd';
import { ABSAntIcon } from '../ABSAntIcon';
import { GlobalApi } from '../../abs/config/api';

export class ABSUploadCard extends React.Component<any, any> {
  state = {
    previewVisible: false,
    previewImage: '',
    fileList: [],
  };

  handleCancel = () => this.setState({ previewVisible: false });

  handlePreview = (file) => {
    this.setState({
      previewImage: file.url || file.thumbUrl,
      previewVisible: true,
    });
  }

  handleChange = (response) => {
    const { file, fileList } = response;
    if (file.status === 'done') {
      this.props.updateFileCode(file.response[0].FileCode);
    }
    this.setState({ fileList });
  }

  render() {
    const { previewVisible, previewImage, fileList } = this.state;
    const uploadButton = (
      <div>
        <ABSAntIcon type="plus" />
        <div className="ant-upload-text">点击上传名片</div>
      </div>
    );
    return (
      <div className="abs-upload-div">
        <Upload
          action={GlobalApi.uploadUrl}
          listType="picture-card"
          onPreview={this.handlePreview}
          onChange={this.handleChange}
        >
          {fileList.length >= 1 ? null : uploadButton}
        </Upload>
        <Modal visible={previewVisible} footer={null} onCancel={this.handleCancel}>
          <img alt="example" style={{ width: '100%' }} src={previewImage} />
        </Modal>
      </div>
    );
  }
}