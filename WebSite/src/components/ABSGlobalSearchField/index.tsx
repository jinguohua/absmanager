import React from 'react';
import { connect } from 'dva';
import { SearchType } from './utils/enum';
import ABSSearchField from '../ABSSearchField';
import ISearchOption from '../ABSSearchField/utils/ISearchOption';

export interface IGlobalSearchFieldProps {
  defaultValue?: string;
  dispatch: any;
  type: SearchType;
  handleChange: (id: number, text: string) => void;
  title: string;
  placeholder: string;
  disabled: boolean;
}

export interface IGlobalSearchFieldState {
  fetching: boolean;
  text: string;
  data: ISearchOption[];
}

const SEARCH_RESULT_COUNT = 10;

class ABSGlobalSearchField extends React.Component<IGlobalSearchFieldProps, IGlobalSearchFieldState> {
  lastFetchId: number = 0;

  constructor(props: IGlobalSearchFieldProps) {
    super(props);
    const { defaultValue } = props;
    this.state = {
      fetching: false,
      text: defaultValue ? defaultValue : '',
      data: [],
    };
  }

  componentWillReceiveProps(nextProps: IGlobalSearchFieldProps) {
    const { defaultValue } = nextProps;
    this.setState({ text: defaultValue ? defaultValue : '', });
  }

  fetchContent = (value: string) => {
    const { type } = this.props;
    if (!value || String(value).trim() === '') {
      this.setState({ data: [], fetching: false });
      return;
    }
    this.lastFetchId += 1;
    const fetchId = this.lastFetchId;
    this.setState({ data: [], fetching: true });
    this.props.dispatch({
      type: 'global/search',
      payload: {
        keyword: value,
        search_type: type,
        count: SEARCH_RESULT_COUNT,
      },
    }).then((response) => {
      if (fetchId !== this.lastFetchId || !Array.isArray(response)) { return; }
      const data: ISearchOption[] = response.map(item => {
        const { id, title, description } = item;
        return {
          text: title,
          value: id,
          desc: description,
        };
      });
      this.setState({ data, fetching: false });
    });
  }

  handleChange = (value: number, option: any) => {
    const { props } = option;
    const text = props && props.title ? props.title : '';
    const { handleChange } = this.props;
    handleChange(value, text);
    this.setState({
      text,
      data: [],
      fetching: false,
    });
  }

  render() {
    const {
      data,
      text,
      fetching,
    } = this.state;
    const { disabled } = this.props;
    const { title, placeholder } = this.props;
    return (
      <ABSSearchField
        title={title}
        placeholder={placeholder}
        fetchContent={this.fetchContent}
        data={data}
        value={text}
        fetching={fetching}
        handleChange={this.handleChange}
        disabled={disabled}
      />
    );
  }
}

const mapStateToProps = ({ global }) => {
  return { ...global };
};

export default connect(mapStateToProps)(ABSGlobalSearchField);