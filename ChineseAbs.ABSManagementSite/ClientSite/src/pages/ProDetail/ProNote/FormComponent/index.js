import React from 'react';
import { Form, Input, Select, Row, Col, Checkbox } from 'antd';
import styles from './index.less';

const FormItem = Form.Item;

const { Option } = Select;

function FormLayout({ getFieldDecorator }) {
  return (
    <div>
      <Form>
        {/* 证券信息 */}
        <div className={styles.linput}>
          <h1>证券详情</h1>
          <Row>
            <Col span={12}>
              <FormItem label="名称">{getFieldDecorator('username')(<Input />)}</FormItem>
            </Col>
            <Col span={12}>
              <FormItem label="偿付类型">
                <Select>
                  <Option value="jack">Jack</Option>
                  <Option value="lucy">Lucy</Option>
                  <Option value="Yiminghe">yiminghe</Option>
                </Select>
              </FormItem>
            </Col>
          </Row>
          <Row>
            <Col span={12}>
              <FormItem label="本金">
                <Input />
              </FormItem>
            </Col>
            <Col span={12}>
              <FormItem label="证券类型">
                <Select>
                  <Option value="jack">Jack</Option>
                  <Option value="lucy">Lucy</Option>
                  <Option value="Yiminghe">yiminghe</Option>
                </Select>
              </FormItem>
            </Col>
          </Row>
          <Row>
            <Col span={12}>
              <FormItem label="期限">
                <Input />
              </FormItem>
            </Col>
            <Col span={12}>
              <FormItem label="到期日">
                <Input />
              </FormItem>
            </Col>
          </Row>
          <Row>
            <Col span={12}>
              <FormItem label="加权期限">
                <Input />
              </FormItem>
            </Col>
            <Col span={6}>
              <FormItem label="发行价格" className={styles.minput}>
                <Input style={{ width: '100' }} />
              </FormItem>
            </Col>
            <Col span={6}>
              <FormItem label="面额" className={styles.minput}>
                <Input />
              </FormItem>
            </Col>
          </Row>
        </div>
        {/* 利率 */}
        <div className={styles.minput}>
          <h1>利率</h1>
          <Row>
            <Col span={6}>
              <FormItem label="类型">
                <Select>
                  <Option value="jack">Jack</Option>
                  <Option value="lucy">Lucy</Option>
                  <Option value="Yiminghe">yiminghe</Option>
                </Select>
              </FormItem>
            </Col>
            <Col span={6}>
              <FormItem label="利率">{getFieldDecorator('username')(<Input />)}</FormItem>
            </Col>
            <Col span={6}>
              <FormItem label="基础利率">
                <Select>
                  <Option value="jack">Jack</Option>
                  <Option value="lucy">Lucy</Option>
                  <Option value="Yiminghe">yiminghe</Option>
                </Select>
              </FormItem>
            </Col>
            <Col span={6}>
              <FormItem label="利差">{getFieldDecorator('username')(<Input />)}</FormItem>
            </Col>
          </Row>
          <Row>
            <Col span={6}>
              <FormItem label="计日方式">
                <Select>
                  <Option value="jack">Jack</Option>
                  <Option value="lucy">Lucy</Option>
                  <Option value="Yiminghe">yiminghe</Option>
                </Select>
              </FormItem>
            </Col>
            <Col span={6}>
              <FormItem label="利息上限">{getFieldDecorator('username')(<Input />)}</FormItem>
            </Col>
            <Col span={6}>
              <Checkbox style={{ width: '200px', 'margin-top': '10px' }}>计息至分配日</Checkbox>
            </Col>
          </Row>
        </div>
      </Form>
    </div>
  );
}
// 表单生成
const WrappedHorizontalLoginForm = Form.create({
  onFieldsChange(props, changedFields) {
    props.onChange(changedFields);
  },
  mapPropsToFields(props) {
    return {
      username: Form.createFormField({
        ...props.username,
      }),
      type: Form.createFormField({ ...props.type }),
      moon: Form.createFormField({ ...props.moon }),
    };
  },
})(props => {
  const { getFieldDecorator } = props.form;

  return <FormLayout getFieldDecorator={getFieldDecorator} />;
});

// 表单组件
class FormComponent extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      fields: {
        username: {
          value: 'benjycui',
        },
        type: {
          value: 'jack',
        },
        moon: {
          value: true,
        },
      },
    };
  }

  handleFormChange = changedFields => {
    this.setState(
      ({ fields }) => ({
        fields: { ...fields, ...changedFields },
      }),
      () => {
        console.log(this.state);
      }
    );
  };

  sendInfo = () => {
    console.log(this.state);
  };

  render() {
    const { fields } = this.state;
    return (
      <div>
        <WrappedHorizontalLoginForm {...fields} onChange={this.handleFormChange} />
      </div>
    );
  }
}

export default FormComponent;
