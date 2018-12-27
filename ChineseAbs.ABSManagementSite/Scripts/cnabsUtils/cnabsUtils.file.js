
function CnabsFilePicker() {
}

CnabsFilePicker.prototype = {
    defaultOption: {
        //选择文件按钮上的文本
        buttonText: '选择文件',

        //是否预览
        preview: true,

        //是否将图片从文件中区分出来（预览时使用）
        divideImage: true,

        //选择文件数量上限
        maxFileCount: 10,

        //选择文件最大size（单位MB）
        maxFileSize: 100,

        //限制选择文件类型（为空时可以选择任意文件）
        //例如： limitFileTypes: ['xls', 'xlsx']
        limitFileTypes: [],

        //选择文件发生变化的时候触发
        onChange: null,

        //显示最后一次修改时间
        showLastModifyTime: false,

        //显示提示
        //提示内容可以在onchange回调函数设置在file.comment中
        //（文件包含提示，且showComment=true && showLastModifyTime时，优先显示提示，不显示最后一次修改时间）
        showComment: false,
    },

    initOption: function (option) {
        this.option = this.defaultOption;
        if (option != undefined && option != null) {
            this.option.buttonText = cnabsHasContent(option.buttonText) ? option.buttonText : '选择文件';

            if (option.preview != undefined) {
                this.option.preview = option.preview;
            }
            if (option.divideImage != undefined) {
                this.option.divideImage = option.divideImage;
            }
            if (option.maxFileCount != undefined) {
                this.option.maxFileCount = option.maxFileCount;
            }
            if (option.maxFileSize != undefined) {
                this.option.maxFileSize = option.maxFileSize;
            }
            if (option.limitFileTypes != undefined) {
                this.option.limitFileTypes = option.limitFileTypes;
            }
            if (option.onChange != undefined) {
                this.option.onChange = option.onChange;
            }
            if (option.showLastModifyTime != undefined) {
                this.option.showLastModifyTime = option.showLastModifyTime;
            }
            if (option.showComment != undefined) {
                this.option.showComment = option.showComment;
            }
        }
    },

    //containerId: 上传控件所在div
    init: function (containerId, option) {
        this.containerId = containerId;
        this.initOption(option);

        this.divId = cnabsRandomHtmlId('upload_file_div');

        this.buttonId = cnabsRandomHtmlId('upload_file_button');
        this.inputId = cnabsRandomHtmlId('upload_file_input');
        this.previewImageDivId = cnabsRandomHtmlId('upload_file_preview_image_div');
        this.previewFileDivId = cnabsRandomHtmlId('upload_file_preview_file_div');

        this.images = [];
        this.files = [];

        var html = '<div class="' + this.divId + '">';
        html += '  <div>';
        html += '    <div style="overflow:hidden;">';
        html += '    <div class="cnabs_btn left" id="' + this.buttonId + '">' + this.option.buttonText + '</div>';

        var multipleHtml = '';
        if (this.option.maxFileCount > 1) {
            multipleHtml = ' multiple ';
        }

        var acceptHtml = '';
        if (this.option.limitFileTypes.length > 0) {
            acceptHtml += ' accept="';
            $.each(this.option.limitFileTypes, function () {
                var fileType = this;
                if (cnabsHasContent(fileType)) {
                    if (fileType[0] != '.') {
                        fileType = '.' + fileType;
                    }

                    acceptHtml += fileType + ', ';
                }
            });
            acceptHtml += '" ';
        }

        html += '    <input id="' + this.inputId + '" ' + multipleHtml + acceptHtml + ' style="display:none;" type="file">';
        html += '  </div>';
        html += '  <div class="cnabs_file_picker_preview cnabs_scrollbar">';
        html += '    <div class="cnabs_file_picker_preview_images" id="' + this.previewImageDivId + '">';
        html += '    </div>';
        html += '    <div class="cnabs_file_picker_preview_files" id="' + this.previewFileDivId + '">';
        html += '    </div>';
        html += '  </div>';
        html += '</div>';

        document.getElementById(this.containerId).innerHTML = html;

        var self = this;
        document.getElementById(this.buttonId).onclick = function () {
            self.triggerInputClickEvent(self);
        };
        document.getElementById(this.inputId).onchange = function () {
            self.addFile(self);
        };
    },

    triggerInputClickEvent: function (self) {
        $("#" + self.inputId).val('');
        $("#" + self.inputId).click();
    },

    clearAll: function (self) {
        document.getElementById(self.previewImageDivId).innerHTML = '';
        document.getElementById(self.previewFileDivId).innerHTML = '';        
        self.images = [];
        self.files = [];
    },

    previewFile: function (self, file) {
        if (!self.option.preview) {
            return;
        }

        var iconSrc = '../../Images/Common/' + cnabsGetFileIconByFileName(file.name);
        var html = '<div class="cnabs_file_picker_each_file" style="background-image:url(' + iconSrc + ');">';
        html += '<span class="left cnabs_ellipsis cnabs_file_picker_file_name">' + file.name + '</span>';        
        if (self.option.showLastModifyTime && !self.option.showComment) {
            html += '<span class="left cnabs_file_picker_file_last_modify_time">' + cnabsFormatDate(file.lastModifiedDate, "yyyy-MM-dd hh:mm") + '</span>';
        }
        if (self.option.showComment) {
            html += '<span class="left cnabs_file_picker_file_show_comment">' + file.comment + '</span>';
        }
        html += '<span class="cnabs_red right cnabs_file_picker_delete_file">删除</span>';
        html += '</div>';
        $('#' + self.previewFileDivId).append(html);
        var files = self.files;
        $('.cnabs_file_picker_delete_file').click(function () {
            for (var i = 0; i < files.length; i++) {
                if (files[i] != undefined) {
                    if (!self.option.showLastModifyTime && !self.option.showComment) {
                        if (files[i].name == $(this).prev().text()) {
                            files.splice(i, 1);
                        }
                    } else {
                        if (files[i].name == $(this).prev().prev().text()) {
                            files.splice(i, 1);
                        }
                    }
                    if (self.option.onChange != null) {
                        self.option.onChange(self.files, self.images);
                    }
                }
            }
            $(this).parent().remove();
        })
    },

    previewImage: function (self, image) {
        if (!self.option.preview) {
            return;
        }

        var prevDiv = document.getElementById(self.previewImageDivId);

        var reader = new FileReader();
        reader.file = image;
        reader.onload = function (evt) {
            var eachPreviewImage;
            evt.target.file.src = evt.target.result;
            prevDiv.innerHTML += '<div class="cnabs_file_picker_image_preview">'
                + '<img class="cnabs_file_picker_image_preview_each" src="' + evt.target.result + '" />'
                + '</div>';

            eachPreviewImage = $('#' + self.previewImageDivId + ' .cnabs_file_picker_image_preview');

            eachPreviewImage.hover(function () {
                $('.cnabs_file_picker_delete_image').remove();
                var targetItem = $(this)[0];
                $(this).append('<div class="cnabs_file_picker_delete_image">×</div>');
                if ($(this).width() > $($(this).parent().parent()).width()) {
                    $('.cnabs_file_picker_delete_image').css({
                        width: $($(this).parent().parent()).width() + 'px'
                    });
                } else {
                    $('.cnabs_file_picker_delete_image').css({
                        width: 100 + "%"
                    });
                }

                var currentImgSrc = $($(this).find('img.cnabs_file_picker_image_preview_each'))[0].src;
                $(".cnabs_file_picker_delete_image").click(function () {
                    for (var i = 0; i < self.images.length; i++) {
                        if (self.images[i] != undefined && self.images[i].src == currentImgSrc) {
                            self.images.splice(i, 1)
                        }
                        if (self.option.onChange != null) {
                            self.option.onChange(self.files, self.images);
                        }
                    }
                    $(targetItem).remove();
                    $('.cnabs_file_picker_delete_image').remove();
                })
            }, function () {
                $('.cnabs_file_picker_delete_image').remove();
            });

        }
        reader.readAsDataURL(image);
    },

    isImage: function (self, fileName) {
        return self.option.divideImage && cnabsIsImage(fileName);
    },

    addFile: function (self) {
        var fileInput = $('#' + self.inputId)[0];
        if (fileInput.files) {

            //设定选择文件数为1时，每次清空已选文件，以达到多次选择时，使用最新文件的效果
            if (self.option.maxFileCount == 1) {
                self.clearAll(self);
            }

            if (self.files.length + self.images.length >= self.option.maxFileCount) {
                cnabsMsgError("选择文件失败：文件数量不能超过" + self.option.maxFileCount);
                return;
            }

            for (var i = 0; i < fileInput.files.length; i++) {
                var file = fileInput.files[i];
                var fileSizeMB = file.size / 1024 / 1024;
                if (fileSizeMB > self.option.maxFileSize) {
                    cnabsMsgError('选择文件失败：单个文件(' + file.name + ')大小不能超过' + self.option.maxFileSize + 'MB');
                    return;
                }

                if (self.option.limitFileTypes.length > 0) {
                    var extension = cnabsGetFileNameExtension(file.name);
                    var exist = false;
                    $.each(self.option.limitFileTypes, function () {
                        if (this.toLowerCase() == extension.toLowerCase()) {
                            exist = true;
                            return false;
                        }
                    });
                    
                    if (!exist) {
                        var limitFileTypeStr = self.option.limitFileTypes.join(', ');
                        cnabsMsgError('选择文件失败：只能选择 ' + limitFileTypeStr + ' 格式的文件');
                        return;
                    }
                }
            }

            var newFiles = [];
            var newImages = [];
            //将文件添加到 images/files中
            for (var i = 0; i < fileInput.files.length; i++) {
                var file = fileInput.files[i];
                if (IsFileRepeat(self.files, file) || IsFileRepeat(self.images, file)) {
                    continue;
                }

                if (self.files.length + self.images.length >= self.option.maxFileCount) {
                    break;
                }

                if (self.isImage(self, file.name)) {
                    self.images.push(file);
                    newImages.push(file);
                } else {
                    self.files.push(file);
                    newFiles.push(file);
                }                
            }

            //触发onChange
            if (self.option.onChange != null) {
                self.option.onChange(self.files, self.images);
            }

            //预览文件
            $.each(newFiles, function () {
                self.previewFile(self, this)
            });

            //预览图片
            $.each(newImages, function () {
                self.previewImage(self, this)
            });
        }
    },

    getFormData: function () {
        var formData = new FormData();
        $.each(this.files, function (i) {
            formData.append("file" + i, this);
        });
        $.each(this.images, function (i) {
            formData.append("image" + i, this);
        });
        return formData;
    },

    totalFileCount: function () {
        return this.files.length + this.images.length;
    },
}
