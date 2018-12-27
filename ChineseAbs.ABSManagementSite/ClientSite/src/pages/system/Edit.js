import { React } from 'react';
import { Modal, Form, Input } from 'antd';
import { EditUser } from '../../services/api';

const FormItem = Form.Item;

const CollectionCreateForm = Form.create({
  onFieldsChange(props, changedFields) {
    props.onChange(changedFields);
  },
  mapPropsToFields(props) {
    return {
      UserName: Form.createFormField({ ...props.UserName }),
      NickName: Form.createFormField({ ...props.NickName }),
      PhoneNumber: Form.createFormField({ ...props.PhoneNumber }),
      Email: Form.createFormField({ ...props.Email }),
      Roles: Form.createFormField({ ...props.Roles }),
    };
  },
})(
  class Dialog extends React.Component {
    constructor(props) {
      super(props);
      this.state = { visible: false };
    }

    render() {
      const { visible, onCancel, onSave, form } = this.props;
      const { getFieldDecorator } = form;
      return (
        <Modal
          visible={visible}
          title="编辑用户"
          okText="确定"
          cancelText="取消"
          onCancel={onCancel}
          onOk={onSave}
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
          </Form>
        </Modal>
      );
    }
  }
);
// eslint-disable-next-line
class EditUserAction extends React.Component {
  constructor(props) {
    super(props);
    const { row } = props;
    this.state = {
      fields: {
        UserName: { value: row.userName },
        NickName: { value: props.row.nickName },
        PhoneNumber: { value: props.row.phoneNumber },
        Email: { value: props.row.email },
        Roles: { value: props.row.roles },
      },
    };
  }

  handleFormChange = changedFields => {
    this.setState(
      ({ fields }) => ({
        fields: { ...fields, ...changedFields },
      }),
      () => {}
    );
  };

  showModal = () => {
    this.setState({ visible: true });
  };

  handleCancel = () => {
    this.setState({ visible: false });
  };

  handleSave = () => {
    const { form } = this.formRef.props;
    form.validateFields((err, values) => {
      if (err) {
        return;
      }
      Object.assign(values, this.data.id);

      EditUser(values).then();
      form.resetFields(); // 重置文本框
      this.setState({ visible: false });
    });
  };

  saveFormRef = formRef => {
    this.formRef = formRef;
  };

  render() {
    const { fields, visible } = this.state;

    return (
      <div>
        <a type="primary" onClick={this.showModal}>
          编辑
        </a>
        <CollectionCreateForm
          {...fields}
          onChange={this.handleFormChange}
          wrappedComponentRef={this.saveFormRef}
          visible={visible}
          onCancel={this.handleCancel}
          onSave={this.handleSave}
          data={this.row}
        />
      </div>
    );
  }
}

export default EditUserAction;

// ReactDOM.render(<CollectionsPage />, mountNode);
