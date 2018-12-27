'use strict';

const path = require('path');
const webpack = require('webpack');
module.exports = {
    entry: {
        vendor: [
            'antd',
            'qs',
            'axios',
            'redux-saga',
            'redux',
            //'core-js',
            'echarts',
            'react-router',
            'dva',
            'perfect-scrollbar',
            'highcharts',
            'lodash',
            'jsplumb'
        ]
    },
    output: {
        path: path.join(__dirname, '../dll/'),
        filename: '[name].dll.js',
        library: '[name]_library'
    },
    plugins: [
        new webpack.DefinePlugin({
            "process.env": {
                NODE_ENV: JSON.stringify('production')
            }
        }),
        new webpack.DllPlugin({
            path: path.join(__dirname, '../dll/', '[name]-manifest.json'),
            name: '[name]_library'
        }),
        new webpack.optimize.UglifyJsPlugin({
            compress: {
                warnings: false,
                drop_console: true,
                drop_debugger: true
            },
            output: {
                comments: false,
            },
            sourceMap: true
        })
    ]
};
