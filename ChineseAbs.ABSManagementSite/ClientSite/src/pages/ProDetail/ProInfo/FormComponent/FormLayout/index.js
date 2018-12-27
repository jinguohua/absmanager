import React from 'react';
import { Form, Input, Select, Row, Col, DatePicker, Button } from 'antd';
import styles from './index.less';
import DateComponent from '../DateComponent/index';

const FormItem = Form.Item;

const { Option } = Select;

class FormLayout extends React.PureComponent {
  render() {
    const {
      form: { getFieldDecorator },
    } = this.props;
    return (
      <div>
        <Form>
          {/* 基本信息 */}
          <div className={styles.linput}>
            <h1>基本信息</h1>
            <Row>
              <Col span={12}>
                <FormItem label="名称">{getFieldDecorator('username')(<Input />)}</FormItem>
              </Col>
              <Col span={12}>
                <FormItem label="简介">
                  <Input />
                </FormItem>
              </Col>
            </Row>
            <Row>
              <Col span={12}>
                <FormItem label="产品类型">
                  {getFieldDecorator('type')(
                    <Select>
                      <Option value="jack">Jack</Option>
                      <Option value="lucy">Lucy</Option>
                      <Option value="Yiminghe">yiminghe</Option>
                    </Select>
                  )}
                </FormItem>
              </Col>
              <Col span={12}>
                <FormItem label="状态">
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
                <FormItem label="资产类型">
                  <Select>
                    <Option value="jack">Jack</Option>
                    <Option value="lucy">Lucy</Option>
                    <Option value="Yiminghe">yiminghe</Option>
                  </Select>
                </FormItem>
              </Col>
            </Row>
          </div>
          {/* 发行信息 */}
          <div className={styles.linput}>
            <h1>发行信息</h1>
            <Row>
              <Col span={12}>
                <FormItem label="交易所">
                  <Select>
                    <Option value="jack">Jack</Option>
                    <Option value="lucy">Lucy</Option>
                    <Option value="Yiminghe">yiminghe</Option>
                  </Select>
                </FormItem>
              </Col>
              <Col span={12}>
                <FormItem label="代码">
                  <Input />
                </FormItem>
              </Col>
            </Row>
            <Row>
              <Col span={12}>
                <FormItem label="发行方式">
                  <Select>
                    <Option value="jack">Jack</Option>
                    <Option value="lucy">Lucy</Option>
                    <Option value="Yiminghe">yiminghe</Option>
                  </Select>
                </FormItem>
              </Col>
              <Col span={12}>
                <FormItem label="发行金额">
                  <Input />
                </FormItem>
              </Col>
            </Row>
          </div>
          {/* 日期信息 */}
          <div className={styles.sinput}>
            <div>
              <h1>日期信息</h1>
              <Row>
                <Col span={8}>
                  <FormItem label="初始起算日">
                    {getFieldDecorator('time')(<DatePicker placeholder="" format="YYYY-MM-DD" />)}
                  </FormItem>
                </Col>
                <Col span={8}>
                  <FormItem label="簿记建档日">
                    <DatePicker placeholder="" />
                  </FormItem>
                </Col>
                <Col span={8}>
                  <FormItem label="产品成立日">
                    <DatePicker placeholder="" />
                  </FormItem>
                </Col>
              </Row>
              <Row>
                <Col span={8}>
                  <FormItem label="上市流通日">
                    <DatePicker placeholder="" />
                  </FormItem>
                </Col>
                <Col span={8}>
                  <FormItem label="法定到期日">
                    <DatePicker placeholder="" />
                  </FormItem>
                </Col>
                <Col span={8}>
                  <FormItem label="日历">
                    <Select>
                      <Option value="jack">Jack</Option>
                      <Option value="lucy">Lucy</Option>
                      <Option value="Yiminghe">yiminghe</Option>
                    </Select>
                  </FormItem>
                </Col>
              </Row>
            </div>
            {/* 证券偿付日 */}
            <div>
              <h3>证券偿付日</h3>
              <DateComponent getFieldDecorator={getFieldDecorator} moon="a" />
            </div>
            {/* 收款日期 */}
            <div>
              <h3>收款日期</h3>
              <DateComponent getFieldDecorator={getFieldDecorator} moon="b" />
            </div>
          </div>
          <FormItem>
            <Button type="primary" htmlType="submit">
              Submit
            </Button>
          </FormItem>
        </Form>
      </div>
    );
  }
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
      type: Form.createFormField({
        ...props.type,
      }),
      amoon: Form.createFormField({
        ...props.amoon,
      }),
      bmoon: Form.createFormField({
        ...props.bmoon,
      }),
      time: Form.createFormField({ ...props.time }),
    };
  },
})(FormLayout);

export default WrappedHorizontalLoginForm;
