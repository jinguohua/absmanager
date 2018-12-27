import React, { Component } from 'react';
import ABSProductOrSecurityListRoute from '../index';

class ProductListRoute extends Component {
  render() {
    return (
      <ABSProductOrSecurityListRoute type="product" />
    );
  }
}

export default ProductListRoute;