import React from 'react';
import './index.less';
import ABSLabelValue, { ABSLabelValueData } from './ABSLabelValue';
import ABSWrapLabel from './ABSWrapLabel';
import { findDOMNode } from 'react-dom';

export interface IABSLabelValueListProps {
  list: Array<ABSLabelValueData>;
  isWrap?: boolean;
}

export interface IABSLabelValueListStates {
  labelValueTitleMinWidth: number;
}

class ABSLabelValueList extends React.Component<IABSLabelValueListProps, IABSLabelValueListStates> {

  labelValueList;

  constructor(props: IABSLabelValueListProps) {
    super(props);
    this.state = {
      labelValueTitleMinWidth: 0,
    };
  }

  componentDidMount() {
    const dom = findDOMNode(this.labelValueList) as Element;
    const labelValueListDom = dom.children; // HTMLCollection(4) [div.abs-label-value, div.abs-label-value, div.abs-label-value, div.abs-label-value]
    const labelValueWidths = [].slice.call(labelValueListDom).map((labelValue) => labelValue.childNodes[0].offsetWidth); // [100, 121, 100, 100];
    const maxLabelValueWidth = Math.max.apply(null, labelValueWidths); // 121

    this.setState({
      labelValueTitleMinWidth: maxLabelValueWidth,
    });
  }

  renderList() {
    const { list, isWrap } = this.props;
    const { labelValueTitleMinWidth } = this.state;
    return list.map((item, index) => {
      if (isWrap) {
        return <div key={index}><ABSWrapLabel item={item} labelValueTitleMinWidth={labelValueTitleMinWidth} key={index} /></div>;
      }
      return <ABSLabelValue item={item} labelValueTitleMinWidth={labelValueTitleMinWidth} key={index} />;
    }
    );
  }

  render() {
    return (
      <div className="abs-label-value-list" ref={view => this.labelValueList = view}>
        {this.renderList()}
      </div>
    );
  }
}

export default ABSLabelValueList;