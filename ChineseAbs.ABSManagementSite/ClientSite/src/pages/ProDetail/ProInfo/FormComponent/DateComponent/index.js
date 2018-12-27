import React from 'react';
import { Form, Select, Row, Col, Checkbox, DatePicker } from 'antd';
import styles from './index.less';

const FormItem = Form.Item;
const { Option } = Select;

function DateComponent({ getFieldDecorator, moon }) {
  return (
    <div>
      <Row>
        <Col span={8}>
          <FormItem label="首次日期">
            <DatePicker placeholder="" className={styles.dinput} />
            &nbsp;
            {getFieldDecorator(`${moon}moon`, {
              valuePropName: 'checked',
              initialValue: true,
            })(<Checkbox style={{ width: '60px' }}>月末</Checkbox>)}
          </FormItem>
        </Col>
        <Col span={8}>
          <FormItem label="频率">
            <Select>
              <Option value="jack">Jack</Option>
              <Option value="lucy">Lucy</Option>
              <Option value="Yiminghe">yiminghe</Option>
            </Select>
          </FormItem>
        </Col>
        <Col span={8}>
          <FormItem label="规则">
            <Select>
              <Option value="jack">Jack</Option>
              <Option value="lucy">Lucy</Option>
              <Option value="Yiminghe">yiminghe</Option>
            </Select>
          </FormItem>
        </Col>
      </Row>
      <Row>
        <Col span={8}>
          <FormItem label="结束日期">
            <DatePicker placeholder="" />
          </FormItem>
        </Col>
        <Col span={8}>
          <a>自定义</a>
        </Col>
      </Row>
    </div>
  );
}

export default DateComponent;
