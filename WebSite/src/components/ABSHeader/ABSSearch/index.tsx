import * as React from 'react';
import { connect } from 'dva';
import { Input, Popover } from 'antd';
import ABSLoading from '../../../components/ABSLoading';
import ABSIcon from '../../../components/ABSIcon';
import SerachHelper from './serachHelper';
import './index.less';
import ABSLink from '../../ABSLink';
import routeConfig from '../../../abs/config/routeConfig';

interface IResult {
  title: string;
  description: string;
  url: string;
}

interface ISearchResult {
  type: string;
  results: Array<IResult>;
}

interface IState {
  loading: boolean;
  visible: boolean;
  keyword: string;
  results: Array<ISearchResult>;
  selectKey: any;
}

class ABSSearch extends React.Component<any, IState> {
  constructor(props: any) {
    super(props);
    this.state = {
      loading: false,
      visible: false,
      selectKey: null,
      keyword: '',
      results: [],
    };
  }

  componentDidMount() {
    document.onclick = () => this.setState({ loading: false, visible: false, results: [] });
  }

  // 判断浏览器是否支持placeholder属性
  isSupportPlaceholder = () => {
    let input = document.createElement('input');
    return 'placeholder' in input;
  }

  // 键盘事件
  handleKeyDown = (e) => {
    var evt = window.event || e;
    // 【Enter事件】
    if (evt.keyCode === 13) {
      location.href = `${routeConfig.search}?keyword=${this.state.keyword}`;
    }

    return;
    
    /*** TODO: 由于性能问题暂时隐藏上下键切换功能，后续需要开放 ***/
    /*
    const { results, selectKey } = this.state;
    // 【Enter事件】
    if (evt.keyCode === 13) {
      if (!selectKey || selectKey === '') {
        location.href = `${routeConfig.search}?keyword=${this.state.keyword}`;
      } else {
        const type = selectKey.split('&')[0];
        const index = Number(selectKey.split('&')[1]);
        const selectResult = results.find(r => r.type === type);
        if (selectResult) {
          const selectItem = selectResult.results[index];
          if (selectItem) {
            location.href = selectItem.url;
          }
        }
      }
    }

    if (results.length <= 0) {
      return;
    }

    // 【上】事件
    if (evt.keyCode === 38) {
      let type;
      let index;
      let newSelectKey;

      // 当前有选中时查找上一个
      if (selectKey) {
        type = selectKey.split('&')[0];
        index = Number(selectKey.split('&')[1]);
        const newIndex = index - 1;
        const selectResult = results.find(r => r.type === type);

        // 在当前Tag中时
        if (selectResult && newIndex >= 0 && newIndex < selectResult.results.length) {
          index = newIndex;
        } else {
          // 不在当前Tag中时查找上一Tag
          const resultIndex = results.findIndex(r => r.type === type) - 1;
          if (resultIndex >= 0 && resultIndex < results.length) {
            type = results[resultIndex].type;
            index = results[resultIndex].results.length - 1;
          }
        }
      }

      newSelectKey = this.parseSelectKey(type, index);
      this.setState({ selectKey: newSelectKey });

      this.resetScrollBottom(type, index);
    }

    // 【下】事件
    if (evt.keyCode === 40) {
      let type;
      let index;
      let newSelectKey;

      // 当前有选中时查找下一个
      if (selectKey) {
        type = selectKey.split('&')[0];
        index = Number(selectKey.split('&')[1]);
        const selectResult = results.find(r => r.type === type);

        // 在当前Tag中时
        if (selectResult && (index + 1) < selectResult.results.length) {
          index = index + 1;
        } else {
          // 不在当前Tag中时查找下一Tag
          const resultIndex = results.findIndex(r => r.type === type) + 1;
          if (resultIndex < results.length) {
            type = results[resultIndex].type;
            index = 0;
          }
        }
      } else {
        // 当前无选中时使用第一个
        index = 0;
        type = results[0].type;
      }

      newSelectKey = this.parseSelectKey(type, index);
      this.setState({ selectKey: newSelectKey });
      this.resetScrollTop(type, index);
    }

    */
  }

  parseSelectKey = (type, index) => {
    return type + '&' + index;
  }

  resetScrollTop = (type, index) => {
    const { results } = this.state;
    const securities = results.find(r => r.type === '证券');
    const ele: any = document.getElementById('my-table');
    const securityLength = securities ? securities.results.length : 0;

    if (securityLength >= 9) {
      if (type === '证券') {
        if (index >= 8) {
          ele.scrollTop = ele.scrollTop + 50;
        }
      } else {
        ele.scrollTop = ele.scrollTop + 50;
      }
      return;
    }

    if (type === '证券') { return; }
    const deals = results.find(r => r.type === '产品');
    const dealLength = deals ? deals.results.length : 0;
    if (securityLength + dealLength >= 9) {
      if (type === '产品') {
        if (index >= (8 - securityLength)) {
          ele.scrollTop = ele.scrollTop + 50;
        }
      } else {
        ele.scrollTop = ele.scrollTop + 50;
      }

      return;
    }

    if (type === '证券' || type === '产品') { return; }
    const organizations = results.find(r => r.type === '机构');
    const organizationLength = organizations ? organizations.results.length : 0;
    if (securityLength + dealLength + organizationLength >= 9) {
      if (type === '机构') {
        if (index >= (8 - securityLength - dealLength)) {
          ele.scrollTop = ele.scrollTop + 50;
        }
      } else {
        ele.scrollTop = ele.scrollTop + 50;
      }
    }

    if (type !== '专家') { return; }
    const experts = results.find(r => r.type === '专家');
    const expertLength = experts ? experts.results.length : 0;
    if (expertLength >= 9) {
      if (index >= (8 - securityLength - dealLength - organizationLength)) {
        ele.scrollTop = ele.scrollTop + 50;
      }

      return;
    }
  }

