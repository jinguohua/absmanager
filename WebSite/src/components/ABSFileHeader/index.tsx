import React from 'react';
import './index.less';
import ABSIconText from '../ABSIconText';
import ABSAvatar from '../ABSAvatar';

export interface IABSNewsDetailsProps {
  title?: any;
  publishDate?: any;
  source?: string;
  avatar?: any;
  isRead?: boolean;
  onRead?: () => void;
  onDelete?: () => void;
}

export interface IABSNewsDetailsState {
}

class ABSFileHeader extends React.Component<IABSNewsDetailsProps, IABSNewsDetailsState> {
  static defaultProps = {
    isRead: true,
  };

  renderDelete() {
    const { onRead, onDelete } = this.props;
    if (onRead) {
      return (
        <>
          <div className="action-button-line" />
          <ABSIconText icon="delete" text="删除" onClick={onDelete} />
        </>
      );
    } else {
      return (
        <>
          <ABSIconText icon="delete" text="删除" onClick={onDelete} />
        </>
      );
    }

  }

  rightrender() {
    const { onRead, onDelete, isRead } = this.props;
    const iconText = {
      icon: isRead ? 'eyes-closed' : 'eyes-open',
      text: isRead ? '标位未读' : '标位已读',
    };
    return (
      <div className="hearder-text-right">
        {onRead ? <ABSIconText isAnt={false} {...iconText} onClick={onRead} /> : null}
        {onDelete ? this.renderDelete() : null}
      </div>
    );
  }

  getAvatar() {
    const { avatar } = this.props;
    if (avatar === undefined) {
      return null;
    }
    return (
      <div className="header-avatar" >
        <ABSAvatar size="small" alt="" imageUrl={avatar} />
      </div>
    );
  }

  render() {
    const { publishDate, title, source } = this.props;
    return (
      <div className="all-view">
        <div className="hearder-view">
          <p className="hearder-text-left">{title}</p>

          {this.rightrender()}
        </div>
        <div className="adress-time">
          {this.getAvatar()}
          {`来源 ：${source} | ${publishDate}`}
        </div>
      </div>
    );
  }
}

export default (ABSFileHeader);