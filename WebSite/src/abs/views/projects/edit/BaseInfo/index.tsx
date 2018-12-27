import React from 'react';
import ABSPanel from '../../../../../components/ABSPanel';
import ABSContainer from '../../../../../components/ABSContainer';

class BaseInfo extends React.Component<any, any> {
    render() {
        return (
            <ABSContainer>
                <ABSPanel title="基础信息">
                    <div>test</div>
                </ABSPanel>
                <ABSPanel title="发行信息">
                    <div>test</div>
                </ABSPanel>
                <ABSPanel title="证券偿付日期">
                    <div>test</div>
                </ABSPanel>
                <ABSPanel title="收款归集日期">
                    <div>test</div>
                </ABSPanel>
                <ABSPanel title="相关机构">
                    <div>test</div>
                </ABSPanel>
            </ABSContainer>
        );
    }
}

export default BaseInfo;