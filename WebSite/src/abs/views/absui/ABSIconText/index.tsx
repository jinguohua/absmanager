import React, { Component } from 'react';
import ABSIconText from '../../../../components/ABSIconText';
import './index.less';
import ABSDescription from '../../../../components/ABSDescription';

export default class ABSUITextIcon extends Component {
  render() {
    return (
      <div className="absui-icon-text">
        <ABSDescription>图标+文字形式(不带背景底色，其中文字 和标一般处于同等大小的视觉效果)</ABSDescription>
        <ABSIconText className="absui-icon-text-space" icon="form" text="编辑" onClick={() => alert('编辑')} />
        <ABSIconText className="absui-icon-text-space" icon="plus" text="添加" onClick={() => alert('添加')} />
        <ABSIconText className="absui-icon-text-space" icon="paper-clip" text="添加附件" onClick={() => alert('添加附件')} />
      </div>
    );
  }
}