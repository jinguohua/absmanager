import React from 'react';
import './ABSColorPalette.less';

export interface IABSColorPaletteProps {
  color: string;
}
 
class ABSColorPalette extends React.Component<IABSColorPaletteProps> {
  render() { 
    const { color } = this.props;
    return (
      <div className="abs-color-palette" style={{backgroundColor: color}} />
    );
  }
}
 
export default ABSColorPalette;