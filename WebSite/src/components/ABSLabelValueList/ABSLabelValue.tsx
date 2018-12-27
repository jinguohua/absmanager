import React from 'react';
import classNames from 'classnames';
import './ABSLabelValue.less';

// tslint:disable-next-line:interface-name
export interface ABSLabelValueData {
  title: string;
  isWrap?: boolean;
  content: string | React.ReactNode;
  contentStyle?: React.CSSProperties;
}

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
      return <p className="abs-label-value-content" dangerouslySetInnerHTML={{__html: content }} style={contentStyle} />;
    }
    return <p className="abs-label-value-content" style={contentStyle}>{content}</p>;
  }

  render() {
    const { item, labelValueTitleMinWidth } = this.props;
    const content = item && item.content;
    const isMultiLines = typeof content === 'string' && /<br>|<br\/>|<br \/>/.test(content as string);
    const classes = classNames('abs-label-value', {
      'abs-label-value-multi-line': isMultiLines,
    });
    return (
      <div className={classes}>
        <p className="abs-label-value-title" style={{ minWidth: labelValueTitleMinWidth }} >{item.title}</p>
        {this.renderContent()}
      </div>
    );
  }
}

export default ABSLabelValue;