import React from 'react';
import './index.less';
import { Upload } from 'antd';
import { GlobalApi } from '../../abs/config/api';
const defaultAvatar = require('../../assets/images/default-avatar.png');

interface IABSUploadAvatarProps {
  updateFileCode: (fileCode: string) => void;
  url?: string;
}

export class ABSUploadAvatar extends React.Component<IABSUploadAvatarProps, any> {
  constructor(props: any) {
    super(props);
    const { url } = props;
    this.state = {
      previewVisible: false,
      previewImage: '',
      imageUrl: url && url !== defaultAvatar ? url : defaultAvatar,
    };
  }

  componentWillReceiveProps(nextProps: IABSUploadAvatarProps) {
    if (nextProps.url) {
      this.setState({
        imageUrl: nextProps.url,
      });
    }
  }

  getBase64(img: any, callback: (imageUrl: any) => void) {
    const reader = new FileReader();
    reader.addEventListener('load', () => callback(reader.result));
    reader.readAsDataURL(img);
  }

  handleChange = (response) => {
    const { updateFileCode } = this.props;
    const { file } = response;
    if (file.status === 'done') {

      this.getBase64(file.originFileObj, imageUrl => this.setState({
        imageUrl,
      }));
      updateFileCode(file.response[0].FileCode);
    }
  }

  renderImage() {
    let { imageUrl } = this.state;

    // imageUrl = imageUrl ? imageUrl : defaultAvatar;
    const uploadText = imageUrl !== defaultAvatar ? '修改头像' : '上传头像';
    const UploadButton = (
      <div className="ant-upload-text">{uploadText}</div>
    );
    return (
      <div className="abs-upload-preview">
        <img src={imageUrl} className="abs-upload-preview-image" alt="avatar" />
        {UploadButton}
      </div>
    );
  }

  render() {
    return (
      <div className="abs-mask">
        <Upload
          name="avatar"
          className="avatar-uploader"
          action={GlobalApi.uploadImgUrl}
          listType="picture-card"
          onChange={this.handleChange}
          showUploadList={false}
        >
          {this.renderImage()}
        </Upload>
      </div>
    );
  }
}