import React, { Component } from 'react';
import ABSAvatar from '../../../../components/ABSAvatar';
import '../../../../styles/coverAnt.less';
import { Row, Col } from 'antd';

class ABSAvatarRoute extends Component {
  render () {
    return (
      <div>
        <Row type="flex" align="middle" style={{ marginBottom: 12}}>
          <Col span={2}>
            <ABSAvatar 
              alt="王启明" 
              imageUrl="http://10.1.1.35:8000/filestore/common/downloadimg/cnabs/8a7faeef-84c2-c11b-f999-08d56deefc53/s"
            />
          </Col>
          <Col span={3}>
            60X60（头像后面+三行文字）
          </Col>
        </Row>
        
        <Row type="flex" align="middle" style={{ marginBottom: 12}}>
          <Col span={2}>
            <ABSAvatar 
              size="default" 
              alt="王小明" 
              imageUrl="http://10.1.1.35:8000/filestore/common/downloadimg/cnabs/8a7faeef-84c2-c11b-f999-08d56deefc53/s"
            />
          </Col>
          <Col span={3}>
            40X40（后面+两行文字）
          </Col>
        </Row>

        <Row type="flex" align="middle" style={{ marginBottom: 12}}>
          <Col span={2}>
            <ABSAvatar 
              size="small" 
              alt="王小明" 
              imageUrl="http://10.1.1.35:8000/filestore/common/downloadimg/cnabs/8a7faeef-84c2-c11b-f999-08d56deefc53/s" 
            />
          </Col>
          <Col span={3}>
            20X20（后面加一行文字或不加，常用语点赞人数头像，信息不太重要等修饰）
          </Col>
        </Row>
{/*         
        <Row>
          <Col span={2}>
            <ABSAvatar 
              size="small" 
              alt="王小明" 
              imageUrl="http://10.1.1.35:8000/filestore/common/downloadimg/cnabs/8a7faeef-84c2-c11b-f999-08d56deefc53/s" 
            />
          </Col>
          <Col span={2}>
            40X40（后面+两行文字）
          </Col>
        </Row> */}
      </div>
    );
  }
}

export default ABSAvatarRoute;