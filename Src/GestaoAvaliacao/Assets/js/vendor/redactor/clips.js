/**
 * function Plugin para inserir spans personalizados
 * @namespace Controller
 * @author Felipe Lopez 
 * @author Julio Silva - 21/10/2015
 */
if (!RedactorPlugins) var RedactorPlugins = {};

RedactorPlugins.clips = function () {
  return {
	init: function () {
		var items = [
		['agrupar', '<span class="numeral prova">R$ 000 000</span>'],
		['÷', '<span class="divider">&#247;</span>'],
		['×', '<span class="multipler">&#215;</span>'],
		['-', '<span class="minus">&#8722;</span>'],
		['+', '<span class="plus">&#43;</span>'],
		['°', '<span class="ordem">&#176;</span>'],
		['≤', '<span class="lower">&#8804;</span>'],
		['≥', '<span class="upper">&#8805;</span>'],
	  ];
	  this.clips.template = $('<ul id="redactor-modal-list">');
	  for (var i = 0; i < items.length; i++) {
		var li = $('<li>');
		var a = $('<a href="javascript:void(0)" class="redactor-clip-link">').text(items[i][0]);
		var div = $('<div class="redactor-clip">').hide().html(items[i][1]);
		li.append(a);
		li.append(div);
		this.clips.template.append(li);
	  }
	  this.modal.addTemplate('clips', '<section>' + this.utils.getOuterHtml(this.clips.template) + '</section>');
	  var button = this.button.add('clips', 'Clips');
	  this.button.addCallback(button, this.clips.show);

	},
	show: function () {
	  this.modal.load('clips', 'Insert Clips', 400);
	  this.modal.createCancelButton();
	  $('#redactor-modal-list').find('.redactor-clip-link').each($.proxy(this.clips.load, this));
	  this.selection.save();
	  this.modal.show();
	},
	load: function (i, s) {
	  $(s).on('click', $.proxy(function (e) {
		e.preventDefault();
		this.clips.insert($(s).next().html());
	  }, this));
	},
	insert: function (html) {
	  this.insert.html(html);
	  this.modal.close();
	  this.observe.load();
	  this.selection.restore();
	  this.code.sync();
	}
  };
};

