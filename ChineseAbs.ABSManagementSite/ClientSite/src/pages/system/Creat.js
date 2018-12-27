import React from 'react';
import { Modal, Form, Input } from 'antd';
import { AddUser } from '../../services/api';

const FormItem = Form.Item;

const CollectionCreateForm = Form.create()(
  // eslint-disable-next-line
  class extends React.Component {
    render() {
      const { visible, onCancel, onCreate, form } = this.props;
      const { getFieldDecorator } = form;
      return (
        <Modal
          visible={visible}
          title="创建用户"
          okText="确定"
          cancelText="取消"
          onCancel={onCancel}
          onOk={onCreate}
        >
          <Form layout="vertical">
            <FormItem>
              用户名称
              {getFieldDecorator('UserName', {
                rules: [{ required: true, message: '请输入用户名称!' }],
              })(<Input />)}
            </FormItem>
            <FormItem className="inputitem">
              用户昵称
              {getFieldDecorator('NickName')(<Input type="text" />)}
            </FormItem>
            <FormItem>
              密码
              {getFieldDecorator('PassWord')(<Input type="password" />)}
            </FormItem>
            <FormItem>
              确认密码
              {getFieldDecorator('RepeatPassword')(<Input type="password" />)}
            </FormItem>
            <FormItem>
              联系电话
              {getFieldDecorator('PhoneNumber')(<Input type="text" />)}
            </FormItem>
            <FormItem>
              电子邮箱
              {getFieldDecorator('Email')(<Input type="email" />)}
            </FormItem>
            <FormItem>
              角色
              {getFieldDecorator('Roles')(<Input type="text" />)}
            </FormItem>
            {/* <FormItem className="collection-create-form_last-form-item">
              {getFieldDecorator('modifier', {
                initialValue: 'public',
              })(
                <Radio.Group>
                  <Radio value="public">Public</Radio>
                  <Radio value="private">Private</Radio>
                </Radio.Group>
              )}
            </FormItem> */}
          </Form>
        </Modal>
      );
    }
  }
);

// eslint-disable-next-line
class CreateUser extends React.Component {
  state = {
    visible: false,
  };

  showModal = () => {
    this.setState({ visible: true });
  };

  handleCancel = () => {
    this.setState({ visible: false });
  };

  handleCreate = () => {
    const { form } = this.formRef.props.form;

    form.validateFields((err, values) => {
      if (err) {
        return;
      }
      AddUser(values).then();

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
          新增
        </a>
        <CollectionCreateForm
          wrappedComponentRef={this.saveFormRef}
          visible={visible}
          onCancel={this.handleCancel}
          onCreate={this.handleCreate}
        />
      </div>
    );
  }
}
export default CreateUser;
