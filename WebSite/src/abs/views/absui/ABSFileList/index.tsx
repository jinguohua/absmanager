import React, { Component } from 'react';
import '../../../../styles/coverAnt.less';
import ABSDescription from '../../../../components/ABSDescription';
import { connect } from 'dva';
import { ABSFileData } from '../../../../components/ABSFileList/ABSFile';
import ABSFileList from '../../../../components/ABSFileList';

interface IABSUIFileListProps {
}

class ABSUIFileList extends Component<IABSUIFileListProps> {

  render () {
    const fileList: Array<ABSFileData> = [
      {
        title: '捷信成功发行2018年第四期个人消费贷款资产支持证券',
        url: '/market.html#/detail/news-info?id=12157',
        source: '中国网财经',
        publishDate: '2018-12-05'
      },
      {
        title: '“千亿级”房企有望突破30家 资产证券化融资金额大幅攀升',
        url: '/market.html#/detail/news-info?id=12158',
        source: '证券日报',
        publishDate: '2018-12-05'
      },
      {
        title: '工行杭州分行投资银行部 完成省内首单ABS投资业务',
        url: '/market.html#/detail/news-info?id=12159',
        source: '杭州日报 ',
        publishDate: '2018-12-05'
      }
    ];
    return (
      <div>
        <ABSDescription style={{ marginTop: 12 }}>文章列表：</ABSDescription>
        <ABSFileList 
          list={fileList}
          loading={false}
        />
      </div>
    );
  }
}

function mapStateToProps({ account }: any) {
  return {
  };
}

export default connect(mapStateToProps)(ABSUIFileList);