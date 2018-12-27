function cnabsUploadFile(title, fileType, maxSize, maxFilesLength, callback) {
    if (title == undefined || title == null) {
        title = "上传文件";
    };
    var commonFile = [];
    var divId = cnabsRandomHtmlId('model_upload');
    var labelId = cnabsRandomHtmlId('label_upload');
    var divStyle = document.getElementById("divId");
    var div = '<div id="' + divId + '" class="module scenario" style="margin: 0px; padding: 0px; overflow: hidden;">';
    div += '<div class="container" role="main"><form method="post" action="" enctype="multipart/form-data" novalidate class="box">';
    div += '<div style="padding: 56px;">';
    div += '<img src="/Images/Common/file.png" /><br>';
    div += '<div class="box__input">';
    div += '<input type="file" name="files[]" id="fileUpload" class="box__file" data-multiple-caption="{count}个文件已被选择" multiple="multiple" />';
    div += '<label for="fileUpload" id="' + labelId + '">';
    div += '<strong>选择文件</strong><span class="box__dragndrop">&nbsp;或拖拽文件到这里上传</span>';
    div += '</label>';
    div += '</div>';
    div += '</div>';
    div += '</form></div>';
    div += '</div>';
    $('body').append(div);

    (function (document, window, index) {
        var forms = document.querySelectorAll('.box');
        Array.prototype.forEach.call(forms, function (form) {
            var input = form.querySelector('input[type="file"]'),
				label = form.querySelector('label'),
				errorMsg = form.querySelector('.box__error span'),
				restart = form.querySelectorAll('.box__restart'),
				droppedFiles = false,
                //拖拽之后，执行的动作
				showFiles = function (files) {
				    commonFile = files;
				    label.classList.add('transcolor');
				    if (files.length == 0 || files.length > maxFilesLength) {
				        changeFontColor();
				    }
				    if (files.length != 0 && files.length <= maxFilesLength) {
				        label.textContent = files.length > 1 ? (input.getAttribute('data-multiple-caption') || '').replace('{count}', files.length) : files[0].name;				        				        				        
				    }				    
				};
            
            input.addEventListener('change', function (e)//向指定元素input添加事件
            {
                showFiles(e.target.files);//执行文件名显示在input框之中,得到一串list                
            });

            // drag&drop files if the feature is available
            form.classList.add('has-advanced-upload'); // letting the CSS part to know drag&drop is supported by the browser

            ['drag', 'dragstart', 'dragend', 'dragover', 'dragenter', 'dragleave', 'drop'].forEach(function (event) {
                form.addEventListener(event, function (e) {                                    
                        e.preventDefault();                    
                        e.stopPropagation();
                });
            });
            ['dragover', 'dragenter'].forEach(function (event) {
                form.addEventListener(event, function () {
                    form.classList.add('is-dragover');
                });
            });
            ['dragleave', 'dragend', 'drop'].forEach(function (event) {
                form.addEventListener(event, function () {
                    form.classList.remove('is-dragover');
                });
            });
            form.addEventListener('drop', function (e) {
                droppedFiles = e.dataTransfer.files; // the files that were dropped
                showFiles(droppedFiles);

            });
            // Firefox focus bug fix for file input
            input.addEventListener('focus', function () { input.classList.add('has-focus'); });
            input.addEventListener('blur', function () { input.classList.remove('has-focus'); });

        });
    }(document, window, 0));

    $('#' + divId).dialog({
        closeText: "",
        title: title,
        width: 500,
        height: 300,
        hide: false,
        resizable: false,
        open: function () {
            $(this).parents('.ui-dialog').attr('tabindex', -1)[0].focus();
        },
        buttons: {
            "确定": function () {                                        
                var isNullFiles = true;//文件上传列表不为空                
                var isFileList = true;//文件上传列表里文件个数的限制                
                var isMaxFile = true;//文件大小的限制                
                var isZipFile = true;//文件格式的限制

                //遍历取出单个文件的相应的信息
                $.each(commonFile, function (index, item) {
                    //文件大小
                    var size = item.size / 1024;
                    var fileSize = size.toFixed(1);
                    //文件类型
                    var uploadModelName = item.name;
                    var uploadModelType = uploadModelName.substr(uploadModelName.lastIndexOf("."));

                    if (fileSize > maxSize) {
                        isMaxFile = false;
                    }                    
                    if (uploadModelType != "." + fileType) {
                        isZipFile = false;
                    }
                })

                //上传列表为空
                if (commonFile.length == 0) {
                    isNullFiles = false;
                }
                //上传列表文件超限
                if (commonFile.length > maxFilesLength) {
                    isFileList = false;
                }
                
                if (!isFileList || !isNullFiles) {
                    cnabsMsgError("上传失败：文件上传列表为空！");
                    return false;
                }
                if (!isZipFile) {                   
                    cnabsMsgError("上传失败：请选择后缀名为zip的文件！");
                    changeFontColor();                    
                    return false;
                }
                if (!isMaxFile) {
                    cnabsMsgError("上传失败：请选择文件大小小于" + maxSize + "KB的文件！");
                    changeFontColor();
                    return false;
                }                                              
                if (isZipFile && isMaxFile) {
                    var val = callback(commonFile);
                    cnabsMsgSuccess("上传模型成功");
                    $(this).dialog("close");
                }                               
            },
            "取消": function () {
                $(this).dialog("close");
            }
        },
        close: function () {
            $('#' + divId).remove();
        }
    });

    //文字样式进行还原
    function changeFontColor() {        
        $("label").removeClass("transcolor");
        document.getElementById(labelId).innerHTML = '<strong>选择文件</strong><span class="box__dragndrop">&nbsp;或拖拽文件到这里上传</span>';
    }
}


