const path = require("path");
const fs = require("fs");
const paths = require("./paths");
const webpack = require("webpack");

function base64(file) {
  // read binary data
  var bitmap = fs.readFileSync(file);
  // convert binary data to base64 encoded string
  return "data:text/css;base64," + new Buffer(bitmap).toString("base64");
}

// 将微信二维码样式转成base64提供给微信接口
// console.log("=========", base64('./src/abs/views/account/login/wechat.less'))

// add other path to paths
paths.appDirectory = fs.realpathSync(process.cwd());
paths.styleTheme = resolveApp("public/theme.less");
paths.envKeyPrefix = "REACT_APP_";
const webpackSetting = {
  app: {
    name: "ABS-System",
    version: process.env.VERSION
  },
  antd: [
    {
      // use antd design ui antd|antd-mobile
      libraryName: "antd",
      libraryDirectory: "es",
      style: true
    }
  ],
  // setting multi enty
  entry: [
    "absui",
    "account",
    "index",
    "projects",
    "products",
    "system",
    "assets"
  ],

  // process env environment variables for production|development or jscode|index.html
  env: {
    development: {
      API_ADDRESS: "api",
      PUBLISH_PATH: "/"
    },
    local: {
      API_ADDRESS: "http://10.1.1.221:10000",
      PUBLISH_PATH: "/app/"
    },
    test: {
      API_ADDRESS: "https://testnew.cn-abs.com",
      PUBLISH_PATH: "/"
    },
    production: {
      API_ADDRESS: "https://www.cn-abs.com",
      PUBLISH_PATH: "/"
    }
  },
  // output setting
  output: {
    path: paths.appBuild,
    // dev set true add module info or comment , production please set false
    pathinfo: isProduction() ? false : true,
    filename: isProduction()
      ? `static/js/[name].${process.env.VERSION}.js`
      : "static/js/[name].js",
    chunkFilename: isProduction()
      ? `static/js/[name].${process.env.VERSION}.chunk.js`
      : "static/js/[name].chunk.js",
    publicPath: isProduction() ? paths.servedPath : "/",
    // dev or production mode
    devtoolModuleFilenameTemplate: isProduction()
      ? info =>
          path
            .relative(paths.appSrc, info.absoluteResourcePath)
            .replace(/\\/g, "/")
      : info => path.resolve(info.absoluteResourcePath).replace(/\\/g, "/")
  }
  // entry: {
  //     common:[
  //         require.resolve("babel-polyfill"),
  //         require.resolve("raf/polyfill"),
  //         // hot update for dev
  //         isProduction() ? '' : require.resolve('react-dev-utils/webpackHotDevClient'),
  //     ],
  //     index: [
  //         parseTmpl('index').entry,
  //     ],
  //     expert: [
  //         parseTmpl('expert').entry,
  //     ],
  // },
};

// entryPath tmpl html-webpack-plugin
const HtmlWebpackPlugin = require("html-webpack-plugin");
// replace antd default less
const lessToJs = require("less-vars-to-js");
const themeVariables = lessToJs(fs.readFileSync(paths.styleTheme, "utf8"));
// load antd js/css small  with ts-import-plugin
const TsImportPluginFactory = require("ts-import-plugin");
// webpack-bundle-analyzer
const BundleAnalyzerPlugin = require("webpack-bundle-analyzer")
  .BundleAnalyzerPlugin;
// environment variables available in index.html
const InterpolateHtmlPlugin = require("react-dev-utils/InterpolateHtmlPlugin");
// environment variables available to the JS code
const getClientEnvironment = require("./env");

module.exports = {
  paths: paths,
  webpackSetting: webpackSetting,
  initWebpackSetting: initWebpackSetting,
  bindImportDllPlugin: bindImportDllPlugin,
  bindProgressPlugin: bindProgressPlugin,
  bindMinChunkSizePlugin: bindMinChunkSizePlugin,
  bindCommonsChunkPlugin: bindCommonsChunkPlugin
};

