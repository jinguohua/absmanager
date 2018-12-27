
function uploadModel() {
    var formData = new FormData();
    var inputUpload = document.getElementById("uploadModel");
    var totalFiles = inputUpload.files.length;
    if (totalFiles > 0) {
        for (var i = 0; i < totalFiles; ++i) {
            var file = inputUpload.files[i];
            var fileSizeMB = file.size / 1024 / 1024;
            if (fileSizeMB > 10) {
              alertify.alert("上传文件不能超过10MB, 所选文件" + file.name + "(" + fileSizeMB.toFixed(2) + "MB)");
                return false;
            }

            formData.append("file", file);
            formData.append("asOfDate", "20170228");
        }
        cnabsAjaxUploadFile('上传模型', '/Demo/UploadModel', formData, function () {
            alertify.alert('存续期管理', '上传模型成功');
        }, function (data) {
          alertify.alert('上传模型失败：' + data.Value);
        });
    }
}

function downloadModel() {
    cnabsAjax('下载模型', '/Demo/DownloadModel', { asOfDate: "20170228" }, function (fileGuid) {
        window.location.href = '/Download?guid=' + fileGuid;
    });
}


function generateByUploadedExcel() {

    var formData = new FormData();
    formData.append("paymentDate", "2017-03-27");
    formData.append("asOfDate", "20170228");

    cnabsAjaxUploadFile('报告下载', '/Demo/GenerateByUploadedExcel', formData, function (fileGuid) {
        //下载文件
        window.location.href = '/Download?guid=' + fileGuid;
    }, function (data) {
        cnabsAlertMore('报告下载失败：' + data.Value.Message, data.Value.StackTrace);
    });
}

function uploadTemplateFile() {

    var formData = new FormData();
    var inputUpload = document.getElementById("uploadTemplateFile");
    var totalFiles = inputUpload.files.length;
    if (totalFiles > 0) {
        for (var i = 0; i < totalFiles; ++i) {
            var file = inputUpload.files[i];
            var fileSizeMB = file.size / 1024 / 1024;
            if (fileSizeMB > 10) {
              alertify.alert("上传文件不能超过10MB, 所选文件" + file.name + "(" + fileSizeMB.toFixed(2) + "MB)");
                return false;
            }

            formData.append("file", file);
        }
        cnabsAjaxUploadFile('上传文件模板', '/Demo/UploadTemplateFile', formData, function () {
          alertify.alert('上传文件模板成功')
        }, function (data) {
          alertify.alert("上传文件模板失败");
        });
    }
}

function downloadTemplateFile() {
    cnabsAjax('下载文件模板', '/Demo/DownloadTemplateFile', {}, function (fileGuid) {
        window.location.href = '/Download?guid=' + fileGuid;
    });
}



function uploadExcelReport() {

    var formData = new FormData();
    var inputUpload = document.getElementById("uploadExcelReport");
    var totalFiles = inputUpload.files.length;
    if (totalFiles > 0) {
        for (var i = 0; i < totalFiles; ++i) {
            var file = inputUpload.files[i];
            var fileSizeMB = file.size / 1024 / 1024;
            if (fileSizeMB > 10) {
              alertify.alert("上传文件不能超过10MB, 所选文件" + file.name + "(" + fileSizeMB.toFixed(2) + "MB)");
                return false;
            }

            formData.append("file", file);
        }
        cnabsAjaxUploadFile('上传服务商报告', '/Demo/UploadExcelReport', formData, function () {
          alertify.alert('上传服务商报告成功')
        }, function (data) {
          alertify.alert('上传服务商报告失败：' + data.Value)
        });
    }
}

function downloadExcelReport() {
    cnabsAjax('下载服务商报告', '/Demo/DownloadExcelReport', {}, function (fileGuid) {
        window.location.href = '/Download?guid=' + fileGuid;
    });
}


