import React, { Component } from 'react';
import './index.less';
import ABSProgress from '../../../../components/ABSProgress';

const testData = [
  {
    date: '2017-09-12',
    title: '受理',
    time: new Date('2017-09-12').getTime(),
  },
  {
    date: '2017-09-26',
    title: '反馈',
    time: new Date('2017-09-26').getTime(),
  },
  {
    date: '2017-10-24',
    title: '回复意见',
    time: new Date('2017-10-24').getTime(),
  },
  {
    date: '2017-11-02',
    title: '通过',
    time: new Date('2017-11-02').getTime(),
  },
  {
    date: '2017-12-01',
    title: '初始起算日',
    time: new Date('2017-12-01').getTime(),
  },
  {
    date: '2018-01-18',
    title: '簿记建档日',
    time: new Date('2018-01-18').getTime(),
  },
  {
    date: '2018-01-23',
    title: '产品成立日',
    time: new Date('2018-01-23').getTime(),
  },
  {
    date: '2018-01-24',
    title: '上市流通日',
    time: new Date('2018-01-24').getTime(),
  },
  {
    date: '2018-02-26',
    title: '首次偿付日',
    time: new Date('2018-02-26').getTime(),
  },
  {
    date: '2018-11-27',
    title: '法定到期日',
    time: new Date('2018-11-27').getTime(),
  },
];

class ABSUIProgress extends Component {
  render () {
    return (
      <ABSProgress className="absui-progress" dataSource={testData} />
    );
  }
}

export default ABSUIProgress;