import React from 'react';
import './index.less';
import '../ABSChart/index.less';
import chartTheme, { specialColor } from '../../components/ABSChart/chartTheme';
import ABSNoContent from '../../components/ABSNoContent';
import ABSLink from '../ABSLink';
import RouteConfig from '../../abs/config/routeConfig';

const px = 'px';
const defaultOption = {
  width: 218,
  height: 211,
  minWidth: 30,
  minHeight: 30,
  // for more artistic, should be odd number
  maxCols: 5,
};
export interface INoteData {
  noteId: number;
  name: string;
  isEquity: boolean;
  rating: string;
  notional: number;
  principal: number;
  hasShot: boolean;
}
interface IStructureProps {
  data: INoteData[];
  title?: string;
  url?: string;
}
interface IStructureState {
  structureState: {
    data: INoteData[];
    layerData: INoteData[];
    totalNotional: number;
    minLayerNum: number;
    minLayerNotional: number;
    minNoteNum: number;
    minNoteNotional: number;
    accuNotional: number;
    accuPercent: number;
  };
  chartColors: string[];
}
export class ABSStructure extends React.Component<IStructureProps, IStructureState> {

  public static defaultProps = {
    title: '证券结构图',
  };

  constructor(props: any) {
    super(props);
    this.state = {
      structureState: {
        data: this.props.data,
        layerData: [],
        totalNotional: 0,
        minLayerNum: 0,
        minLayerNotional: 0,
        minNoteNum: 0,
        minNoteNotional: 0,
        accuNotional: 0,
        accuPercent: 0
      },
      chartColors: chartTheme.colors
    };
  }
  initialState() {
    this.state = {
      structureState: {
        data: this.props.data,
        layerData: [],
        totalNotional: 0,
        minLayerNum: 0,
        minLayerNotional: 0,
        minNoteNum: 0,
        minNoteNotional: 0,
        accuNotional: 0,
        accuPercent: 0
      },
      chartColors: chartTheme.colors,
    };
  }
  render() {
    this.initialState();
    if (!this.props.data || this.props.data.length === 0) {
      return <ABSNoContent />;
    }
    const { title, url } = this.props;
    const isShot =
      this.props.data.filter((element: any) => {
        return element.hasShot === true;
      }).length === 1;
    return (
      <div className="abs-box-3">
        {url ? <ABSLink to={url} children={<div className="abs-chart-titles-url">{title}</div>} /> : <div className="abs-chart-titles">{title}</div>}
        {this.buildStructureHtml(isShot)}
        <div className="abs-structure-legend">
          {
            isShot ?
              <div className="abs-structure-legend-item">
                <div className="abs-structure-legend-payed" />
                <span className="">已偿付</span>
                <div className="abs-structure-legend-current" />
                <span className="">当前证券</span>
              </div>
              : <div className="abs-structure-legend-item">
                <div className="abs-structure-legend-payed" />
                <span className="">已偿付</span>
              </div>
          }
        </div>
      </div>
    );
  }

