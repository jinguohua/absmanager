const path = require('path');
const fs = require('fs-extra');
const chalk = require('chalk');

const appDirectory = fs.realpathSync(process.cwd());
const sourceDir = path.resolve(appDirectory, 'build');

function testDeploy() {
  console.log(chalk.yellow('\n\n-----------------begin testdeploy-----------------'));
  const publishDir = 'd:\\Release\\ChineseAbsReact'; 
  if (fs.existsSync(publishDir)) {
    console.log(chalk.yellow('\n\n-----------------testdeploy-----------------'));
    console.log(chalk.cyan('clear deploy project.................'));
    fs.emptyDirSync(publishDir);
    console.log(chalk.green('clear deploy finish.................'));
    console.log(chalk.cyan('publish deploy project.................'));
    fs.copySync(sourceDir, publishDir);
    console.log(chalk.green('publish deploy finish.................'));
  }
} 

function webDeploy(appPath) {
  let buldEnv = readEnv();
  if (buldEnv.isDeploy) {
    console.log(chalk.yellow('\n\n-----------------webdeploy-----------------'));
    console.log(chalk.cyan('=> webdeploy start.....'));
    var FtpDeploy = require('ftp-deploy');
    var ftpDeploy = new FtpDeploy();

    var config = {
      user: `${buldEnv.account}`,
      password: `${buldEnv.password}`,
      host: `${buldEnv.host}`,
      port: buldEnv.port,
      localRoot: sourceDir,
      remoteRoot: appPath,
      exclude: ['.git', '*.map'],
      include: ['*'],
      exclude: ['.git', '.idea', 'tmp/*']
    }

    ftpDeploy.deploy(config, function (err) {
      if (err) {
        console.log(err);
        console.log(chalk.red('=> webdeploy error:' + err));
      }
      else
        console.log(chalk.green('=> webdeploy finished....'));
    });

    ftpDeploy.on('uploading', function (data) {
      data.totalFileCount;       // total file count being transferred 
      data.transferredFileCount; // number of files transferred 
      data.percentComplete;      // percent as a number 1 - 100 
      data.filename;             // partial path with filename being uploaded 
    });

    ftpDeploy.on('uploaded', function (data) {
      console.log(chalk.magenta('---- ' + data.filename));         // same data as uploading event 
    });

    ftpDeploy.on('upload-error', function (data) {
      console.log(chalk.red('=> webdeploy upload error:' + data.err));
    });
  }
};

function readEnv() {
  let buildEnv = {
    isDeploy: true,
    account: '',
    password: '',
    host: '',
    port: ''
  };

  if (process.argv) {
    process.argv.forEach(item => {
      if (item.startsWith('webapp_deploy_account')) {
        let tempArr = item.split(':');
        let account = tempArr[1];
        let password = tempArr[2];
        let host = tempArr[3];
        let port = tempArr[4];

        buildEnv = {
          isDeploy: true,
          account: account,
          password: password,
          host: host,
          port: port
        }
        return false;
      }
    })
  }

  return buildEnv;

}

module.exports = {
  testDeploy: testDeploy,
  webDeploy: webDeploy
};

