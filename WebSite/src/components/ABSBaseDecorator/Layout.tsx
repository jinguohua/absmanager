import React from 'react';

export interface IRootLayoutProps {
  children: React.ReactNode;
}
 
export interface IRootLayoutState {
  
}
 
class RootLayout extends React.Component<IRootLayoutProps, IRootLayoutState> {
  render() { 
    const { children } = this.props;
    return children;
  }
}
 
export default RootLayout;