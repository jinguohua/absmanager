import React, { Component } from 'react';
// import { Divider } from 'antd';
// import { Link } from 'dva/router';
import ABSList from '../../../../components/ABSList';
// import ABSIconText from '../../../../components/ABSIconText';
import '../../../../styles/coverAnt.less';
import commonUtils from '../../../../utils/commonUtils';
import ABSDescription from '../../../../components/ABSDescription';
import ABSIconText from '../../../../components/ABSIconText';
import { Divider } from 'antd';
import ABSPagination from '../../../../components/ABSPagination';
import ABSListView from '../../../../components/ABSList/ABSListView';

const columnsData = [
  {
    title: '证券简称',
    dataIndex: 'description',
    // render: (text, record) => <p dangerouslySetInnerHTML={{ __html: record.description_link }} />
  }, {
    title: '证券代码',
    dataIndex: 'security_code',
  }, {
    title: '发行量(亿)',
    dataIndex: 'notional',
    className: 'abs-right',
    render: (text) => commonUtils.formatContent(text, true, false, true, 4)
  }, {
    title: '剩余量(亿)',
    dataIndex: 'principal',
    className: 'abs-right',
    render: (text) => commonUtils.formatContent(text, true, false, true, 4)
  }, {
    title: '当前利率(%)',
    dataIndex: 'current_coupon',
    className: 'abs-right',
    render: (text) => commonUtils.formatContent(text, true, true, true, 3)
  }, {
    title: '还本方式',
    dataIndex: 'repayment_of_principal',
  }, {
    title: '加权年限',
    dataIndex: 'initial_wal_legal',
    className: 'abs-right',
    render: (text) => commonUtils.formatContent(text, true)
  }, {
    title: '证券定价',
    dataIndex: 'clean_price',
    className: 'abs-right',
    render: (text) => commonUtils.formatContent(text, true, false, true, 4)
  }, {
    title: '当前评级',
    dataIndex: 'current_rating_pause',
    className: 'abs-center',
  }
];

const columnsData2 = [
  {
    title: '证券简称',
    dataIndex: 'description',
    // render: (text, record) => <p dangerouslySetInnerHTML={{ __html: record.description_link }} />
  }, {
    title: '证券代码',
    dataIndex: 'security_code',
  }, {
    title: '发行量(亿)',
    dataIndex: 'notional',
    className: 'abs-right',
    render: (text) => commonUtils.formatContent(text, true, false, true, 4)
  }, {
    title: '剩余量(亿)',
    dataIndex: 'principal',
    className: 'abs-right',
    render: (text) => commonUtils.formatContent(text, true, false, true, 4)
  }, {
    title: '当前利率(%)',
    dataIndex: 'current_coupon',
    className: 'abs-right',
    render: (text) => commonUtils.formatContent(text, true, true, true, 3)
  }, {
    title: '还本方式',
    dataIndex: 'repayment_of_principal',
  }, {
    title: '加权年限',
    dataIndex: 'initial_wal_legal',
    className: 'abs-right',
    render: (text) => commonUtils.formatContent(text, true)
  }, {
    title: '证券定价',
    dataIndex: 'clean_price',
    className: 'abs-right',
    render: (text) => commonUtils.formatContent(text, true, false, true, 4)
  }, {
    title: '当前评级',
    dataIndex: 'current_rating_pause',
    className: 'abs-center',
  }, {
    title: '操作',
    dataIndex: 'action',
    render: () => (
      <div>
        <a href="javascript:;">
          <ABSIconText className="absui-icon-text-space" icon="delete" text="删除" onClick={() => alert('删除')} />
        </a>
        <Divider type="vertical" />
        <a href="javascript:;">
          <ABSIconText className="absui-icon-text-space" icon="edit" text="修改" onClick={() => alert('修改')} />
        </a>
        <Divider type="vertical" />
        <a href="javascript:;">
          <ABSIconText className="absui-icon-text-space" icon="download" text="下载" onClick={() => alert('下载')} />
        </a>
      </div>
    ),
  }
];

const columnsData3 = [
  {
    title: '证券简称',
    dataIndex: 'short_name',
    key: 'short_name',
    render: (text, record) => text
  }, {
    title: '证券代码',
    dataIndex: 'code',
    key: 'code',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '起息日',
    dataIndex: 'closing_date',
    key: 'closing_date',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '交易场所',
    dataIndex: 'exchange',
    key: 'exchange',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '市场分类',
    dataIndex: 'market_type',
    key: 'market_type',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '产品分类',
    dataIndex: 'deal_type',
    key: 'deal_type',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '产品细分',
    dataIndex: 'asset_sub_category',
    key: 'asset_sub_category',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '最新评级1',
    dataIndex: 'latest_rating1',
    key: 'latest_rating1',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '最新评级2',
    dataIndex: 'latest_rating2',
    key: 'latest_rating2',
    render: (text: string) => commonUtils.formatContent(text, true)
  }
];