  // 没有评级的证券分层
  getLayerWithoutRating(options: any) {
    let allData: INoteData[] = [];
    allData.push(options);
    return allData;
  }
  // 有评级的证券分层
  getLayerWithRating(options: any) {
    let data = options;
    let ratings = Array.from(
      new Set(
        data.map((item, index) => {
          return item.rating;
        })));
    let allData: INoteData[] = [];
    ratings.forEach((item: any) => {
      // 筛选评级为NR或者null的非次级证券 筛选评级为NR或者null的次级证券
      if (item === null || item === 'NR') {
        allData.push(data.filter(function (r: any) {
          return r.rating === item && !r.isEquity;
        }));
        allData.push(data.filter(function (r: any) {
          return r.rating === item && r.isEquity;
        }));
      } else {
        allData.push(data.filter(function (r: any) {
          return r.rating === item;
        }));
      }
    });
    return allData;
  }
  // 获取每个证券的结构
  buildEachNoteHtml(layer: INoteData[], divHegiht: any, hasLeftExtra: boolean, hasRightExtra: boolean, isShot: boolean) {
    let layerNotional = 0;
    layer.forEach(function (r: any) {
      return layerNotional += r.notional;
    });
    // determine the min height num
    let layerArr: boolean[] = [];
    let that = this;
    layer.forEach(function (r: any, index: number) {
      if (r.notional / layerNotional < defaultOption.minWidth / defaultOption.width) {
        that.state.structureState.minNoteNum++;
        that.state.structureState.minNoteNotional += r.notional;
        layerArr[index] = true;
      } else {
        layerArr[index] = false;
      }
    });
    let allDiv: JSX.Element[] = [];
    layer.forEach(function (note: any, idx: number) {
      // 定义宽度
      let noteWidth = layerArr[idx] ? defaultOption.minWidth :
        note.notional / (layerNotional - that.state.structureState.minNoteNotional) *
        (defaultOption.width - that.state.structureState.minNoteNum * (defaultOption.minWidth + 2)) - 2;
      let adjWidth = ((layer.length > 1 && idx === layer.length - 1) ? noteWidth : noteWidth) + px;
      // 添加显示文本
      let span;
      if (hasLeftExtra && idx === 0 || hasRightExtra && idx === layer.length - 1) {
        span = (<span className="structure-inner-text" style={{ lineHeight: divHegiht + px, width: adjWidth, color: note.hasShot ? '#000' : '#fff' }}>...</span>);
      } else {
        span = (
          <span className="structure-inner-text" style={{ width: adjWidth, lineHeight: divHegiht + px }}>
            <span className="structure-comment-text">{that.formatPercent(note.name, note.notional, that.state.structureState, note.rating)}</span>
          </span>
        );
      }
      // 添加偿付信息
      let noteDiv = (
        <div key={idx} style={{ height: divHegiht + px, width: adjWidth, backgroundColor: (isShot ? (note.hasShot ? specialColor : chartTheme.colors[0]) : chartTheme.colors[note.noteIndex]) }}
          className={note.hasShot ? (layer.length > 1 ? 'structure-select-content' : 'structure-select-single-content') : (layer.length > 1 ? 'structure-inner-content' : 'structure-inner-single-content')}>
          {span}
          <div className="structure-payment" style={{ height: (1 - note.principal / note.notional) * 100 + '%', width: adjWidth }} />
        </div>);
      allDiv.push(<ABSLink key={idx} to={`${RouteConfig.investmentSecurityInfo}${note.noteId}`}>{noteDiv}</ABSLink>);
    }, this);
    return allDiv;
  }
  // 获取每个层级的结构
  getLayerHtml(layer: INoteData[], isMinHeight: boolean, isShot: boolean) {
    let layerNotional = 0;
    layer.forEach(function (r: any) {
      return layerNotional += r.notional;
    });
    let divHegiht = isMinHeight ? defaultOption.minHeight
      : (layerNotional / (this.state.structureState.totalNotional - this.state.structureState.minLayerNotional) *
        (defaultOption.height - this.state.structureState.minLayerNum * defaultOption.minHeight));
    let outDiv = (<div>.</div>);
    this.state.structureState.minNoteNum = 0;
    this.state.structureState.minNoteNotional = 0;

    if (layer.length > defaultOption.maxCols) {
      // 寻找当前选中证券
      let selectIdx = -1;
      for (let i = 0; i < layer.length; i++) {
        if (layer[i].hasShot) {
          selectIdx = i;
          break;
        }
      }
      // 层级显示方式，包括使用'...'
      if (selectIdx === -1) {
        layer.length = defaultOption.maxCols;
        outDiv = (
          <div style={{ height: divHegiht + px, width: defaultOption.width + px }} className="structure-inner-container">
            {this.buildEachNoteHtml(layer, divHegiht, false, true, isShot)}
          </div>
        );
      } else {
        let idxContainer: number[] = [],
          // 存储最大列的证券
          transferFlag = true;
        let leftExtra = true,
          rightExtra = true;
        let leftStep = 1,
          rightStep = 1;
        idxContainer.push(selectIdx); // inital the container
        while (idxContainer.length < defaultOption.maxCols) {
          if (transferFlag) {
            let temp = selectIdx - leftStep;
            if (temp > -1) {
              idxContainer.push(temp);
            } else {
              leftExtra = false;
            }
            leftStep++;
          } else {
            let _temp = selectIdx + rightStep;
            if (_temp < layer.length) {
              idxContainer.push(_temp);
            } else {
              rightExtra = false;
            }
            rightStep++;
          }
          transferFlag = !transferFlag;
        }
        var allIndex = ',' + idxContainer.join(',') + ',';
        var tempLayer: INoteData[] = [];
        layer.forEach(function (item: any, ind: number) {
          if (allIndex.indexOf(',' + ind + ',') !== -1) {
            tempLayer.push(item);
          }
        });
        if (tempLayer.length > 0) {
          outDiv = (
            <div style={{ height: divHegiht + px, width: defaultOption.width + px }} className="structure-inner-container">
              {this.buildEachNoteHtml(tempLayer, divHegiht, leftExtra, rightExtra, isShot)}
            </div>
          );
        }
      }
    } else {
      outDiv = (
        <div style={{ height: divHegiht + px, width: defaultOption.width + px }} className="structure-inner-container">
          {this.buildEachNoteHtml(layer, divHegiht, false, false, isShot)}
        </div>
      );
    }
    return outDiv;
  }
  // 获取整个产品结构图
  buildStructureHtml(isShot: boolean) {
    let that = this;
    let data = that.state.structureState.data;
    if (Array.isArray(data) && data.length > 0) {
      let sum = 0;
      data.forEach(function (r: any, index: number) {
        r.noteIndex = index;
        return sum += r.notional;
      });
      that.state.structureState.totalNotional = sum;
      if (that.state.structureState.totalNotional === 0) {
        return (
          <div className="structure-outer-container" style={{ width: defaultOption.width + px, height: defaultOption.height + px }} />
        );
      }
      // 划分不同的层级
      let hasRating = data.filter(function (r: any) {
        return r.rating !== null;
      }).length > 0;
      let layerData: INoteData[] = [];
      if (!hasRating) {
        layerData = this.getLayerWithoutRating(that.state.structureState.data);
      } else {
        layerData = this.getLayerWithRating(that.state.structureState.data);
      }
      // 获取每个层级结构
      let child: JSX.Element[] = [];
      if (layerData.length > 0) {
        // 获取每个层级的最小高度
        let layerArr: boolean[] = [];
        layerData.forEach(function (layer: any, index: number) {
          if (Array.isArray(layer) && layer.length > 0) {
            let sums = 0;
            layer.forEach(function (r: any) {
              return sums += r.notional;
            });
            let isMinusHeight = (sums / sum) < (defaultOption.minHeight / defaultOption.height);
            if (isMinusHeight) {
              that.state.structureState.minLayerNum++;
              that.state.structureState.minLayerNotional += sums;
              layerArr[index] = true;
            } else {
              layerArr[index] = false;
            }
          }
        });
        layerData.forEach(function (layer: any, index: number) {
          if (Array.isArray(layer) && layer.length > 0) {
            child.push(<div key={index}>{that.getLayerHtml(layer, layerArr[index], isShot)}</div>);
          }
        });
      }
      return (
        <div className="structure-outer-container" style={{ width: defaultOption.width + px, height: defaultOption.height + px }}>
          {child}
        </div>
      );
    }
    return (
      <div className="structure-outer-container" style={{ width: defaultOption.width + px, height: defaultOption.height + px }} />
    );
  }
  // 格式化百分数，使每层数字之和为100%
  formatPercent(text: string, cur: number, scope: any, rating?: any) {
    if (scope.totalNotional - cur === scope.accuNotional) {
      let lastRatio = 100 - scope.accuPercent + '%';
      return (rating === undefined || rating === null || rating === 'NR') ?
       [text, ' ', lastRatio].join('') : [text, ' ', lastRatio, ' ', rating].join('');
    }
    scope.accuNotional += cur;
    var ratio = (cur / scope.totalNotional * 100).toFixed(0) + '%';
    scope.accuPercent += parseInt(ratio.substr(0, ratio.length - 1), 10);
    return (rating === undefined || rating === null || rating === 'NR') ?
       [text, ' ', ratio].join('') : [text, ' ', ratio, ' ', rating].join('');
  }
}
