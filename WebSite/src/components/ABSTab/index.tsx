import React from 'react';
import { Tabs } from 'antd';
import './index.less';

const TabPane = Tabs.TabPane;

export interface IABSTabProps {
  panesList: any;
  activeKey: string;
  type?: string;
  onChange?: (activeKey: string) => void;
  className?: string;
}

export class ABSTab extends React.Component<IABSTabProps> {
  static defaultProps = {
    type: null,
    activeKey: '1',
  };
  
  render() {
    const { panesList, activeKey, type, onChange, className } = this.props;
    const classNames = type ? `abs-tab-${type}` : `abs-tab`;
    return (
      <div className={classNames}>
        <Tabs defaultActiveKey={activeKey} onChange={onChange} className={className}>
          {
            panesList.map(pane => 
              <TabPane tab={pane.title} key={pane.key}>
                {pane.content}
              </TabPane>
            )
          }
        </Tabs>
      </div>
    );
  }
}