function initWebpackSetting(webpackConfig) {
  // #######step 1
  // reset entry
  //webpackConfig.entry = webpackSetting.entry;
  webpackConfig.entry = {};
  // set common entry
  let commonEntry = new Array();
  commonEntry.push(require.resolve("babel-polyfill"));
  commonEntry.push(require.resolve("raf/polyfill"));
  if (!isProduction()) {
    commonEntry.push(require.resolve("react-dev-utils/webpackHotDevClient"));
  }
  webpackConfig.entry.common = commonEntry;

  // set other custom entry from setting
  webpackSetting.entry.forEach(item => {
    let entryPath = parseTmpl(item);
    if (!fs.existsSync(entryPath.entry)) {
      fs.copyFileSync(paths.appIndexJs, entryPath.entry);
    }
    webpackConfig.entry[item] = entryPath.entry;
  });

  // #######step 2
  // reset output filename and repleact publish path
  webpackConfig.output = webpackSetting.output;
  let envSetting = getEnvSetting();

  webpackConfig.output.publicPath = envSetting["PUBLISH_PATH"];
  // isProduction() ? process.env.APPPATH :
  //     webpackSetting.env.development.PUBLISH_PATH,

  // #######step 3
  // add ts-import-plugin load antd only use module/css
  // add set less-loader modify antd less theme
  webpackConfig.module.rules.forEach(item => {
    if (item && item.oneOf) {
      // one of two modify ts-loader|less-loader
      item.oneOf.forEach(rule => {
        // set ts-loader for antd onlye use module/css
        if (rule.test && rule.test.source) {
          if (rule.test.source.includes(".(ts|tsx)$")) {
            let tsLoaderOptions = {
              transpileOnly: true,
              getCustomTransformers: () => ({
                before: [TsImportPluginFactory(webpackSetting.antd)]
              })
            };

            // if rule->use
            if (rule.use) {
              rule.use.forEach(item => {
                if (item.loader === require.resolve("ts-loader")) {
                  item.options = tsLoaderOptions;
                }
              });
            }
            // if rule->loader
            if (rule.loader && rule.loader === require.resolve("ts-loader")) {
              rule.options = tsLoaderOptions;
            }
          }

          // add less-loader for modify antd theme
          if (rule.test.source.includes(".css$")) {
            let lessLoader = {
              loader: require.resolve("less-loader"),
              options: {
                modifyVars: themeVariables
              }
            };

            // production rule-loader
            if (rule.loader) {
              rule.test = /\.(css|less)$/;
              rule.loader.push(lessLoader);
            }

            // development rule->use
            if (rule.use) {
              rule.test = /\.(css|less)$/;
              rule.use.push(lessLoader);
            }
          }
        }
      });
    }
  });

  // #######step 4
  // defineplugin  environment variables available to the JS code
  // let envSetting = getEnvSetting(); // isProduction() ? webpackSetting.env.production : webpackSetting.env.development;
  for (let prop in envSetting) {
    process.env[`${paths.envKeyPrefix}${prop}`] = envSetting[prop];
  }
  let publicUrl;
  if (isProduction()) {
    publicUrl = webpackSetting.output.publicPath.slice(0, -1);
  } else {
    publicUrl = "";
  }
  // reset InterpolateHtmlPlugin|DefinePlugin on step 5
  const env = getClientEnvironment(publicUrl);

  // #######step 5
  // find old HtmlWebpackPlugin and remove
  let tempArr = new Array();
  webpackConfig.plugins.forEach(item => {
    if (item instanceof HtmlWebpackPlugin) {
      tempArr.push(item);
    }
    // reset
    if (item instanceof webpack.DefinePlugin) {
      tempArr.push(item);
      //item =  new webpack.DefinePlugin(env.stringified);
    }

    if (item instanceof InterpolateHtmlPlugin) {
      tempArr.push(item);
      //item =  new InterpolateHtmlPlugin(env.raw);
    }
  });
  // remove old HtmlWebpackPlugin
  tempArr.forEach(item => {
    let itemIndex = webpackConfig.plugins.indexOf(item);
    webpackConfig.plugins.splice(itemIndex, 1);
  });

  // add InterpolateHtmlPlugin|DefinePlugin
  webpackConfig.plugins.push(new InterpolateHtmlPlugin(env.raw));
  webpackConfig.plugins.push(new webpack.DefinePlugin(env.stringified));
  // add new HtmlWebpackPlugin
  const htmlWebpackPlugins = createHtmlWebpackPlugin();
  htmlWebpackPlugins.forEach(item => {
    webpackConfig.plugins.push(item);
  });

  // #######step 5
  // production add webpack-bundle-analyzer
  if (isProduction()) {
    const bundleAnalyzerPlugin = new BundleAnalyzerPlugin({
      analyzerMode: "static",
      // analyzerHost: '127.0.0.1',
      // analyzerPort: 3334,
      reportFilename: "report.html",
      defaultSizes: "parsed",
      // Automatically open report in default browser
      openAnalyzer: false,
      // If `true`, Webpack Stats JSON file will be generated in bundles output directory
      generateStatsFile: false,
      // Name of Webpack Stats JSON file that will be generated if `generateStatsFile` is `true`.
      statsFilename: "stats.json",
      statsOptions: null,
      // Log level. Can be 'info', 'warn', 'error' or 'silent'.
      logLevel: "info"
    });
    webpackConfig.plugins.push(bundleAnalyzerPlugin);
  }
}

