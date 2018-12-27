import { React, Spin, alert, DatePicker } from 'react';
import './ProjectSearch.css';
import { Form, Input, Col, Row, Button } from 'antd';
import SAFSSelect from '../SAFSSelect/index';

const FormItem = Form.Item;
const styleSelect = {
  width: 200,
};
const styleDate = {};
const { dateFormat } = 'YYYY-MM-DD';
class ProjectSearch extends React.Component {
  constructor(props) {
    super(props);
    this.state = {};
  }

  handleSubmit = e => {
    e.preventDefault();

    this.validateFields((err, values) => {
      // var params = values;
      // this.setState({
      //   name: values.name,
      //   startDate: values.startDate,
      //   endDate: values.endDate,
      //   type: values.type,
      //   status: values.values,
      // });

      const filters = [
        {
          field: 'name',
          value: values.name,
          rule: 'like',
        },
        {
          field: 'startDate',
          value: values.startDate != null ? values.startDate.format(dateFormat) : null,
          rule: '>=',
        },
        {
          field: 'endDate',
          value: values.endDate != null ? values.endDate.format(dateFormat) : null,
          rule: '<=',
        },
        {
          field: 'projectType',
          value: values.projectType,
          rule: 'eq',
        },
        {
          field: 'status',
          value: values.status,
          rule: 'eq',
        },
      ];
      this.handleSearchP(filters);
    });
  };

  handleQuery = () => {
    alert('query...');
    this.handle();
  };

  handleClear = () => {
    alert('clear...');
  };

  render() {
    const {
      form: { getFieldDecorator },
    } = this.props;
    const { fetching } = this.props;
    const { name } = this.state;
    const { showSearch } = true;
    return (
      <div>
        {fetching && <Spin />}

        <Form onSubmit={this.handleSubmit} className="login-form">
          <Row>
            <Col span={1}>
              <strong>名称</strong>
            </Col>
            <Col span={4}>
              <FormItem>
                {getFieldDecorator(`name`, {
                  rules: [
                    {
                      required: false,
                      message: 'Input something!',
                    },
                  ],
                })(<Input style={styleSelect} defaultValue="产品" value={name} />)}
              </FormItem>
            </Col>
            <Col span={2}>
              {' '}
              <strong>发行日期</strong>
            </Col>
            <Col span={3}>
              <FormItem>
                {getFieldDecorator(`startDate`, {
                  rules: [
                    {
                      required: false,
                      message: 'Input something!',
                    },
                  ],
                })(<DatePicker style={styleDate} />)}
              </FormItem>
            </Col>
            <Col span={3}>
              <FormItem>
                {getFieldDecorator(`endDate`, {
                  rules: [
                    {
                      required: false,
                      message: 'Input something!',
                    },
                  ],
                })(<DatePicker style={styleDate} />)}
              </FormItem>
            </Col>
            <Col span={2}>
              <Button type="primary" htmlType="submit">
                查询
              </Button>
            </Col>
          </Row>
          <Row>
            <Col span={1}>
              <strong>类型</strong>
            </Col>
            <Col span={4}>
              <FormItem>
                {getFieldDecorator(`projectType`)(
                  <SAFSSelect style={styleSelect} showSearch={showSearch} category="ProjectType" />
                )}
              </FormItem>
            </Col>
            <Col span={2}>
              <strong>状态</strong>
            </Col>
            <Col span={6}>
              <FormItem>
                {getFieldDecorator(`status`)(
                  <SAFSSelect
                    style={styleSelect}
                    showSearch={showSearch}
                    category="num.EProjectStatus"
                  />
                )}
              </FormItem>
            </Col>
            <Col span={2}>
              <Button type="dashed">清空</Button>
            </Col>
          </Row>
        </Form>
      </div>
    );
  }
}

export default ProjectSearch;
