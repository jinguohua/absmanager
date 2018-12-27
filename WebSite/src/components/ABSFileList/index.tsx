import React from 'react';
import './index.less';
import PerfectScrollbar from 'react-perfect-scrollbar';
import ABSFile, { ABSFileData } from './ABSFile';
import ABSLoading from '../ABSLoading';
import ABSNoContent from '../ABSNoContent';

export interface IABSFileListProps {
  list: Array<ABSFileData>;
  loading?: boolean;
  pageSize?: any;
}

export interface IABSFileListState {

}

class ABSFileList extends React.Component<IABSFileListProps, IABSFileListState> {
  static defaultProps = {
    pageSize: 999
  };

  newsListView() {
    const { list, loading, pageSize } = this.props;
    if (list === undefined || loading === undefined) {
      return null;
    }
    if (loading) {
      return <ABSLoading />;
    }
    if (!list || list.length === 0) {
      return <div className="abs-file-list"><ABSNoContent /></div>;
    } else {
      return list.map((item, index) => {
        return (
          <ABSFile
            data={item}
            key={index}
            isLast={index === (pageSize - 1)}
          />
        );
      });
    }
  }

  render() {
    return (
      <PerfectScrollbar>
        <div>
          {this.newsListView()}
        </div>
      </PerfectScrollbar>
    );
  }
}

export default ABSFileList;