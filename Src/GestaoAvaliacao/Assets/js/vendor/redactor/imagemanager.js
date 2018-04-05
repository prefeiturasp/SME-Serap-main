/**
 * function Plugin para upload de imagem - manipulando a API interna do redactor.
 * @namespace Controller
 * @author julio cesar da silva - 21/10/2015
 */
if (!RedactorPlugins) var RedactorPlugins = {};

RedactorPlugins.imagemanager = function () {
	
	return {
		init: function () {
			var _redactor = this;
			this.upload['sendData'] = function (file, formData, e) {
				if (_redactor.opts.uploadType == 'customSender') {
					_redactor.upload.customSendData(file, e);
				}
				else {
					if (this.upload.type == 'image') {
						formData = _redactor.upload.getHiddenFields(_redactor.opts.uploadImageFields, formData);
						formData = _redactor.upload.getHiddenFields(_redactor.upload.imageFields, formData);
					}
					else {
						formData = _redactor.upload.getHiddenFields(_redactor.opts.uploadFileFields, formData);
						formData = _redactor.upload.getHiddenFields(_redactor.upload.fileFields, formData);
					}
					var xhr = new XMLHttpRequest();
					xhr.open('POST', _redactor.upload.url);
					xhr.onreadystatechange = $.proxy(function () {
						if (xhr.readyState == 4) {
							var data = xhr.responseText;
							data = data.replace(/^\[/, '');
							data = data.replace(/\]$/, '');
							var json;
							try {
								json = (typeof data === 'string' ? $.parseJSON(data) : data);
							}
							catch (err) {
								json = {
									error: true
								};
							}
							_redactor.progress.hide();
							if (!_redactor.upload.direct) {
								_redactor.upload.$droparea.removeClass('drag-drop');
							}
							_redactor.upload.callback(json, _redactor.upload.direct, e);
						}
					}, _redactor);
					xhr.send(formData);
				}
			};
			this.upload['customSendData'] = function(file, e) {
				var reader = new FileReader();
				reader.readAsDataURL(file);
				var splitedType = file.type.toUpperCase().split("/");
				var output_format = splitedType[1];
				var img = new Image();
				var loadImage = $.proxy(function (eventFile) {
					img.onload = null;
					if (output_format !== "GIF" || _redactor.opts.imageConfig.gifCompression ||
						(img.naturalWidth > _redactor.opts.imageConfig.maxResolutionWidth || img.naturalHeight > _redactor.opts.imageConfig.maxResolutionHeight))
						img.src = compressor.compress(img, _redactor.opts.imageConfig.quality, output_format,
							_redactor.opts.imageConfig.maxResolutionWidth,
							_redactor.opts.imageConfig.maxResolutionHeight).src;
					var base64 = img.src.replace(/^data:image\/(jpg|png|jpeg|bmp|gif);base64,/, "");
					var fileCompressed = {
						ContentLength: file.size,
						ContentType: file.type,
						FileName: file.name,
						InputStream: base64,
						FileType: _redactor.opts.getTypeFileCallback()
					};
					_redactor.upload.sendServer(fileCompressed, e);
				}, _redactor);
				reader.onload = $.proxy(function (eventFile) {
					if (splitedType[0] != "IMAGE") {

					    _redactor.core.setCallback('imageRules', 'O arquivo ' + file.name + ' n&atilde;o &eacute; do tipo imagem.');
						_redactor.progress.hide();
						_redactor.modal.close();
						return;
					}
					var allowExtension = _redactor.opts.imageConfig.extensions.indexOf(output_format);
					if (allowExtension == -1) {
					    _redactor.core.setCallback('imageRules', 'A extens&atilde;o de imagem ' + '.' + output_format + " n&atilde;o &eacute; permitida.");
						_redactor.progress.hide();
						_redactor.modal.close();
						return;
					}
					if ((parseInt(file.size) / 1024) > _redactor.opts.imageConfig.maxSizeFile) {
						_redactor.core.setCallback('imageRules', 'Imagem ultrapassa o tamanho limite de ' + _redactor.opts.imageConfig.maxSizeFile + ' kB');
						_redactor.progress.hide();
						_redactor.modal.close();
						return;
					}
					img.onload = loadImage
					img.src = eventFile.target.result;
				}, _redactor);
			};
			this.upload['sendServer'] = function (file, e) {
				$.ajax({
					url: _redactor.upload.url,
					type: "POST",
					data: JSON.stringify(file),
					contentType: "application/json",
					success: $.proxy(function (json) {
						_redactor.progress.hide();
						if (!_redactor.upload.direct)
							_redactor.upload.$droparea.removeClass('drag-drop');
						_redactor.upload.callback(json, _redactor.upload.direct, e);
					}, _redactor),
					error: $.proxy(function (json) {
						_redactor.core.setCallback('imageUploadError', json.responseJSON.message);
						_redactor.progress.hide();
						_redactor.modal.close();
						return;
					}, _redactor),
				});
			};
		}
	};
};

