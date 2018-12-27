import React from 'react';
import './ABSWrapLabel.less';
import { ABSLabelValueData } from './ABSLabelValue';

export interface IABSLabelValueProps {
  item: ABSLabelValueData;
  labelValueTitleMinWidth: number;
}

class ABSLabelValue extends React.Component<IABSLabelValueProps> {

  renderContent() {
    const { item } = this.props;
    const { contentStyle } = item;
    let { content } = item;
    content = content === undefined ? null : content;
    if (typeof content === 'string') {
      return <p className="abs-wrap-label-value-content" dangerouslySetInnerHTML={{ __html: content }} style={contentStyle} />;
    }
    return <p className="abs-wrap-label-value-content" style={contentStyle}>{content}</p>;
  }

  renderContents() {
    const { item } = this.props;
    const { contentStyle } = item;
    let { content } = item;
    content = content === undefined ? null : content;
    if (typeof content === 'string') {
      return <p className="abs-wrap-label-value-contents" dangerouslySetInnerHTML={{ __html: content }} style={contentStyle} />;
    }
    return <div className="abs-wrap-label-value-wrap">{content}</div>;
  }

  render() {
    const { item, labelValueTitleMinWidth } = this.props;
    const content = item && item.content;
    if (!content) {
      return <div />;
    }
    if (String(content).length > 15 || typeof content !== 'string') {
      if (!item.isWrap) {
        return (
          <div>
            <p className="abs-wrap-label-value-title" style={{ minWidth: labelValueTitleMinWidth }} >{item.title}</p>
            {this.renderContent()}
          </div>
        );
      }
    }
    return (
      <div className="abs-wrap-label-value">
        <p className="abs-wrap-label-value-titles">{item.title}</p>
        {this.renderContents()}
      </div>
    );
  }
}

export default ABSLabelValue;