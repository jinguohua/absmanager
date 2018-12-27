import React from 'react';
import { Avatar } from 'antd';
import './index.less';

export declare type avatarSize = 'large' | 'small' | 'default' | 100;
export declare type avatarShape = 'circle' | 'square';

export interface IABSAvatarProps {
  size?: avatarSize;
  shape?: avatarShape;
  alt: string;
  imageUrl: string;
  className?: string;
}
 
export default class ABSAvatar extends React.Component<IABSAvatarProps> {
  static defaultProps = {
    size: 'large',
    shape: 'circle',
  };
  render() { 
    const { size, shape, imageUrl } = this.props;
    return (
      <div className="abs-avatar">
          <Avatar 
            icon="user" 
            className={`abs-avatar-icon-${size}`} 
            size={size} 
            shape={shape} 
            src={imageUrl} 
          />
      </div>
    );
  }
}