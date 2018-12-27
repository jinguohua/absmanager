import React from 'react';
import ABSList from '../ABSList';
import ABSPagination from '../ABSPagination';
import './index.less';
import ABSPanel from '../ABSPanel';
import ABSContainer from '../ABSContainer';
import PerfectScrollbar from 'react-perfect-scrollbar';

export interface IABSListContainerProps {
  columnsData?: any;
  title: string;
  comment?: string;
  actionType: string;
  payload: object;
  model: string;
  scroll?: any;
  pageStyle?: any;
  emptyText?: string;
  removePerfectScrollBar?: boolean;
  onRow?: (record: any, index: number) => any;
  bordered?: boolean;
  onChange?: any;
  selectable?: boolean;
}

interface IABSListContainerState {
  isNoData: boolean;
}

class ABSListContainer extends React.Component<IABSListContainerProps, IABSListContainerState> {

  static defaultProps() {
    return {
      removePerfectScrollBar: false,
      bordered: false,
    };
  }

  constructor(props: any) {
    super(props);
    this.state = {
      isNoData: false,
    };
  }

  componentDidMount() {
    const container = document.querySelectorAll('.abs-list-container .scrollbar-container')[0] as HTMLElement;
    setTimeout(() => {
      container.scrollLeft = 1;
    }, 1500);
  }

  render() {
    const { selectable, pageStyle, columnsData, title, comment, actionType, payload, model, scroll, emptyText, onRow, removePerfectScrollBar, bordered, onChange } = this.props;
    const { isNoData } = this.state;
    return (
      <div className={!isNoData ? 'abs-list-container' : 'abs-list-container-no-data'} >
        <ABSContainer removePerfectScrollBar={removePerfectScrollBar}>
          <ABSPanel title={title} comment={comment} removePadding={true}>
            <PerfectScrollbar>
              <ABSList
                selectable={selectable}
                actionType={actionType}
                payload={payload}
                columnsData={columnsData}
                model={model}
                scroll={scroll}
                emptyText={emptyText}
                onRow={onRow}
                bordered={bordered}
              />
            </PerfectScrollbar>
          </ABSPanel>
        </ABSContainer>
        <div className={!isNoData ? 'abs-list-container-footer' : 'abs-list-container-footer-no-data'} style={pageStyle}>
          <ABSPagination
            className="abs-deal-list-pagination"
            actionType={actionType}
            payload={payload}
            model={model}
            onChange={onChange}
            onLoadWithNoData={this.onLoadWithNoData}
          />
        </div>
      </div>
    );
  }

  onLoadWithNoData = (flag: boolean) => {
    this.setState({ isNoData: flag });
  }
}

export default ABSListContainer;