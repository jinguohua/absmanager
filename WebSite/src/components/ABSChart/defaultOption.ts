import RouteConfig from '../../abs/config/routeConfig';
export default {
    title: {
        text: '',
        useHTML: true
    },
    xAxis: {
        labels: {
            rotation: -35,
        },
    },
    credits: {
        href: RouteConfig.chartCnabsLink,
        text: 'CNABS',
        position: {
            y: -6
        }
    },
    series: [],
};