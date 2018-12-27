import React, { Component } from 'react';
import { ABSInput } from '../../../../components/ABSInput/index';
import { ABSAntIcon } from '../../../../components/ABSAntIcon';
import '../../../../styles/coverAnt.less';
import ABSDescription from '../../../../components/ABSDescription';
// import { Select } from 'antd';

// const Option = Select.Option;
// const selectAfter = (
//   <Select defaultValue=".com" style={{ width: 80 }}>
//     <Option value=".com">.com</Option>
//     <Option value=".jp">.jp</Option>
//     <Option value=".cn">.cn</Option>
//     <Option value=".org">.org</Option>
//   </Select>
// );
// const selectBefore = (
//   <Select defaultValue="Http://" style={{ width: 90 }}>
//     <Option value="Http://">Http://</Option>
//     <Option value="Https://">Https://</Option>
//   </Select>
// );

interface IParticipateProjectState {
  test1: string;
  largetext: string;
  righttext: string;
}
class ABSInputRoute extends Component<any, IParticipateProjectState> {
  constructor(props: any) {
    super(props);
    this.state = {
      test1: '',
      largetext: '',
      righttext: '',
    };
  }

  inputChange = (value) => {
    this.setState({ test1: value });
  }

  largInputChange = (value) => {
    this.setState({ largetext: value });
  }

  rightinputChange = value => {
    this.setState({ righttext: value });
  }

  deletBtn = (e: any) => {
    this.setState({ righttext: '' });
  }
  render() {
    const { test1, largetext, righttext } = this.state;
    return (
      <div>
        <ABSDescription>小输入框样式</ABSDescription>
        <ABSInput
          className="small-view"
          placeholder="请输入内容"
          size="default"
          onChange={this.inputChange}
          value={test1}
        />

        <ABSDescription style={{ marginTop: 32}}>右边增加删除输入内容按钮</ABSDescription>
        <ABSInput
          className="small-view"
          prefix={<ABSAntIcon type="user" style={{ color: '#979797' }} />}
          placeholder="右边带按钮"
          size="default"
          onChange={this.rightinputChange}
          value={righttext}
          deletBtn={(e) => this.deletBtn(e)}
          issuffix={true}
        />

        {/* <ABSInput
          className="small-view"
          placeholder="addonAfter addonBefore"
          size="default"
          onChange={this.inputChange}
          value={test1}
          addonAfter={selectAfter}
          addonBefore={selectBefore}
        /> */}

        <ABSDescription style={{ marginTop: 32}}>禁用模式样例</ABSDescription>
        <ABSInput
          className="small-view"
          placeholder="禁用模式"
          type="text"
          disabled={true}
          readOnly={false}
          defaultValue=""
          id=""
          size="default"
          onChange={this.inputChange}
        />

        <ABSDescription style={{ marginTop: 32}}>只读模式样例</ABSDescription>
        <ABSInput
          className="small-view"
          placeholder="只读模式"
          type="text"
          disabled={false}
          readOnly={true}
          defaultValue=""
          id=""
          size="default"
          onChange={this.inputChange}
        />

        <ABSDescription style={{ marginTop: 32}}>大尺寸输入框的样例</ABSDescription>
        <ABSInput
          placeholder="请输入内容"
          type="text"
          size="default"
          onChange={this.largInputChange}
          value={largetext}
          style={{ width: 420 }}
        />

      </div>
    );
  }
}

export default ABSInputRoute;