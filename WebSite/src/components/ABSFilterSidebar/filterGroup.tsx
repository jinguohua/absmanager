import * as React from 'react';

interface IProps {
  title: string;
  content: React.ReactElement<any>;
}

class FilterGroup extends React.Component<IProps> {
  render() {
    const { title, content } = this.props;
    return (
      <div className="filter-group">
        <p>{title}</p>
        {content}
      </div>
    );
  }
}

export default FilterGroup;