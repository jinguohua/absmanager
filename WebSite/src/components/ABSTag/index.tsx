import React from 'react';
import { Tag } from 'antd';
import './index.less';

export interface IABSTagProps {
  size?: string;
  color: string;
  content: string;
  visible?: boolean;
}

export class ABSTag extends React.Component<IABSTagProps> {
  render() {
    const { size, color, content, visible } = this.props;
    return (
      <div className="abs-tag">
        <Tag
          color={color}
          visible={visible}
          className={`abs-tag-${size}`}
        >
          {content}
        </Tag>
      </div>
    );
  }
}