  resetScrollBottom = (type, index) => {
    const { results } = this.state;
    const securities = results.find(r => r.type === '证券');
    const ele: any = document.getElementById('my-table');
    const securityLength = securities ? securities.results.length : 0;

    if (securityLength >= 9) {
      if (type === '证券') {
        if (index >= 7) {
          ele.scrollTop = ele.scrollTop - 50;
        }
      } else {
        ele.scrollTop = ele.scrollTop - 50;
      }
      return;
    }

    if (type === '证券') { return; }
    const deals = results.find(r => r.type === '产品');
    const dealLength = deals ? deals.results.length : 0;
    if (securityLength + dealLength >= 9) {
      if (type === '产品') {
        if (index >= (8 - securityLength)) {
          ele.scrollTop = ele.scrollTop - 50;
        }
      } else {
        ele.scrollTop = ele.scrollTop - 50;
      }

      return;
    }

    if (type === '证券' || type === '产品') { return; }
    const organizations = results.find(r => r.type === '机构');
    const organizationLength = organizations ? organizations.results.length : 0;
    if (securityLength + dealLength + organizationLength >= 9) {
      if (type === '机构') {
        if (index >= (8 - securityLength - dealLength)) {
          ele.scrollTop = ele.scrollTop - 50;
        }
      } else {
        ele.scrollTop = ele.scrollTop - 50;
      }
    }
  }

  // 当搜索框改变时
  handleSearchChange = (e) => {
    e.nativeEvent.stopImmediatePropagation();
    const serachValue = e.target.value.trim();
    if (serachValue === '') {
      this.setState({ loading: false, visible: false, results: [] });
      return;
    }
    this.setState({ loading: true, keyword: serachValue });

    this.props.dispatch({
      type: 'global/search',
      payload: {
        keyword: serachValue
      }
    }).then((response) => {
      this.setState({
        loading: false,
      });

      if (!response) {
        return;
      }

      this.setState({
        visible: true,
        results: SerachHelper.parseResults(response),
      });
    });
  }

  render() {
    const { loading, visible, keyword, results, selectKey } = this.state;
    const inputSuffix = loading ? <ABSLoading size="small" color="blue" />
      : <ABSIcon type="search" />;

    const popoverContent = (
      <table className="abs-search-popover-content" id="my-table">
        <tbody>
          {
            results.map((item) => {
              return (
                item.results.map((resultItem, index) => {
                  return (
                    <tr className={'abs-search-popover-content-item'} key={resultItem.title}>
                      {index === 0 ? <td rowSpan={item.results.length} className="abs-search-popover-content-item-tag">{item.type}</td> : null}
                      <td className={selectKey === this.parseSelectKey(item.type, index) ? 'abs-search-popover-content-item-content abs-search-popover-content-item-content-select' : 'abs-search-popover-content-item-content'} >
                        <ABSLink to={resultItem.url} target="_blank">
                          <div className="abs-search-popover-content-item-content-div" key={resultItem.title} >
                            <span className="abs-search-popover-content-item-content-title">{resultItem.title}</span>
                            <span className="abs-search-popover-content-item-content-description">{resultItem.description}</span>
                          </div>
                        </ABSLink>
                      </td>
                    </tr>
                  );
                })
              );
            })
          }
          <tr className="abs-search-popover-content-item">
            <td className="abs-search-popover-content-item-content abs-search-popover-content-item-emtry">&nbsp;</td>
          </tr>
          <tr className="abs-search-popover-content-item">
            <td colSpan={2} className="abs-search-popover-content-item-footer">
              <ABSLink to={`${routeConfig.search}?keyword=${keyword}`}>
                <div className="abs-search-popover-content-item-footer-div">
                  查看更多<ABSIcon type="direction-right" />
                </div>
              </ABSLink>
            </td>
          </tr>
        </tbody>
      </table>
    );

    return (
      <div className="abs-search">
        <Popover placement={'bottomLeft'} content={popoverContent} visible={visible} overlayClassName="abs-search-popover">
          <div className="abs-search-div" >
            <Input className="abs-search-input" id="abs-search-input-element"
              placeholder="证券、产品"
              suffix={inputSuffix}
              onChange={(e) => this.handleSearchChange(e)}
              onClick={(e) => this.handleSearchChange(e)}
              onKeyDown={(e) => this.handleKeyDown(e)}
            />
          </div>
        </Popover>
      </div>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.global };
}

export default connect(mapStateToProps)(ABSSearch);