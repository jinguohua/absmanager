import React, { Component } from 'react';
import Information from './information';
import ParticipateProject from './ParticipateProject';
import ABSContainer from '../../../../components/ABSContainer';
import DistributionInfo from './DistributionInfo';
import ABSMinContainer from '../../../../components/ABSMinContainer';
import './index.less';

class Originator extends Component {
  render() {
    return (
      <ABSContainer>
        <ABSMinContainer>
          <Information />
          <ParticipateProject />
          <DistributionInfo />
        </ABSMinContainer>
      </ABSContainer>
    );
  }
}

export default Originator;