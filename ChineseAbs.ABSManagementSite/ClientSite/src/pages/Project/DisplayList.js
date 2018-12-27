import React, { PureComponent } from 'react';
import ProductFilterable from '@/components/Project/ProductFilterable';

class DisplayList extends PureComponent {
  constructor() {
    super();
    this.state = {};
  }

  componentDidMount() {}

  componentWillUnmount() {
    this.hasUnmount = true;
  }

  render() {
    return (
      <div>
        <h1>产品列表</h1>
        <ProductFilterable />
      </div>
    );
  }
}

export default DisplayList;
