import React, { Component } from 'react';
import '../../../../styles/coverAnt.less';
import ABSLinkCardList from '../../../../components/ABSLinkCardList';

class ABSUILinkCardList extends Component {
  render () {
    const list = [
      {
        name: '蓝色',
        key: '蓝色',
        url: 'icon-text',
      },
      {
        name: '红色',
        key: '红色',
        url: 'url-list',
      },
      {
        name: '黄色',
        key: '红色',
        url: 'url-list',
      },
      {
        name: '灰色',
        key: '红色',
        url: 'url-list',
      },
      {
        name: '橙色',
        key: '红色',
        url: 'url-list',
      }
    ];
    return (
      <ABSLinkCardList list={list} numberOflines={3}/>
    );
  }
}

export default ABSUILinkCardList;