// production or development
function isProduction() {
  if (process.env.NODE_ENV === "production") {
    return true;
  } else {
    return false;
  }
}

// env check
function getEnvSetting() {
  switch (process.env.BUILD_ENV) {
    case "production":
      return webpackSetting.env.production;
    case "test":
      return webpackSetting.env.test;
    case "local":
      return webpackSetting.env.local;
    case "development":
      return webpackSetting.env.development;
    default:
      return webpackSetting.env.development;
  }
}

// parse file setting
function parseTmpl(key) {
  let publicPath = `${paths.appPublic}/${key}.html`;
  let srcPath = `${paths.appSpaSrc}/${key}/index.tsx`;

  return {
    name: key,
    entry: srcPath,
    output: path.basename(publicPath),
    tmpl: publicPath
  };
}
// get full file path
function resolveApp(relativePath) {
  return path.resolve(paths.appDirectory, relativePath);
}

// create HtmlWebpackPlugin from webpack entry
function createHtmlWebpackPlugin() {
  let tempArr = new Array();
  webpackSetting.entry.forEach(entry => {
    if (entry !== "common" && entry !== "vendor" && entry !== "absComponents") {
      let entryPath = parseTmpl(entry);
      if (!fs.existsSync(entryPath.tmpl)) {
        fs.copyFileSync(paths.appHtml, entryPath.tmpl);
      }

      let item = new HtmlWebpackPlugin({
        inject: true,
        title: webpackSetting.app.name,
        template: entryPath.tmpl,
        filename: entryPath.output,
        chunks: ["common", "absComponents", entryPath.name],
        //dev stop minify | production use minify format js/css
        minify: isProduction()
          ? {
              removeComments: true,
              collapseWhitespace: true,
              removeRedundantAttributes: true,
              useShortDoctype: true,
              removeEmptyAttributes: true,
              removeStyleLinkTypeAttributes: true,
              keepClosingSlash: true,
              minifyJS: true,
              minifyCSS: true,
              minifyURLs: true
            }
          : false
      });
      tempArr.push(item);
    }
  });
  return tempArr;
}

function bindImportDllPlugin(config) {
  config.plugins.push(
    new webpack.DllReferencePlugin({
      context: path.resolve(__dirname, "../"),
      manifest: require("../dll/vendor-manifest.json")
    })
  );

  const AddAssetHtmlPlugin = require("add-asset-html-webpack-plugin");
  console.log("### ### ###" + __dirname + "/../dll/vendor.dll.js");
  config.plugins.push(
    new AddAssetHtmlPlugin({
      filepath: path.resolve(__dirname + "/../dll/vendor.dll.js"),
      outputPath: "",
      publicPath: "",
      includeSourcemap: false,
      hash: true
    })
  );
}

function bindProgressPlugin(config) {
  var temp = 0;
  config.plugins.push(
    new webpack.ProgressPlugin((percentage, message, ...args) => {
      if (percentage > temp) {
        args.forEach((x, i) => {
          if (x != undefined && x.length != undefined && x.length > 50) {
            args[i] = x.substr(0, 50) + "...";
          }
        });
        console.info(percentage.toFixed(2), message, ...args);
        temp += 0.01;
      }
    })
  );
}

function bindMinChunkSizePlugin(config) {
  config.plugins.push(
    new webpack.optimize.MinChunkSizePlugin({
      minChunkSize: 100000 // Minimum number of characters
    })
  );
}

function bindCommonsChunkPlugin(config) {
  var entryArray = [];

  // Append all custom components to abs common entry
  var folder = path.resolve(__dirname, "../src/components");
  var folderItems = fs.readdirSync(folder);
  folderItems.forEach(function(item, index) {
    var curPath = path.resolve(folder, item);
    var info = fs.statSync(curPath);
    if (
      info.isDirectory() &&
      fs.existsSync(path.resolve(curPath, "index.tsx"))
    ) {
      entryArray.push(curPath);
    }
  });

  // Append all custom utils(ts files) to abs common entry
  var appendTsFileToEntry = function(folder) {
    folder = path.resolve(__dirname, folder);
    folderItems = fs.readdirSync(folder);
    folderItems.forEach(function(item, index) {
      var curPath = path.resolve(folder, item);
      var info = fs.statSync(curPath);
      if (info.isFile() && curPath.endsWith(".ts")) {
        entryArray.push(curPath);
      }
    });
  };

  appendTsFileToEntry("../src/utils");
  appendTsFileToEntry("../src/utils/authUtil");
  appendTsFileToEntry("../src/utils/http/store");
  appendTsFileToEntry("../src/utils/http/request");

  config.entry.absComponents = entryArray;

  config.plugins.push(
    new webpack.optimize.CommonsChunkPlugin({
      names: "absComponents",
      minChunks: Infinity
    })
  );
}