angular.module('myApp', [])
  .controller('MainCtrl',function ($scope) {

    $scope.showList = function (event) {
      var target = event.currentTarget;
      $(target).parent().find('ul').toggle();
    };

    $scope.changeSelected = function (event) {
      var target = event.target;
      var curTarget = event.currentTarget;
      $(curTarget).hide().parent().find('.cur-selected').html($(target).html());
    };

    $scope.checkItem = function (event) {
      var target = event.currentTarget;
      $(target).find('span:first-child').toggleClass('fa-square-o').toggleClass('fa-check-square-o');
    };

    $scope.exportReport = function () {
      location.href = '/Demo/ExportReport';
    }

    $scope.importData = function () {
      location.href = '/Demo/ImportData';
    }

    $scope.configProject = function () {
      location.href = '/Demo/ConfigProject';
    }

    $scope.triggerUploadInput = function () {
      $("#uploadInput").val('');
      $("#uploadInput").click();
    }

    $scope.triggerUploadTemplateFile = function () {
      $("#uploadTemplateFile").val('');
      $("#uploadTemplateFile").click();
    }

    $scope.triggerUploadModel = function () {
      $("#uploadModel").val('');
      $("#uploadModel").click();
    }

    $scope.triggerUploadExcelReport = function () {
      $("#uploadExcelReport").val('');
      $("#uploadExcelReport").click();
    }

    //撤销审核
    $scope.cancelAudit = function (event) {
      var $targetDom = $(event.currentTarget),
        $trDom = $targetDom.parent().parent().parent(),
        $submitBtn = $trDom.find('td:last-child').find('button:nth-child(3)'),
        $statusDom = $trDom.find('td:nth-child(2)'),
        id = $trDom.data('id'),
        dataList = [];

      $submitBtn.prop('disabled', '');
      $statusDom.html('等待审核');
      $targetDom.prop('disabled', 'disabled');

      dataList.push({
        id: id,
        status: '等待审核',
        submitBtn: 1,
        cancelBtn: 0
      });

      localStorage.setItem('reportList',JSON.stringify(dataList));
    }

    //提交审核
    $scope.submitAudit = function(event){
      var $targetDom = $(event.currentTarget),
        $trDom = $targetDom.parent().parent().parent(),
        $cancelBtn = $trDom.find('td:last-child').find('button:last-child'),
        $statusDom = $trDom.find('td:nth-child(2)'),
        id = $trDom.data('id'),
        dataList = [];

      $cancelBtn.prop('disabled', '');
      $statusDom.html('正在审核');
      $targetDom.prop('disabled', 'disabled');

      dataList.push({
        id: id,
        status: '正在审核',
        submitBtn: 0,
        cancelBtn: 1
      });

      localStorage.setItem('reportList', JSON.stringify(dataList));
    }


    $scope.timerList = {};
    $scope.showSubMenu = function (event) {
      var target = event.currentTarget,
        $parent = $(target).parent(),
        $ul =  $parent.find('ul');
      $ul.toggle();
    };
    $scope.autoHideSubMenu = function (event) {
      var target = event.currentTarget,
        $span = $(target).parent(),
        $menuWrapper = $span.parent(),
        $ul =  $span.find('ul'),
        $index = $menuWrapper.index($span);

      $scope.timerList[$index] = setTimeout(function(){
        $ul.hide();
      },2000);
    };
    $scope.clearTimer = function(event){
      var target = event.currentTarget,
        $span = $(target).parent(),
        $menuWrapper = $span.parent(),
        $ul =  $span.find('ul'),
        $index = $menuWrapper.index($span);

      if(typeof $scope.timerList[$index] !== 'undefined'&&$scope.timerList[$index] !==null){
        clearTimeout($scope.timerList[$index]);
      }
    }

  });


$(function () {
  loadingAnimation();
  init();
});

function init() {
  var dataList = JSON.parse(localStorage.getItem('reportList'));
  
  dataList.length>0&&dataList.forEach(e=>{
    var $trDom = $('tr[data-id='+ e.id +']');
    $trDom.find('td:nth-child(2)').html(e.status);
    $trDom.find('td:last-child').find('button:nth-child(3)').prop('disabled',e.submitBtn==0?'disabled':'');    
    $trDom.find('td:last-child').find('button:last-child').prop('disabled',e.cancelBtn==0?'disabled':""); 
  });
}

function loadingAnimation(){
  var waitingHtml = [
    '<div class="loading-container">',
    '  <div class="sk-cube-grid">',
    '    <div class="sk-cube sk-cube1"></div>',
    '    <div class="sk-cube sk-cube2"></div>',
    '    <div class="sk-cube sk-cube3"></div>',
    '    <div class="sk-cube sk-cube4"></div>',
    '    <div class="sk-cube sk-cube5"></div>',
    '    <div class="sk-cube sk-cube6"></div>',
    '    <div class="sk-cube sk-cube7"></div>',
    '    <div class="sk-cube sk-cube8"></div>',
    '    <div class="sk-cube sk-cube9"></div>',
    '  </div>',
    '</div>'
  ].join(""),

  $waitingDom = $(waitingHtml);

  $('body').append($waitingDom);
  setTimeout(function(){
    $waitingDom.addClass("autoHide");
    setTimeout(function(){
      $waitingDom.hide();

      $(".auto-show-wrapper").addClass("autoShow");

    },500);
  },500);
}



//tookit工具包
; (function (window) {
  var toolkit = window.toolkit = {
    showMsg: function (msg, type, parentSelector) {
      msg = typeof msg !== 'undefined' ? msg : '';
      type = typeof type !== 'undefined' ? type : 'success';
      parentSelector = typeof parentSelector !== 'undefined' ? parentSelector : 'body';

      var msgCfg = {
        "success": {
          "icon": "fa-smile-o",
          "title": "成功！"
        },
        "warning": {
          "icon": "fa-meh-o",
          "title": "警告！"
        },
        "error": {
          "icon": "fa-frown-o",
          "title": "错误！"
        }
      };
      var cfg = msgCfg[type];

      var msgDom = [
        '<div class="msg-container">',
        '  <div class="msg-wrapper ' + type + '">',
        '    <div class="icon"><i class="fa ' + cfg.icon + '"></i></div>',
        '    <div class="msg">',
        '      <h2>' + cfg.title + '</h2>',
        '      <h3>' + msg + '</h3>',
        '    </div>',
        '    <button class="btn-close"><i class="fa fa-times"></i></button>',
        '  </div>',
        '</div>'
      ].join("");
      var $msgDom = $(msgDom),
        $btnClose = $msgDom.find('.btn-close');
      $(parentSelector).append($msgDom).show();

      $btnClose.on('click', function () {
        $btnClose.off('click');
        $msgDom.hide().remove();
      });

    }

  }
})(window);