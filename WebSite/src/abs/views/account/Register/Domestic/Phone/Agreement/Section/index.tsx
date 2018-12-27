import * as React from 'react';
import './index.less';

interface IProps {
  title?: string;
  content: string;
}

export default class Section extends React.Component<IProps, any> {
  render() {
    const {title, content} = this.props;

    return (
      <div className="abs-agreement-section">
        <div className="abs-agreement-section-title">
          {title}
        </div>
        <div className="abs-agreement-section-content">
          {content}
        </div>
      </div>
    );
  }
}