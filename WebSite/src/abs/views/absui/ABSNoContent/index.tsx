import React from 'react';
import ABSNoContent from '../../../../components/ABSNoContent';
import './index.less';

export interface IABSNoContentSampleProps {
  
}
 
export interface IABSNoContentSampleState {
  
}
 
class ABSNoContentSample extends React.Component<IABSNoContentSampleProps, IABSNoContentSampleState> {
  render() { 
    return (
      <div className="abs-no-content-sample">
        <ABSNoContent />
      </div>
    );
  }
}
 
export default ABSNoContentSample;