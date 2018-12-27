import React from 'react';
import { Switch } from 'antd';
import classNames from 'classnames';
import './index.less';

export declare type switchSize = 's' | 'm';
export declare type switchType = 'on' | 'off' | 'disabled' ;

export interface IABSSwitchProps {
  type: switchType;
  size: switchSize;
  disabled?: boolean;
  switchChangeFun?: any;
  className?: string;
  defaultChecked?: boolean;
}
export class ABSSwitch extends React.Component<IABSSwitchProps> {
  render() { 
    const { type, size, disabled, switchChangeFun, className, defaultChecked } = this.props;
    const classes = classNames('', className, {
      [`abs-switch-${type}-${size}`]: true,
    });
    return(
      <div className="abs-switch">
        <Switch 
          className={classes}          
          disabled={disabled} 
          onChange={switchChangeFun} 
          defaultChecked={defaultChecked}
        />        
      </div>
    );
  }
}