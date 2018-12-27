import * as React from 'react';
import { connect } from 'dva';
import { ABSButton } from '../../../../../components/ABSButton';
import ABSRegisteredProcess from '../../../../../components/ABSRegisteredProcess';
import { ABSUploadCard } from '../../../../../components/ABSUploadCard';
import './index.less';
import ABSMessage from '../../../../../components/ABSMessage';
import routerConfig from '../../../../config/routeConfig';

class Overseas extends React.Component<any, any> {
  constructor(props: any) {
    super(props);
    this.state = {
      fileCode: '',
      errorMsg: ''
    };
  }

  updateFileCode(fileCode: string) {
    this.setState({ fileCode });
    if (fileCode && fileCode !== '') {
      this.setState({ errorMsg: '' });
    }
  }

  onSubmit = (e) => {
    const { fileCode } = this.state;

    if (!fileCode || fileCode === '') {
      this.setState({ errorMsg: '请上传名片' });
      return;
    }

    this.props.dispatch({
      type: 'account/registerOverseas',
      payload: {
        file_code: fileCode
      }
    }).then((response) => {
      if (!response) {
        return;
      }
      if (response.is_success) {
        location.href = routerConfig.registerSuccess;
      } else {
        ABSMessage.error(response.fail_msg);
      }
    });
  }

  render() {
    const {errorMsg} = this.state;

    return (
      <div className="abs-overseas">
        <ABSRegisteredProcess />
        <div className="abs-overseas-upload">
          <ABSUploadCard updateFileCode={fileCode => this.updateFileCode(fileCode)} />
          <span className="abs-overseas-upload-errorMsg">{errorMsg}</span> 
          <ABSButton onClick={this.onSubmit} htmlType="submit" className="abs-register-btn">提交</ABSButton>
        </div>
      </div>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.account };
}

export default connect(mapStateToProps)(Overseas); 