import React from 'react';
import { Modal, Form, Input } from 'antd';

const FormItem = Form.Item;

const CollectionCreateForm = Form.create()(
  class extends React.Component {
    constructor(props) {
      super(props);
      this.state = { visible: true };
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
export default CollectionCreateForm;
