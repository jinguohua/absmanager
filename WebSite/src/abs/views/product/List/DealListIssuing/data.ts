
import commonUtils from '../../../../../utils/commonUtils';
import { addLink } from './index';
import { IProgressNode } from '../../../../../components/ABSProgress/ABSProgressItem';

export const dealListIssuingColumnsData = [
  {
    title: '产品简称',
    dataIndex: 'short_name',
    key: 'short_name',
    render: (text, record) => addLink(text, record)
  }, {
    title: '交易场所',
    dataIndex: 'exchange',
    key: 'exchange',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    className: 'abs-right',
    title: '发行金额(亿)',
    dataIndex: 'expected_offering',
    key: 'expected_offering',
    render: (text: string) => commonUtils.formatContent(text, true, false, true, 2, 8)
  }, {
    title: '当前状态',
    key: 'current_status',
    dataIndex: 'current_status',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '更新日期',
    dataIndex: 'update_date',
    key: 'update_date',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '发行/管理人',
    dataIndex: 'issuer',
    key: 'issuer',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '原始权益人',
    dataIndex: 'originator',
    key: 'originator',
    render: (text: string) => commonUtils.formatContent(text, true)
  }
];

export function formatCycleData(response: any): null | IProgressNode[] {
  if (!response) {
    return null;
  }
  const circleList = response;
  if (!Array.isArray(circleList)) {
    return null;
  }
  const productCycle: IProgressNode[] = [];
  for (const item of circleList) {
    if (!item) {
      continue;
    }
    const obj: IProgressNode = {
      date: '',
      title: '',
      time: 0,
    };
    if (item.tag_name) {
      obj.title = item.tag_name;
    }
    if (!item.date) {
      continue;
    }
    obj.date = item.date;
    const timeValue = Date.parse(obj.date);
    if (isNaN(timeValue)) {
      continue;
    }
    obj.time = timeValue;
    productCycle.push(obj);
  }
  return productCycle;
}