import React from 'react';
import './index.less';

export interface IABSCommentProps {
  children: React.ReactNode|string;
}

export default class ABSComment extends React.Component<IABSCommentProps> {
  public render() {
    const {children} = this.props;
    return (
      <div className="abs-comment">
        {children}
      </div>
    );
  }
}
