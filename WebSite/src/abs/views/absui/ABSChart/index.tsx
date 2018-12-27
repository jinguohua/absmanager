import React, { Component } from 'react';
import ABSChart from '../../../../components/ABSChart';
import getDemoConfig from './demoData';

export interface IAppProps {
}
export interface IAppState {
}
class ABSChartRoute extends Component<IAppProps, IAppState> {
    render() {
        const config = getDemoConfig();
        return (
            <ABSChart config={config} />
        );
    }
}

export default ABSChartRoute;