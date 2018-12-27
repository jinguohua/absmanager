import React from 'react';
import { Modal, Form, Input } from 'antd';
import { ResetPassword } from '../../services/api';

const FormItem = Form.Item;

const CollectionCreateForm = Form.create()(
  class extends React.Component {
    constructor(props) {
      super(props);
      // const{data}=this.props;

      this.state = { visible: true, userName: props.row.userName };
    }

    render() {
      const { visible, onCancel, onOk, form } = this.props;
      const { getFieldDecorator } = form;
      return (
        <Modal
          visible={visible}
          title="重置密码"
          okText="确定"
          cancelText="取消"
          onCancel={onCancel}
          onOk={onOk}
        >
          <Form layout="vertical">
            <FormItem>
              密码
              {getFieldDecorator('password')(<Input type="password" />)}
            </FormItem>
            <FormItem>
              确认密码
              {getFieldDecorator('repeatPassword')(<Input type="password" />)}
            </FormItem>
          </Form>
        </Modal>
      );
    }
  }
);
// eslint-disable-next-line
class RePassdwordButton extends React.Component {
  constructor(props) {
    super(props);
    this.state = { visible: false, userName: props.row.userName };
  }

  showModal = () => {
    this.setState({ visible: true });
  };

  handleCancel = () => {
    this.setState({ visible: false });
  };

  handleOK = () => {
    const { form } = this.formRef.props;
    form.validateFields((err, values) => {
      if (err) {
        return;
      }
      const {
        userName: { id },
      } = this.state;

      Object.assign(values, id);
      ResetPassword(values).then();

      form.resetFields(); // 重置文本框
      this.setState({ visible: false });
    });
  };

  saveFormRef = formRef => {
    this.formRef = formRef;
  };

  render() {
    const { visible } = this.state;
    return (
      <div>
        <a type="primary" onClick={this.showModal}>
          重置密码
        </a>
        <CollectionCreateForm
          wrappedComponentRef={this.saveFormRef}
          visible={visible}
          onCancel={this.handleCancel}
          onOk={this.handleOK}
          data={this.data}
        />
      </div>
    );
  }
}

export default RePassdwordButton;
