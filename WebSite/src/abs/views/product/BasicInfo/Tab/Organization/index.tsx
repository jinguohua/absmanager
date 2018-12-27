import React from 'react';
import PerfectScrollbar from 'react-perfect-scrollbar';
import './index.less';
import List from './List';
import { IOrganizationListItem } from './Item';

export interface IOrganizationProps {
  organizations: IOrganizationListItem[];
}

export interface IOrganizationState {

}

class Organization extends React.Component<IOrganizationProps, IOrganizationState> {
  render() {
    const { organizations } = this.props;
    return (
      <div className="product-organization">
        <PerfectScrollbar>
          <List dataSource={organizations} />
        </PerfectScrollbar>
      </div>
    );
  }
}

export default Organization;