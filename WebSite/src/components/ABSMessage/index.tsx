import React from 'react';
import { message } from 'antd';
import { ABSAntIcon } from '../ABSAntIcon';
import './index.less';
import ABSParagraph from '../ABSParagraph';

declare type ConfigContent = React.ReactNode | string;
declare type ConfigOnClose = () => void;
interface IIconNodeProps {
  type: string;
}

const durationNumber = 5;

function IconNode (props: IIconNodeProps) {
  return (
    <div className="abs-message-div-icon">
      <span />
      <ABSAntIcon className="abs-ant-icon-m" type={props.type} theme="filled" />
    </div>
  );
}
class Message {
  public success(content: ConfigContent, onClose?: ConfigOnClose) {
    message.open({ content: <ABSParagraph style={{display: 'inline-block'}}>{content}</ABSParagraph>, duration: durationNumber, type: 'success', onClose, icon: <IconNode type="check-circle" />});
  }
  public error(content: ConfigContent, onClose?: ConfigOnClose) {
    message.open({ content: <ABSParagraph style={{display: 'inline-block'}}>{content}</ABSParagraph>, duration: durationNumber, type: 'error', onClose, icon: <IconNode type="close-circle" />});
  }
  public warning(content: ConfigContent, onClose?: ConfigOnClose) {
    message.open({ content: <ABSParagraph style={{display: 'inline-block'}}>{content}</ABSParagraph>, duration: durationNumber, type: 'warning', onClose, icon: <IconNode type="exclamation-circle" />});
  }
}

message.config({
  top: 70,
  duration: durationNumber,
});

const ABSMessage = new Message();

export default ABSMessage;