// const contentData = [
//   {
//     key: '1',
//     name: 'John BOB',
//     age: 32,
//     address: 'New York No. 1 Lake Park',
//     tags: 111,
//     id: 111,
//   }, {
//     key: '2',
//     name: 'Jim Green',
//     age: 42,
//     address: 'London No. 1 Lake Park',
//     tags: 2222,
//     id: 222,
//   }, {
//     key: '3',
//     name: 'Joe Black',
//     age: 32,
//     address: 'Sidney No. 1 Lake Park',
//     tags: 333,
//     id: 333,
//   }
// ];
// const columnsData1 = [
//     {
//       title: 'Name',
//       dataIndex: 'name',
//       key: 'name',
//       render: (text, record) => <Link to={'www.baidu.com?id=' + record.id}>{text}</Link>,
//     }, {
//       className: 'abs-right',
//       title: 'Age',
//       dataIndex: 'age',
//       key: 'age',
//     }, {
//       title: 'Address',
//       dataIndex: 'address',
//       key: 'address',
//     }, {
//       className: 'abs-right',
//       title: 'Tags',
//       key: 'tags',
//       dataIndex: 'tags',
//       tags: 'tags',
//     }, {
//       className: 'abs-right',
//       title: 'Days',
//       dataIndex: 'days',
//       key: 'days',
//       days: 'days',
//     }
//   ];
// const contentData1 = [
  //   {
  //     key: '1',
  //     name: 'John BOB',
  //     age: 32,
  //     address: 'New York No. 1 Lake Park',
  //     tags: 111,
  //     days: 100,
  //     id: 1,
  //   }, {
  //     key: '2',
  //     name: 'Jim Green',
  //     age: 42,
  //     address: 'London No. 1 Lake Park',
  //     tags: 2222,
  //     days: 200,
  //     id: 21,
  //   }, {
  //     key: '3',
  //     name: 'Joe Black',
  //     age: 32,
  //     address: 'Sidney No. 1 Lake Park',
  //     tags: 333,
  //     days: 365,
  //     id: 122,
  //   }
  // ];

class ABSListRoute extends Component {
  // render 提取出来
  // constructor(props: any) {
  //   super(props);
  //   this.isShowLink(columnsData);
  // }
  // isShowLink = (datas) => {
  //   datas.map((item, index) => {
  //     if (item.key === 'name') {
  //         item.render = (text, record) => {
  //           return (
  //             <div key={index}>
  //               <Link to={'www.baidu.com?id=' + record.id}>{text}</Link>
  //             </div>
  //           );
  //         };
  //     }
  //   });
  //   return datas;
  // } 

  onChange (e: any) {
    // 打印点击了第几页
  }
  render () {
    return (
      <div>
        <ABSDescription>列表1: （纯列表）表头高为36px；其余行高为32px；金额和数字右对齐、其余左对齐，注意（带括号字段可以特殊处理对齐效果）</ABSDescription>
        <ABSList
          columnsData={columnsData}
          actionType="product/getStructureList"
          payload={{ deal_id: 371 }}
          model="product.structureList.note_list"
        />

        <ABSDescription style={{ marginTop: 32}}>列表2: （图标+文字按钮）</ABSDescription>
        <ABSList
          columnsData={columnsData2}
          actionType="product/getStructureList"
          payload={{ deal_id: 371 }}
          model="product.structureList.note_list"
        />

        <ABSDescription style={{ marginTop: 32}}>列表: 暂无数据列表</ABSDescription>
        <ABSListView
          columnsData={columnsData}
          contentData={null}
          loading={false}
        />

        <ABSDescription style={{ marginTop: 32}}>列表: 带分页列表</ABSDescription>
        <ABSList
          columnsData={columnsData3} 
          actionType="product/getDealProductList" 
          payload={{ filter_type_id: 1, page_size: 10 }}
          model="product.dealProductList"
        />
        <ABSPagination style={{ float: 'right', marginTop: 12, marginRight: 12 }}>
          actionType="product/getDealProductList" 
          payload={{ filter_type_id: 1, page_size: 10 }}
          model="product.dealProductList"
        </ABSPagination>

        <ABSDescription style={{ marginTop: 32}}>列表4: 带边框</ABSDescription>
        <ABSList
          columnsData={columnsData}
          actionType="product/getStructureList"
          payload={{ deal_id: 371 }}
          bordered={true}
          model="product.structureList.note_list"
        />
      </div>
    );
  }
}

export default ABSListRoute;