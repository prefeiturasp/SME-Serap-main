$('#summernote').summernote({
    placeholder: '',
    tabsize: 2,
    height: 120,
    toolbar: [
        ['style', ['style']],
        ['font', ['bold', 'underline', 'italic', 'clear']],
        ['color', ['color']],
        ['font', ['fontsize']],
        ['para', ['ul', 'ol', 'paragraph']]
    ]
});

function obterCampoContextoSummernote() {
    return $("div .note-editable");
}

function obterScopeModalContexto() {
    return angular.element($('#txtModalContexto').get(0)).scope();
}

$('div .note-editable').bind('DOMNodeInserted DOMSubtreeModified DOMNodeRemoved', function () {
    var campoContexto = obterCampoContextoSummernote();
    var htmlModalContexto = campoContexto.html();
    var scope = obterScopeModalContexto();
    scope.e1_dadosModalContexto.text = htmlModalContexto;
    scope.$apply();
});

