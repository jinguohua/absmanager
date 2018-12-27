import React from 'react';
import { ABSStructure, INoteData } from '../../../../components/ABSStructure';

export interface IABSUIStructureProps {
  
}
 
export interface IABSUIStructureState {
  
}
 
class ABSUIStructure extends React.Component<IABSUIStructureProps, IABSUIStructureState> {
  render() { 
    const data: Array<INoteData> = [
      {
        noteId: 1523,
        name: 'A1',
        isEquity: false,
        rating: 'AAA',
        notional: 39.5,
        principal: 0,
        // orderNumber: 1,
        hasShot: false,
        // securityTypeId: 1,
        // ratingId: 2,
      },
      {
        noteId: 1524,
        name: 'A2',
        isEquity: false,
        rating: 'AAA',
        notional: 45.5,
        principal: 0,
        // orderNumber: 2,
        hasShot: false,
        // securityTypeId: 1,
        // ratingId: 2,
      },
      {
        noteId: 1525,
        name: 'A3',
        isEquity: false,
        rating: 'AAA',
        notional: 35,
        principal: 0,
        // orderNumber: 3,
        hasShot: false,
        // securityTypeId: 1,
        // ratingId: 2
      },
      {
        noteId: 1526,
        name: 'B',
        isEquity: false,
        rating: 'AA-',
        notional: 9.9,
        principal: 0,
        // orderNumber: 4,
        hasShot: false,
        // securityTypeId: 2,
        // ratingId: 6
      },
      {
        noteId: 1527,
        name: 'Sub',
        isEquity: true,
        rating: 'NR',
        notional: 9.7486,
        principal: 8.59241604,
        // orderNumber: 5,
        hasShot: false,
        // securityTypeId:3,
        // ratingId: 25
      }
    ];
    return <ABSStructure data={data} />;
  }
}
 
export default ABSUIStructure;