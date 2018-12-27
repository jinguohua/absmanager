import React from 'react';
import './index.less';
import SectionTitle from '../SectionTitle';
import RowTitle from '../RowTitle';

export interface IOrganizationListItemAgency {
  id: number;
  name: string;
}

export interface IOrganizationListItem {
  role: string;
  agencys: IOrganizationListItemAgency[];
}

export interface IItemProps {
  item: IOrganizationListItem;
}

export interface IItemState {

}

class Item extends React.Component<IItemProps, IItemState> {
  render() {
    const { item } = this.props;
    if (!item) { return null; }
    return (
      <div className="organization-list-item">
        <SectionTitle className="organization-list-item-title" title={item.role} />
        {this.renderOrganizationMembers()}
      </div>
    );
  }

  renderOrganizationMembers() {
    const { item } = this.props;
    const members = item.agencys;
    if (!Array.isArray(members)) { return null; }
    return members.map((value, index, array) => {
      if (!value || !value.name) { return null; }
      const name = value.name;
      const id = value.id;
      return (
        <RowTitle
          key={index}
          title={name}
          id={id}
        />
      );
    });
  }
}

export default Item;