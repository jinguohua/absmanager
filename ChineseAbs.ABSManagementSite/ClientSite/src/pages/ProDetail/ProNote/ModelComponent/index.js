import React from 'react';
import { Modal } from 'antd';
import FormComponent from '../FormComponent/index';

function CollectionCreateForm(props) {
  const { visible, onCancel, onCreate } = props;

  return (
    <Modal
      visible={visible}
      title="Create a new collection"
      okText="Create"
      onCancel={onCancel}
      onOk={onCreate}
      width="700px"
    >
      <FormComponent />
    </Modal>
  );
}

class CollectionsPage extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      visible: false,
    };
  }

  showModal = e => {
    e.preventDefault();
    this.setState({ visible: true });
    console.log(this.props);
  };

  handleCancel = () => {
    this.setState({ visible: false });
  };

  handleCreate = () => {
    const {
      props: { form },
    } = this.formRef;
    form.validateFields((err, values) => {
      if (err) {
        return;
      }

      console.log('Received values of form: ', values);
      form.resetFields();
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
          Edit
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

export default CollectionsPage;
