import * as React from 'react';
import './index.less';
import { ABSAntIcon } from '../../../../../../components/ABSAntIcon';
import routerConfig from '../../../../../config/routeConfig';

export default class RegisterSuccess extends React.Component<any, any> {
  constructor(props: any) {
    super(props);
    this.state = {
      second: 5,
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
      location.href = routerConfig.introdution;
    }
  }

  render() {
    const { second } = this.state;

    return (
      <div className="abs-registerSuccess">
        <div className="abs-registerSuccess-title">
          <ABSAntIcon className="abs-ant-icon-l" type="check-circle" theme="filled" style={{ color: '#52C41A' }} />
          <span className={'abs-registerSuccess-title-span'}> 资料已成功提交</span>
        </div>
        <div className="abs-registerSuccess-content">
          尊敬的用户，你好！我们已收到您提交的个人资料，我们会在24小时内审核完成，
          并以邮件形式通知到您，请您耐心等待。
            页面将在 <span className="abs-registerSuccess-content-second">{second}</span> 秒后，
            自动跳转到 <a href={routerConfig.introdution}>CNABS主页</a> 。
        </div>
      </div>
    );
  }
}