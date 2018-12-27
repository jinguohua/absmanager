import React, { Component } from 'react';
import '../../../../styles/coverAnt.less';
import ABSDescription from '../../../../components/ABSDescription';
import { Row } from 'antd';
import { ABSUI_MARGIN_BOTTOM } from '../../../../utils/constant';
import routeConfig from '../../../config/routeConfig';
import ABSLink from '../../../../components/ABSLink';

class ABSLinkRoute extends Component {
  render() {
    return (
      <div>
        <ABSDescription>跳转链接</ABSDescription>
        <Row style={{ marginBottom: ABSUI_MARGIN_BOTTOM }}>
          <ABSLink to={routeConfig.home}>跳转链接</ABSLink>
        </Row>
      </div>
    );
  }
}

export default ABSLinkRoute;