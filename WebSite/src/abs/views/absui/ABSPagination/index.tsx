import React, { Component } from 'react';
import '../../../../styles/coverAnt.less';
import ABSPaginationView from '../../../../components/ABSPagination/ABSPaginationView';
import ABSDescription from '../../../../components/ABSDescription';

interface IABSUIPaginationState {
  current: number;
  morePageCurrent: number;
}

class ABSUIPagination extends Component<any, IABSUIPaginationState> {
  constructor(props: any) {
    super(props);
    this.state = {
      current: 1,
      morePageCurrent: 5,
    };
  }

  onChange = (page) => {
    this.setState({ 
      current: page,
    });
  }

  onMorePageChange = (page) => {
    this.setState({ 
      morePageCurrent: page,
    });
  }

  render () {
    const { current, morePageCurrent } = this.state;
    return (
      <div className="absui-pagination">
        <ABSDescription>基础分页</ABSDescription>
        <ABSPaginationView current={current} pageSize={10} total={34} onChange={this.onChange} />
        <ABSDescription style={{ marginTop: 32}}>更多分页</ABSDescription>
        <ABSPaginationView current={morePageCurrent} pageSize={10} total={104} onChange={this.onMorePageChange} />
      </div>
    );
  }
}

export default ABSUIPagination;