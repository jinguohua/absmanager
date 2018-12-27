import * as React from 'react';
import './index.less';
import ABSSteps from '../../../../../components/ABSSteps';
import { connect } from 'dva';
import Phone from './Phone';
import Account from './Account';
import User from './User';

interface IState {
  phoneForm: object;
  accountForm: object;
  defaultSelectValue: number;
  channelData: any;
}

class Domestic extends React.Component<any, IState> {
  step: ABSSteps | null;
  constructor(props: any) {
    super(props);
    this.state = {
      phoneForm: {},
      accountForm: {},
      defaultSelectValue: 0,
      channelData: []
    };
  }

  componentDidMount() {
    this.props.dispatch({
      type: 'global/filter',
      payload: {
        filter_subtype_id: 21
      }
    }).then((response) => {
      if (response) {
        const selectItem = response.find(r => r.selected);
        this.setState({
          defaultSelectValue: selectItem ? selectItem.key : 0,
          channelData: this.parseData(response)
        });
      }
    });
  }

  parseData = (data) => {
    return data.map((item) => {
      return {
        value: item.key,
        labelName: item.value,
        className: 'abs-select'
      };
    });
  }

  onNext = (formParams) => {
    if (this.step) {
      const current = this.step.next();
      if (current === 1) {
        this.setState({ phoneForm: formParams });
      }
      if (current === 2) {
        this.setState({ accountForm: formParams });
      }
    }
  }

  render() {
    const { phoneForm, accountForm, channelData, defaultSelectValue } = this.state;
    // const {phoneForm, accountForm} = this.state;

    const steps = [
      {
        title: '验证手机号',
        component: () => <Phone onClick={this.onNext} />,
      },
      {
        title: '账号信息',
        component: () => <Account onClick={this.onNext} channelData={channelData} defaultSelectValue={defaultSelectValue} />,
      },
      {
        title: '用户信息',
        component: () => <User onClick={this.onNext} phoneForm={phoneForm} accountForm={accountForm} />,
      }];

    return (
      <div className="abs-domestic">
        <ABSSteps className="abs-domestic-steps" ref={step => this.step = step} initial={0} steps={steps} />
      </div>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.global };
}

export default connect(mapStateToProps)(Domestic); 