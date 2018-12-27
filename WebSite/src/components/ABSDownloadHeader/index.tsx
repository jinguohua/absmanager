import React from 'react';
import './index.less';
import { ABSButton } from '../ABSButton';

export interface IHeaderProps {
  style?: React.CSSProperties;
  title: string;
  actionType: any;
  payload: any; 
  dispatch: ({ }: any) => void;
}

class ABSDownloadHeader extends React.Component<any> {
  downloadButton = (e) => {
    const { actionType, payload, dispatch } = this.props;
    dispatch({
      type: actionType,
      payload: payload,
    });
  }

  render() {
    const { style, title } = this.props;
    return (
      <div className="abs-download-header" style={style}>
        <div className="abs-download-header-content">
          <h2 className="abs-download-header-content-title">{title}</h2>
        </div>
        <div className="abs-download-header-buttons">
          <ABSButton icon="download" style={{ marginRight: 20 }} onClick={this.downloadButton}>{'下载'}</ABSButton>
        </div>
      </div>
    );
  }
}

export default ABSDownloadHeader;