import * as React from 'react';
import { Badge } from 'antd';
import './index.less';
import ABSIcon from '../../ABSIcon';

interface IProps {
  data: any;
}

interface IState {
  badgeClassName: string;
  visible: boolean;
}

class ABSNotice extends React.Component<IProps, IState> {
  constructor(props: any) {
    super(props);
    this.state = {
      badgeClassName: 'abs-badge-small',
      visible: false,
    };
  }

  componentDidMount() {
    const self = this;
    // 有新消息时闪烁小红点，1秒1次
    if (this.props.data > 0) {
      setInterval(function () {
        self.setState({ badgeClassName: self.state.badgeClassName === 'abs-badge-big' ? 'abs-badge-small' : 'abs-badge-big' });
      }, 1000);
    }
  }

  render() {
    const { data } = this.props;
    const { badgeClassName } = this.state;

    return (
      <a href="#">
        <div className="abs-notice">
          <Badge className={badgeClassName} dot={data > 0}>
            <ABSIcon type="bell" />
          </Badge>
        </div>
      </a>
    );
  }
}

export default ABSNotice;