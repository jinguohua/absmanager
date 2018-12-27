import React, { Component } from 'react';
import ABSAnchorPoint from '../../../../components/ABSAnchorPoint';

const prefix = '#/anchor-point/';
const links = [
  { title: '基本资料', href: `${prefix}router1` },
  { title: '教育经历', href: `${prefix}router2` },
  { title: 'ABS项目经历', href: `${prefix}router3` },
  { title: '工作经历', href: `${prefix}router4` },
  { title: '其他职务', href: `${prefix}router5` },
  { title: '奖项与荣誉', href: `${prefix}router6` },
  { title: '近期活动', href: `${prefix}router7` },
  { title: '著作与文章', href: `${prefix}router8` },
  { title: '个人简介', href: `${prefix}router9` }
];

class ABSAnchorPointCard extends Component {
  render() {
    return (
      <ABSAnchorPoint links={links} />
    );
  }
}

export default ABSAnchorPointCard; 