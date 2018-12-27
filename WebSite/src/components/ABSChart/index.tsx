import * as React from 'react';
import ReactHighcharts from 'react-highcharts';
import HighchartMore from 'highcharts/highcharts-more';
import './index.less';
import ChartTheme from './chartTheme';
import ABSNoContent from '../../components/ABSNoContent';
import ABSLoading from '../ABSLoading';

HighchartMore(ReactHighcharts.Highcharts);
ReactHighcharts.Highcharts.setOptions(ChartTheme);

interface IProps {
  config: any;
  title?: any;
  showTitleBar?: boolean;
  loading?: boolean;
}

class ABSChart extends React.PureComponent<IProps, any> {
  render() {
    const { config, showTitleBar, loading } = this.props;

    if (config == null || config === undefined) {
      return null;
    }

    if (loading) {
      return <ABSLoading />;
    }

    const afterRender = (chart) => {
      // console.log(chart);
    };
    const chartDiv = (
      <div className="abs-chart">
        <ReactHighcharts config={config} callback={afterRender} />
      </div>
    );
    if (!config.series || config.series.length === 0) {
      if (config.title) {
        return (
          <div style={{ width: '100%' }}>
          <p className="abs-chart-title">{config.title.text}</p>
            {/* <p dangerouslySetInnerHTML={{ __html: config.title.text }} /> */}
            <ABSNoContent />
          </div>
        );
      } else {
        return (
          <div style={{ width: '100%' }}>
            <ABSNoContent />
          </div>
        );
      }
    }
    return (
      showTitleBar === true ?
        (
          <div className="abs-box-3">
            <div className="abs-box-3-title">
              <i className="fc fc-cloud-download fr abs-link-color abs-font-l" />
            </div>
            {chartDiv}
          </div>
        )
        :
        <div style={{ width: '100%' }}>{chartDiv}</div>
    );
  }
}
export default ABSChart;