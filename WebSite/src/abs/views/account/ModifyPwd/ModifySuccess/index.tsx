import * as React from 'react';
import './index.less';
import { ABSAntIcon } from '../../../../../components/ABSAntIcon';
// import routerConfig from '../../../../config/routeConfig';
import { connect } from 'dva';

class ModifySuccess extends React.Component<any, any> {
  constructor(props: any) {
    super(props);
    this.state = {
      second: 3,
    };
  }

  componentDidMount() {
    setInterval(this.startCount, 1000);
  }

  // 5s倒计时
  startCount = () => {
    const { second } = this.state;

    if (second > 0) {
      this.setState({ second: second - 1 });
    } else {
      this.onGoHome();
      // location.href = routerConfig.login;
    }
  }

  onGoHome = () => {
    this.props.dispatch({
      type: 'global/logout',
    });
  }

  render() {
    const { second } = this.state;

    return (
      <div className="abs-ModifySuccess">
        <div className="abs-ModifySuccess-title">
          <ABSAntIcon className="abs-ant-icon-l" type="check-circle" theme="filled" style={{ color: '#52C41A' }} />
          <span className={'abs-ModifySuccess-title-span'}> 密码修改成功</span>
        </div>
        <div className="abs-ModifySuccess-content">
          页面将在 <span className="abs-ModifySuccess-content-second">{second}</span> 秒后，
            退出 <a onClick={this.onGoHome}>请重新登录</a> 。
        </div>
      </div>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.account };
}

export default connect(mapStateToProps)(ModifySuccess); 