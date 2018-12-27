import React from 'react';
import ABSErrorPage from '../ABSErrorPage';

export default class ErrorBoundary extends React.Component<any, any> {

  constructor(props: any) {
    super(props);
    this.state = { hasError: false };
  }

  componentDidCatch(error: any, info: any) {
    console.error(error);
    this.setState({
      hasError: true,
    });
  }

  render() {
    if (this.state.hasError) {
      return (
        <div>
          <ABSErrorPage />
        </div>
      );
    }

    return this.props.children;
  }
}