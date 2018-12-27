import React from 'react';
import ABSPaginationView from '../ABSPagination/ABSPaginationView';
import { connect } from 'dva';

interface IListData {
  items?: Array<any>;
  current?: number;
  total?: number;
  pageSize?: number;
}

export interface IABSPaginationProps {
  actionType?: string;
  payload?: object;
  model?: string;

  dispatch?: any;
  total?: number;
  pageSize?: number;

  className?: string;
  style?: React.CSSProperties;
  hideOnSinglePage?: boolean;
  onLoadWithNoData?: (flag: boolean) => void;
  onChange?: any;
}

interface IABSPaginationState {
  current: number;
}

const PAGE_SIZE = 10;

class ABSPagination extends React.Component<
  IABSPaginationProps,
  IABSPaginationState
> {
  static defaultProps = {
    pageSize: PAGE_SIZE
  };

  constructor(props: IABSPaginationProps) {
    super(props);
    this.state = {
      current: 1
    };
  }

  componentWillReceiveProps(nextProps: any) {
    const { onLoadWithNoData } = this.props;
    const { current } = this.state;
    if (nextProps.total === 0 && current === 1) {
      if (onLoadWithNoData) {
        onLoadWithNoData(true);
      }
    } else {
      if (onLoadWithNoData) {
        onLoadWithNoData(false);
      }
    }
  }

  onChange = (current, pageSize) => {
    const { dispatch, actionType, payload, onChange } = this.props;

    if (actionType && actionType.length > 0) {
      dispatch({
        type: actionType,
        payload: {
          ...payload,
          page: current,
          pageSize
        }
      });
      this.setState({ current });
    }

    if (onChange) {
      onChange();
    }
  }

  render() {
    const { total, pageSize, style, className, hideOnSinglePage } = this.props;
    const { current } = this.state;
    return (
      <ABSPaginationView
        total={total}
        current={current}
        pageSize={pageSize}
        onChange={this.onChange}
        style={style}
        className={className}
        hideOnSinglePage={hideOnSinglePage}
      />
    );
  }
}

function getData(state: any, model: any): Array<any> | IListData {
  const modelPropsArray = model.split('.');
  let data = state[modelPropsArray[0]];

  for (const props of modelPropsArray.slice(1)) {
    if (data[props]) {
      data = data[props];
    }
  }

  return data;
}

const mapStateToProps = (state, { model, contentData, pageSize }) => {
  if (model && model.length > 0) {
    const modelName = model.split('.')[0];
    const data: Array<any> | IListData = getData(state, model);

    // 不带分页的列表
    if (data && Array.isArray(data)) {
      return {
        total: data.length,
        current: 1,
        pageSize,
        loading: state.loading.models[modelName]
      };
    } else {
      // 带分页的列表
      // const { current, pageSize, total } = data;
      return {
        ...data,
        loading: state.loading.models[modelName]
      };
    }
  }

  if (!model && contentData && contentData.length > 0) {
    return { total: contentData.length, current: 1, pageSize };
  }
  return { total: pageSize, pageSize, current: 1 };
};

export default connect(mapStateToProps)(ABSPagination);
