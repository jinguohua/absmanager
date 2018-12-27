import React from 'react';
import ABSLink from '../../../../../../../components/ABSLink';
import RouteConfig from '../../../../../../config/routeConfig';

export interface IRowTitleProps {
  title: string;
  id: number;
}

export interface IRowTitleState {

}

class RowTitle extends React.Component<IRowTitleProps, IRowTitleState> {
  render() {
    const { title, id } = this.props;
    const text = title ? title : '';
    const to = `${RouteConfig.organizationDetail}${id}`;
    return (
      <div className="product-link">
        <ABSLink to={to} target="_blank">{text}</ABSLink>
      </div>
    );
  }
}

export default RowTitle;