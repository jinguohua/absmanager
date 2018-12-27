import React from 'react';
import OrganizationListItem, { IOrganizationListItem } from '../Item';
import './index.less';

export interface IListProps {
  dataSource: IOrganizationListItem[];
}

export interface IListState {

}

class List extends React.PureComponent<IListProps, IListState> {
  render() {
    const { dataSource } = this.props;
    if (!Array.isArray(dataSource)) { return null; }
    return (
      <div className="organization-list">
        {this.renderListView()}
      </div>
    );
  }

  renderListView() {
    const { dataSource } = this.props;
    return dataSource.map((value, index, array) => {
      return (
        <OrganizationListItem
          key={index}
          item={value}
        />
      );
    });
  }
}

export default List;